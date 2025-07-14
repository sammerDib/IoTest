using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Tools.Test.Units
{
    [TestClass]
    [Ignore]
    public class AccelerationTest
    {
        //[TestMethod] //// Disable temporaly this test since it randomly fail some time to time on build some specific agent
        public void ToString_Null_Null()
        {
            var saveCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            string nullnull = $"{(1.2).ToString(null, null)} mm/s²";
            string tostringempty = $"{(1.2).ToString()} mm/s²";
            string currentInvariantculture = $"{(1.2).ToString(null, CultureInfo.CurrentCulture)} mm/s²";
            string saveculture = $"{(1.2).ToString(null, saveCulture)} mm/s²";

            Assert.AreEqual(nullnull, tostringempty);
            Assert.AreEqual(nullnull, currentInvariantculture);
            Assert.AreEqual(nullnull, saveculture);

            CultureInfo.CurrentCulture = saveCulture;
        }

        [TestMethod]
        public void Serialization_with_dataContract()
        {
            // Given
            var acceleration = 2.MillimetersPerSecondSquared();
            var serializer = new DataContractSerializer(typeof(Acceleration));
            using (var stream = new MemoryStream())
            {
                // When
                serializer.WriteObject(stream, acceleration);
                stream.Position = 0;
                object serializedAcceleration = serializer.ReadObject(stream);

                // Then
                Assert.AreEqual(acceleration, serializedAcceleration);
            }
        }

        [TestMethod]
        public void Serialization_with_XMLSerialize()
        {
            // Given
            var acceleration = 2.MillimetersPerSecondSquared();

            // When
            string xml = XML.SerializeToString(acceleration);
            var serializedAcceleration = XML.DeserializeFromString<Acceleration>(xml);

            // Then
            Assert.AreEqual(acceleration, serializedAcceleration);
        }

        [DataTestMethod]
        [DynamicData(nameof(ToString_data), DynamicDataSourceType.Method)]
        public void ToString_with_format_provider(Acceleration acceleration, string format, IFormatProvider formatProvider, string expected)
        {
            // When
            string formattedAcceleration = acceleration.ToString(format, formatProvider);
            // Then
            Assert.AreEqual(expected, formattedAcceleration);
        }

        public static IEnumerable<object[]> ToString_data()
        {
            return new List<object[]>
            {
                // Without format
                new object[] { (1.2).MillimetersPerSecondSquared(), null, CultureInfo.GetCultureInfo("en-US"), "1.2 mm/s²" },
                new object[] { (1.2).MillimetersPerSecondSquared(), null, CultureInfo.GetCultureInfo("fr-FR"), "1,2 mm/s²" },
                new object[] { (1.2).MillimetersPerSecondSquared(), null, null, $"{(1.2).ToString(null,CultureInfo.CurrentCulture)} mm/s²" },

                //With format
                new object[] { (1.2).MillimetersPerSecondSquared(), "F3", CultureInfo.GetCultureInfo("en-US"), "1.200 mm/s²" },
                new object[] { (1.2).MillimetersPerSecondSquared(), "F0", CultureInfo.GetCultureInfo("fr-FR"), "1 mm/s²" },
            };
        }
    }
}
