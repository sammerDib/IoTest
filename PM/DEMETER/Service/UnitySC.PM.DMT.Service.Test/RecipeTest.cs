using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Test
{
    [TestClass]
    public class RecipeTest : BaseTest
    {
        IDMTRecipeService Service => ClassLocator.Default.GetInstance<IDMTRecipeService>();


        [TestMethod]
        public void BasicTest()
        {
            Service.Test();
        }
    }
}
