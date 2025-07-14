using System.Runtime.InteropServices;
using System.Security;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.SystemInformation
{
    public static class OsVersionHelper
    {
        /// <summary>
        /// https://docs.microsoft.com/fr-fr/windows-hardware/drivers/ddi/wdm/ns-wdm-_osversioninfoexw
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct OsVersionInfoEx
        {
            internal uint dwOSVersionInfoSize;
            internal uint dwMajorVersion;
            internal uint dwMinorVersion;
            internal uint dwBuildNumber;
            internal uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal string szCSDVersion;
            internal ushort wServicePackMajor;
            internal ushort wServicePackMinor;
            internal ushort wSuiteMask;
            internal ProductType wProductType;
            internal byte wReserved;
        }

        private enum ProductType : byte
        {
            /// <summary>
            /// The operating system is Windows 10, Windows 8, Windows 7,...
            /// </summary>
            /// <remarks>VER_NT_WORKSTATION</remarks>
            Workstation = 0x0000001,
            /// <summary>
            /// The system is a domain controller and the operating system is Windows Server.
            /// </summary>
            /// <remarks>VER_NT_DOMAIN_CONTROLLER</remarks>
            DomainController = 0x0000002,
            /// <summary>
            /// The operating system is Windows Server. Note that a server that is also a domain controller
            /// is reported as VER_NT_DOMAIN_CONTROLLER, not VER_NT_SERVER.
            /// </summary>
            /// <remarks>VER_NT_SERVER</remarks>
            Server = 0x0000003
        }


        // https://docs.microsoft.com/en-us/windows-hardware/drivers/ddi/wdm/nf-wdm-rtlgetversion
        [SecurityCritical]
        [DllImport("ntdll.dll", EntryPoint = "RtlGetVersion", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int RtlGetVersion(ref OsVersionInfoEx lpVersionInformation);

        public static WindowsVersions GetWindowsVersion()
        {
            var osvi = new OsVersionInfoEx
            {
                dwOSVersionInfoSize = (uint)Marshal.SizeOf(typeof(OsVersionInfoEx))
            };

            if (RtlGetVersion(ref osvi) != 0)
                return WindowsVersions.Unknown;

            if (osvi.dwMajorVersion == 10 && osvi.dwMinorVersion == 0 && osvi.wProductType == ProductType.Workstation)
                return WindowsVersions.Windows10;

            if (osvi.dwMajorVersion == 10 && osvi.dwMinorVersion == 0 && osvi.wProductType == ProductType.Server)
                return WindowsVersions.WindowsServer2016Or2019;

            if (osvi.dwMajorVersion == 6 && osvi.dwMinorVersion == 3 && osvi.wProductType == ProductType.Server)
                return WindowsVersions.WindowsServer2012R2;

            if (osvi.dwMajorVersion == 6 && osvi.dwMinorVersion == 3 && osvi.wProductType == ProductType.Workstation)
                return WindowsVersions.Windows81;

            if (osvi.dwMajorVersion == 6 && osvi.dwMinorVersion == 2 && osvi.wProductType == ProductType.Workstation)
                return WindowsVersions.Windows8;

            if (osvi.dwMajorVersion == 6 && osvi.dwMinorVersion == 2 && osvi.wProductType == ProductType.Server)
                return WindowsVersions.WindowsServer2012;

            if (osvi.dwMajorVersion == 6 && osvi.dwMinorVersion == 1 && osvi.wProductType == ProductType.Workstation)
                return WindowsVersions.Windows7;

            if (osvi.dwMajorVersion == 6 && osvi.dwMinorVersion == 1 && osvi.wProductType == ProductType.Server)
                return WindowsVersions.WindowsServer2008R2;

            if (osvi.dwMajorVersion == 6 && osvi.dwMinorVersion == 0 && osvi.wProductType == ProductType.Server)
                return WindowsVersions.WindowsServer2008;

            if (osvi.dwMajorVersion == 6 && osvi.dwMinorVersion == 0 && osvi.wProductType == ProductType.Workstation)
                return WindowsVersions.WindowsVista;

            return WindowsVersions.Unknown;
        }

        public enum WindowsVersions
        {
            Unknown,
            /// <summary>
            /// 6.0
            /// </summary>
            WindowsVista,
            /// <summary>
            /// 6.0
            /// </summary>
            WindowsServer2008,
            /// <summary>
            /// 6.1
            /// </summary>
            Windows7,
            /// <summary>
            /// 6.1
            /// </summary>
            WindowsServer2008R2,
            /// <summary>
            /// 6.2
            /// </summary>
            Windows8,
            /// <summary>
            /// 6.2
            /// </summary>
            WindowsServer2012,
            /// <summary>
            /// 6.3
            /// </summary>
            Windows81,
            /// <summary>
            /// 6.3
            /// </summary>
            WindowsServer2012R2,
            /// <summary>
            /// 10.0
            /// </summary>
            Windows10,
            /// <summary>
            /// 10.0
            /// </summary>
            WindowsServer2016Or2019
        }
    }
}
