using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Client.Proxy.Camera
{
    public class CameraInputParametersVM : ObservableObject, ICameraInputParams
    {
        private double _gain;

        public double Gain
        {
            get
            {
                return _gain;
            }
            set
            {
                if (_gain == value)
                    return;
                _gain = value;
                OnPropertyChanged();
            }
        }

        private double _expostureTimeMs;

        public double ExposureTimeMs
        {
            get
            {
                return _expostureTimeMs;
            }
            set
            {
                if (_expostureTimeMs == value)
                    return;
                _expostureTimeMs = value;
                OnPropertyChanged();
            }
        }

        private double _frameRate;

        public double FrameRate
        {
            get
            {
                return _frameRate;
            }
            set
            {
                if (_frameRate == value)
                    return;
                _frameRate = value;
                OnPropertyChanged();
            }
        }

        private string _colorMode;

        public string ColorMode
        {
            get
            {
                return _colorMode;
            }
            set
            {
                if (_colorMode == value)
                    return;
                _colorMode = value;
                OnPropertyChanged();
            }
        }
            
    }
}
