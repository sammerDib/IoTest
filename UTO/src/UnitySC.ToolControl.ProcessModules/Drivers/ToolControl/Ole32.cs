using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl
{
    [ComVisible(false)]
    public class Ole32
    {
        #region Public Fields

        public const int ROTFLAGS_ALLOWANYCLIENT = 0x1;

        #endregion Public Fields

        /// Returns a pointer to an implementation of IBindCtx (a bind context object).
        /// This object stores information about a particular moniker-binding operation.

        #region Public Methods

        public static void Check(uint hr)
        {
            if (!Succeeded(hr))
            {
                throw new COMException(Marshal.GetExceptionForHR((int)hr).Message, (int)hr);
            }
        }

        [DllImport("ole32.dll")]
        public static extern uint CreateBindCtx(uint reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
        public static extern uint CreateItemMoniker(string delim, string item, out IMoniker pMoniker);

        /// Returns a pointer to the IRunningObjectTable
        /// interface on the local running object table (ROT).
        [DllImport("ole32.dll")]
        public static extern uint GetRunningObjectTable(uint reserved,
            out IRunningObjectTable pprot);

        [DllImport("oleaut32.dll")]
        public static extern uint RegisterActiveObject([MarshalAs(UnmanagedType.IUnknown)] object punk, ref Guid rclsid,
            uint dwFlags, out int pdwRegister);

        public static bool Succeeded(uint hr) => (hr & 0x80000000) == 0;

        #endregion Public Methods
    }
}
