using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace ADCCalibDMT
{
    public static class ExtensionMethods
    {
#if FDSYNC
        public static TResult SafeInvoke<T, TResult>(this T isi, Func<T, TResult> call) where T : ISynchronizeInvoke
        {
            if (isi.InvokeRequired)
            {
                IAsyncResult result = isi.BeginInvoke(call, new object[] { isi });
                object endResult = isi.EndInvoke(result);
                return (TResult)endResult;
            }
            else
            {
                return call(isi);
            }
        }

        public static void SafeInvoke<T>(this T isi, Action<T> call) where T : ISynchronizeInvoke
        {
            if (isi.InvokeRequired)
                isi.BeginInvoke(call, new object[] { isi });
            else
                call(isi);
        }
#else
        public static void SafeInvoke<T, TResult>(this T isi, Func<T, TResult> call) where T : ISynchronizeInvoke
        {
            if (isi.InvokeRequired)
            {
                IAsyncResult result = isi.BeginInvoke(call, new object[] { isi });
            }
            else
            {
                call(isi);
            }
        }

        public static void SafeInvoke<T>(this T isi, Action<T> call) where T : ISynchronizeInvoke
        {
            if (isi.InvokeRequired)
                isi.BeginInvoke(call, new object[] { isi });
            else
                call(isi);
        }

#endif
    }
}
