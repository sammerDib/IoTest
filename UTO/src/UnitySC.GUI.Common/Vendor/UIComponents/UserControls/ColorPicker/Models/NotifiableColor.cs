using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Models
{
    public class NotifiableColor : Notifier
    {
        private readonly IColorStateStorage _storage;

        public NotifiableColor(IColorStateStorage colorStateStorage)
        {
            _storage = colorStateStorage;
        }

        public void UpdateEverything(ColorState oldValue)
        {
            ColorState currentValue = _storage.ColorState;

            if (!currentValue.A.Equals(oldValue.A)) OnPropertyChanged(nameof(A));

            if (!currentValue.RgbR.Equals(oldValue.RgbR)) OnPropertyChanged(nameof(RgbR));
            if (!currentValue.RgbG.Equals(oldValue.RgbG)) OnPropertyChanged(nameof(RgbG));
            if (!currentValue.RgbB.Equals(oldValue.RgbB)) OnPropertyChanged(nameof(RgbB));

            if (!currentValue.HsvH.Equals(oldValue.HsvH)) OnPropertyChanged(nameof(HsvH));
            if (!currentValue.HsvS.Equals(oldValue.HsvS)) OnPropertyChanged(nameof(HsvS));
            if (!currentValue.HsvV.Equals(oldValue.HsvV)) OnPropertyChanged(nameof(HsvV));

            if (!currentValue.HslH.Equals(oldValue.HslH)) OnPropertyChanged(nameof(HslH));
            if (!currentValue.HslS.Equals(oldValue.HslS)) OnPropertyChanged(nameof(HslS));
            if (!currentValue.HslL.Equals(oldValue.HslL)) OnPropertyChanged(nameof(HslL));
        }

        public double A
        {
            get => _storage.ColorState.A * 255;
            set
            {
                var state = _storage.ColorState;
                state.A = value / 255;
                _storage.ColorState = state;
            }
        }

        public double RgbR
        {
            get => _storage.ColorState.RgbR * 255;
            set
            {
                var state = _storage.ColorState;
                state.RgbR = value / 255;
                _storage.ColorState = state;
            }
        }

        public double RgbG
        {
            get => _storage.ColorState.RgbG * 255;
            set
            {
                var state = _storage.ColorState;
                state.RgbG = value / 255;
                _storage.ColorState = state;
            }
        }

        public double RgbB
        {
            get => _storage.ColorState.RgbB * 255;
            set
            {
                var state = _storage.ColorState;
                state.RgbB = value / 255;
                _storage.ColorState = state;
            }
        }

        public double HsvH
        {
            get => _storage.ColorState.HsvH;
            set
            {
                var state = _storage.ColorState;
                state.HsvH = value;
                _storage.ColorState = state;
            }
        }

        public double HsvS
        {
            get => _storage.ColorState.HsvS * 100;
            set
            {
                var state = _storage.ColorState;
                state.HsvS = value / 100;
                _storage.ColorState = state;
            }
        }

        public double HsvV
        {
            get => _storage.ColorState.HsvV * 100;
            set
            {
                var state = _storage.ColorState;
                state.HsvV = value / 100;
                _storage.ColorState = state;
            }
        }
        public double HslH
        {
            get => _storage.ColorState.HslH;
            set
            {
                var state = _storage.ColorState;
                state.HslH = value;
                _storage.ColorState = state;
            }
        }

        public double HslS
        {
            get => _storage.ColorState.HslS * 100;
            set
            {
                var state = _storage.ColorState;
                state.HslS = value / 100;
                _storage.ColorState = state;
            }
        }

        public double HslL
        {
            get => _storage.ColorState.HslL * 100;
            set
            {
                var state = _storage.ColorState;
                state.HslL = value / 100;
                _storage.ColorState = state;
            }
        }
    }
}
