using System.Text.RegularExpressions;

namespace UnitySC.Shared.Tools.Communication
{
    public struct SerialPortCommand<T>
    {
        public T Message;
        public Regex AcknowlegedResponsePattern;
    }
}
