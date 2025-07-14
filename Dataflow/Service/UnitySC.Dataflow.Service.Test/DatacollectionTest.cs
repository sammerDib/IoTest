using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.XmlDiffPatch;
using SimpleInjector;
using UnitySC.Dataflow.Configuration;
using UnitySC.Dataflow.Operations.Implementation;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.SecsGem;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Serialize;

using System.Xml.Linq;
using System.Xml.Serialization;
using System.Threading;


namespace UnitySC.Dataflow.Service.Test
{

    [TestClass]
    public class DatacollectionTest
    {
        private Container _container;
        public const string AnaDC_TargetFileName = "AnaDC_TargetTest_SecsVariableList.xml";
        public const string AnaDC_ResultFileName = "AnaDC_ResultTest_SecsVariableList.xml";
        public const string AnaDC_DiffFileName = "AnaDC_ResultTestError_Diff_SecsVariableList.xml";
        public const string AnaDC_LogFileName = "Last_AnaDataCollection.xml";

        public const String AnaDC_ConvertDllTested = "UnitySC.DataCollectionConverter.ANA.SG.dll";

        [TestInitialize]
        public void Init()
        {
            _container = new Container();
            Bootstrapper.Register(_container);

            // Clean space for new test with previous files
            Cleanup();

            if (File.Exists(AnaDC_DiffFileName))
                File.Delete(AnaDC_DiffFileName);

            var dfConfiguration = ClassLocator.Default.GetInstance<DFServerConfiguration>();
            Assert.IsTrue((dfConfiguration != null), "TestInitialize - dfConfiguration does not exist !");
            var serviceConfiguration = ClassLocator.Default.GetInstance<IServiceDFConfigurationManager>();
            Assert.IsTrue((serviceConfiguration != null), "TestInitialize - serviceConfiguration does not exist !");

            // Prepare DataCollectionConverters files
            if (!Directory.Exists(".\\DataCollectionConverters"))
                Directory.CreateDirectory(".\\DataCollectionConverters");
            Assert.IsTrue(Directory.Exists(".\\DataCollectionConverters"), "TestInitialize - CreateDirectory DataCollectionConverters failed !");
            var finalConverterDllFilePathName = $"{Environment.CurrentDirectory}\\.\\DataCollectionConverters\\{AnaDC_ConvertDllTested}";
            if (!File.Exists(finalConverterDllFilePathName))
                File.Copy(AnaDC_ConvertDllTested, finalConverterDllFilePathName, true);
            Assert.IsTrue(File.Exists(finalConverterDllFilePathName), "TestInitialize - finalConverterDllFilePathName does not exist !");
        }

        [TestCleanup()]
        public void Cleanup()
        {
            var serviceConfiguration = ClassLocator.Default.GetInstance<IServiceDFConfigurationManager>();
            var dfConfiguration = ClassLocator.Default.GetInstance<DFServerConfiguration>();

            if (File.Exists(AnaDC_LogFileName)) File.Delete(AnaDC_LogFileName);
            if (File.Exists(dfConfiguration.LogDataCollectionPathFile)) File.Delete(dfConfiguration.LogDataCollectionPathFile);
            if (File.Exists(serviceConfiguration.LogConfigurationFilePath)) File.Delete(serviceConfiguration.LogConfigurationFilePath);

            if (Directory.Exists(".\\DataCollectionConverters"))
                Directory.Delete(".\\DataCollectionConverters", true);
        }

        [TestMethod]
        public void DataCollectionTest_IsConversionOperational()
        {
            var originalCulture = Thread.CurrentThread.CurrentCulture;
            var originalUICulture = Thread.CurrentThread.CurrentUICulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                var pmdfOperation = ClassLocator.Default.GetInstance<PMDFOperations>();
                Assert.IsTrue((pmdfOperation != null), "PMDFOperations instance null !");
                pmdfOperation.Init();

                var dfConfiguration = ClassLocator.Default.GetInstance<DFServerConfiguration>();
                var serviceConfiguration = ClassLocator.Default.GetInstance<IServiceDFConfigurationManager>();

                // Create a dummy AnaDataCollection
                dfConfiguration.LogDataCollectionPathFile = AnaDC_LogFileName;
                var anaDC = CreateDummyDataForAnaDC();
                Assert.IsTrue((anaDC != null), "Dummy Data for AnaDataCollection failed !");

                // Create a log
                pmdfOperation.UpdateDatacollectionLog(anaDC);
                Assert.IsTrue(File.Exists(dfConfiguration.LogDataCollectionPathFile), "AnaDataCollection logging failed ! Log file was not created !");

                // Do a conversion in SecsVariableList
                var dcConvert = ClassLocator.Default.GetInstance<IDataCollectionConvert>();
                SecsVariableList secsVariableList = dcConvert.ConvertToSecsVariableList(anaDC);

                var expected = secsVariableList.DatacontractSerializeToString(true);

                File.WriteAllText(AnaDC_ResultFileName, expected, Encoding.UTF8);

                Assert.IsTrue(File.Exists(AnaDC_ResultFileName), "Conversion result AnaDataCollection failed. Result file was not created !");

                string fsTarget = File.ReadAllText(AnaDC_TargetFileName);
                string fsExpected = File.ReadAllText(AnaDC_ResultFileName);
                Assert.AreEqual(fsTarget, fsExpected, "Output SecsVariableList XML file is not equal to target");

                File.Delete(AnaDC_ResultFileName);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
                Thread.CurrentThread.CurrentUICulture = originalUICulture;
            }
        }

        private static ANADataCollection CreateDummyDataForAnaDC()
        {
            var anaDC = new ANADataCollection();
            anaDC.LotID = "LotTest";
            anaDC.LoadportID = 1;
            anaDC.SlotID = 1;
            anaDC.CarrierID = "FOUPTest";
            anaDC.SubstrateID = $"{anaDC.CarrierID}.{anaDC.SlotID}";
            anaDC.AcquiredID = "MyAcquiredID";
            anaDC.ControlJobID = "CJ1";
            anaDC.ProcessJobID = "PJ1";
            anaDC.ProcessEndTime = DateTime.Parse("04/22/2025 16:41:33", CultureInfo.InvariantCulture);
            anaDC.ProcessStartTime = DateTime.Parse("04/22/2025 16:36:33", CultureInfo.InvariantCulture);
            anaDC.RecipeID = "MyRecipe";


            anaDC.WaferMeasureData = new DVIDWaferMeasureData();
            anaDC.WaferMeasureData.Name = "Points Measures";
            anaDC.WaferMeasureData.WaferMeasuresData = new List<DCPointMeasureDataForMeasure>();
            anaDC.AllDiesStatistics = new DVIDAllDiesStatistics();
            anaDC.AllDiesStatistics.Name = "Dies Statistics";
            anaDC.AllDiesStatistics.DiesStatistics = new List<DCDiesStatisticsForMeasure>();
            anaDC.WaferStatistics = new DVIDWaferStatistics();
            anaDC.WaferStatistics.Name = "Global Wafer Statistics";
            anaDC.WaferStatistics.WaferStatistics = new List<DCWaferStatisticsForMeasure>();
            // Nb type mesures => TSV1, TSV2, TSV3
            for (int j = 1; j < 4; j++)
            {
                // Wafer Stats
                var WaferMeasureStatFormMeasure = new DCWaferStatisticsForMeasure();
                WaferMeasureStatFormMeasure.MeasureName = "TSV" + j;
                WaferMeasureStatFormMeasure.WaferStatisticsForMeasure = new List<DCData>();
                var WaferStatForMeasureData_TSVDepth = new DCDataDouble();
                WaferStatForMeasureData_TSVDepth.Unit = "µm";
                WaferStatForMeasureData_TSVDepth.IsMeasured = true;
                var WaferStatForMeasureData_TSVCDWidth = new DCDataDouble();
                WaferStatForMeasureData_TSVCDWidth.Unit = "µm";
                WaferStatForMeasureData_TSVCDWidth.IsMeasured = true;
                var WaferStatForMeasureData_TSVCDLength = new DCDataDouble();
                WaferStatForMeasureData_TSVCDLength.Unit = "µm";
                WaferStatForMeasureData_TSVCDLength.IsMeasured = true;

                // DieStats
                var newDCDieStatsForMeasure = new DCDiesStatisticsForMeasure();
                newDCDieStatsForMeasure.MeasureName = "TSV" + j;
                newDCDieStatsForMeasure.DiesStatisticsForMeasure = new List<DCDieStatistics>();

                // Point measure 
                var newDCPointsForMeasure = new DCPointMeasureDataForMeasure();
                newDCPointsForMeasure.MeasureName = newDCDieStatsForMeasure.MeasureName;
                newDCPointsForMeasure.WaferMeasuresDataForMeasure = new List<DCPointMeasureData>();

                var cptTotalPoints = 0;
                // Nb die
                for (int k = 0; k < 2; k++)
                {
                    // die allocation
                    var newDCDieStatsForMeasure_Location = new DCDieStatistics();
                    newDCDieStatsForMeasure_Location.DieStatistics = new List<DCData>();

                    // Die location
                    newDCDieStatsForMeasure_Location.ColumnIndex = k;
                    newDCDieStatsForMeasure_Location.RowIndex = k + 1;

                    var newDCDieAverageDataValues = new List<double> { 0d, 0d, 0d };

                    var countI = 2;
                    // Nb point mesure
                    for (int i = 0; i < countI; i++)
                    {

                        // Points
                        var newDCPointsData = new DCPointMeasureData();
                        newDCPointsData.PointMeasuresData = new List<DCData>();

                        // Die loaction for a point
                        newDCPointsData.DieRowIndex = newDCDieStatsForMeasure_Location.RowIndex;
                        newDCPointsData.DieColumnIndex = newDCDieStatsForMeasure_Location.ColumnIndex;

                        // Points location
                        newDCPointsData.CoordinateX = 400.1 + i;
                        newDCPointsData.CoordinateY = 500.2 + i;

                        // TSV Depth
                        //----------
                        // point data
                        var newDCData = new DCDataDouble();
                        newDCData.Value = 38.38 + i + j;
                        newDCData.IsMeasured = true;
                        newDCData.Name = "TSV Depth";
                        newDCData.Unit = "µm";
                        newDCPointsData.PointMeasuresData.Add(newDCData);
                        // Die average point (only 1)
                        newDCDieAverageDataValues[0] += newDCData.Value;
                        // Wafer stat
                        WaferStatForMeasureData_TSVDepth.Value += newDCData.Value;
                        WaferStatForMeasureData_TSVDepth.Name = "TSV Depth Wafer Average ";
                        WaferStatForMeasureData_TSVDepth.Unit = "µm";

                        // TSV CD Width
                        //----------
                        // point
                        newDCData = new DCDataDouble();
                        newDCData.Value = 5.5 + i + j;
                        newDCData.IsMeasured = true;
                        newDCData.Name = "TSV CD Width";
                        newDCData.Unit = "µm";
                        newDCPointsData.PointMeasuresData.Add(newDCData);
                        // Die average point (only 1)
                        newDCDieAverageDataValues[1] += newDCData.Value;
                        // Wafer stat
                        WaferStatForMeasureData_TSVCDWidth.Value += newDCData.Value;
                        WaferStatForMeasureData_TSVCDWidth.Name = "TSV CD Width Wafer Average ";
                        WaferStatForMeasureData_TSVCDWidth.Unit = "µm";

                        // TSV CD Length
                        //----------
                        // point
                        newDCData = new DCDataDouble();
                        newDCData.Value = 5.6 + i + j;
                        newDCData.IsMeasured = true;
                        newDCData.Name = "TSV CD Length";
                        newDCData.Unit = "µm";
                        newDCPointsData.PointMeasuresData.Add(newDCData);
                        // Die average point (only 1)
                        newDCDieAverageDataValues[2] += newDCData.Value;
                        // Wafer stat
                        WaferStatForMeasureData_TSVCDLength.Value += newDCData.Value;
                        WaferStatForMeasureData_TSVCDLength.Name = "TSV CD Length Wafer Average ";
                        WaferStatForMeasureData_TSVCDLength.Unit = "µm";

                        //Add new Point
                        newDCPointsForMeasure.WaferMeasuresDataForMeasure.Add(newDCPointsData);

                        cptTotalPoints++;
                    }

                    //Dies 
                    var newDCDieAverageData = new DCDataDouble();
                    newDCDieAverageData.Name = "TSV Depth Die Average";
                    newDCDieAverageData.Unit = "µm";
                    newDCDieAverageData.Value = newDCDieAverageDataValues[0] / countI;
                    newDCDieStatsForMeasure_Location.DieStatistics.Add(newDCDieAverageData);

                    newDCDieAverageData = new DCDataDouble();
                    newDCDieAverageData.Name = "TSV CD Width Die Average";
                    newDCDieAverageData.Unit = "µm";
                    newDCDieAverageData.Value = newDCDieAverageDataValues[1] / countI;
                    newDCDieStatsForMeasure_Location.DieStatistics.Add(newDCDieAverageData);

                    newDCDieAverageData = new DCDataDouble();
                    newDCDieAverageData.Name = "TSV CD Length Die Average ";
                    newDCDieAverageData.Unit = "µm";
                    newDCDieAverageData.Value = newDCDieAverageDataValues[2] / countI;
                    newDCDieStatsForMeasure_Location.DieStatistics.Add(newDCDieAverageData);

                    newDCDieStatsForMeasure.DiesStatisticsForMeasure.Add(newDCDieStatsForMeasure_Location);
                }
                // points
                anaDC.WaferMeasureData.WaferMeasuresData.Add(newDCPointsForMeasure);

                // Dies
                anaDC.AllDiesStatistics.DiesStatistics.Add(newDCDieStatsForMeasure);

                // Wafer stats
                if (cptTotalPoints > 0)
                {
                    WaferStatForMeasureData_TSVDepth.Value = WaferStatForMeasureData_TSVDepth.Value / cptTotalPoints;
                    WaferStatForMeasureData_TSVCDWidth.Value = WaferStatForMeasureData_TSVCDWidth.Value / cptTotalPoints;
                    WaferStatForMeasureData_TSVCDLength.Value = WaferStatForMeasureData_TSVCDLength.Value / cptTotalPoints;
                }
                else
                {
                    WaferStatForMeasureData_TSVDepth.Value = double.NaN;
                    WaferStatForMeasureData_TSVCDWidth.Value = double.NaN;
                    WaferStatForMeasureData_TSVCDLength.Value = double.NaN;
                }
                WaferMeasureStatFormMeasure.WaferStatisticsForMeasure.Add(WaferStatForMeasureData_TSVDepth);
                WaferMeasureStatFormMeasure.WaferStatisticsForMeasure.Add(WaferStatForMeasureData_TSVCDWidth);
                WaferMeasureStatFormMeasure.WaferStatisticsForMeasure.Add(WaferStatForMeasureData_TSVCDLength);
                anaDC.WaferStatistics.WaferStatistics.Add(WaferMeasureStatFormMeasure);
            }
            return anaDC;
        }
    }
}
