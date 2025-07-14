using System;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.CommandConstants
{
    /// <summary>
    ///     Define all constants used by GWID and SWID commands of RE201 LoadPort.
    ///     A carrier type is defined as "AUTO", "CST", ... in RE201 documentation.
    ///     However, by doing tests on real machine and on emulator, HW is not following that documentation.
    ///     According to InfoPads results, the returned carrier type could be "CST0", "CST1", ..., "CSTF".
    ///     Here is the list of carriers type existing in RE201 configuration (retrieved from RE201 maintenance mode software):
    ///     - "AUTO"
    ///     - "CST"
    /// </summary>
    public class CarrierTypeConstants
    {
        public const string Auto = "AUTO";
        public const string NotIdentified = "----";

        #region Prefixes

        public const string Cassette = "CST";

        #endregion Prefixes

        #region Indexing

        /// <summary>
        ///     Special carrier indexing is from <see cref="SpecialCarrierStartIndex" /> to <see cref="SpecialCarrierStopIndex" />
        ///     and is not numeric.
        /// </summary>
        public const char SpecialCarrierStartIndex = 'A';

        /// <summary>
        ///     Special carrier indexing is from <see cref="SpecialCarrierStartIndex" /> to <see cref="SpecialCarrierStopIndex" />
        ///     and is not numeric.
        /// </summary>
        public const char SpecialCarrierStopIndex = 'F';

        public const int NbCassetteTypes = 15;

        #endregion Indexing

        #region Helpers

        /// <summary>
        ///     Returns a <see cref="CarrierType" /> and it's configuration index, if any.
        ///     If there is no index contained in <paramref name="carrierType" />, return null instead of int.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if <paramref name="carrierType" /> lenght is not 4.</exception>
        /// <exception cref="ArgumentException">
        ///     if <paramref name="carrierType" /> is not recognized (invalid carrier type
        ///     prefix).
        /// </exception>
        public static Tuple<CarrierType, uint?> ToCarrierTypeAndIndex(string carrierType)
        {
            if (carrierType.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(carrierType), carrierType,
                    @"Given string must have a 4 characters length.");
            }

            if (carrierType.Equals(Auto))
            {
                return new Tuple<CarrierType, uint?>(CarrierType.Auto, null);
            }

            if (carrierType.Equals(NotIdentified))
            {
                return new Tuple<CarrierType, uint?>(CarrierType.NotIdentified, null);
            }

            if (carrierType.StartsWith(Cassette))
            {
                if (uint.TryParse(carrierType[3].ToString(), out var index))
                {
                    return new Tuple<CarrierType, uint?>(CarrierType.Cassette, index);
                }

                // Special carrier index is a character from 'A' to 'F' and starts from 10
                var specialIndex = (uint)carrierType[3] - SpecialCarrierStartIndex + 10;
                return new Tuple<CarrierType, uint?>(CarrierType.Cassette, specialIndex);
            }

            throw new ArgumentException($@"Given string is not recognized. Value=""{carrierType}""",
                nameof(carrierType));
        }

        /// <summary>
        ///     Do the reverse operation of <see cref="ToCarrierTypeAndIndex" />.
        /// </summary>
        public static string ToString(CarrierType carrierType, uint? carrierTypeIndex)
        {
            switch (carrierType)
            {
                case CarrierType.Auto:
                    return Auto;

                case CarrierType.NotIdentified:
                    return NotIdentified;

                case CarrierType.Cassette:
                    if (carrierTypeIndex != null)
                    {
                        if (carrierTypeIndex < 10)
                        {
                            return Cassette + carrierTypeIndex;
                        }

                        return Cassette + (char)(carrierTypeIndex + SpecialCarrierStartIndex - 1);
                    }

                    return Cassette;

                case CarrierType.Special:
                    if (carrierTypeIndex != null)
                    {
                        if (carrierTypeIndex < 10)
                        {
                            return Cassette + carrierTypeIndex;
                        }

                        return Cassette + (char)(carrierTypeIndex + SpecialCarrierStartIndex - 1);
                    }

                    return Cassette;

                default:
                    throw new ArgumentOutOfRangeException(nameof(carrierType), carrierType, null);
            }
        }

        #endregion Helpers
    }
}
