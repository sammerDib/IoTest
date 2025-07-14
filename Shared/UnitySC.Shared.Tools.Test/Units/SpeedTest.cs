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
    public class SpeedTest
    {
        [TestMethod]
        public void Serialization_with_dataContract()
        {
            // Given
            var speed = 2.MillimetersPerSecond();
            var serializer = new DataContractSerializer(typeof(Speed));
            using (var stream = new MemoryStream())
            {
                // When
                serializer.WriteObject(stream, speed);
                stream.Position = 0;
                object serializedSpeed = serializer.ReadObject(stream);

                // Then
                Assert.AreEqual(speed, serializedSpeed);
            }
        }

        [TestMethod]
        public void Serialization_with_XMLSerialize()
        {
            // Given
            var speed = 2.MillimetersPerSecond();

            // When
            string xml = XML.SerializeToString(speed);
            var serializedSpeed = XML.DeserializeFromString<Speed>(xml);

            // Then
            Assert.AreEqual(speed, serializedSpeed);
        }

        [DataTestMethod]
        [DynamicData(nameof(ToString_data), DynamicDataSourceType.Method)]
        public void ToString_with_format_provider(Speed speed, string format, IFormatProvider formatProvider, string expected)
        {
            // When
            string formattedSpeed = speed.ToString(format, formatProvider);

            // Then
            Assert.AreEqual(expected, formattedSpeed);
        }

        public static IEnumerable<object[]> ToString_data()
        {
            return new List<object[]>
            {
                // Without format
                new object[] { (1.2).MillimetersPerSecond(), null, CultureInfo.GetCultureInfo("en-US"), "1.2 mm/s" },
                new object[] { (1.2).MillimetersPerSecond(), null, CultureInfo.GetCultureInfo("fr-FR"), "1,2 mm/s" },
                new object[] { (1.2).MillimetersPerSecond(), null, null, $"{(1.2).ToString(null,CultureInfo.CurrentCulture)} mm/s" },

                //With format
                new object[] { (1.2).MillimetersPerSecond(), "F3", CultureInfo.GetCultureInfo("en-US"), "1.200 mm/s" },
                new object[] { (1.2).MillimetersPerSecond(), "F0", CultureInfo.GetCultureInfo("fr-FR"), "1 mm/s" },
            };
        }
    }
}
