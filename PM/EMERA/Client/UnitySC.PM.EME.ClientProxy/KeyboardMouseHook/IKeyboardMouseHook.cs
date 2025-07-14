using System;
using System.Windows.Input;

namespace UnitySC.PM.EME.Client.Proxy.KeyboardMouseHook
{
    public class KeyGlobalEventArgs : EventArgs
    {
        public KeyGlobalEventArgs(Key currentKey, bool isKeyDown)
        {
            CurrentKey = currentKey;
            IsKeyDown = isKeyDown;
        }

        public Key CurrentKey { get; }
        public bool IsKeyDown { get; }
    }

    public interface IKeyboardMouseHook
    {
        event EventHandler<KeyGlobalEventArgs> KeyUpDownEvent;
        event EventHandler<KeyGlobalEventArgs> KeyPressedEvent;

        event EventHandler MouseDownEvent;

        void SetKeyUp(Key keyUp);

        void SetKeyDown(Key keyDown);

        void SetKeyPressed(Key keyPressed);

        // When keyboard is handled, the keyboard events are not transmitted
        void HandleKeyBoard();

        void UnhandleKeyBoard();

        void StartCaptureMouseDown();

        void StopCaptureMouseDown();

        void SetMouseDown();
    }
}
