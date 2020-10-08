using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAX.CIM.Differ;
using DAX.CIM.NetSamScada.AssetXmlWriter.Diff;
using DAX.CIM.NetSamScada.EquipmentXmlWriter.Diff;
//using DAX.CIM.NetSamScada.EquipmentXmlWriter.Delta;
using DAX.Cson;
using GoCommando;

namespace DAX.CimCmd.Commands
{
    [Command("diff")]
    [Description("Generates a difference file from two CIM files")]
    public class DiffCommand : BaseCommandWithOutputFile
    {
        [Parameter("fromFolder")]
        [Description("Specifies which files to use as the starting point")]
        public string FromFilePath { get; set; }

        [Parameter("toFolder")]
        [Description("Specifies which files to use as the ending point")]
        public string ToFilePath { get; set; }

        protected override async Task Execute()
        {
            await base.Execute();

            //AssertFileExists(FromFilePath);
            //AssertFileExists(ToFilePath);

            Console.Out.WriteLine("Generate equipment delta...");

            string equipmentDeltaFile1 = FromFilePath + "\\konstant_equipment.jsonl";
            string equipmentDeltaFile2 = ToFilePath + "\\konstant_equipment.jsonl";
            string equipmentOutputFile = base.ExportFolderName + "\\konstant_equipment_psi_delta.xml";

            // Create folder if not exists
            if (!System.IO.Directory.Exists(base.ExportFolderName))
                System.IO.Directory.CreateDirectory(base.ExportFolderName);

            var equipmentCimObjects = EquipmentDeltaDiffer.CreateEquipmentDeltaXML(equipmentDeltaFile1, equipmentDeltaFile2, equipmentOutputFile);

            Console.Out.WriteLine("Finish creating equipment delta file: " + equipmentOutputFile);

            // ASSET

            Console.Out.WriteLine("Generate asset delta...");

            string assetDeltaFile1 = FromFilePath + "\\konstant_asset.jsonl";
            string assetDeltaFile2 = ToFilePath + "\\konstant_asset.jsonl";
            string assetOutputFile = base.ExportFolderName + "\\konstant_asset_psi_delta.xml";

            AssetDeltaDiffer.CreateAssetDeltaXML(assetDeltaFile1, assetDeltaFile2, assetOutputFile, equipmentCimObjects);

            Console.Out.WriteLine("Finish creating asset delta file: " + equipmentOutputFile);

        }
    }
}