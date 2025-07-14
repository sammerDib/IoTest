using System.Collections.Generic;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Equipment.Abstractions
{
    public class EquipmentConstantChangedEventArgs : System.EventArgs
    {
        #region Properties

        public List<EquipmentConstant> EquipmentConstants { get; }

        #endregion

        #region Constructor

        public EquipmentConstantChangedEventArgs(List<EquipmentConstant> equipmentConstants)
        {
            EquipmentConstants = new List<EquipmentConstant>(equipmentConstants);
        }

        #endregion
    }
}
