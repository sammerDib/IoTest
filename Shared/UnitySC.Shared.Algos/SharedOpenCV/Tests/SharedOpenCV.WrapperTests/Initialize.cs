using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedOpenCV.WrapperTests
{
    [TestClass]
    public class Initialize
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Console.WriteLine("AssemblyInitialize");
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }
    }
}
