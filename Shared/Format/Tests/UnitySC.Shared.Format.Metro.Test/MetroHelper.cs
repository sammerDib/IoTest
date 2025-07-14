using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Test
{
    public class MetroHelper
    {
        public static WaferDimensionalCharacteristic CreateWafer()
        {
            var wafer = new WaferDimensionalCharacteristic();
            wafer.Category = "1.15";
            wafer.WaferShape = WaferShape.Notch;
            wafer.Diameter = 300.Millimeters();
            wafer.Notch = new NotchDimentionalCharacteristic()
            {
                Angle = 0.Degrees(),
                Depth = 1.Millimeters(),
                DepthPositiveTolerance = 0.25.Millimeters(),
                AngleNegativeTolerance = 1.Degrees(),
                AnglePositiveTolerance = 5.Degrees(),
            };
            return wafer;
        }

        public static WaferMap CreateWaferMap()
        {
            var res = new WaferMap();

            res.RotationAngle = 0.3.Degrees();
            res.DieGridTopLeftXPosition = -150.Millimeters();
            res.DieGridTopLeftYPosition = 150.Millimeters();
            res.DieSizeWidth = 2.8.Millimeters();
            res.DieSizeHeight = 2.8.Millimeters();
            res.DiePitchHeight = 0.2.Millimeters();
            res.DiePitchWidth = 0.2.Millimeters();

            var nbColumns = 100;
            var nbRows = 100;

            bool[][] diesPresence = new bool[nbColumns][];

            for (int i = 0; i < nbColumns; i++)
            {
                diesPresence[i] = new bool[nbRows];
            }

            for (int i = 0; i < nbColumns; i++)
            {
                for (int j = 0; j < nbRows; j++)
                {
                    if (i == 0 || j == 0 || i == nbColumns - 1 || j == nbRows - 1)
                        diesPresence[i][j] = false;
                    else
                        diesPresence[i][j] = true;
                }
            }

            res.DieReferenceColumnIndex = 5;
            res.DieReferenceRowIndex = 5;
            res.SetDiesPresences(diesPresence);
            return res;
        }

        public static RemoteProductionResultInfo CreateAutomResultInfo()
        {

            return new RemoteProductionResultInfo()
            {
                DFRecipeName = "DFRecipeName",
                PMRecipeName = "PMRecipeName",
                ProcessJobID = "ProcessJobID",
                LotID = "LotID",
                CarrierID = "CarrierID",
                SlotID = 4,
                StartRecipeTime = new DateTime(2024, 02, 03, 17, 10, 06)
            };
        }

        public static void AreEqualAutomInfo(RemoteProductionResultInfo expected, RemoteProductionResultInfo actual, string testName)
        {
            if (actual == null)
                Assert.IsNull(expected, $"{testName} AutomationInfo should be Null");
            else
            {
                Assert.IsNotNull(expected, $"{testName} AutomationInfo Should NOT be NULL");

                Assert.AreEqual(expected.DFRecipeName, actual.DFRecipeName, $"{testName} AutomationInfo DFRecipeName ");
                Assert.AreEqual(expected.PMRecipeName, actual.PMRecipeName, $"{testName} AutomationInfo PMRecipeName ");
                Assert.AreEqual(expected.ProcessJobID, actual.ProcessJobID, $"{testName} AutomationInfo ProcessJobID");
                Assert.AreEqual(expected.LotID, actual.LotID, $"{testName} AutomationInfo LotID");
                Assert.AreEqual(expected.CarrierID, actual.CarrierID, $"{testName} AutomationInfo CarrierID");
                Assert.AreEqual(expected.SlotID, actual.SlotID, $"{testName} AutomationInfo SlotID");
                Assert.AreEqual(expected.StartRecipeTime, actual.StartRecipeTime, $"{testName} AutomationInfo StartRecipeTime");
            }
        }

        public static class StaticRandom
        {
            private static int s_seed;

            private static ThreadLocal<Random> s_threadLocal = new ThreadLocal<Random>
                (() => new Random(Interlocked.Increment(ref s_seed)));

            static StaticRandom()
            {
                s_seed = Environment.TickCount;
            }

            public static Random Instance { get { return s_threadLocal.Value; } }
        }
    }
}
