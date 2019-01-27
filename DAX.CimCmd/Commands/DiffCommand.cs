using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAX.CIM.Differ;
using DAX.CIM.NetSamScada.EquipmentXmlWriter.Delta;
using DAX.Cson;
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

        [Parameter("format")]
        [Description("Specifies either XML (detault) or Jsonl")]
        public string Format { get; set; }

        protected override async Task Execute()
        {
            await base.Execute();

            AssertFileExists(FromFilePath);
            AssertFileExists(ToFilePath);

            //Console.WriteLine("Generate diff in here");

            if (Format == null || Format.ToLower() == "xml")
            {
                Console.Out.WriteLine("Generating XML delta...");
                DeltaWriter.WriteDeltaXML(FromFilePath, ToFilePath, ExportFilePath);
            }
            else
            {
                Console.Out.WriteLine("Generating Json delta...");

                var differ = new CimDiffer();
                var serializer = new CsonSerializer();

                var prevFile = serializer.DeserializeObjects(File.OpenRead(FromFilePath));

                var nextFile = serializer.DeserializeObjects(File.OpenRead(ToFilePath));

                var diffObjs = differ.GetDiff(prevFile, nextFile).ToList();

                Console.Out.WriteLine("Number of diff objects: " + diffObjs.Count);

                using (var destination = File.Open(ExportFilePath, FileMode.Create))
                {
                    using (var source = serializer.SerializeObjects(diffObjs))
                    {
                        source.CopyTo(destination);
                    }
                }
            }

            Console.Out.WriteLine("Diff file written: " + ExportFilePath);

        }
    }
}