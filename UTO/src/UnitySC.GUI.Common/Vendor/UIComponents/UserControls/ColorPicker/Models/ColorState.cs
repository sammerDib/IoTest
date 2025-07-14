using System;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Models
{
    public struct ColorState
    {
        public ColorState(double rgbR, double rgbG, double rgbB, double a, double hsvH, double hsvS, double hsvV, double hslH, double hslS, double hslL)
        {
            _rgbR = rgbR;
            _rgbG = rgbG;
            _rgbB = rgbB;
            A = a;
            _hsvH = hsvH;
            _hsvS = hsvS;
            _hsvV = hsvV;
            _hslH = hslH;
            _hslS = hslS;
            _hslL = hslL;
        }

        public void SetArgb(double a, double r, double g, double b)
        {
            A = a;
            _rgbR = r;
            _rgbG = g;
            _rgbB = b;
            RecalculateHsvFromRgb();
            RecalculateHslFromRgb();
        }

        public double A { get; set; }

        private double _rgbR;

        public double RgbR
        {
            get => _rgbR;
            set
            {
                _rgbR = value;
                RecalculateHsvFromRgb();
                RecalculateHslFromRgb();
            }
        }

        private double _rgbG;

        public double RgbG
        {
            get => _rgbG;
            set
            {
                _rgbG = value;
                RecalculateHsvFromRgb();
                RecalculateHslFromRgb();
            }
        }

        private double _rgbB;

        public double RgbB
        {
            get => _rgbB;
            set
            {
                _rgbB = value;
                RecalculateHsvFromRgb();
                RecalculateHslFromRgb();
            }
        }

        private double _hsvH;

        public double HsvH
        {
            get => _hsvH;
            set
            {
                _hsvH = value;
                RecalculateRgbFromHsv();
                RecalculateHslFromHsv();
            }
        }

        private double _hsvS;

        public double HsvS
        {
            get => _hsvS;
            set
            {
                _hsvS = value;
                RecalculateRgbFromHsv();
                RecalculateHslFromHsv();
            }
        }

        private double _hsvV;

        public double HsvV
        {
            get => _hsvV;
            set
            {
                _hsvV = value;
                RecalculateRgbFromHsv();
                RecalculateHslFromHsv();
            }
        }

        private double _hslH;

        public double HslH
        {
            get => _hslH;
            set
            {
                _hslH = value;
                RecalculateRgbFromHsl();
                RecalculateHsvFromHsl();
            }
        }

        private double _hslS;

        public double HslS
        {
            get => _hslS;
            set
            {
                _hslS = value;
                RecalculateRgbFromHsl();
                RecalculateHsvFromHsl();
            }
        }

        private double _hslL;

        public double HslL
        {
            get => _hslL;
            set
            {
                _hslL = value;
                RecalculateRgbFromHsl();
                RecalculateHsvFromHsl();
            }
        }

        private void RecalculateHslFromRgb()
        {
            var (h, s, l) = ColorSpaceHelper.RgbToHsl(_rgbR, _rgbG, _rgbB);

            if (!h.Equals(-1))
            {
                _hslH = h;
            }

            if (!s.Equals(-1))
            {
                _hslS = s;
            }

            _hslL = l;
        }

        private void RecalculateHslFromHsv()
        {
            var (h, s, l) = ColorSpaceHelper.HsvToHsl(_hsvH, _hsvS, _hsvV);
            _hslH = h;

            if (!s.Equals(-1))
            {
                _hslS = s;
            }

            _hslL = l;
        }

        private void RecalculateHsvFromRgb()
        {
            var (h, s, v) = ColorSpaceHelper.RgbToHsv(_rgbR, _rgbG, _rgbB);

            if (!h.Equals(-1))
            {
                _hsvH = h;
            }

            if (!s.Equals(-1))
            {
                _hsvS = s;
            }

            _hsvV = v;
        }

        private void RecalculateHsvFromHsl()
        {
            var (h, s, v) = ColorSpaceHelper.HslToHsv(_hslH, _hslS, _hslL);
            _hsvH = h;

            if (!s.Equals(-1))
            {
                _hsvS = s;
            }

            _hsvV = v;
        }

        private void RecalculateRgbFromHsl()
        {
            var (r, g, b) = ColorSpaceHelper.HslToRgb(_hslH, _hslS, _hslL);
            _rgbR = r;
            _rgbG = g;
            _rgbB = b;
        }

        private void RecalculateRgbFromHsv()
        {
            var (r, g, b) = ColorSpaceHelper.HsvToRgb(_hsvH, _hsvS, _hsvV);
            _rgbR = r;
            _rgbG = g;
            _rgbB = b;
        }
    }
}
