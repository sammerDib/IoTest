using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Devices.Controller.JobDefinition;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Devices.Controller;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;
using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum;

namespace UnitySC.Equipment.Devices.Controller
{
    [Device]
    [Interlock(Type = typeof(IsRobotInitialized))]
    [Interlock(Type = typeof(IsLocationReadyForTransfer))]
    [Interlock(Type = typeof(IsLocationServedByRobot))]
    [Interlock(Type = typeof(IsSlotValid))]
    [Interlock(Type = typeof(IsLocationAccessedByRobot))]
    public interface IController : Abstractions.Devices.Controller.IController
    {
        #region Commands

        [Command]
        [Pre(Type = typeof(IsMaintenanceOrEngineering))]
        [Pre(Type = typeof(IsCurrentActivityNull))]
        [Pre(Type = typeof(CheckCarrierOpened))]
        [Pre(Type = typeof(CheckLoadPortReady))]
        [Pre(Type = typeof(CheckSourceSlotNotEmpty))]
        [Pre(Type = typeof(CheckRobotReady))]
        [Pre(Type = typeof(CheckRobotArmEmpty))]
        [Pre(Type = typeof(CheckRobotArmEnabled))]
        [Pre(Type = typeof(CheckAlignerReady))]
        [Pre(Type = typeof(CheckAlignerEmpty))]
        [Pre(Type = typeof(CheckProcessModuleReady))]
        [Pre(Type = typeof(CheckProcessModuleEmpty))]
        void LoadProcessModule(
            IMaterialLocationContainer loadPort,
            byte sourceSlot,
            RobotArm robotArm,
            [Unit(AngleUnit.Degree)] Angle alignAngle,
            AlignType alignType,
            EffectorType effectorType,
            IMaterialLocationContainer processModule);

        [Command]
        [Pre(Type = typeof(IsMaintenanceOrEngineering))]
        [Pre(Type = typeof(IsCurrentActivityNull))]
        [Pre(Type = typeof(CheckCarrierOpened))]
        [Pre(Type = typeof(CheckLoadPortReady))]
        [Pre(Type = typeof(CheckDestinationSlotEmpty))]
        [Pre(Type = typeof(CheckRobotReady))]
        [Pre(Type = typeof(CheckRobotArmEmpty))]
        [Pre(Type = typeof(CheckRobotArmEnabled))]
        [Pre(Type = typeof(CheckProcessModuleReady))]
        [Pre(Type = typeof(CheckProcessModuleNotEmpty))]
        void UnloadProcessModule(
            IMaterialLocationContainer processModule,
            RobotArm robotArm,
            EffectorType effectorType,
            IMaterialLocationContainer loadPort,
            byte destinationSlot);

        [Command]
        [Pre(Type = typeof(IsMaintenanceOrEngineering))]
        [Pre(Type = typeof(AreSubDevicesIdle))]
        [Pre(Type = typeof(IsCurrentActivityNull))]
        void Clean();

        [Command]
        [Pre(Type = typeof(IsIdleOrExecuting))]
        void CreateJob(Job job);

        [Command]
        [Pre(Type = typeof(IsIdleOrExecuting))]
        [Pre(Type = typeof(IsCurrentActivityNull))]
        [Pre(Type = typeof(CheckJob))]
        void StartJobExecution(Job job);

        [Command]
        [Pre(Type = typeof(IsIdleOrEngineering))]
        [Pre(Type = typeof(IsCurrentActivityNull))]
        void RequestManualMode();

        [Command]
        [Pre(Type = typeof(IsMaintenanceOrIdle))]
        [Pre(Type = typeof(IsCurrentActivityNull))]
        void RequestEngineeringMode();

        [Command]
        [Pre(Type = typeof(IsJobPausable))]
        void Pause(string jobName);

        [Command]
        [Pre(Type = typeof(IsJobResumable))]
        void Resume(string jobName);

        [Command]
        [Pre(Type = typeof(IsJobStoppable))]
        void Stop(string jobName, StopConfig stopConfig);

        #endregion

        #region Status

        /// <summary>
        /// current activity step name
        /// </summary>
        [Status]
        string CurrentActivityStep { get; }

        /// <summary>
        /// current substrate throughput
        /// </summary>
        [Status]
        double SubstrateThroughput { get; }

        #endregion
    }
}
