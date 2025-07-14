using System.Collections.Generic;
using System.Linq;
using System.Timers;

using Agileo.GUI.Services.Icons;
using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary
{
    public class EquipmentConstantsPanel : BaseE30VariablesPanel
    {
        private readonly List<int> _variableToNotify = new ();
        private readonly Timer _valueChangedTimer;

        public EquipmentConstantsPanel(string relativeId, IIcon icon = null) : base(false, relativeId, icon)
        {
            _valueChangedTimer = new Timer(RefreshDelay);
            _valueChangedTimer.Elapsed += ValueChangedTimerElapsed;
        }

        private void ValueChangedTimerElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var viewModel in _variableToNotify.Select(GetVariableViewModel))
            {
                viewModel?.NotifyValueChanged();
            }

            _variableToNotify.Clear();
        }

        private void EquipmentConstantChanged(object sender, VariableEventArgs e)
        {
            if (_variableToNotify.Contains(e.Variable.ID)) return;

            _variableToNotify.Add(e.Variable.ID);
        }

        #region Overrides of BaseE30VariablesPanel

        protected override IEnumerable<E30Variable> LoadVariables()
        {
            return App.ControllerInstance.GemController.E30Std.DataServices == null
                ? Enumerable.Empty<E30Variable>()
                : App.ControllerInstance.GemController.E30Std.DataServices.GetVariables(Class.ECV);
        }

        protected override void InternalSetValue(DataItem newValue)
        {
            if (App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices == null)
            {
                return;
            }

            App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices.SetValueByID(SelectedVariable.Variable.ID, newValue);
            SelectedVariable.NotifyValueChanged();
        }

        protected override void OnRefreshDelayChanged()
        {
            _valueChangedTimer.Interval = RefreshDelay;
        }

        public override void OnSetup()
        {
            if(App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices != null)
            {
                App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices
                    .EquipmentConstantChanged += EquipmentConstantChanged;
            }

            base.OnSetup();
        }

        public override void OnShow()
        {
            base.OnShow();
            _valueChangedTimer.Start();
        }

        public override void OnHide()
        {
            base.OnHide();
            _valueChangedTimer.Stop();
            _variableToNotify.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices != null)
            {
                App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices
                    .EquipmentConstantChanged -= EquipmentConstantChanged;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
