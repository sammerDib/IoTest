using System.Text.RegularExpressions;

namespace UnitySC.Shared.Tools.Communication
{
    public struct SerialPortQuery<T>
    {
        public T Message;
        public Regex ResponsePattern;
    }
}
