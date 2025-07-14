using System.Collections.Generic;

using UnitySCSharedAlgosOpenCVWrapper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class BareWaferAlignmentAdaptersTest
    {
        [TestMethod]
        public void Expect_ConvertEdgePosition_to_convert_all_known_positions()
        {
            var positionMappings = new Dictionary<EdgePosition, WaferEdgePositions>
            {
                { EdgePosition.BOTTOM, WaferEdgePositions.Bottom },
                { EdgePosition.LEFT, WaferEdgePositions.Left },
                { EdgePosition.TOP, WaferEdgePositions.Top },
                { EdgePosition.RIGHT, WaferEdgePositions.Right }
            };

            foreach (var position in positionMappings.Keys)
            {
                Assert.AreEqual(positionMappings[position], TypeConverter.ConvertEdgePosition(position));
            }
        }

        [TestMethod]
        public void Expect_ConvertWaferType_to_convert_all_algorithm_known_positions()
        {
            var waferTypeMapping = new Dictionary<WaferShape, WaferType>
            {
                {WaferShape.Notch , WaferType.NOTCH},
            };

            foreach (var type in waferTypeMapping.Keys)
            {
                Assert.AreEqual(waferTypeMapping[type], TypeConverter.ConvertWaferShape(type));
            }
        }

        [TestMethod]
        public void Expect_ConvertWaferType_to_throw_for_unknown_wafer_types()
        {
            var unknownWaferTypeMapping = new List<WaferShape>()
            {
                WaferShape.Flat, WaferShape.NonFlat, WaferShape.Sample
            };

            string message = $"Converting unknown WaferShape should lead to exception";
            foreach (var waferShape in unknownWaferTypeMapping)
            {
                Assert.ThrowsException<UnsupportedWaferException>(() => TypeConverter.ConvertWaferShape(waferShape));
            }
        }
    }
}
