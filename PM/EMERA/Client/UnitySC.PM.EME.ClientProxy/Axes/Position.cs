using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.EME.Client.Proxy.Axes
{
    public class Position : ObservableObject, ICloneable
    {
        #region Fields

        private double _y;
        private double _x;
        private double _z;
        public readonly ReferentialBase Referential;

        #endregion Fields

        public Position(ReferentialBase referential, double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
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

        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
                OnPropertyChanged();
            }
        }

        public object Clone()
        {
            return new Position(
                Referential,
                X,
                Y,
                Z);
        }

        public void UpdatePosition(PositionBase position)
        {
            switch (position)
            {
                case XYZPosition xyzPosition:
                    X = xyzPosition.X;
                    Y = xyzPosition.Y;
                    Z = xyzPosition.Z;
                    break;
                case XPosition xPosition:
                    X = xPosition.Position;
                    break;
                case YPosition yPosition:
                    Y = yPosition.Position;
                    break;
                case ZPosition zPosition:
                    Z = zPosition.Position;
                    break;                
                case XYPosition xyPosition:
                    X = xyPosition.X;
                    Y = xyPosition.Y;
                    break;
            }           
        }      

        public XYZPosition ToXyzPosition()
        {
            return new XYZPosition(Referential, X, Y, Z);
        }
        #endregion Public methods
    }
}
