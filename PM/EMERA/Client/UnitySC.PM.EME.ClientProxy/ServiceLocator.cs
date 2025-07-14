using UnitySC.PM.EME.Client.Proxy.KeyboardMouseHook;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Proxy
{
    public class ServiceLocator
    {
        public static IKeyboardMouseHook KeyboardMouseHook => ClassLocator.Default.GetInstance<IKeyboardMouseHook>();
    }
}
