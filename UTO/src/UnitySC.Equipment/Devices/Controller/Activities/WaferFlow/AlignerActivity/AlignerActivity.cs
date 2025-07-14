using System.Text;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Devices.Controller.JobDefinition;

namespace UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.AlignerActivity
{
    public partial class AlignerActivity : WaferMachineActivity.WaferMachineActivity
    {
        #region Fields

        private readonly Abstractions.Devices.Aligner.Aligner _aligner;
        private readonly Abstractions.Devices.SubstrateIdReader.SubstrateIdReader _frontSideReader;
        private readonly Abstractions.Devices.SubstrateIdReader.SubstrateIdReader _backSideReader;

        private readonly double _alignAngle;
        private readonly OcrProfile _profile;

        private readonly bool _isSubstrateReaderEnabled;
        private readonly bool _isSubstrateIdVerificationEnabled;
        #endregion

        #region Constructor

        public AlignerActivity(Controller controller, double alignAngle, OcrProfile profile, bool isSubstrateReaderEnabled, bool isSubstrateIdVerificationEnabled)
            : base(nameof(AlignerActivity), controller)
        {
            // Store necessary parameters
            _aligner = Efem.TryGetDevice<Abstractions.Devices.Aligner.Aligner>();

            _frontSideReader =
                _aligner.TryGetDevice<Abstractions.Devices.SubstrateIdReader.SubstrateIdReader>(1);
            _backSideReader =
                _aligner.TryGetDevice<Abstractions.Devices.SubstrateIdReader.SubstrateIdReader>(2);

            _alignAngle = alignAngle;
            _profile = profile;
            _isSubstrateReaderEnabled = isSubstrateReaderEnabled;
            _isSubstrateIdVerificationEnabled = isSubstrateIdVerificationEnabled;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Align angle : {_alignAngle}");

            if (_profile != null)
            {
                stringBuilder.AppendLine($"Scribe angle : {_profile.Parameters.ScribeAngle}");
                stringBuilder.AppendLine($"Front reader recipe : {_profile.Parameters.FrontRecipeName}");
                stringBuilder.AppendLine($"Back reader recipe : {_profile.Parameters.BackRecipeName}");
                stringBuilder.AppendLine($"Reader Side : {_profile.Parameters.ReaderSide}");
            }

            stringBuilder.AppendLine($"Is Substrate Reader Enabled : {_isSubstrateReaderEnabled}");
            stringBuilder.AppendLine($"Is Substrate Id Verification Enabled : {_isSubstrateIdVerificationEnabled}");

            Logger.Info($"New {nameof(AlignerActivity)} activity created", stringBuilder.ToString());

            CreateStateMachine();
            StateMachine = m_AlignerActivity;
        }

        #endregion

        #region Public Methods

        public void CancelCurrentSubstrate()
        {
            PostEvent(new CancelSubstrate());
        }

        public void ProceedWithCurrentSubstrate()
        {
            PostEvent(new ProceedWithSubstrate());
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

            if (_aligner.State != OperatingModes.Idle)
            {
                return new ErrorDescription(
                    ErrorCode.CommandNotValidForCurrentState,
                    "Aligner is not IDLE. Not allowed to start");
            }

            if (_frontSideReader != null
                && _frontSideReader.State != OperatingModes.Idle)
            {
                return new ErrorDescription(
                    ErrorCode.CommandNotValidForCurrentState,
                    "Front side reader is not IDLE. Not allowed to start");
            }

            if (_backSideReader != null
                && _backSideReader.State != OperatingModes.Idle)
            {
                return new ErrorDescription(
                    ErrorCode.CommandNotValidForCurrentState,
                    "Back side reader is not IDLE. Not allowed to start");
            }

            if (_aligner.Location.Substrate == null)
            {
                return new ErrorDescription(
                    ErrorCode.CommandNotValidForCurrentState,
                    "Material is not present on Aligner. Not allowed to start.");
            }

            return new ErrorDescription();
        }

        public override ErrorDescription Abort()
        {
            var errorDescription = base.Abort();

            _aligner.InterruptAsync(InterruptionKind.Abort);

            return errorDescription;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_AlignerActivity.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Actions

        private void AlignToScribeAngleEntry(Event ev)
        {
            PerformAction(() =>
            {
                _aligner.Align(
                    Angle.FromDegrees(_profile.Parameters.ScribeAngle),
                    AlignType.AlignWaferWithoutCheckingSubO_FlatLocation);
            }, new AlignerDone());
        }

        private void ReadSubstrateIdEntry(Event ev)
        {
            var substrateId = _aligner.Location.Wafer.SubstrateId;
            var acquiredId = Efem.ReadSubstrateId(
                _profile.Parameters.ReaderSide,
                _profile.Parameters.FrontRecipeName,
                _profile.Parameters.BackRecipeName);

            PostEvent(new ReaderDone());
            OnSubstrateIdReadingHasBeenFinished(!string.IsNullOrWhiteSpace(acquiredId) && !acquiredId.Contains("*"), substrateId, acquiredId);
        }

        private void AlignEntry(Event ev)
        {
            PerformAction(() =>
            {
                OnWaferAlignStart();
                _aligner.Align(
                    Angle.FromDegrees(_alignAngle),
                    AlignType.AlignWaferWithoutCheckingSubO_FlatLocation);
                OnWaferAlignEnd();
            }, new AlignerDone());
        }

        #endregion

        #region Conditionals

        private bool SubstrateReaderEnabled(Event ev) => _isSubstrateReaderEnabled && _profile?.Parameters != null && string.IsNullOrEmpty(_aligner.Location.Wafer.AcquiredId);

        private bool SubstrateReaderDisabled(Event ev) => !_isSubstrateReaderEnabled || _profile?.Parameters == null || !string.IsNullOrEmpty(_aligner.Location.Wafer.AcquiredId);

        private bool SubstrateIdVerificationEnabled(Event ev) => _isSubstrateIdVerificationEnabled;

        private bool SubstrateIdVerificationDisabled(Event ev) => !_isSubstrateIdVerificationEnabled;

        #endregion
    }
}
