using System.ComponentModel;

using Agileo.GUI.Components;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataValidation
{
    public class DataValidationPanel : NotifyDataErrorPanel
    {
        public DataValidationPanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            Rules.Add(new DelegateRule(nameof(StringValue), () => StringValue.ToLowerInvariant() is "true" or "false" ? null : "Only 'true' or 'false' are accepted as value."));
            Rules.Add(new DelegateRule(nameof(IntRuledValue), () => IntRuledValue >= 5 ? null : "The value need to be greater than 4."));
            Rules.Add(new DelegateRule(nameof(IntMinComparedValue), () => IntMinComparedValue < IntMaxComparedValue ? null : $"The value need to be less than the maximum value '{IntMaxComparedValue}'"));
            Rules.Add(new DelegateRule(nameof(IntMaxComparedValue), () => IntMinComparedValue < IntMaxComparedValue ? null : $"The value need to be greater than the minimum value '{IntMinComparedValue}'"));

            ErrorsChanged += OnErrorsChanged;
        }

        #region Properties

        private string _stringValue = "True";

        public string StringValue
        {
            get => _stringValue;
            set => SetAndRaiseIfChanged(ref _stringValue, value);
        }

        private int _simpleIntValue;

        public int SimpleIntValue
        {
            get => _simpleIntValue;
            set => SetAndRaiseIfChanged(ref _simpleIntValue, value);
        }

        private int _intRuledValue = 5;

        public int IntRuledValue
        {
            get => _intRuledValue;
            set => SetAndRaiseIfChanged(ref _intRuledValue, value);
        }

        private int _intMinComparedValue;

        public int IntMinComparedValue
        {
            get => _intMinComparedValue;
            set
            {
                SetAndRaiseIfChanged(ref _intMinComparedValue, value);
                // Raises a PropertyChanged to trigger dependent validation.
                OnPropertyChanged(nameof(IntMaxComparedValue));
            }
        }

        private int _intMaxComparedValue = 5;

        public int IntMaxComparedValue
        {
            get => _intMaxComparedValue;
            set
            {
                SetAndRaiseIfChanged(ref _intMaxComparedValue, value);
                // Raises a PropertyChanged to trigger dependent validation.
                OnPropertyChanged(nameof(IntMinComparedValue));
            }
        }

        #endregion

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (HasErrors)
            {
                Messages.Show(new UserMessage(MessageLevel.Error, new InvariantText("There is at least one field in error.")));
            }
            else
            {
                Messages.HideAll();
            }
        }
    }
}
