using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.ANA.Client.CommonUI
{
    /// <summary>
    /// Logique d'interaction pour RecipeEditor.xaml
    /// </summary>
    [Export(typeof(IPmInit))]
    [UCMetadata(ActorType = ActorType.ANALYSE)]
    class PmInit : IPmInit
    {
 
        public ActorType ActorType => ActorType.ANALYSE;


        private static IntPtr _hookID = IntPtr.Zero;

        public void BootStrap()
        {
            Bootstrapper.Register();

            // _hookID = SetHook(_proc);

            



        }


        public void Init(bool isStandalone)
        {
            throw new NotImplementedException();
        }


        //private static IntPtr SetHook(LowLevelKeyboardProc proc)

        //{

        //    using (Process curProcess = Process.GetCurrentProcess())

        //    using (ProcessModule curModule = curProcess.MainModule)

        //    {

        //        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,

        //            GetModuleHandle(curModule.ModuleName), 0);

        //    }

        //}


    }
}
