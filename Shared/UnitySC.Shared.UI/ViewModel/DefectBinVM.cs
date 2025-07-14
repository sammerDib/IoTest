using System;
using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.UI.ViewModel
{
    public class DefectBinVM : ObservableObject
    {
        public event Action<object, System.Collections.Specialized.NotifyCollectionChangedEventArgs> OnDefBinChange;

        private readonly Random _rnd;

        private int _roughBin;

        public int RoughBin
        {
            get => _roughBin; set { if (_roughBin != value) { _roughBin = value; OnPropertyChanged(); OnDefBinChange?.Invoke(this, null); } }
        }

        private string _label;

        public string Label
        {
            get => _label; set { if (_label != value) { _label = value; OnPropertyChanged(); OnDefBinChange?.Invoke(this, null); } }
        }

        private Color _color;

        public Color Color
        {
            get => _color; set { if (_color != value) { _color = value; OnPropertyChanged(); OnDefBinChange?.Invoke(this, null); } }
        }

        public DefectBinVM()
        {
            _roughBin = -1;
            _label = string.Empty;
            _color = Color.Transparent;
            _rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        }

        public DefectBinVM(int roughBin, string label, int color)
        {
            _roughBin = roughBin;
            _label = label;
            _color = Color.FromArgb(color);

            _rnd = new Random(_roughBin);
        }

        public DefectBinVM(DefectBin model)
        {
            if (model != null)
            {
                _roughBin = model.RoughBin;
                _label = model.Label;
                _color = Color.FromArgb(model.Color);
                _rnd = new Random(_roughBin);
            }
            else
            {
                _roughBin = -1;
                _label = string.Empty;
                _color = Color.Transparent;
                _rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            }
        }

        private Color RandomColor(bool randomOpacity = false)
        {
            if (!randomOpacity)
                return Color.FromArgb(255, (byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255));
            return Color.FromArgb((byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255));
        }

        private AutoRelayCommand _randomColorCommand;

        public AutoRelayCommand RandomColorCommand
        {
            get
            {
                return _randomColorCommand ?? (_randomColorCommand = new AutoRelayCommand(
              () =>
              {
                  Color = RandomColor();
              },
              () => { return true; }));
            }
        }
    }
}
