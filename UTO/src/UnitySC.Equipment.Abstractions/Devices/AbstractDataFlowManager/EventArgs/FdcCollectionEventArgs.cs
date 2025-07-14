using System.Collections.Generic;

using UnitySC.Shared.Data.FDC;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs
{
    public class FdcCollectionEventArgs
    {
        #region Properties

        public List<FDCData> FdcData { get; }

        #endregion

        #region Constructor

        public FdcCollectionEventArgs(List<FDCData> fdcData)
        {
            FdcData = fdcData;
        }

        #endregion
    }
}
