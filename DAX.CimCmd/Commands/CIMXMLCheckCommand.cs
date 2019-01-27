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
using DAX.IO.CIM.XMLCheckerLib;
using DAX.IO.Writers;
using GoCommando;
using Serilog.Core;

namespace DAX.CimCmd.Commands
{
    [Command("psi-xml-check")]
    [Description("Checks CIM XML - including PSI rules")]
    public class CIMXMLCheckCommand : BaseCommand
    {
        [Parameter("file")]
        [Description("Specifies which CIM XML file to check")]
        public string CimXmlFile { get; set; }

        protected override async Task Execute()
        {

            CIMXMLChecker checker = new CIMXMLChecker(CimXmlFile);
            checker.BuildLookupInfo();
            checker.CheckLowVoltageACLSUniqueNames();
            checker.CheckCableboxSubstationUniqueNames();
            checker.CheckAllUsagePoints(true);
        }

    }
}