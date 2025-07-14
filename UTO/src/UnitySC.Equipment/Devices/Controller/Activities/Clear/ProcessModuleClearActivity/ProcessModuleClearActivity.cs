using System.Text;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    public partial class ProcessModuleClearActivity : MachineActivity
    {
        #region Fields

        private readonly DriveableProcessModule _processModule;
        private readonly RobotArm _robotArm;

        private readonly Abstractions.Devices.Robot.Robot _robot;

        #endregion

        #region Constructor

        public ProcessModuleClearActivity(
            Controller controller,
            DriveableProcessModule processModule,
            RobotArm robotArm)
            : base($"{processModule.Name}_ClearActivity", controller)
        {
            _robot = Efem.TryGetDevice<Abstractions.Devices.Robot.Robot>();

            _processModule = processModule;
            _robotArm = robotArm;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Robot arm : {_robotArm}");

            Logger.Info($"New {processModule.Name}_ClearActivity created", stringBuilder.ToString());

            CreateStateMachine();
            StateMachine = m_ProcessModuleClearActivity;
        }

        #endregion

        #region Overrides

        public override ErrorDescription Check(ActivityManager context)
        {
            var error = base.Check(context);
            if (error.ErrorCode != ErrorCode.Ok)
            {
                return error;
            }

            if (context.Activity != null)
            {
                return new ErrorDescription(
                    ErrorCode.Busy,
                    $"{context.Activity.Id} activity is already in use. Not allowed to start.");
            }

            return new ErrorDescription();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_ProcessModuleClearActivity.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Actions

        private void PreparePmEntry(Event ev)
        {
            PerformAction(
               () =>
               {
                   if (_processModule.TransferState == EnumPMTransferState.NotReady
                       || _processModule.TransferState
                       == EnumPMTransferState.ReadyToLoad_SlitDoorClosed
                       || _processModule.TransferState
                       == EnumPMTransferState.ReadyToUnload_SlitDoorClosed)
                   {
                       _processModule.PrepareTransferAsync(TransferType.Pick, _robotArm, _processModule.Location.Wafer.MaterialType, _processModule.Location.Wafer.MaterialDimension);
                   }
               },
                new PmDone());
        }

        private void GoInFrontOfPmEntry(Event ev)
        {
            PerformAction(
                () => _robot.GoToSpecifiedLocation(_processModule, 1, _robotArm, false),
                new RobotDone());
        }

        private void PrepareTransferOnPmEntry(Event ev)
        {
            try
            {
                _processModule.PrepareTransfer(TransferType.Pick, _robotArm, _processModule.Location.Wafer.MaterialType, _processModule.Location.Wafer.MaterialDimension);
                PostEvent(new PmReadyToTransfer());
            }
            catch
            {
                PostEvent(new PmNotReadyToTransfer());
            }
        }

        private void WaitPmReadyToTransferEntry(Event ev)
        {
            _processModule.ReadyToTransfer += ProcessModule_ReadyToTransfer;
        }

        private void PickOnPmEntry(Event ev)
        {
            PerformAction(() => _robot.Pick(_robotArm, _processModule, 1), new RobotDone(), false);
        }

        private void PostTransferOnPmEntry(Event ev)
        {
            PerformAction(
                () => _processModule.PostTransfer(),
                new PmDone());
        }

        #endregion

        #region Event Handler

        private void ProcessModule_ReadyToTransfer(object sender, System.EventArgs e)
        {
            _processModule.ReadyToTransfer -= ProcessModule_ReadyToTransfer;
            PostEvent(new PmReadyToTransfer());
        }

        #endregion
    }
}
