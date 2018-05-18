using System;
using System.IO;
using System.Threading.Tasks;
using GoCommando;

namespace DAX.CimCmd.Commands
{
    [Command("diff")]
    [Description("Generates a difference file from two CIM files")]
    public class DiffCommand : BaseCommandWithOutputFile
    {
        [Parameter("from")]
        [Description("Specifies which file to use as the starting point")]
        public string FromFilePath { get; set; }

        [Parameter("to")]
        [Description("Specifies which file to use as the ending point")]
        public string ToFilePath { get; set; }

        protected override async Task Execute()
        {
            await base.Execute();

            AssertFileExists(FromFilePath);
            AssertFileExists(ToFilePath);

            Console.WriteLine("Generate diff in here");
        }
    }
}