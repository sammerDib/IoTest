using AcquisitionAdcExchange;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.DataLoader.Rendering
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class AcquisitionDataViewModel : ObservableRecipient
    {
        private AcquisitionData _data;
        private SelectRenderingImageViewModel _selectRenderingImageViewModel;

        public AcquisitionDataViewModel(AcquisitionData data, SelectRenderingImageViewModel selectRenderingImageViewModel)
        {
            _data = data;
            _selectRenderingImageViewModel = selectRenderingImageViewModel;
        }

        public string Filename => _data.Filename;

        /// <summary>
        /// To enable the image in the rendering
        /// </summary>
        public bool IsEnabled
        {
            get => _data.IsEnabled;
            set
            {
                if (_data.IsEnabled != value)
                {
                    _data.IsEnabled = value;
                    OnPropertyChanged();
                    _selectRenderingImageViewModel.NotifyPropertyChanged(nameof(_selectRenderingImageViewModel.IncludeAll));
                }
            }
        }

        public string ToolTip => _data?.ToString();

        /// <summary>
        /// X position mm
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y position mm
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Width mm
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Heigh mm
        /// </summary>
        public float Height { get; set; }

    }
}
