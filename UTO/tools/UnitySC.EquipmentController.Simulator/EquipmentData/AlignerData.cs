using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;

namespace UnitySC.EquipmentController.Simulator.EquipmentData
{
    /// <summary>
    /// Class containing all data about device aligner.
    /// It's objective is not to provide any logic or automatic behavior.
    /// It only aims to concentrate all known data sent from the EfemController.
    /// </summary>
    internal class AlignerData
    {
        internal AlignerStatus AlignerStatus { get; set; }

        internal bool IsAlignerCarrierPresent { get; set; }

        internal string WaferIdFrontSide { get; set; }

        internal string WaferIdBackSide { get; set; }
    }
}
