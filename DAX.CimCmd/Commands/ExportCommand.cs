using System;
using System.Threading.Tasks;
using GoCommando;

namespace DAX.CimCmd.Commands
{
    [Command("export")]
    [Description("Generates an export")]
    public class ExportCommand : BaseCommandWithOutputFile
    {
        [Parameter("from")]
        [Description("Specifies which file to use as the starting point")]
        public string FromFilePath { get; set; }

        [Parameter("diff")]
        [Description("Specifies which diff file to apply")]
        public string DiffFilePath { get; set; }

        protected override async Task Execute()
        {
            await base.Execute();

            AssertFileExists(FromFilePath);
            AssertFileExists(DiffFilePath);

            Console.WriteLine("Do full export in here ");
        }
    }
}