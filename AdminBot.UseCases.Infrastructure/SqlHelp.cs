using System.IO;
using System.Reflection;

namespace AdminBot.UseCases.Infrastructure
{
    public static class SqlHelp
    {
        public static string LoadFromFile(string fileName)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(location);
            return File.ReadAllText($"{directory}/SqlQueries/{fileName}");
        }
    }
}