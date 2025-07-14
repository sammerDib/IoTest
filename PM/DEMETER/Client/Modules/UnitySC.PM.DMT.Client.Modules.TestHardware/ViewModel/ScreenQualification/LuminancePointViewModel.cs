using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class LuminancePointViewModel : ObservableRecipient
    {
        private float? _luminance;

        public float? Luminance
        {
            get => _luminance;
            set
            {
                if (_luminance != value)
                {
                    _luminance = value;
                    Messenger.Send(new LuminanceChange { NewLuminance = _luminance });
                    OnPropertyChanged();
                }
            }
        }

        public double TopPosition { get; set; }
        public double LeftPosition { get; set; }
        public Point Point { get; set; }
        public string Name { get; set; }
    }
}
