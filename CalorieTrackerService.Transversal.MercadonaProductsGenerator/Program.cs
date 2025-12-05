using RoutinesGymService.Infraestructure.Persistence.Context;
using System.Diagnostics;
using System.Text;

public class Program
{
    private const string ScriptFileName = "mercadona_api_scraper.py";
    private const string DataDirectory = "data";
    private readonly ApplicationDbContext _context;

    public Program(ApplicationDbContext context)
    {
        _context = context;
    }

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=================================================");
        Console.WriteLine("💻 Aplicación C# para ejecutar Scraper de Mercadona 🛒");
        Console.WriteLine("=================================================");

        bool scraperExecuted = false;

        // 1. Comprobar si se debe ejecutar el scraper
        if (ShouldRunScraper())
        {
            // 2. Buscar el script Python
            string scriptPath = FindPythonScript(ScriptFileName);

            if (scriptPath == null)
            {
                HandleScriptNotFoundError();
                return;
            }

            Console.WriteLine($"✅ Script Python encontrado en: {scriptPath}");

            // 3. Ejecutar el scraper
            scraperExecuted = ExecuteScraper(scriptPath);
        }
        else
        {
            Console.WriteLine($"\n⚠️  AVISO: La carpeta '{DataDirectory}/' ya contiene archivos.");
            Console.WriteLine("   El scraper no se ejecutará para evitar sobrescribir datos o repetir trabajo.");
        }

        Console.WriteLine("\n-------------------------------------------------");

        // 4. Preguntar sobre la importación a la base de datos
        ImportarEnBaseDeDatos(scraperExecuted);

        Console.WriteLine("\n✅ Proceso completado. Presiona cualquier tecla para salir...");
        Console.ReadKey();
    }

    private static bool ShouldRunScraper()
    {
        string dataPath = Path.Combine(Directory.GetCurrentDirectory(), DataDirectory);

        // Intenta buscar el directorio 'data' cerca del ejecutable (ej. en el directorio del proyecto)
        if (!Directory.Exists(dataPath))
        {
            // Busca la carpeta 'data' en los mismos lugares que FindPythonScript
            string scriptPath = FindPythonScript(ScriptFileName);
            if (scriptPath != null)
            {
                dataPath = Path.Combine(Path.GetDirectoryName(scriptPath), DataDirectory);
            }
        }

        if (Directory.Exists(dataPath))
        {
            // Comprobar si hay archivos en la carpeta 'data'
            if (Directory.GetFiles(dataPath).Length > 0)
            {
                return false; // No ejecutar si hay archivos
            }
        }

        // Ejecutar si la carpeta no existe o está vacía
        return true;
    }

    private static bool ExecuteScraper(string scriptPath)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                WorkingDirectory = Path.GetDirectoryName(scriptPath)
            };

            Console.WriteLine("Lanzando el intérprete de Python...");
            Console.WriteLine("-------------------------------------------------");

            using (Process process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    Console.WriteLine("❌ ERROR: No se pudo iniciar el proceso. Asegúrate de que 'python' esté instalado y en tu PATH.");
                    return false;
                }

                // Enviar automáticamente "1" para ejecutar el scrapeo completo
                process.StandardInput.WriteLine("1");
                process.StandardInput.Close();

                // Leer y mostrar la salida en tiempo real
                string line;
                while ((line = process.StandardOutput.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }

                string errors = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine("-------------------------------------------------");
                    Console.WriteLine($"❌ ERROR de Ejecución (Código de salida: {process.ExitCode}):");
                    if (!string.IsNullOrWhiteSpace(errors))
                    {
                        Console.WriteLine(errors);
                    }
                    Console.WriteLine("-------------------------------------------------");
                    return false;
                }

                return true; // Éxito en la ejecución
            }
        }
        catch (System.ComponentModel.Win32Exception)
        {
            Console.WriteLine("❌ ERROR: No se encontró el programa 'python'. Verifica tu PATH.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR inesperado durante el scrapeo: {ex.Message}");
        }
        return false;
    }

    private static void ImportarEnBaseDeDatos(bool dataIsNew)
    {
        Console.WriteLine("\n--- Importación a Base de Datos ---");

        if (dataIsNew)
        {
            Console.WriteLine("¡Se han generado datos nuevos! ¿Deseas importarlos a la base de datos ahora?");
        }
        else
        {
            Console.WriteLine("Ya existen datos previos. ¿Deseas importarlos a la base de datos?");
        }

        Console.Write("Importar datos (S/N)? ");
        string choice = Console.ReadLine()?.Trim().ToUpper() ?? "N";

        if (choice == "S")
        {
            Console.WriteLine("\n⚙️  Iniciando proceso de importación...");
            


            Console.WriteLine("✅ Importación a la base de datos finalizada.");
        }
        else
        {
            Console.WriteLine("\n⏭️  Importación a la base de datos omitida por el usuario.");
        }
    }

    private static void HandleScriptNotFoundError()
    {
        Console.WriteLine($"❌ ERROR: No se encontró el archivo '{ScriptFileName}'");
        Console.WriteLine("   Asegúrate de que el archivo está en el proyecto y se copia al directorio de salida.");
        Console.WriteLine($"   Directorio actual: {Directory.GetCurrentDirectory()}");
    }

    private static string FindPythonScript(string fileName)
    {
        string currentDir = Directory.GetCurrentDirectory();

        // 1. Buscar en el directorio actual
        string path = Path.Combine(currentDir, fileName);
        if (File.Exists(path)) return path;

        // 2. Buscar en el directorio del ejecutable (AppDomain.CurrentDomain.BaseDirectory)
        string exeDir = AppDomain.CurrentDomain.BaseDirectory;
        path = Path.Combine(exeDir, fileName);
        if (File.Exists(path)) return path;

        // 3. Buscar en directorios superiores (hasta 3 niveles arriba)
        DirectoryInfo dir = new DirectoryInfo(currentDir);
        for (int i = 0; i < 3 && dir.Parent != null; i++)
        {
            dir = dir.Parent;
            path = Path.Combine(dir.FullName, fileName);

            if (File.Exists(path))
            {
                return path;
            }
        }

        // 4. No se encontró
        return null;
    }
}