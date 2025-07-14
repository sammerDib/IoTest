using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class PatternRecDisplayVM : ObservableObject, IModalDialogViewModel
    {
        private PatternRecognitionData _patternRec = null;

        public PatternRecognitionData PatternRec
        {
            get => _patternRec;
            set
            {
                if (_patternRec != value)
                {
                    _patternRec = value;
                    UpdateRoiSizePositionInPixels();
                    OnPropertyChanged();
                }
            }
        }

        private double _roiLeft = 0;

        public double RoiLeft
        {
            get => _roiLeft; set { if (_roiLeft != value) { _roiLeft = value; OnPropertyChanged(); } }
        }

        private double _roiTop = 0;

        public double RoiTop
        {
            get => _roiTop; set { if (_roiTop != value) { _roiTop = value; OnPropertyChanged(); } }
        }

        private double _roiWidth = 0;

        public double RoiWidth
        {
            get => _roiWidth; set { if (_roiWidth != value) { _roiWidth = value; OnPropertyChanged(); } }
        }

        private double _roiHeight = 0;

        public double RoiHeight
        {
            get => _roiHeight; set { if (_roiHeight != value) { _roiHeight = value; OnPropertyChanged(); } }
        }

        public double ImageWidth => PatternRec.PatternReference.DataWidth;

        public double ImageHeight => PatternRec.PatternReference.DataHeight;

        public double Gamma => PatternRec.Gamma;

        private void UpdateRoiSizePositionInPixels()
        {
            var pixelSizemm = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters;
            if (PatternRec.RegionOfInterest != null)
            {
                RoiWidth = PatternRec.RegionOfInterest.Width.ToPixels(pixelSizemm.Millimeters());
                RoiHeight = PatternRec.RegionOfInterest.Height.ToPixels(pixelSizemm.Millimeters());
                RoiLeft = PatternRec.RegionOfInterest.X.ToPixels(pixelSizemm.Millimeters());
                RoiTop = PatternRec.RegionOfInterest.Y.ToPixels(pixelSizemm.Millimeters());
            }
            else
            {
                RoiWidth = PatternRec.PatternReference.DataWidth;
                RoiHeight = PatternRec.PatternReference.DataHeight;
                RoiLeft = 0;
                RoiTop = 0;
            }
        }

        public bool? DialogResult { get; }
    }
}
