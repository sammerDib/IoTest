using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Enums;

using ErrorCode = UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.ErrorCode;
using OperationMode = UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420
{
    [Device]
    public interface IRA420 : IAligner
    {
        #region Statuses

        #region State

        [Status]
        OperationMode OperationMode { get; }

        [Status]
        OriginReturnCompletion OriginReturnCompletion { get; }

        [Status]
        CommandProcessing CommandProcessing { get; }

        [Status]
        OperationStatus OperationStatus { get; }

        [Status]
        bool IsNormalSpeed { get; }

        [Status]
        string MotionSpeedPercentage { get; }

        [Status]
        string ErrorControllerCode { get; }

        [Status]
        ErrorControllerId ErrorControllerName { get; }

        [Status]
        string ErrorCode { get; }

        [Status]
        ErrorCode ErrorDescription { get; }

        [Status]
        XAxisPosition XAxisPosition { get; }

        [Status]
        YAxisPosition YAxisPosition { get; }

        [Status]
        ZAxisPosition ZAxisPosition { get; }

        [Status]
        uint SelectedSize { get; }

        [Status]
        MaterialType SelectedMaterialType { get; }

        [Status]
        SampleDimension SelectedSubstrateSize { get; }

        #endregion State

        #region GPIO

        [Status(Category = "Inputs")]
        bool I_ExhaustFanRotating { get; }

        [Status(Category = "Inputs")]
        bool I_SubstrateDetectionSensor1 { get; }

        [Status(Category = "Inputs")]
        bool I_SubstrateDetectionSensor2 { get; }

        [Status(Category = "Outputs")]
        bool O_AlignerReadyToOperate { get; }

        [Status(Category = "Outputs")]
        bool O_TemporarilyStop { get; }

        [Status(Category = "Outputs")]
        bool O_SignificantError { get; }

        [Status(Category = "Outputs")]
        bool O_LightError { get; }

        [Status(Category = "Outputs")]
        bool O_SubstrateDetection { get; }

        [Status(Category = "Outputs")]
        bool O_AlignmentComplete { get; }

        [Status(Category = "Outputs")]
        bool O_SpindleSolenoidValveChuckingOFF { get; }

        [Status(Category = "Outputs")]
        bool O_SpindleSolenoidValveChuckingON { get; }

        #endregion GPIO

        #endregion Statuses

        #region Commands

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void GetStatuses();

        #endregion Commands
    }
}
