using System;
using System.Linq;

using Agileo.MessageDataBus;
using Agileo.SemiDefinitions;

using UnitySC.GUI.Common.Vendor.Communication.Mdb;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.TagsSpy
{
    public static class TagsHelper
    {
        private static readonly TagTypeSwitch<Quality?> QualityGetter = new TagTypeSwitch<Quality?>()
            .Case<byte>(x => x?.Quality)
            .Case<bool>(x => x?.Quality)
            .Case<double>(x => x?.Quality)
            .Case<float>(x => x?.Quality)
            .Case<int>(x => x?.Quality)
            .Case<long>(x => x?.Quality)
            .Case<sbyte>(x => x?.Quality)
            .Case<short>(x => x?.Quality)
            .Case<string>(x => x?.Quality)
            .Case<uint>(x => x?.Quality)
            .Case<ulong>(x => x?.Quality)
            .Case<ushort>(x => x?.Quality);

        private static readonly TagTypeSwitch<string> PathGetter = new TagTypeSwitch<string>()
            .Case<byte>(x => x?.Path)
            .Case<bool>(x => x?.Path)
            .Case<double>(x => x?.Path)
            .Case<float>(x => x?.Path)
            .Case<int>(x => x?.Path)
            .Case<long>(x => x?.Path)
            .Case<sbyte>(x => x?.Path)
            .Case<short>(x => x?.Path)
            .Case<string>(x => x?.Path)
            .Case<uint>(x => x?.Path)
            .Case<ulong>(x => x?.Path)
            .Case<ushort>(x => x?.Path);

        private static readonly TagTypeSwitch<int?> ClientIdGetter = new TagTypeSwitch<int?>()
            .Case<byte>(x => x?.ClientID)
            .Case<bool>(x => x?.ClientID)
            .Case<double>(x => x?.ClientID)
            .Case<float>(x => x?.ClientID)
            .Case<int>(x => x?.ClientID)
            .Case<long>(x => x?.ClientID)
            .Case<sbyte>(x => x?.ClientID)
            .Case<short>(x => x?.ClientID)
            .Case<string>(x => x?.ClientID)
            .Case<uint>(x => x?.ClientID)
            .Case<ulong>(x => x?.ClientID)
            .Case<ushort>(x => x?.ClientID);

        private static readonly TagTypeSwitch<object> ValueGetter = new TagTypeSwitch<object>()
            .Case<byte>(x => x?.Value)
            .Case<bool>(x => x?.Value)
            .Case<double>(x => x?.Value)
            .Case<float>(x => x?.Value)
            .Case<int>(x => x?.Value)
            .Case<long>(x => x?.Value)
            .Case<sbyte>(x => x?.Value)
            .Case<short>(x => x?.Value)
            .Case<string>(x => x?.Value)
            .Case<uint>(x => x?.Value)
            .Case<ulong>(x => x?.Value)
            .Case<ushort>(x => x?.Value);

        public static Quality? GetQuality(this BaseTag tag) => QualityGetter.Switch(tag);

        public static string GetPath(this BaseTag tag) => PathGetter.Switch(tag);

        public static int? GetClientId(this BaseTag tag) => ClientIdGetter.Switch(tag);

        public static object GetValue(this BaseTag tag) => ValueGetter.Switch(tag);

        /// <summary>
        /// Attempts to assign the specified value to the tag.
        /// </summary>
        public static bool TrySetValue(this BaseTag tag, string value)
        {
            if (tag.ValueType.IsEnum)
            {
                var myEnumValue = Enum.Parse(tag.ValueType, value);
                tag.GetType().GetProperty(nameof(Tag<object>.Value))?.SetValue(tag, myEnumValue, null);
                return true;
            }

            return new TagTypeSwitch()
                .Case<byte>(x => x.Value = Convert.ToByte(value))
                .Case<bool>(x => x.Value = Convert.ToBoolean(value))
                .Case<double>(x => x.Value = Convert.ToDouble(value))
                .Case<float>(x => x.Value = Convert.ToSingle(value))
                .Case<int>(x => x.Value = Convert.ToInt32(value))
                .Case<long>(x => x.Value = Convert.ToInt64(value))
                .Case<sbyte>(x => x.Value = Convert.ToSByte(value))
                .Case<short>(x => x.Value = Convert.ToInt16(value))
                .Case<string>(x => x.Value = Convert.ToString(value))
                .Case<uint>(x => x.Value = Convert.ToUInt32(value))
                .Case<ulong>(x => x.Value = Convert.ToUInt64(value))
                .Case<ushort>(x => x.Value = Convert.ToUInt16(value))
                .Switch(tag);
        }

        public static void SetValue(this BaseTag tag, object value)
        {
            var valueType = value.GetType();

            if (tag.ValueType != valueType)
            {
                throw new InvalidOperationException(
                    $"Value type '{valueType}' and tag value type '{tag.ValueType}' differs.");
            }

            new TagTypeSwitch()
                .Case<byte>(x => x.Value = (byte)value)
                .Case<bool>(x => x.Value = (bool)value)
                .Case<double>(x => x.Value = (double)value)
                .Case<float>(x => x.Value = (float)value)
                .Case<int>(x => x.Value = (int)value)
                .Case<long>(x => x.Value = (long)value)
                .Case<sbyte>(x => x.Value = (sbyte)value)
                .Case<short>(x => x.Value = (short)value)
                .Case<string>(x => x.Value = (string)value)
                .Case<uint>(x => x.Value = (uint)value)
                .Case<ulong>(x => x.Value = (ulong)value)
                .Case<ushort>(x => x.Value = (ushort)value)
                .Switch(tag);
        }

        /// <summary>
        /// Returns true if the specified value can be assigned to the tag, otherwise false.
        /// </summary>
        public static bool CanBeSetWith(this BaseTag tag, string value)
        {
            if (value == null)
            {
                return false;
            }

            if (tag.ValueType == null)
            {
                return false;
            }

            if (tag.ValueType.IsEnum)
            {
                return Enum.GetNames(tag.ValueType)
                    .Any(
                        enumValue => string.Equals(
                            enumValue,
                            value,
                            StringComparison.InvariantCultureIgnoreCase));
            }

            return new TagTypeSwitch<bool>()
                .Case<byte>(x => byte.TryParse(value, out _))
                .Case<bool>(x => bool.TryParse(value, out _))
                .Case<double>(x => double.TryParse(value, out _))
                .Case<float>(x => float.TryParse(value, out _))
                .Case<int>(x => int.TryParse(value, out _))
                .Case<long>(x => long.TryParse(value, out _))
                .Case<sbyte>(x => sbyte.TryParse(value, out _))
                .Case<short>(x => short.TryParse(value, out _))
                .Case<string>(x => true)
                .Case<uint>(x => uint.TryParse(value, out _))
                .Case<ulong>(x => ulong.TryParse(value, out _))
                .Case<ushort>(x => ushort.TryParse(value, out _))
                .Switch(tag);
        }
    }
}
