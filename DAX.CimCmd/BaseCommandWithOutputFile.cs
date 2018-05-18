using System.IO;
using System.Threading.Tasks;
using GoCommando;

namespace DAX.CimCmd
{
    public abstract class BaseCommandWithOutputFile : BaseCommand
    {
        [Parameter("out")]
        [Description("Specifies file path of the export file to generate")]
        public string ExportFilePath { get; set; }

        [Parameter("force", optional: true)]
        [Description("Indicates that the export file should be overwritten if it already exists")]
        public bool Force { get; set; }

        protected override async Task Execute()
        {
            if (File.Exists(ExportFilePath) && !Force)
            {
                throw new GoCommandoException($"The file '{ExportFilePath}' already exists - please use the -force flag if you intend to overwrite it");
            }
        }
    }
}