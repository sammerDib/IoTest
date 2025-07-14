using System;
using System.Windows.Forms;

namespace Common.WinForms
{
    public class MainForm
    {
        /// <summary>
        /// Main form. Thread safe.
        /// Should be set at main form creation.
        /// </summary>
        public static volatile Form Instance;

        /// <summary>
        /// Invokes an action in the GUI thread.
        /// </summary>
        public static void InvokeInlineIfMessageLoopRunning(Action a)
        {
            Instance?.InvokeInlineIfMessageLoopRunning(a);
        }
    }
}
