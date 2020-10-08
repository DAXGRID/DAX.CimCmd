using System.IO;
using System.Threading.Tasks;
using GoCommando;

namespace DAX.CimCmd
{
    public abstract class BaseCommandWithOutputFile : BaseCommand
    {
        [Parameter("outFolder")]
        [Description("Specifies folder where exported files should be placed")]
        public string ExportFolderName { get; set; }

        [Parameter("force", optional: true)]
        [Description("Indicates that the export file should be overwritten if it already exists")]
        public bool Force { get; set; }

        protected override async Task Execute()
        {
            if (File.Exists(ExportFolderName) && !Force)
            {
                throw new GoCommandoException($"The file '{ExportFolderName}' already exists - please use the -force flag if you intend to overwrite it");
            }
        }
    }
}