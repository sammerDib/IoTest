using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.DMT.Service.Test
{
    [TestClass]
    public class BaseTest
    {
        private Container _container;
        [TestInitialize]
        public void Init()
        {
            _container = new Container();
            Bootstrapper.Register(_container);
        }
    }
}
