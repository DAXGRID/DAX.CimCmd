using System.IO;
using System.Threading.Tasks;
using GoCommando;

namespace DAX.CimCmd
{
    public abstract class BaseCommand : ICommand
    {
        public void Run()
        {
            Execute().Wait();
        }

        protected abstract Task Execute();

        protected static string AssertFileExists(string filePath)
        {
            if (File.Exists(filePath)) return filePath;

            throw new FileNotFoundException($"Could not find file '{filePath}'");
        }
    }
}