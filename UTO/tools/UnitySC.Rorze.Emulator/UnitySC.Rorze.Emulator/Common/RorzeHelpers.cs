using System;
using System.Linq;

namespace UnitySC.Rorze.Emulator.Common
{
    public static class RorzeHelpers
    {
        public static char RequestedSpeedToRorzeSpeed(int input)
        {
            var rorzeSpeed = input / 5;
            if (rorzeSpeed >= 0 && rorzeSpeed <= 9)
            {
                return (char)('0' + rorzeSpeed);
            }
            else
            {
                return (char)(rorzeSpeed + 'A' - 0xA);
            }
        }

        public static string[] GetCommandParameter(string command)
        {
            var indexOfParameters = command.IndexOf('(') + 1;
            var parameters = command.Substring(
                indexOfParameters,
                command.Length - indexOfParameters - 2);
            return parameters.Split(',');
        }

        public static byte[] StringToByteArray(string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
