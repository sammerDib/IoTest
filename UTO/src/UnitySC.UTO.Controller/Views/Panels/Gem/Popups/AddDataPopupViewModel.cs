using System.Collections.Generic;

using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Popups
{
    public class AddDataPopupViewModel : Notifier
    {
        #region Constructors

        static AddDataPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(AddDataPopupViewModel), typeof(AddDataPopupView));
        }

        // For design time
        public AddDataPopupViewModel()
        {
            if (IsInDesignMode)
            {
                AvailableData = new List<string>()
                {
                    "Pause5",
                    "pause6",
                    "pause7",
                    "pause8",
                    "pause9",
                    "pause10",
                    "pause11"
                };
            }
        }

        public AddDataPopupViewModel(List<string> availableData, bool allowCustomValue = false)
        {
            AllowCustomValue = allowCustomValue;
            AvailableData = availableData;
        }

        #endregion

        #region Properties

        public bool AllowCustomValue { get; }

        public List<string> AvailableData { get; }

        private List<string> _selectedData = new();

        public List<string> SelectedData
        {
            get => _selectedData;
            set => SetAndRaiseIfChanged(ref _selectedData, value);
        }

        #endregion
    }
}
