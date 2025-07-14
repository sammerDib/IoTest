using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Helpers;

using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts.E84SignalsViewer
{
    public class E84Signals : Notifier
    {
        static E84Signals()
        {
            DataTemplateGenerator.Create(typeof(E84Signals), typeof(E84SignalView));
        }

        public E84Signals(LoadPort loadport)
        {
            LoadPort = loadport;
        }

        #region Properties

        public  LoadPort LoadPort { get; }

        private bool _isValidOrLReqChecked;

        public bool IsValidOrLReqChecked
        {
            get => _isValidOrLReqChecked;
            set => SetAndRaiseIfChanged(ref _isValidOrLReqChecked, value);
        }

        private bool _isCs0OrUreqChecked;

        public bool IsCs0OrUreqChecked
        {
            get => _isCs0OrUreqChecked;
            set => SetAndRaiseIfChanged(ref _isCs0OrUreqChecked, value);
        }

        private bool _isCs1OrVaChecked;

        public bool IsCs1OrVaChecked
        {
            get => _isCs1OrVaChecked;
            set => SetAndRaiseIfChanged(ref _isCs1OrVaChecked, value);
        }

        private bool _isAmAvblOrREadyChecked;

        public bool IsAmAvblOrREadyChecked
        {
            get => _isAmAvblOrREadyChecked;
            set => SetAndRaiseIfChanged(ref _isAmAvblOrREadyChecked, value);
        }

        private bool _isTrReqOrVs0Checked;

        public bool IsTrReqOrVs0Checked
        {
            get => _isTrReqOrVs0Checked;
            set => SetAndRaiseIfChanged(ref _isTrReqOrVs0Checked, value);
        }

        private bool _isBusyOrVs1Checked;

        public bool IsBusyOrVs1Checked
        {
            get => _isBusyOrVs1Checked;
            set => SetAndRaiseIfChanged(ref _isBusyOrVs1Checked, value);
        }

        private bool _isComptOrHoAblChecked;

        public bool IsComptOrHoAblChecked
        {
            get => _isComptOrHoAblChecked;
            set => SetAndRaiseIfChanged(ref _isComptOrHoAblChecked, value);
        }

        private bool _isContOrEsChecked;

        public bool IsContOrEsChecked
        {
            get => _isContOrEsChecked;
            set => SetAndRaiseIfChanged(ref _isContOrEsChecked, value);
        }

        #endregion
    }
}
