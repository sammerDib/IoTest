using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    [DataContract] 
    public class Filter : IEquatable<Filter>
    {
        public Filter()
        {
        }

        public Filter(Filter filter, Length shiftX, Length shiftY, Length pixelSize,
            FilterCalibrationStatus status)
        {
            Name = filter.Name;
            Type = filter.Type;
            Position = filter.Position;
            ShiftX = shiftX;
            ShiftY = shiftY;
            PixelSize = pixelSize;
            CalibrationStatus = status;
        }

        public Filter(string name, EMEFilter type, double position, Length shiftX, Length shiftY,
            Length pixelSize)
        {
            Name = name;
            Type = type;
            Position = position;
            ShiftX = shiftX;
            ShiftY = shiftY;
            PixelSize = pixelSize;
        }

        public Filter(string name, EMEFilter type, double position)
        {
            Name = name;
            Type = type;
            Position = position;
        }

        public Filter(string name, EMEFilter type, double position, double distanceOnFocus)
        {
            Name = name;
            Type = type;
            Position = position;
            DistanceOnFocus = distanceOnFocus;
        }

        [DataMember] 
        public string Name { get; set; }

        [DataMember] 
        public double Position { get; set; }

        [DataMember] 
        public EMEFilter Type { get; set; }

        [DataMember]
        public Length ShiftX { get; set; } = 0.0.Millimeters();
        
        [DataMember] 
        public Length ShiftY { get; set; } = 0.0.Millimeters();

        [DataMember] 
        public Length PixelSize { get; set; } = 1.0.Micrometers();

        [DataMember]
        public FilterCalibrationStatus CalibrationStatus { get; set; } = new FilterCalibrationStatus { State = FilterCalibrationState.Uncalibrated };

        [DataMember]
        public double DistanceOnFocus { get; set; }

        public bool Equals(Filter other)
        {
            return !(other is null) &&
                other.Name == Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
