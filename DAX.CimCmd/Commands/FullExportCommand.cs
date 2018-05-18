using System;
using System.Threading.Tasks;
using GoCommando;

namespace DAX.CimCmd.Commands
{
    [Command("full-export")]
    [Description("Generates a full export")]
    public class FullExportCommand : BaseCommandWithOutputFile
    {
        protected override async Task Execute()
        {
            await base.Execute();

            Console.WriteLine("Do full export in here");
        }
    }
}