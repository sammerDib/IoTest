using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum;
using UnitySC.Equipment.Devices.Controller.JobDefinition;
using UnitySC.Equipment.Devices.Controller.Simulation;

namespace UnitySC.Equipment.Devices.Controller
{
    public partial class Controller : ISimDevice
    {
        #region Properties

        protected internal ControllerSimulationData SimulationData { get; private set; }

        #endregion

        #region ISimDevice

        /// <inheritdoc />
        public ISimDeviceView SimDeviceView
            => new ControllerSimulationView(this) { DataContext = SimulationData };

        private void SetUpSimulatedMode() => SimulationData = new ControllerSimulationData(this);

        #endregion ISimDevice

        #region Device Commands

        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            base.InternalSimulateInitialize(mustForceInit, tempomat);
            InternalInitialize(mustForceInit);
        }

        protected virtual void InternalSimulateLoadProcessModule(
            IMaterialLocationContainer loadPort,
            byte sourceSlot,
            RobotArm robotArm,
            Angle alignAngle,
            AlignType alignType,
            EffectorType effectorType,
            IMaterialLocationContainer processModule,
            Tempomat tempomat)
            => InternalLoadProcessModule(
                loadPort,
                sourceSlot,
                robotArm,
                alignAngle,
                alignType,
                effectorType,
                processModule);

        protected virtual void InternalSimulateUnloadProcessModule(
            IMaterialLocationContainer processModule,
            RobotArm robotArm,
            EffectorType effectorType,
            IMaterialLocationContainer loadPort,
            byte destinationSlot,
            Tempomat tempomat)
            => InternalUnloadProcessModule(
                processModule,
                robotArm,
                effectorType,
                loadPort,
                destinationSlot);

        protected virtual void InternalSimulateClean(Tempomat tempomat) => StartClearActivity();

        protected virtual void InternalSimulateRequestManualMode(Tempomat tempomat)
            => InternalRequestManualMode();

        protected virtual void InternalSimulateStartJobExecution(Job job, Tempomat tempomat)
            => InternalStartJobExecution(job);

        protected virtual void InternalSimulatePause(string jobName, Tempomat tempomat)
            => InternalPause(jobName);

        protected virtual void InternalSimulateResume(string jobName, Tempomat tempomat)
            => InternalResume(jobName);

        protected virtual void InternalSimulateStop(
            string jobName,
            StopConfig stopConfig,
            Tempomat tempomat)
            => InternalStop(jobName, stopConfig);

        protected virtual void InternalSimulateRequestEngineeringMode(Tempomat tempomat)
            => InternalRequestEngineeringMode();

        protected virtual void InternalSimulateCreateJob(Job job, Tempomat tempomat)
            => InternalCreateJob(job);

        #endregion
    }
}
