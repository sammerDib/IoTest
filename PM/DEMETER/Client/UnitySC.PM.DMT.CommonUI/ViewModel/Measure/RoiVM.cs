using System;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Measure
{
    public class RoiVM : ObservableObject
    {
        public Side CameraId;

        private readonly CameraSupervisor _cameraSupervisor;
        private bool _modified;
        
        public RoiVM(CameraSupervisor cameraSupervisor)
        {
            _cameraSupervisor = cameraSupervisor;
        }

        public double WaferRadius { get; set; } = 100000;

        private RoiType _roiType;

        public RoiType RoiType
        {
            get => _roiType;
            set
            {
                if (_roiType != value)
                {
                    Synchronize(Synchro.ToMicrons);
                    _roiType = value;
                    Synchronize(Synchro.ToPixels);
                    OnPropertyChanged();
                }
            }
        }

        public Rect Rect
        {
            get => _rect;
            set
            {
                if (_rect != value)
                {
                    _rect = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MicronX));
                    OnPropertyChanged(nameof(MicronY));
                    OnPropertyChanged(nameof(MicronWidth));
                    OnPropertyChanged(nameof(MicronHeight));
                    Synchronize(Synchro.ToPixels);
                }
            }
        }

        private Rect _rect;

        public double EdgeExclusion
        {
            get => _edgeExclusion;
            set
            {
                if (_edgeExclusion != value)
                {
                    _edgeExclusion = value;
                    OnPropertyChanged();
                    Synchronize(Synchro.ToPixels);
                }
            }
        }

        private double _edgeExclusion;

        public double ImageRoiX
        { get => _imageRoiX; set { if (_imageRoiX != value) { _modified = true; _imageRoiX = value; OnPropertyChanged(); OnPropertyChanged(nameof(ImageRoiMaxWidth)); } } }

        private double _imageRoiX;

        public double ImageRoiY
        { get => _imageRoiY; set { if (_imageRoiY != value) { _modified = true; _imageRoiY = value; OnPropertyChanged(); OnPropertyChanged(nameof(ImageRoiMaxWidth)); } } }

        private double _imageRoiY;

        public double ImageRoiWidth
        { get => _imageRoiWidth; set { if (_imageRoiWidth != value) { _modified = true; _imageRoiWidth = value; OnPropertyChanged(); } } }

        private double _imageRoiWidth;

        public double ImageRoiHeight
        { get => _imageRoiHeight; set { if (_imageRoiHeight != value) { _modified = true; _imageRoiHeight = value; OnPropertyChanged(); } } }

        private double _imageRoiHeight;

        public double ImageRoiMaxWidth => _cameraSupervisor.GetCalibratedImageSize(CameraId).Width - ImageRoiX;

        public double ImageRoiMaxHeight => _cameraSupervisor.GetCalibratedImageSize(CameraId).Height - ImageRoiY;
      
        public enum Synchro
        { ToMicrons, ToPixels };

        public void Synchronize(Synchro direction)
        {
            switch (direction)
            {
                case Synchro.ToMicrons:
                    if (!_modified)
                        return;
                    if (RoiType == RoiType.Rectangular)
                    {
                        Rect pixelRect = new Rect(ImageRoiX, ImageRoiY, ImageRoiWidth, ImageRoiHeight);
                        Rect micronRect = _cameraSupervisor.CalibratedImageToMicrons(CameraId, pixelRect);
                        Rect = micronRect;
                    }
                    break;

                case Synchro.ToPixels:
                    {
                        Rect pixelRect = _cameraSupervisor.MicronsToCalibratedImage(CameraId, SurroundingRect);
                        ImageRoiX = pixelRect.X;
                        ImageRoiY = pixelRect.Y;
                        ImageRoiWidth = pixelRect.Width;
                        ImageRoiHeight = pixelRect.Height;
                    }
                    break;

                default:
                    throw new ApplicationException("unknown synchro direction: " + direction);
            }
            _modified = false;
        }

        public double MicronX
        {
            get
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular: return _rect.X;
                    case RoiType.WholeWafer: return -WaferRadius + EdgeExclusion;
                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
            set
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular:
                        if (_rect.X != value)
                        {
                            _rect.X = value;
                            OnPropertyChanged();
                            OnPropertyChanged(nameof(Rect));
                        }
                        break;

                    case RoiType.WholeWafer:
                        throw new NotImplementedException("MicronX");
                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
        }

        public double MicronY
        {
            get
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular: return _rect.Y;
                    case RoiType.WholeWafer: return -WaferRadius + EdgeExclusion;
                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
            set
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular:
                        if (_rect.Y != value)
                        {
                            _rect.Y = value;
                            OnPropertyChanged();
                            OnPropertyChanged(nameof(Rect));
                        }
                        break;

                    case RoiType.WholeWafer:
                        throw new NotImplementedException("MicronY");
                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
        }

        public double MicronWidth
        {
            get
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular: return _rect.Width;
                    case RoiType.WholeWafer: return 2 * (WaferRadius - EdgeExclusion);
                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
            set
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular:
                        if (_rect.Width != value)
                        {
                            _rect.Width = value;
                            OnPropertyChanged();
                            OnPropertyChanged(nameof(Rect));
                        }
                        break;

                    case RoiType.WholeWafer:
                        throw new NotImplementedException("MicronWidth");
                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
        }

        public double MicronHeight
        {
            get
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular: return _rect.Height;
                    case RoiType.WholeWafer: return 2 * (WaferRadius - EdgeExclusion);
                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
            set
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular:
                        if (_rect.Height != value)
                        {
                            _rect.Height = value;
                            OnPropertyChanged();
                            OnPropertyChanged(nameof(Rect));
                        }
                        break;

                    case RoiType.WholeWafer:
                        throw new NotImplementedException("MicronHeight");
                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
        }

        public Rect SurroundingRect
        {
            get
            {
                switch (RoiType)
                {
                    case RoiType.Rectangular:
                        return Rect;

                    case RoiType.WholeWafer:
                        return RoiHelper.CreateSurroundingRectForWholeWaferRoi(WaferRadius, EdgeExclusion);

                    default:
                        throw new ApplicationException("unknown RoiType: " + RoiType);
                }
            }
        }
    }
}
