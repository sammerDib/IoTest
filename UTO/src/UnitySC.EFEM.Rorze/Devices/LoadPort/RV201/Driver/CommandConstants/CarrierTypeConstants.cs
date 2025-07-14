using System;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.CommandConstants
{
    /// <summary>
    /// Define all constants used by GWID and SWID commands of RV201 LoadPort.
    /// A carrier type is defined as "AUTO", "FOUP", "FOSB", ... in RV201 documentation.
    /// However, by doing tests on real machine and on emulator, HW is not following that documentation.
    /// According to InfoPads results, the returned carrier type could be "FUP1", "FUP2", ..., "FUP7".
    /// Here is the list of carriers type existing in RV201 configuration (retrieved from RV201 maintenance mode software):
    ///     - "AUTO"
    ///     - FOUPs: "FUP1", "FUP2", ..., "FUP7"
    ///     - FosBs: "FSB1", "FSB2", ..., "FSB5"
    ///     - Open carriers: "OCP1", "OCP2", "OCP3"
    ///     - "FPO1"
    ///     - "CSTG", "CSTH", ..., "CSTV"
    /// </summary>
    public class CarrierTypeConstants
    {
        public const string Auto          = "AUTO";
        public const string NotIdentified = "----";

        #region Prefixes

        public const string Foup             = "FUP";
        public const string FosB             = "FSB";
        public const string OpenCassette     = "OCP";
        public const string Foup1Slot        = "FPO";
        public const string CarrierOnAdapter = "CAS";

        // TODO: check that Special carriers are defined only as it
        // TODO: (what about OPNx and CASx saw obtained when looking at RV201 data by carrier table (stored in emulator)?)
        public const string SpecialCarrier = "CST";

        #endregion Prefixes

        #region Indexing

        public const int NbFoupTypes             = 7;
        public const int NbFosBTypes             = 5;
        public const int NbOpenCassetteTypes     = 3;
        public const int NbFoup1SlotTypes        = 1;
        public const int NbCarrierOnAdapterTypes = 4;

        /// <summary>
        /// Special carrier indexing is from <see cref="SpecialCarrierStartIndex"/> to <see cref="SpecialCarrierStopIndex"/> and is not numeric.
        /// </summary>
        public const char SpecialCarrierStartIndex = 'G';

        /// <summary>
        /// Special carrier indexing is from <see cref="SpecialCarrierStartIndex"/> to <see cref="SpecialCarrierStopIndex"/> and is not numeric.
        /// </summary>
        public const char SpecialCarrierStopIndex = 'V';

        #endregion Indexing

        #region Helpers

        /// <summary>
        /// Returns a <see cref="CarrierType"/> and it's configuration index, if any.
        /// If there is no index contained in <paramref name="carrierType"/>, return null instead of int.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if <paramref name="carrierType"/> lenght is not 4.</exception>
        /// <exception cref="ArgumentException"> if <paramref name="carrierType"/> is not recognized (invalid carrier type prefix).</exception>
        public static Tuple<CarrierType, uint?> ToCarrierTypeAndIndex(string carrierType)
        {
            if (carrierType.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(carrierType), carrierType, @"Given string must have a 4 characters length.");

            if (carrierType.Equals(Auto))
                return new Tuple<CarrierType, uint?>(CarrierType.Auto, null);

            if (carrierType.Equals(NotIdentified))
                return new Tuple<CarrierType, uint?>(CarrierType.NotIdentified, null);

            if (carrierType.StartsWith(Foup))
                return new Tuple<CarrierType, uint?>(CarrierType.FOUP, uint.Parse(carrierType[3].ToString()));

            if (carrierType.StartsWith(FosB))
                return new Tuple<CarrierType, uint?>(CarrierType.FOSB, uint.Parse(carrierType[3].ToString()));

            if (carrierType.StartsWith(OpenCassette))
                return new Tuple<CarrierType, uint?>(CarrierType.OpenCassette, uint.Parse(carrierType[3].ToString()));

            if (carrierType.StartsWith(Foup1Slot))
                return new Tuple<CarrierType, uint?>(CarrierType.Foup1Slot, uint.Parse(carrierType[3].ToString()));

            if (carrierType.StartsWith(CarrierOnAdapter))
                return new Tuple<CarrierType, uint?>(CarrierType.CarrierOnAdapter, uint.Parse(carrierType[3].ToString()));

            if (carrierType.StartsWith(SpecialCarrier))
            {
                // Special carrier index is a character from 'K' to 'V'
                uint index = (uint) carrierType[3] - SpecialCarrierStartIndex + 1;
                return new Tuple<CarrierType, uint?>(CarrierType.Special, index);
            }

            throw new ArgumentException($@"Given string is not recognized. Value=""{carrierType}""", nameof(carrierType));
        }

        /// <summary>
        /// Do the reverse operation of <see cref="ToCarrierTypeAndIndex"/>.
        /// </summary>
        public static string ToString(CarrierType carrierType, uint? carrierTypeIndex)
        {
            switch (carrierType)
            {
                case CarrierType.Auto:
                    return Auto;

                case CarrierType.FOUP:
                    if (carrierTypeIndex != null)
                        return Foup + carrierTypeIndex;
                    else
                        return Foup;

                case CarrierType.FOSB:
                    if (carrierTypeIndex != null)
                        return FosB + carrierTypeIndex;
                    else
                        return FosB;

                case CarrierType.OpenCassette:
                    if (carrierTypeIndex != null)
                        return OpenCassette + carrierTypeIndex;
                    else
                        return OpenCassette;

                case CarrierType.Foup1Slot:
                    if (carrierTypeIndex != null)
                        return Foup1Slot + carrierTypeIndex;
                    else
                        return Foup1Slot;

                case CarrierType.CarrierOnAdapter:
                    if (carrierTypeIndex != null)
                        return CarrierOnAdapter + carrierTypeIndex;
                    else
                        return CarrierOnAdapter;

                case CarrierType.Special:
                    if (carrierTypeIndex != null)
                        return SpecialCarrier + (char)(carrierTypeIndex + SpecialCarrierStartIndex - 1);
                    else
                        return SpecialCarrier;

                case CarrierType.NotIdentified:
                    return NotIdentified;

                default:
                    throw new ArgumentOutOfRangeException(nameof(carrierType), carrierType, null);
            }
        }

        #endregion Helpers
    }
}
