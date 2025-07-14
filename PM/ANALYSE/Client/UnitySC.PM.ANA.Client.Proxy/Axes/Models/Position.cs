using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Client.Proxy.Axes.Models
{
    public class Position : ObservableObject, ICloneable
    {
        #region Fields

        private double _y;
        private double _x;
        private double _zTop;
        private double _zBottom;
        private AxisSpeed _speed;
        public readonly ReferentialBase Referential;

        #endregion Fields

        public Position(ReferentialBase referential, double x, double y, double zTop, double zBottom, AxisSpeed speed)
        {
            _x = x;
            _y = y;
            _zTop = zTop;
            _zBottom = zBottom;
            _speed = speed;
            Referential = referential;
        }

        #region Public methods

        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                OnPropertyChanged();
            }
        }

        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                OnPropertyChanged();
            }
        }

        public double ZTop
        {
            get
            {
                return _zTop;
            }
            set
            {
                _zTop = value;
                OnPropertyChanged();
            }
        }

        public double ZBottom
        {
            get
            {
                return _zBottom;
            }
            set
            {
                _zBottom = value;
                OnPropertyChanged();
            }
        }

        public AxisSpeed Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
                OnPropertyChanged();
            }
        }

        public object Clone()
        {
            return new Position(
                Referential,
                X,
                Y,
                ZTop,
                ZBottom,
                Speed
            );
        }

        public PositionBase ToAxesPosition()
        {
            return new XYZTopZBottomPosition(
                Referential,
                X,
                Y,
                ZTop,
                ZBottom);
        }

        public void UpdatePosition(double x, double y, double zTop, double zBottom)
        {
            X = x;
            Y = y;
            ZTop = zTop;
            ZBottom = zBottom;
        }

        public void UpdatePosition(XYZTopZBottomPosition position)
        {
            UpdatePosition(position.X, position.Y, position.ZTop, position.ZBottom);
        }

        #endregion Public methods
    }
}
