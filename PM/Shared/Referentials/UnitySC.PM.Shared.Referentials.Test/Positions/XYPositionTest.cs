using System.IO;
using System.Runtime.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Referentials.TestUtils.Positions;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Referentials.Test.Positions
{
    [TestClass]
    public class XYPositionTest
    {
        [TestMethod]
        public void Serialization_with_dataContract()
        {
            // Given
            var position = XYPositionFactory.Build();
            var serializer = new DataContractSerializer(typeof(XYPosition));
            using (var stream = new MemoryStream())
            {
                // When
                serializer.WriteObject(stream, position);
                stream.Position = 0;
                var serializedPosition = serializer.ReadObject(stream);

                // Then
                Assert.AreEqual(position, serializedPosition);
            }
        }

        [TestMethod]
        public void Serialization_with_XmlSerializer()
        {
            // Given
            var position = XYPositionFactory.Build();

            // When
            string xml = XML.SerializeToString(position);
            var serializedPosition = XML.DeserializeFromString<XYPosition>(xml);

            // Then
            Assert.AreEqual(position, serializedPosition);
        }
    }
}
