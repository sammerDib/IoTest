using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.Shared;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Configuration.Test
{
    [TestClass]
    public class HardwareConfigurationTest
    {

        [TestMethod]
        [DynamicData(nameof(TestCases), DynamicDataSourceType.Method)]
        public void IsParseable(string configFile)
        {
            Assert.IsTrue(File.Exists(configFile),
                $"{AnaHardwareManager.HardwareConfigurationFileName} does not exist as expected following path: {configFile}"
            );
            var configuration = XML.Deserialize<AnaHardwareConfiguration>(configFile);
            Assert.IsNotNull(configuration);
        }

        public static IEnumerable<object[]> TestCases()
        {
            return Directory
                .GetDirectories(ConfigurationAndCalibrationPath)
                .Where(toolConfigPath =>
                    !toolConfigPath.EndsWith(CurrentProjectName) &&
                    !toolConfigPath.EndsWith(XYCalibrationRecipesPath))
                .Select(toolConfigPath => new
                {
                    ToolName = Path.GetFileName(toolConfigPath),
                    FilePath = Path.Combine(toolConfigPath,
                            PMServiceConfigurationManager.ConfigurationFolderName,
                            AnaHardwareManager.HardwareConfigurationFileName
                    )
                })
                .Select(config => new object[] { config.FilePath });
        }

        private static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if USE_ANYCPU
        private static string ConfigurationAndCalibrationPath => Path.Combine(AssemblyDirectory, "..", "..", "..");
#else
        private static string ConfigurationAndCalibrationPath => Path.Combine(AssemblyDirectory, "..", "..", "..", "..");
#endif
        private static string CurrentProjectName => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        private static string XYCalibrationRecipesPath => "XYCalibrationRecipes";
    }
}
