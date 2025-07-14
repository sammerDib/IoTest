using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NetArchTest.Rules;

namespace UnitySC.PM.EME.Architecture.Test
{
    [TestClass]
    public class EmeraArchitectureTests
    {
        private List<Assembly> _assemblies;

        [TestInitialize]
        public void GetAllAssemblies()
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _assemblies = Directory.GetFiles(assemblyFolder, "Unity*.dll").Select(f => Assembly.LoadFrom(f)).ToList();
        }

        [TestMethod]
        public void ServiceDoesNotDependOnClient()
        {
            // Given
            var serviveTypes = Types.InAssemblies(_assemblies).That().ResideInNamespaceContaining("UnitySC.PM.EME.Service");

            // When
            var result = serviveTypes.Should().NotHaveDependencyOn("UnitySC.PM.EME.Client").GetResult();

            //Assert
            var failingTypes = result.FailingTypeNames ?? new List<string>();
            Assert.IsTrue(result.IsSuccessful, $"List of failing types:\n{string.Join("\n", failingTypes)}");
        }

        [TestMethod]
        public void SharedDoesNotDependOnEmera()
        {
            // Given
            var sharedTypes = Types.InAssemblies(_assemblies).That().ResideInNamespaceContaining("UnitySC.Shared")
                .Or().ResideInNamespaceContaining("UnitySC.PM.Shared");

            // When
            var result = sharedTypes.Should().NotHaveDependencyOn("UnitySC.PM.EME").GetResult();

            //Assert
            var failingTypes = result.FailingTypeNames ?? new List<string>();
            Assert.IsTrue(result.IsSuccessful, $"List of failing types:\n{string.Join("\n", failingTypes)}");
        }

        [TestMethod]
        public void NoAnalyseTypes()
        {
            // Given
            var types = Types.InAssemblies(_assemblies);

            // When
            var result = types.Should().NotResideInNamespaceContaining("UnitySC.PM.ANA").GetResult();

            //Assert
            var failingTypes = result.FailingTypeNames ?? new List<string>();
            Assert.IsTrue(result.IsSuccessful, $"List of failing types:\n{string.Join("\n", failingTypes)}");
        }

        [TestMethod]
        public void NoDemeterTypes()
        {
            // Given
            var types = Types.InAssemblies(_assemblies);

            // When
            var result = types.Should().NotResideInNamespaceContaining("UnitySC.PM.DMT").GetResult();

            //Assert
            var failingTypes = result.FailingTypeNames ?? new List<string>();
            Assert.IsTrue(result.IsSuccessful, $"List of failing types:\n{string.Join("\n", failingTypes)}");
        }
    }
}
