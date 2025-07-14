using System;
using System.Runtime.InteropServices;

namespace UnitySC.Shared.Tools
{
    public class ShellFileOperation
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
        private struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public FO_Func wFunc;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;

            public FILEOP_FLAGS fFlags;

            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;

            public IntPtr hNameMappings;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        }

        private enum FO_Func : uint
        {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004,
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHFileOperation([In, Out] ref SHFILEOPSTRUCT lpFileOp);

        [Flags]
        private enum FILEOP_FLAGS : ushort
        {
            FOF_MULTIDESTFILES = 0x0001,
            FOF_CONFIRMMOUSE = 0x0002,
            FOF_SILENT = 0x0004,  // don't create progress/report
            FOF_RENAMEONCOLLISION = 0x0008,
            FOF_NOCONFIRMATION = 0x0010,  // Don't prompt the user.
            FOF_WANTMAPPINGHANDLE = 0x0020,  // Fill in SHFILEOPSTRUCT.hNameMappings

            // Must be freed using SHFreeNameMappings
            FOF_ALLOWUNDO = 0x0040,

            FOF_FILESONLY = 0x0080,  // on *.*, do only files
            FOF_SIMPLEPROGRESS = 0x0100,  // means don't show names of files
            FOF_NOCONFIRMMKDIR = 0x0200,  // don't confirm making any needed dirs
            FOF_NOERRORUI = 0x0400,  // don't put up error UI
            FOF_NOCOPYSECURITYATTRIBS = 0x0800,  // dont copy NT file Security Attributes
            FOF_NORECURSION = 0x1000,  // don't recurse into directories.
            FOF_NO_CONNECTED_ELEMENTS = 0x2000,  // don't operate on connected elements.
            FOF_WANTNUKEWARNING = 0x4000,  // during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
            FOF_NORECURSEREPARSE = 0x8000,  // treat reparse points as objects, not containers
        }

        private static string StringArrayToMultiString(string[] stringArray)
        {
            string multiString = "";

            if (stringArray == null)
                return "";

            for (int i = 0; i < stringArray.Length; i++)
                multiString += stringArray[i] + '\0';

            multiString += '\0';

            return multiString;
        }

        public static bool Copy(string source, string dest)
        {
            return Copy(new string[] { source }, new string[] { dest });
        }

        public static bool Copy(string[] source, string[] dest)
        {
            var FileOpStruct = new SHFILEOPSTRUCT
            {
                hwnd = IntPtr.Zero,
                wFunc = FO_Func.FO_COPY,

                pFrom = StringArrayToMultiString(source),
                pTo = StringArrayToMultiString(dest),

                fFlags = 0,
                lpszProgressTitle = "",
                fAnyOperationsAborted = false,
                hNameMappings = IntPtr.Zero
            };

            int retval = SHFileOperation(ref FileOpStruct);

            if (retval != 0) return false;
            return true;
        }

        public static bool Move(string source, string dest)
        {
            return Move(new string[] { source }, new string[] { dest });
        }

        public static bool Delete(string file, bool silent)
        {
            var FileOpStruct = new SHFILEOPSTRUCT
            {
                hwnd = IntPtr.Zero,
                wFunc = FO_Func.FO_DELETE,

                pFrom = StringArrayToMultiString(new string[] { file }),
                pTo = null
            };

            if (silent)
                FileOpStruct.fFlags = FILEOP_FLAGS.FOF_SILENT | FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_NOERRORUI;
            else
                FileOpStruct.fFlags = 0;
            FileOpStruct.lpszProgressTitle = "";
            FileOpStruct.fAnyOperationsAborted = false;
            FileOpStruct.hNameMappings = IntPtr.Zero;

            int retval = SHFileOperation(ref FileOpStruct);

            if (retval != 0) return false;
            return true;
        }

        public static bool Move(string[] source, string[] dest)
        {
            var FileOpStruct = new SHFILEOPSTRUCT
            {
                hwnd = IntPtr.Zero,
                wFunc = FO_Func.FO_MOVE,

                pFrom = StringArrayToMultiString(source),
                pTo = StringArrayToMultiString(dest),

                fFlags = 0,
                lpszProgressTitle = "",
                fAnyOperationsAborted = false,
                hNameMappings = IntPtr.Zero
            };

            int retval = SHFileOperation(ref FileOpStruct);

            if (retval != 0) return false;
            return true;
        }
    }
}