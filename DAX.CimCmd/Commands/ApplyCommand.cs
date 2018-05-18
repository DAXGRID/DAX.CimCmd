using System;
using System.Threading.Tasks;
using GoCommando;

namespace DAX.CimCmd.Commands
{
    [Command("apply")]
    public class ApplyCommand : BaseCommandWithOutputFile
    {


        protected override async Task Execute()
        {
            await base.Execute();

            Console.WriteLine("Apply diff in here");
        }
    }
}