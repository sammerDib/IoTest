using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    [TestClass]
    public class EmeResultTypeConverterTests
    {
        [TestMethod]
        [DataRow(EMELightType.DirectionalDarkField0Degree, EMEFilter.NoFilter, ResultType.EME_Visible0)]
        [DataRow(EMELightType.DirectionalDarkField90Degree, EMEFilter.NoFilter, ResultType.EME_Visible90)]
        public void ShouldGetResultTypeFromFilter(EMELightType lightType, EMEFilter value, ResultType expected)
        {
            Assert.AreEqual(expected, EmeResultTypeConverter.GetResultTypeFromFilterAndLight(value, lightType));
        }
    }
}
