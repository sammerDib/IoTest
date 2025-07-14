using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Configuration;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Simulation;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

using OperationMode = UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420
{
    public partial class RA420 : ISimDevice
    {
        #region Properties

        protected internal RA420SimulationData SimulationData { get; private set; }

        #endregion

        #region ISimDevice

        public ISimDeviceView SimDeviceView
            => new RA420SimulatorUserControl() { DataContext = SimulationData };

        #endregion ISimDevice

        #region Commands

        protected virtual void InternalSimulateGetStatuses(Tempomat tempomat)
        {
            //Do nothing in simulated mode
        }

        protected override void InternalSimulatePrepareTransfer(
            EffectorType effector,
            SampleDimension dimension,
            MaterialType materialType,
            Tempomat tempomat)
        {
            if (O_LightError)
            {
                SimulationData.O_LightError = false;
            }

            if (SelectedSubstrateSize != dimension || SelectedMaterialType != materialType)
            {
                //Retrieve configuration to find size according to wafer's dimension and type
                var sampleDimensionPerPosition =
                    Configuration.SubstrateInformationsPerPositions.FirstOrDefault(
                        x => x.Value.SubstrateSize == dimension
                             && x.Value.MaterialType == materialType);
                if (sampleDimensionPerPosition.Equals(
                        default(KeyValuePair<uint, SubstrateInformations>)))
                {
                    throw new InvalidOperationException(
                        $"Substrate information with dimension = {Location.Wafer.MaterialDimension} and material type = {Location.Wafer.MaterialType} have not been configured in Aligner.");
                }

                SelectedSubstrateSize = sampleDimensionPerPosition.Value.SubstrateSize;
                SelectedMaterialType = sampleDimensionPerPosition.Value.MaterialType;
            }

            switch (effector)
            {
                case EffectorType.VacuumI:
                    if (ZAxisPosition != ZAxisPosition.ZAxisAtVeryTop)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1300));
                        ZAxisPosition = ZAxisPosition.ZAxisAtVeryTop;
                    }

                    break;
                case EffectorType.VacuumU:
                    if (ZAxisPosition != ZAxisPosition.ZAxisAtVeryBottom)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1300));
                        ZAxisPosition = ZAxisPosition.ZAxisAtVeryBottom;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(effector), effector, null);
            }

            State = OperatingModes.Idle;
        }

        protected override void InternalSimulateAlign(
            Angle target,
            AlignType alignType,
            Tempomat tempomat)
        {
            if (O_LightError)
            {
                SimulationData.O_LightError = false;
            }

            OperationStatus = OperationStatus.Moving;
            tempomat.Sleep(Duration.FromSeconds(5));
            OperationStatus = OperationStatus.Stop;
            if (Location.Material != null)
            {
                ((Wafer)Location.Material).OrientationAngle = target;
            }

            State = OperatingModes.Idle;
        }

        protected override void InternalSimulateClamp(Tempomat tempomat)
        {
            if (O_LightError)
            {
                SimulationData.O_LightError = false;
            }

            OperationStatus = OperationStatus.Moving;
            ZAxisPosition = ZAxisPosition.ZAxisUndefinedPosition;
            tempomat.Sleep(Duration.FromSeconds(1));
            OperationStatus = OperationStatus.Stop;
            ZAxisPosition = ZAxisPosition.ZAxisAtVeryBottom;
            WaferPresence = Location.Wafer != null
                ? WaferPresence.Present
                : WaferPresence.Absent;
            State = OperatingModes.Idle;
        }

        protected override void InternalSimulateUnclamp(Tempomat tempomat)
        {
            if (O_LightError)
            {
                SimulationData.O_LightError = false;
            }

            OperationStatus = OperationStatus.Moving;
            tempomat.Sleep(Duration.FromSeconds(1));
            OperationStatus = OperationStatus.Stop;
            WaferPresence = Location.Wafer != null
                ? WaferPresence.Present
                : WaferPresence.Absent;
            State = OperatingModes.Idle;
        }

        protected override void InternalSimulateSetDateAndTime(Tempomat tempomat)
        {
            //Do nothing in simulation mode
        }

        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            base.InternalSimulateInitialize(mustForceInit, tempomat);
            CommandProcessing = CommandProcessing.Processing;
            OperationStatus = OperationStatus.Moving;
            tempomat.Sleep(Duration.FromSeconds(2));
            CommandProcessing = CommandProcessing.Stop;
            OperationStatus = OperationStatus.Stop;
            OriginReturnCompletion = OriginReturnCompletion.Completed;
            ZAxisPosition = ZAxisPosition.ZAxisAtOrigin;
            if (WaferPresence == WaferPresence.Unknown)
            {
                WaferPresence = WaferPresence.Absent;
            }
        }

        #endregion

        #region Private Methods

        private void SetUpSimulatedMode()
        {
            SimulationData = new RA420SimulationData(this);
            SimulationData.PropertyChanged += SimulationData_PropertyChanged;
            OperationMode = OperationMode.Remote;
            OriginReturnCompletion = OriginReturnCompletion.NotCompleted;
            CommandProcessing = CommandProcessing.Stop;
            OperationStatus = OperationStatus.Stop;
            MotionSpeedPercentage = "100%";
            IsNormalSpeed = true;
            ErrorControllerName = ErrorControllerId.WholeOfTheAligner;
            ErrorDescription = Aligner.RA420.Driver.Enums.ErrorCode.None;
            ZAxisPosition = ZAxisPosition.ZAxisUndefinedPosition;
            SelectedMaterialType = MaterialType.SiliconWithNotch;
            Location.PropertyChanged += Location_PropertyChanged;
        }

        private void DisposeSimulatedMode()
        {
            if (SimulationData != null)
            {
                SimulationData.PropertyChanged -= SimulationData_PropertyChanged;
                SimulationData = null;
            }

            Location.PropertyChanged -= Location_PropertyChanged;
        }

        #endregion

        #region Event Handlers

        private void Location_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            WaferPresence = Location.Wafer != null
                ? WaferPresence.Present
                : WaferPresence.Absent;
        }

        private void SimulationData_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SimulationData.I_ExhaustFanRotating):
                    I_ExhaustFanRotating = SimulationData.I_ExhaustFanRotating;
                    break;
                case nameof(SimulationData.I_SubstrateDetectionSensor1):
                    I_SubstrateDetectionSensor1 = SimulationData.I_SubstrateDetectionSensor1;
                    break;
                case nameof(SimulationData.I_SubstrateDetectionSensor2):
                    I_SubstrateDetectionSensor2 = SimulationData.I_SubstrateDetectionSensor2;
                    break;
                case nameof(SimulationData.O_AlignerReadyToOperate):
                    O_AlignerReadyToOperate = SimulationData.O_AlignerReadyToOperate;
                    break;
                case nameof(SimulationData.O_AlignmentComplete):
                    O_AlignmentComplete = SimulationData.O_AlignmentComplete;
                    break;
                case nameof(SimulationData.O_LightError):
                    O_LightError = SimulationData.O_LightError;
                    break;
                case nameof(SimulationData.O_SignificantError):
                    O_SignificantError = SimulationData.O_SignificantError;
                    break;
                case nameof(SimulationData.O_SpindleSolenoidValveChuckingOFF):
                    O_SpindleSolenoidValveChuckingOFF =
                        SimulationData.O_SpindleSolenoidValveChuckingOFF;
                    break;
                case nameof(SimulationData.O_SpindleSolenoidValveChuckingON):
                    O_SpindleSolenoidValveChuckingON =
                        SimulationData.O_SpindleSolenoidValveChuckingON;
                    break;
                case nameof(SimulationData.O_SubstrateDetection):
                    O_SubstrateDetection = SimulationData.O_SubstrateDetection;
                    break;
                case nameof(SimulationData.O_TemporarilyStop):
                    O_TemporarilyStop = SimulationData.O_TemporarilyStop;
                    break;
            }
        }

        #endregion
    }
}
