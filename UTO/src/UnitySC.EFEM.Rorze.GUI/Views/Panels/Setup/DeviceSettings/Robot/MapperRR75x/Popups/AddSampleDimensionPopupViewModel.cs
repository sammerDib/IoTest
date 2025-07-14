using System.Collections.Generic;

using Agileo.SemiDefinitions;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.MapperRR75x.Popups
{
    public class AddSampleDimensionPopupViewModel : NotifyDataError
    {
        static AddSampleDimensionPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(AddSampleDimensionPopupViewModel), typeof(AddSampleDimensionPopupView));
        }
        public AddSampleDimensionPopupViewModel(List<SampleDimension> sampleDimensions)
        {
            _sampleDimensions = sampleDimensions;
            NewSampleDimension = SampleDimension.NoDimension;
        }

        #region Fields

        private readonly List<SampleDimension> _sampleDimensions;

        #endregion

        #region Properties

        private SampleDimension _newSampleDimension;

        public SampleDimension NewSampleDimension
        {
            get => _newSampleDimension;
            set => SetAndRaiseIfChanged(ref _newSampleDimension, value);
        }

        #endregion

        #region Public

        public bool ValidateSampleDimension()
        {
            if (NewSampleDimension == SampleDimension.NoDimension)
            {
                return false;
            }

            if (_sampleDimensions.Contains(NewSampleDimension))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
