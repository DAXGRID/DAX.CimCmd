using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DAX.CIM.NetSamScada;
using DAX.CIM.NetSamScada.AssetXmlWriter;
using DAX.CIM.NetSamScada.EquipmentXmlWriter;
using DAX.CIM.NetSamScada.PreProcessors;
using DAX.CIM.PhysicalNetworkModel;
using DAX.Cson;
using DAX.IO;
using DAX.IO.CIM;
using DAX.IO.CIM.DataModel;
using DAX.IO.CIM.Processing;
using DAX.IO.CIM.Serialize;
using DAX.IO.Writers;
using GoCommando;
using Serilog.Core;

namespace DAX.CimCmd.Commands
{
    [Command("psi-xml-export")]
    [Description("Generates an full PSI XML export")]
    public class PSIExportCommand : BaseCommandWithOutputFile
    {
        [Parameter("configFile")]
        [Description("Specifies which CIM Adapter config to use")]  
        public string ConfigFileName { get; set; }
                     
        protected override async Task Execute()
        {
            await base.Execute();

            AssertFileExists(ConfigFileName);

            Console.WriteLine("Reading GIS data based on configuration: " + ConfigFileName + " (can take several minutes)...");

            var config = new TransformationConfig().LoadFromFile(ConfigFileName);

            var transformer = config.InitializeDataTransformer("test");
            ((CIMGraphWriter)transformer.GetFirstDataWriter()).DoNotRunPreCheckConnectivity();
            ((CIMGraphWriter)transformer.GetFirstDataWriter()).DoNotLogToTable();

            transformer.TransferData();

            var writer = (CIMGraphWriter)transformer.GetFirstDataWriter();
            var graph = writer.GetCIMGraph();

            // Create folder if not exists
            if (!System.IO.Directory.Exists(base.ExportFolderName))
                System.IO.Directory.CreateDirectory(base.ExportFolderName);

            ExportEquipmentPsiFile(config, graph);
            ExportAssetPsiFile(config, graph);

        }

        private void ExportEquipmentPsiFile(TransformationConfig config, CIMGraph graph)
        {
            var stopWatch = Stopwatch.StartNew();

            var cimObjects = GetIdentifiedObjects(config, graph, true, false, true).ToList();

            var exportFileName = base.ExportFolderName + "\\konstant_equipment_psi.xml";

            Console.WriteLine("Serializing to PSI/Visue CIM Equipment XML: " + exportFileName + " (can take several minutes)...");

            var converter = new NetSamEquipmentXMLConverter(cimObjects, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });

            var result = converter.GetCimObjects().ToList();

            var xmlProfile = converter.GetXMLData(result);

            XmlSerializer xmlSerializer = new XmlSerializer(xmlProfile.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(exportFileName);
            xmlSerializer.Serialize(file, xmlProfile);
            file.Close();

            Console.WriteLine("Finish exporting Equipment CIM XML!");
        }

        private void ExportAssetPsiFile(TransformationConfig config, CIMGraph graph)
        {
            var stopWatch = Stopwatch.StartNew();

            var cimObjects = GetIdentifiedObjects(config, graph, false, true, false).ToList();

            var exportFileName = base.ExportFolderName + "\\konstant_asset_psi.xml";

            Console.WriteLine("Serializing to PSI/Visue CIM Asset XML: " + exportFileName + " (can take several minutes)...");

            var converter = new NetSamAssetXMLConverter(cimObjects);

            var result = converter.GetCimObjects().ToList();

            var xmlProfile = converter.GetXMLData(result);

            XmlSerializer xmlSerializer = new XmlSerializer(xmlProfile.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(exportFileName);
            xmlSerializer.Serialize(file, xmlProfile);
            file.Close();

            Console.WriteLine("Finish exporting Asset CIM XML!");
        }

        static IEnumerable<IdentifiedObject> GetIdentifiedObjects(TransformationConfig config, CIMGraph graph, bool includeEquipment = false, bool includeAssets = false, bool includeLocations = false)
        {
            var serializer = config.InitializeSerializer("DAX") as IDAXSerializeable;
            var daxcimSerializer = (DAXCIMSerializer)serializer;
            var dataRepository = CIMMetaDataManager.Repository;
            var objects = graph.CIMObjects;

            return daxcimSerializer.GetIdentifiedObjects(dataRepository, objects, includeEquipment, includeAssets, includeLocations);
        }
    }
}