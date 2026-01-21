using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Npgsql;
using Microsoft.Extensions.Configuration;

public class Program
{
    #region Main
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=================================================");
        Console.WriteLine("Mercadona Products Generator");
        Console.WriteLine("=================================================\n");
        
        string? scriptPath = FindPythonScript("mercadona_api_scraper.py");

        if (scriptPath == null)
        {
            Console.WriteLine("ERROR: No se encontro mercadona_api_scraper.py");
            Console.ReadKey();
            return;
        }

        string scriptDir = Path.GetDirectoryName(scriptPath)!;
        string fullProductsPath = Path.Combine(scriptDir, "data", "mercadona_products_full.json");

        if (File.Exists(fullProductsPath))
        {
            string jsonString = File.ReadAllText(fullProductsPath);
            using (JsonDocument doc = JsonDocument.Parse(jsonString))
            {
                JsonElement productsElement = doc.RootElement.GetProperty("products");
                List<MercadonaProduct> products = JsonSerializer.Deserialize<List<MercadonaProduct>>(productsElement.GetRawText()) ?? new List<MercadonaProduct>();
                
                Console.WriteLine($"Cargados {products.Count} productos correctamente.");
                Console.WriteLine($"¿Quieres insertar los datos en bbdd [s/n]?");
                
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key.ToString().ToLower() == "s")
                {
                    await InsertMercadonaProductsAsync(products);
                }
            }
        }
        else
        {
            Console.WriteLine($"Script encontrado: {scriptPath}\n");
            ExecuteScraper(scriptPath);
        }

        Console.WriteLine("\nPresiona cualquier tecla para salir...");
        Console.ReadKey();
    }
    #endregion

    #region Ejecutar Scraper Python
    private static void ExecuteScraper(string scriptPath)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"-u \"{scriptPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(scriptPath),
                StandardOutputEncoding = Encoding.UTF8
            };

            startInfo.Environment["PYTHONIOENCODING"] = "utf-8";

            Console.WriteLine("Ejecutando scraper...\n");

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                Console.WriteLine("ERROR: No se pudo iniciar Python");
                return;
            }

            // Leer output en tiempo real
            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                if (line != null)
                {
                    Console.WriteLine(line);
                }
            }

            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine("\nScraper completado exitosamente!");
            }
            else
            {
                Console.WriteLine($"\nERROR: El scraper termino con codigo {process.ExitCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }

    private static string? FindPythonScript(string fileName)
    {
        // 1. Buscar en el directorio del ejecutable
        string exeDir = AppDomain.CurrentDomain.BaseDirectory;
        string exePath = Path.Combine(exeDir, fileName);
        if (File.Exists(exePath))
            return exePath;

        // 2. Buscar en el directorio del proyecto (subir 3 niveles desde bin/Debug/net10.0)
        string projectDir = Path.GetFullPath(Path.Combine(exeDir, "..", "..", ".."));
        string projectPath = Path.Combine(projectDir, fileName);
        if (File.Exists(projectPath))
            return projectPath;

        // 3. Buscar en el directorio actual
        string currentPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        if (File.Exists(currentPath))
            return currentPath;

        // 4. Búsqueda recursiva como último recurso
        try
        {
            var files = Directory.GetFiles(projectDir, fileName, SearchOption.AllDirectories);
            if (files.Length > 0)
                return files[0];
        }
        catch { }

        return null;
    }
    #endregion

    #region InsertMercadonaProducts
    static async Task InsertMercadonaProductsAsync(List<MercadonaProduct> products)
    {
        string? appsettingsPath = FindAppsettingsJson();

        if (appsettingsPath == null)
        {
            Console.WriteLine("ERROR: No se encontró appsettings.json");
            return;
        }

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(appsettingsPath)!)
            .AddJsonFile(Path.GetFileName(appsettingsPath), optional: false, reloadOnChange: true)
            .Build();

        string? connectionString = config.GetConnectionString("PostgreSQLConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("ERROR: No se encontró la cadena de conexión 'PostgreSQLConnection'");
            return;
        }

        await using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = """
        INSERT INTO mercadona_products (
            id, 
            name, 
            brand, 
            description, 
            price, 
            unit_price,
            category_id, 
            category_name, 
            image_url,
            published, 
            available_online, 
            share_url,
            scraped_at, 
            main_category_id, 
            main_category_name,
            subcategory_id, 
            subcategory_name
        )
        VALUES (
            @id, 
            @name, 
            @brand, 
            @description, 
            @price, 
            @unit_price,
            @category_id, 
            @category_name, 
            @image_url,
            @published, 
            @available_online, 
            @share_url,
            @scraped_at, 
            @main_category_id, 
            @main_category_name,
            @subcategory_id, 
            @subcategory_name
        )
        ON CONFLICT (id) DO NOTHING;
        """;

        Console.WriteLine("Insertando productos en la base de datos...");

        await using NpgsqlTransaction tx = await conn.BeginTransactionAsync();

        try
        {
            int insertedCount = 0;

            List<MercadonaProduct> databaseProducts = new List<MercadonaProduct>();
            List<MercadonaProduct> newOrChangedProducts = new List<MercadonaProduct>();

            #region Cargar productos existentes

            const string selectSql = "SELECT id, name, brand, description, price, unit_price, category_id, category_name, image_url, published, available_online, share_url, scraped_at, main_category_id, main_category_name, subcategory_id, subcategory_name FROM mercadona_products;";

            await using (NpgsqlCommand selectCmd = new NpgsqlCommand(selectSql, conn, tx))
            await using (NpgsqlDataReader reader = await selectCmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    databaseProducts.Add(new MercadonaProduct
                    {
                        Id = reader.GetString(0),
                        Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        Brand = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Price = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        UnitPrice = reader.IsDBNull(5) ? null : reader.GetDouble(5),
                        CategoryId = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                        CategoryName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                        ImageUrl = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                        Published = reader.GetBoolean(9),
                        AvailableOnline = reader.GetBoolean(10),
                        ShareUrl = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                        ScrapedAt = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                        MainCategoryId = reader.GetInt32(13),
                        MainCategoryName = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                        SubcategoryId = reader.GetInt32(15),
                        SubcategoryName = reader.IsDBNull(16) ? string.Empty : reader.GetString(16)
                    });
                }
            }

            #endregion

            #region Insertar nuevos productos

            int skippedCount = 0;
            Console.WriteLine($"Cargados {databaseProducts.Count} productos desde la base de datos.");

            foreach (MercadonaProduct p in products)
            {
                bool exists = databaseProducts.Any(dbP =>
                    dbP.Id == p.Id &&
                    dbP.Name == p.Name &&
                    dbP.Brand == p.Brand &&
                    dbP.Price == p.Price &&
                    dbP.UnitPrice == p.UnitPrice &&
                    dbP.Description == p.Description &&
                    dbP.CategoryName == p.CategoryName &&
                    dbP.ImageUrl == p.ImageUrl &&
                    dbP.Published == p.Published &&
                    dbP.AvailableOnline == p.AvailableOnline &&
                    dbP.MainCategoryName == p.MainCategoryName &&
                    dbP.SubcategoryName == p.SubcategoryName);

                if (exists)
                {
                    Console.WriteLine($"[OMITIDO] El producto ID: {p.Id} ({p.Name}) ya existe sin cambios.");
                    skippedCount++;
                    continue; 
                }

                await using var cmd = new NpgsqlCommand(sql, conn, tx);

                cmd.Parameters.AddWithValue("@id", p.Id);
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@brand", string.IsNullOrEmpty(p.Brand) ? DBNull.Value : p.Brand);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(p.Description) ? DBNull.Value : p.Description);
                cmd.Parameters.AddWithValue("@price", string.IsNullOrEmpty(p.Price) ? DBNull.Value : p.Price);
                cmd.Parameters.AddWithValue("@unit_price", p.UnitPrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@category_id", string.IsNullOrEmpty(p.CategoryId) ? DBNull.Value : p.CategoryId);
                cmd.Parameters.AddWithValue("@category_name", string.IsNullOrEmpty(p.CategoryName) ? DBNull.Value : p.CategoryName);
                cmd.Parameters.AddWithValue("@image_url", string.IsNullOrEmpty(p.ImageUrl) ? DBNull.Value : p.ImageUrl);
                cmd.Parameters.AddWithValue("@published", p.Published);
                cmd.Parameters.AddWithValue("@available_online", p.AvailableOnline);
                cmd.Parameters.AddWithValue("@share_url", string.IsNullOrEmpty(p.ShareUrl) ? DBNull.Value : p.ShareUrl);
                cmd.Parameters.AddWithValue("@scraped_at", string.IsNullOrEmpty(p.ScrapedAt) ? DBNull.Value : p.ScrapedAt);
                cmd.Parameters.AddWithValue("@main_category_id", p.MainCategoryId);
                cmd.Parameters.AddWithValue("@main_category_name", string.IsNullOrEmpty(p.MainCategoryName) ? DBNull.Value : p.MainCategoryName);
                cmd.Parameters.AddWithValue("@subcategory_id", p.SubcategoryId);
                cmd.Parameters.AddWithValue("@subcategory_name", string.IsNullOrEmpty(p.SubcategoryName) ? DBNull.Value : p.SubcategoryName);

                await cmd.ExecuteNonQueryAsync();
                insertedCount++;

                if (insertedCount % 100 == 0)
                {
                    Console.WriteLine($"Procesados {insertedCount} nuevos/cambiados...");
                }
            }

            Console.WriteLine($"--- RESUMEN ---");
            Console.WriteLine($"Insertados/Actualizados: {insertedCount}");
            Console.WriteLine($"Omitidos por duplicado exacto: {skippedCount}");


            await tx.CommitAsync();
            Console.WriteLine($"✔ Insertados {products.Count} productos correctamente (duplicados omitidos).");
            
            #endregion
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine($"✖ Error al insertar productos: {ex.Message}");
            throw;
        }
    }

    private static string? FindAppsettingsJson()
    {
        string fileName = "appsettings.json";
        DirectoryInfo? currentDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (currentDir != null)
        {
            FileInfo[] files = currentDir.GetFiles(fileName, SearchOption.AllDirectories);

            FileInfo? targetFile = files.FirstOrDefault(f => f.FullName.Contains("CalorieTrackerService.Service.WebApi"));

            if (targetFile != null)
                return targetFile.FullName;

            currentDir = currentDir.Parent;
        }

        return null;
    }

    #endregion
}