using System.Reflection;

namespace CalorieTrackerService.Transversal.CleanSolution
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("We will delete all bin and obj folders in the solution before compile");

            string location = Assembly.GetExecutingAssembly().Location;
            string? solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(location)).FullName).FullName).FullName).FullName;
            Console.WriteLine("SolutionDirectory: " + solutionDirectory);

            if (solutionDirectory != null)
            {
                List<string> proyects = new List<string>()
                {
                    "CalorieTrackerService.Application.DataTransferObject",
                    "CalorieTrackerService.Application.Interface",
                    "CalorieTrackerService.Application.Mapper",
                    "CalorieTrackerService.Application.UseCase",

                    "CalorieTrackerService.Domain.Model",

                    "CalorieTrackerService.Infraestrucutre.Persistence",

                    "CalorieTrackerService.Service.WebApi",

                    "CalorieTrackerService.Transversal.Common",
                    "CalorieTrackerService.Transversal.JsonInterchange",
                    "CalorieTrackerService.Transversal.Security",
                };

                _DeleteBinFolders(solutionDirectory, proyects);
                _DeleteObjFolders(solutionDirectory, proyects);

                Console.WriteLine("Done");
                Console.ReadLine();
            }
        }
        
        private static void _DeleteBinFolders(string solutionDirectory, List<string> proyects)
        {
            foreach (string proyectFolder in proyects)
            {
                string binDirectory = Path.Combine(Path.Combine(solutionDirectory, proyectFolder), "bin");
                Console.WriteLine("Deleted bin folders: {0}", binDirectory);

                try
                {
                    if (Directory.Exists(binDirectory)) Directory.Delete(binDirectory, true);
                }
                catch { }
            }
        }

        private static void _DeleteObjFolders(string solutionDirectory, List<string> proyects)
        {
            foreach (string proyectFolder in proyects)
            {
                string objDirectory = Path.Combine(Path.Combine(solutionDirectory, proyectFolder), "obj");
                Console.WriteLine("Deleted obj folders: {0}", objDirectory);

                try
                {
                    if (Directory.Exists(objDirectory)) Directory.Delete(objDirectory, true);
                }
                catch { }
            }
        }
    }
}