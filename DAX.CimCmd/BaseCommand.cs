using System.IO;
using System.Threading.Tasks;
using DAX.Util;
using GoCommando;
using Serilog;

namespace DAX.CimCmd
{
    public abstract class BaseCommand : ICommand
    {
        public void Run()
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console()
              .CreateLogger();

            Logger.WriteToConsole = true;

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