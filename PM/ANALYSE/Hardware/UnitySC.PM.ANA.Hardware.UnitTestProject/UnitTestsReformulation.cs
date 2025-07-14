using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTestsReformulation
    {
        private const string OPTIONAL_PRESENT = "DEFAULT USER CONTENT";
        private const string STAGE_ERROR01_USERCONTENT = "EXPLICITATION DE L'ERREUR ERROR01-STAGE";
        private const string NOT_OPTIONAL_PRESENT = "REFORMULATION";
        private const string EMPTY_STRING = "";
        private const string LISE_ERROR_WILDCARD = "EXPLICATION DE L'ERREUR LISE-ERROR WITH PARAM DUMMY";
        private const string STAGE_ERROR_WILDCARD = "EXPLICATION DE L'ERREUR STAGE-ERROR WITH PARAM 1 AND 2";
        private const string LISE_ERROR_FORMAT = "EXPLICATION DE L'ERREUR LISE-ERROR FORMAT {0}";
        private const string LISE_ERROR_FORMAT_MULTIPLE = "EXPLICATION DE L'ERREUR LISE-ERROR FORMAT MULTIPLE TEST_1, TEST_2, {42}";
        private const string LISE_ERROR_PERMUTATION = "EXPLICATION DE L'ERREUR LISE-TEST PERMUTATION PARAM2 ET PARAM1";
        private const string LISE_ERROR_WITH_SPACE = "EXPLICATION DE L'ERREUR LISE-TEST WITH SPACE PARAM1, PARAM2, PARAM3";

        [TestInitialize]
        public void Init()
        {

            // Configuration
            var currentConfiguration = new FakeConfigurationManager();

            // Init logger
            SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);

            ReformulationMessageManager.Init("ReformulationTest.xml",Mock.Of<ILogger>());
        }

        [TestMethod]
        public void TestFindOneRecord()
        {
            Assert.AreEqual(MessageLevel.Warning, ReformulationMessageManager.GetLevel("Stage", "Error01"));
            Assert.AreEqual(MessageLevel.Warning, ReformulationMessageManager.GetLevel("Stage", "Error01", MessageLevel.Question));
            Assert.AreEqual(MessageLevel.None, ReformulationMessageManager.GetLevel("NotExisting", "Error01"));
            Assert.AreEqual(MessageLevel.Information, ReformulationMessageManager.GetLevel("NotExisting", "Error01", MessageLevel.Information));
            Assert.AreEqual(STAGE_ERROR01_USERCONTENT, ReformulationMessageManager.GetUserContent("Stage", "Error01"));
            Assert.IsNull(ReformulationMessageManager.GetUserContent("NotExisting", "Error01"));
            Assert.AreEqual(OPTIONAL_PRESENT, ReformulationMessageManager.GetUserContent("NotExisting", "Error01", OPTIONAL_PRESENT));
            Assert.AreEqual(EMPTY_STRING, ReformulationMessageManager.GetUserContent("Stage", "Error02", OPTIONAL_PRESENT));
            Assert.AreEqual(EMPTY_STRING, ReformulationMessageManager.GetUserContent("Stage", "Error02"));
        }

        [TestMethod]
        public void TestFindWithWildCard()
        {
            Assert.AreEqual(MessageLevel.Error, ReformulationMessageManager.GetLevel("Lise", "ERROR WITH PARAM DUMMY"));
            Assert.AreEqual(LISE_ERROR_WILDCARD, ReformulationMessageManager.GetUserContent("Lise", "ERROR WITH PARAM DUMMY"));
            Assert.IsNull(ReformulationMessageManager.GetUserContent("Lise", "ERROR WITH PARAM"));
            Assert.AreEqual(STAGE_ERROR_WILDCARD, ReformulationMessageManager.GetUserContent("Stage", "ERROR WITH PARAM 1 AND 2"));
            Assert.AreEqual(LISE_ERROR_FORMAT, ReformulationMessageManager.GetUserContent("Lise", "ERROR FORMAT"));
            Assert.AreEqual(LISE_ERROR_FORMAT_MULTIPLE, ReformulationMessageManager.GetUserContent("Lise", "ERROR FORMAT MULTIPLE TEST_1 AND TEST_2"));
            Assert.AreEqual(LISE_ERROR_PERMUTATION, ReformulationMessageManager.GetUserContent("Lise", "TEST PERMUTATION PARAM1 AND PARAM2"));
            Assert.AreEqual(LISE_ERROR_WITH_SPACE, ReformulationMessageManager.GetUserContent("Lise", "TEST WITH SPACE PARAM1, PARAM2 AND PARAM3"));
        }
    }
}
