using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml.Serialization;
using FluentAssertions;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public static class SerializationUtils
    {
        public static TSerializableClass AssertXmlSerializable<TSerializableClass>(TSerializableClass toSerialize) where TSerializableClass : class
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(TSerializableClass));
                try
                {
                    serializer.Serialize(stream, toSerialize);
                }
                catch (InvalidOperationException ex)
                {
                    Assert.Fail($"Could not XML serialize class \"{typeof(TSerializableClass)}\": {ex.Message}");
                }

                // Set position to 0 to start reading from the beginning
                stream.Position = 0;
                object output = null;
                try
                {
                    output = serializer.Deserialize(stream);
                }
                catch (InvalidOperationException ex)
                {
                    Assert.Fail($"Could not XML deserialize class \"{typeof(TSerializableClass)}\": {ex.Message}");
                }
                output.Should().BeOfType(typeof(TSerializableClass));
                return output as TSerializableClass;
            }
        }

        public static TSerializableClass AssertDataContractSerializable<TSerializableClass>(TSerializableClass toSerialize) where TSerializableClass : class
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(TSerializableClass));
                try
                {
                    serializer.WriteObject(stream, toSerialize);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Could not DataContract serialize class \"{typeof(TSerializableClass)}\": {ex.Message}");
                }

                // Set position to 0 to start reading from the beginning
                stream.Position = 0;
                object output = null;
                try
                {
                    output = serializer.ReadObject(stream);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Could not DataContract deserialize class \"{typeof(TSerializableClass)}\": {ex.Message}");
                }
                output.Should().BeOfType(typeof(TSerializableClass));
                return output as TSerializableClass;
            }
        }
    }
}
