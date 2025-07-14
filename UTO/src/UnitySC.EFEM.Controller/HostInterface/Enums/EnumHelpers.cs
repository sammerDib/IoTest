using System;

namespace UnitySC.EFEM.Controller.HostInterface.Enums
{
    public static class EnumHelpers
    {
        public static bool TryParseEnumValue<TEnum>(string value, out TEnum result)
        {
            if (!int.TryParse(value, out int intvalue))
            {
                result = default(TEnum);
                return false;
            }

            if (!Enum.IsDefined(typeof(TEnum), intvalue))
            {
                result = default(TEnum);
                return false;
            }

            result = (TEnum)Enum.Parse(typeof(TEnum), value);
            return true;
        }
    }
}
