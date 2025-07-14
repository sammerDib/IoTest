using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnitySC.Shared.Tools.Collection
{
    public class NaturalStringComparer : IComparer<string>
    {
        internal static class NativeMethods
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            public static extern int StrCmpLogicalW(string psz1, string psz2);
        }

        private readonly int _modifier = 1;

        public NaturalStringComparer() : this(false)
        {
        }

        public NaturalStringComparer(bool descending)
        {
            if (descending) _modifier = -1;
        }

        public int Compare(string a, string b)
        {
            return NativeMethods.StrCmpLogicalW(a ?? "", b ?? "") * _modifier;
        }
    }
}