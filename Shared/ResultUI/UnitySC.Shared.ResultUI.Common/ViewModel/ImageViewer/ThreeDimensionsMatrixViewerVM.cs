using System;
using System.IO;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts.ThreeDimensions;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer
{
    public class ThreeDimensionsMatrixViewerVM : BaseMatrixViewerVM
    {
        #region Properties

        private MatrixDefinition _3dMatrix;

        private float? _referenceValue;
        private string _unit;

        public ThreeDimensionsChartVM Chart { get; }

        private bool _hideOutOfRange;

        public bool HideOutOfRange
        {
            get { return _hideOutOfRange; }
            set
            {
                SetProperty(ref _hideOutOfRange, value);
                UpdateChartData();
            }
        }
        
        private bool _displayMousePos;

        public bool DisplayMousePos
        {
            get { return _displayMousePos; }
            set
            {
                SetProperty(ref _displayMousePos, value);
                Chart.AllowUserInteraction(value);
            }
        }

        public double CurrentResolution => _3dMatrix?.Resolution ?? double.NaN;

        #endregion

        public ThreeDimensionsMatrixViewerVM(string unit, string xUnit, string yUnit) : base(null)
        {
            _unit = unit;
            Chart = new ThreeDimensionsChartVM(unit, xUnit, yUnit);
        }

        #region Overrides of BaseMatrixViewerVM

        public override void UpdateMinMax(float min, float max)
        {
            base.UpdateMinMax(min, max);
            UpdateChartColorMap();
            if (HideOutOfRange)
            {
                UpdateChartData();
            }
        }

        public override void Initialize(ColorMap colorMap, float min, float max)
        {
            base.Initialize(colorMap, min, max);
            UpdateChartColorMap();
            Chart.SetData(0, 0, Array.Empty<float>(), true, null);
        }

        public override void UpdateColorMap(ColorMap colorMap)
        {
            base.UpdateColorMap(colorMap);
            UpdateChartColorMap();
        }

        #endregion

        #region Private

        private void UpdateChartData()
        {
            if (_3dMatrix == null) return;
            Chart.SetData(_3dMatrix.Width, _3dMatrix.Height, _3dMatrix.Values, !_hideOutOfRange, _referenceValue);
        }

        private void UpdateChartColorMap()
        {
            float min = _referenceValue.HasValue ? Min - _referenceValue.Value : Min;
            float max = _referenceValue.HasValue ? Max - _referenceValue.Value : Max;
            Chart.UpdateColorMap(ColorMap, min, max);
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Chart.Dispose();
        }

        #endregion

        public void SetData(MatrixDefinition matrix)
        {
            _3dMatrix = matrix;
            UpdateChartData();
            OnPropertyChanged(nameof(CurrentResolution));
        }

        public void SetReferenceValue(float? referenceValue)
        {
            _referenceValue = referenceValue;

            UpdateChartColorMap();
            UpdateChartData();

            Chart.UpdateZAxisTitle(referenceValue.HasValue ? $"Relative value ({_unit})" : $"Value ({_unit})");
        }

        #region Commands

        private AutoRelayCommand _export3DCommand;

        public AutoRelayCommand Export3DCommand => _export3DCommand ?? (_export3DCommand = new AutoRelayCommand(Export3DCommandExecute));

        private void Export3DCommandExecute()
        {
            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            var saveFileDialogSettings = new SaveFileDialogSettings()
            {
                Title = "Export 3D",
                AddExtension = true,
                DefaultExt = "3da",
                FileName = "export3D",                
                CheckFileExists = false,
                OverwritePrompt = true,
                Filter = "3da file (*.3da)|*.3da|bcrf file (*.bcrf)|*.bcrf"
            };
            if (dialogService.ShowSaveFileDialog(saveFileDialogSettings) == true)
            {
                try
                {
                    MatrixDefinition.ProcessMatrix(_3dMatrix, saveFileDialogSettings.FileName);
                }
                catch (Exception ex)
                {
                    var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                    notifierVM.AddMessage(new UnitySC.Shared.Tools.Service.Message(MessageLevel.Error, $"Failed to export to 3D file : <{ex.Message}>"));
                }                                                             
            }
        }
        #endregion
    }
}
