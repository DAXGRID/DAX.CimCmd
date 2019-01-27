using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DAX.CIM.NetSamScada;
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

            //config.DataReaders[0].ConfigParameters.Add(new ConfigParameter { Name = "Extent", Value = "548694,552348,6187246,6189391" });

            var transformer = config.InitializeDataTransformer("test");
            ((CIMGraphWriter)transformer.GetFirstDataWriter()).DoNotRunPreCheckConnectivity();
                    
            transformer.TransferData();
                   
            var writer = (CIMGraphWriter)transformer.GetFirstDataWriter();
            var graph = writer.GetCIMGraph();

            var stopWatch = Stopwatch.StartNew();
                        
            var cimObjects = GetIdentifiedObjects(config, graph, true, true, true).ToList();

            Console.WriteLine("Serializing to PSI/Visue CIM XML: " + base.ExportFilePath + " (can take several minutes)...");

            var converter = new NetSamEquipmentXMLConverter(cimObjects, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });

            var result = converter.GetCimObjects().ToList();

            var xmlProfile = converter.GetXMLData(result);
            
            XmlSerializer xmlSerializer = new XmlSerializer(xmlProfile.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(base.ExportFilePath);
            xmlSerializer.Serialize(file, xmlProfile);
            file.Close();

            Console.WriteLine("Finish exporting CIM XML!");
            Console.WriteLine("It is recomended to run psi-xml-check command to check CIM XML file.");

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