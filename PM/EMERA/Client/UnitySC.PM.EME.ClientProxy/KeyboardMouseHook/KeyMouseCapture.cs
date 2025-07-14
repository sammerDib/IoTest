using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace UnitySC.PM.EME.Client.Proxy.KeyboardMouseHook
{
    public static class KeyMouseCapture
    {
        [Flags]
        private enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        private const int WH_KEYBOARD = 2;
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;
        private static KeyMouseProc _keyProc = KeyHookCallback;
        private static KeyMouseProc _mouseProc = MouseHookCallback;

        public static DateTime LastKeyPressTime;

        public static void SetMouseHook()
        {
            _hookMouseID = SetMouseHook(_mouseProc);
        }

        public static void SetKeyboardHook()
        {
            _hookKeyID = SetKeyHook(_keyProc);
        }

        public static void KeyboardUnHook()
        {
            UnhookWindowsHookEx(_hookKeyID);
        }

        public static void MouseUnHook()
        {
            UnhookWindowsHookEx(_hookMouseID);
        }

        private static IntPtr SetKeyHook(KeyMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                //return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                return SetWindowsHookEx(WH_KEYBOARD, proc, (IntPtr)0,
                            GetCurrentThreadId());
            }
        }

        private static IntPtr SetMouseHook(KeyMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                //return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr _hookKeyID = IntPtr.Zero;
        private static IntPtr _hookMouseID = IntPtr.Zero;


        // When keyboard is handled, the key events are not transmitted
        public static bool IsKeyboardHandled { get; internal set; } = false;

        private delegate IntPtr KeyMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr KeyHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Feel free to move the const to a private field.
            const int HC_ACTION = 0;
            if (nCode == HC_ACTION)
            {
                var key = KeyInterop.KeyFromVirtualKey((int)wParam);
                //KeyEventArgs args = new KeyEventArgs(key);

                bool isKeyDown = ((ulong)lParam & 0x40000000) == 0;
                if (isKeyDown)
                {
                    //Console.WriteLine("Key Down : " + key);
                    ServiceLocator.KeyboardMouseHook.SetKeyDown(key);
                }
                else
                {
                    ServiceLocator.KeyboardMouseHook.SetKeyPressed(key);
                    bool isLastKeyUp = ((ulong)lParam & 0x80000000) == 0x80000000;
                    if (isLastKeyUp)
                    {
                        //Console.WriteLine("Key Up : " + key);
                        ServiceLocator.KeyboardMouseHook.SetKeyUp(key);
                    }

                }

                //Console.WriteLine($"Key Hook : {key}  , lParam : {lParam}");


            }

            if (IsKeyboardHandled)
                return new IntPtr(1);

            return CallNextHookEx(_hookKeyID, nCode, wParam, lParam);
        }

        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam || MouseMessages.WM_RBUTTONDOWN == (MouseMessages)wParam))
            {
                ServiceLocator.KeyboardMouseHook.SetMouseDown();
            }
            return CallNextHookEx(_hookMouseID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            KeyMouseProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetCurrentThreadId();
    }
}
