using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    [Command("export")]
    [Description("Generates an full CIM export")]
    public class ExportCommand : BaseCommandWithOutputFile
    {
        [Parameter("configFile")]
        [Description("Specifies which CIM Adapter config to use")]
        public string ConfigFileName { get; set; }
                     
        protected override async Task Execute()
        {
            await base.Execute();

            AssertFileExists(ConfigFileName);
         
            Console.WriteLine("Creating full CIM network dump based on configuration: " + ConfigFileName);

            var config = new TransformationConfig().LoadFromFile(ConfigFileName);

            //config.DataReaders[0].ConfigParameters.Add(new ConfigParameter { Name = "Extent", Value = "548694,552348,6187246,6189391" });

            var transformer = config.InitializeDataTransformer("test");
            ((CIMGraphWriter)transformer.GetFirstDataWriter()).DoNotRunPreCheckConnectivity();
                    
            transformer.TransferData();
                   
            var writer = (CIMGraphWriter)transformer.GetFirstDataWriter();
            var graph = writer.GetCIMGraph();

            var stopWatch = Stopwatch.StartNew();
                        
            var allCimObjects = GetIdentifiedObjects(config, graph, true, true, true).ToList();

            var cson = new CsonSerializer();

            using (var destination = File.Open(base.ExportFilePath, FileMode.Create))
            {
                using (var source = cson.SerializeObjects(allCimObjects))
                {
                    source.CopyTo(destination);
                }
            }



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