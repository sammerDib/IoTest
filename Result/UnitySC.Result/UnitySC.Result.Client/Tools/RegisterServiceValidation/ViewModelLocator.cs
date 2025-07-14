using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.Shared.Tools;

namespace WpfUnityControlRegisterValidation
{
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance { get; private set; }
        public MainRegisterVM MainRegisterVM => ClassLocator.Default.GetInstance<MainRegisterVM>();

        public ViewModelLocator()
        {
            Instance = this;
        }

        public static void Register()
        {
            ClassLocator.Default.Register<MainRegisterVM>(true);
        }
    }
}
