using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

namespace Altaview
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
            {
                try
                {
                    isi.BeginInvoke(call, new object[] { isi });
                }
                catch (Exception)
                {

                }
            }
            else

                call(isi);
        }
#endif
        /// <summary>
        /// Displays the form as modal during a threadpool execution.
        /// Exceptions during threadpool execution are propagated to the caller.
        /// </summary>
        public static void ShowModalDuringThreadPool(this Form this_, Action during)
        {
            this_.Load += (object sender, EventArgs e) =>
            {
                ThreadPool.QueueUserWorkItem((Object ù) =>
                {
                    Exception duringEx = null;

                    try
                    {
                        during();
                    }
                    catch (Exception ex)
                    {
                        duringEx = ex;
                    }
                    finally
                    {
                        this_.BeginInvoke((Action)(() =>
                        {
                            this_.Close();

                            if (duringEx != null)
                            {
                                throw duringEx;
                            }
                        }));
                    }
                });
            };

            this_.ShowDialog();
        }
    }
}
