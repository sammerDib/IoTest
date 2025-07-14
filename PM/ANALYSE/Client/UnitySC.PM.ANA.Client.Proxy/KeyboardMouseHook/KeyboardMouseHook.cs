using System;
using System.Windows.Input;

namespace UnitySC.PM.ANA.Client.Proxy.KeyboardMouseHook
{
    public class KeyboardMouseHook : IKeyboardMouseHook
    {
        public event EventHandler<KeyGlobalEventArgs> KeyUpDownEvent;

        public event EventHandler MouseDownEvent;
        public event EventHandler<KeyGlobalEventArgs> KeyPressedEvent;

        public KeyboardMouseHook()
        {
            KeyMouseCapture.SetKeyboardHook();
        }

        public void HandleKeyBoard()
        {
            KeyMouseCapture.IsKeyboardHandled = true;
        }

        public void UnhandleKeyBoard()
        {
            KeyMouseCapture.IsKeyboardHandled = false;
        }

        public void SetKeyDown(Key keyDown)
        {
            KeyUpDownEvent?.Invoke(this, new KeyGlobalEventArgs(keyDown, true));
        }

        public void SetKeyUp(Key keyUp)
        {
            KeyUpDownEvent?.Invoke(this, new KeyGlobalEventArgs(keyUp, false));
        }

        public void SetKeyPressed(Key keyPressed)
        {
            KeyPressedEvent?.Invoke(this, new KeyGlobalEventArgs(keyPressed, false));
        }

        public void SetMouseDown()
        {
            MouseDownEvent?.Invoke(this,null);
        }

        public void StartCaptureMouseDown()
        {
            KeyMouseCapture.SetMouseHook();
        }

        public void StopCaptureMouseDown()
        {
            KeyMouseCapture.MouseUnHook();
        }
    }
}
