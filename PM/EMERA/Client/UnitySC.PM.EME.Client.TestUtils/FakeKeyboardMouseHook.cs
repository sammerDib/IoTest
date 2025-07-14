using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using UnitySC.PM.EME.Client.Proxy.KeyboardMouseHook;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeKeyboardMouseHook : IKeyboardMouseHook
    {
        public event EventHandler<KeyGlobalEventArgs> KeyUpDownEvent;
        public event EventHandler<KeyGlobalEventArgs> KeyPressedEvent;
        public event EventHandler MouseDownEvent;

        public void HandleKeyBoard()
        {
            throw new NotImplementedException();
        }

        public void SetKeyDown(Key keyDown)
        {
            throw new NotImplementedException();
        }

        public void SetKeyPressed(Key keyPressed)
        {
            throw new NotImplementedException();
        }

        public void SetKeyUp(Key keyUp)
        {
            throw new NotImplementedException();
        }

        public void SetMouseDown()
        {
            throw new NotImplementedException();
        }

        public void StartCaptureMouseDown()
        {
            throw new NotImplementedException();
        }

        public void StopCaptureMouseDown()
        {
            throw new NotImplementedException();
        }

        public void UnhandleKeyBoard()
        {
            throw new NotImplementedException();
        }
    }
}
