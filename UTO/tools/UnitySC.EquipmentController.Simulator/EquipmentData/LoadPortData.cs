using System.Collections.Generic;

using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;
using UnitySC.Equipment.Abstractions.Material;

namespace UnitySC.EquipmentController.Simulator.EquipmentData
{
    /// <summary>
    /// Class containing all data about device LoadPort.
    /// It's objective is not to provide any logic or automatic behavior.
    /// It only aims to concentrate all known data sent from the EfemController.
    /// </summary>
    internal class LoadPortData
    {
        internal LoadPortStatus LoadPortStatus { get; set; }

        /// <summary>
        /// Indicates whether the carrier is present or not on the LP.
        /// </summary>
        /// <remarks>Several events indicate it: general status event (STAT) and carrier presence event (LPSR).</remarks>
        internal bool IsCarrierPresent { get; set; }

        internal bool IsCarrierCorrectlyPlaced { get; set; }

        internal bool IsHandOffBtnPressed { get; set; }

        internal int WaferSize { get; set; }

        internal string CarrierID { get; set; }

        internal uint CarrierType { get; set; }

        /// <summary>
        /// Represent the mapping slot states from the bottom to the top.
        /// </summary>
        internal List<SlotState> MappingData { get; set; }
    }
}
