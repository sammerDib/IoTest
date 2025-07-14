using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Shared.TC.Shared.Data;

using TransferType = UnitySC.Shared.TC.Shared.Data.TransferType;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule
{
    public abstract partial class UnityProcessModule
    {
        #region Simulated Commands

        protected override void InternalSimulatePrepareTransfer(
            TransferType transferType,
            RobotArm arm,
            MaterialType materialType,
            SampleDimension dimension,
            Tempomat tempomat)
        {
            if (ProcessModuleState != ProcessModuleState.Idle)
            {
                // Timeout
                throw new InvalidOperationException($"{Name} is not ready for transfer.");
            }

            if (!IsDoorOpen || !IsReadyToLoadUnload)
            {
                Task.Run(
                    () => { base.InternalSimulatePrepareTransfer(transferType, arm, materialType, dimension, tempomat); });
                throw new InvalidOperationException($"{Name} is not ready for transfer.");
            }
        }

        #endregion

        #region Public Methods

        public abstract List<StatusVariable> SimulateGetStatusVariables(List<int> ids);

        public abstract List<EquipmentConstant> SimulateGetEquipmentConstants(List<int> ids);

        public abstract List<CommonEvent> SimulateGetCollectionEvents();

        #endregion
    }
}
