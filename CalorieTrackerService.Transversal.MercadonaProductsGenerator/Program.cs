using RoutinesGymService.Infraestructure.Persistence.Context;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public class Program
{
    private readonly ApplicationDbContext _context;

    public Program(ApplicationDbContext context)
    {
        _context = context;
    }

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
                    _context.MercadonaProducts.AddRange(products);
                    await _context.SaveChangesAsync();
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
}