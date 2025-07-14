using System;

namespace VersionSwitcher
{
    public class PasswordCheckEventArgs: EventArgs
    {
        public bool IsPasswordValid { get; set; }

        public PasswordCheckEventArgs(bool isPasswordValid)
        {
            IsPasswordValid = isPasswordValid;
        }
    }
}
