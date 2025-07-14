using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Test.Algo
{
    [TestClass]
    public class WaferMapResultTest
    {
        private struct TestData
        {
            public string Name { get; set; }
            public string DiesPresenceSerialized { get; set; }
            public Matrix<bool> DiesPresence { get; set; }
        }

        private static readonly TestData s_nominalCaseData = new TestData
        {
            Name = "Nominal case",
            DiesPresenceSerialized = "1000;0100;0010;",
            DiesPresence = new Matrix<bool>(new bool[,] {
                    { true, false, false, false },
                    { false, true, false, false },
                    { false, false, true, false } })
        };

        private static readonly TestData[] s_testDatas = {
            s_nominalCaseData,
            new TestData {
                Name = "Null",
                DiesPresenceSerialized = null,
                DiesPresence = null
            },
            new TestData {
                Name = "Empty matrix",
                DiesPresenceSerialized = "",
                DiesPresence = new Matrix<bool>(0, 0)
            },
            new TestData {
                Name = "One empty row",
                DiesPresenceSerialized = ";",
                DiesPresence = new Matrix<bool>(1, 0)
            }
        };

        private bool CheckDiePresenceSerialized_serialization_of_DiesPresence(Matrix<bool> diesPresence, string expectedDiePresenceSerialized)
        {
            // Given an empty WaferMapResult
            var waferMapResult = new WaferMapResult();

            // When setting DiesPresence, but keeping DiesPresenceSerialized unset
            waferMapResult.DiesPresence = diesPresence;

            // Then DiesPresenceSerialized getter returns the proper seialization value
            if (expectedDiePresenceSerialized == null)
                return waferMapResult.DiesPresenceSerialized == null;

            string TestDiesPresenceSerialized = waferMapResult.DiesPresenceSerialized.Replace("\r", "").Replace("\n", "");
            return expectedDiePresenceSerialized == TestDiesPresenceSerialized;
        }

        [TestMethod]
        public void DiePresenceSerialized_serialization_of_DiesPresence()
        {
            foreach (var testData in s_testDatas)
            {
                bool testPassed = CheckDiePresenceSerialized_serialization_of_DiesPresence(testData.DiesPresence, testData.DiesPresenceSerialized);
                Assert.IsTrue(testPassed, $"Serialization issue on \"{testData.Name}\" test");
            }
        }

        private bool CheckDiePresence_deserialization_of_DiesPresenceSerialized(string diePresenceSerialized, Matrix<bool> expectedDiesPresence)
        {
            // Given an empty WaferMapResult
            var waferMapResult = new WaferMapResult();

            // When setting DiesPresenceSerialized, but keeping DiesPresence unset
            waferMapResult.DiesPresenceSerialized = diePresenceSerialized;

            // Then the content has been properly deserialized into DiesPresence
            if (expectedDiesPresence is null)
            {
                return waferMapResult.DiesPresence is null;
            }
            return expectedDiesPresence.Equals(waferMapResult.DiesPresence);
        }

        [TestMethod]
        public void DiePresence_deserialization_of_DiesPresenceSerialized()
        {
            foreach (var testData in s_testDatas)
            {
                bool testPassed = CheckDiePresence_deserialization_of_DiesPresenceSerialized(testData.DiesPresenceSerialized, testData.DiesPresence);
                Assert.IsTrue(testPassed, $"Deserialization issue on \"{testData.Name}\" test");
            }
        }

        [TestMethod]
        public void Xml_serialization_takes_the_custom_serialization_of_DiesPresence()
        {
            // Given a WaferMapResult with set DiesPresence
            var waferMapResult = new WaferMapResult();
            waferMapResult.DiesPresence = s_nominalCaseData.DiesPresence;

            // When serializing WaferMapResult in XML
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(WaferMapResult));
            serializer.Serialize(stream, waferMapResult);

            // Move back stream to beginning for reading it
            stream.Position = 0;

            // Then the XML serialization contains the specific serialization of DiesPresence
            using (XmlReader reader = XmlReader.Create(stream))
            {
                while (!reader.EOF && reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element)
                        continue;
                    if (reader.Name != "DiesPresence")
                        continue;

                    string diesPresenceSerialization = reader.ReadElementContentAsString().Replace("\r", "").Replace("\n", "");
                    Assert.AreEqual(s_nominalCaseData.DiesPresenceSerialized, diesPresenceSerialization);
                    return;
                }
            }
            Assert.Fail("XMLReader did not find DiesPresence in the serialized WaferMapResult");
        }

        [TestMethod]
        public void Xml_deserialization_fills_DiesPresence()
        {
            // Given a WaferMapResult with set DiesPresence
            var waferMapResult = new WaferMapResult();
            waferMapResult.DiesPresence = s_nominalCaseData.DiesPresence;

            // When serializing WaferMapResult in XML and then deserializing it
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(WaferMapResult));
            serializer.Serialize(stream, waferMapResult);
            // Move back stream to beginning for reading it
            stream.Position = 0;
            var newWaferMapResult = (WaferMapResult)serializer.Deserialize(stream);

            // Then deserialized DiesPresence is equal to the one before serialization
            Assert.IsTrue(newWaferMapResult.DiesPresence.Equals(waferMapResult.DiesPresence));
        }
    }
}
