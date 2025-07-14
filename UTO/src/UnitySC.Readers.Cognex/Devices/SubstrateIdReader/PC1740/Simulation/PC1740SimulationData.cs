using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Simulation
{
    public class PC1740SimulationData : SimulationData
    {
        #region Constructor

        public PC1740SimulationData(PC1740 substrateIdReader)
            : base(substrateIdReader)
        {
            SubstrateId = "Subst1";
        }

        #endregion

        #region Properties

        private string _substrateId;
        public string SubstrateId
        {
            get => _substrateId;
            set => SetAndRaiseIfChanged(ref _substrateId, value, nameof(SubstrateId));
        }

        #endregion
    }
}
