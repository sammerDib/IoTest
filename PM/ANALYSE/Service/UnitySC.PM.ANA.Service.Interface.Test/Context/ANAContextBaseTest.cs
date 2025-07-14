using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.TestUtils.Context;

namespace UnitySC.PM.ANA.Service.Interface.Test.Context
{
    [TestClass]
    public class AnaContextBaseTest
    {
        private static readonly IDictionary<Type, Func<object>> s_factories = new Dictionary<Type, Func<object>>
        {
            { typeof(ContextsList), () => ContextsSetFactory.Build() },
            { typeof(AnaPositionContext), () => AnaPositionContextFactory.Build() },
            { typeof(ObjectivesContext), () => ObjectivesContextFactory.Build() }
        };

        private static readonly HashSet<Type> s_ignoredTypes = new HashSet<Type> {
            typeof(ImageAcquisitionContextBase)
        };

        [DataTestMethod]
        [DynamicData(nameof(AllSerializableTypes), DynamicDataSourceType.Method)]
        public void DataContractSerialize(Type type)
        {
            object input = null;
            try
            {
                input = s_factories.ContainsKey(type)
                    ? s_factories[type]()
                    : Activator.CreateInstance(type);
            }
            catch (MissingMethodException)
            {
                Assert.Inconclusive("No factory provided and default constructor declared");
            }

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(ANAContextBase));
                serializer.WriteObject(stream, input);
                stream.Position = 0;
                var output = serializer.ReadObject(stream);
                output.Should().BeOfType(input.GetType());
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(AllSerializableTypes), DynamicDataSourceType.Method)]
        public void XmlSerialize(Type type)
        {
            object input = null;
            try
            {
                input = s_factories.ContainsKey(type)
                    ? s_factories[type]()
                    : Activator.CreateInstance(type);
            }
            catch (MissingMethodException)
            {
                Assert.Inconclusive("No factory provided and default constructor declared");
            }

            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(ANAContextBase));
                serializer.Serialize(stream, input);
                stream.Position = 0;
                var output = serializer.Deserialize(stream);
                output.Should().BeOfType(input.GetType());
            }
        }

        public static IEnumerable<object[]> AllSerializableTypes()
        {
            return Assembly.GetAssembly(typeof(ANAContextBase))
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(ANAContextBase)) && !s_ignoredTypes.Contains(type))
                .Select(t => new object[] { t });
        }
    }
}
