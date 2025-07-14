using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [DataContract]
    public class XYCalibrationData : ICalibrationData
    {
        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public string User { get; set; }

        public string Information => string.Format($"Created by {User} on {CreationDate}");

        [DataMember]
        public Angle ShiftAngle { get; set; }

        [DataMember]
        public Length ShiftX { get; set; }

        [DataMember]
        public Length ShiftY { get; set; }

        [DataMember]
        public List<Correction> Corrections { get; set; }

        [DataMember]
        public List<XYPosition> UncomputableCorrections { get; set; }

        [DataMember]
        public XYCalibrationInput Input { get; set; }

        [DataMember]
        public Length WaferCalibrationDiameter { get; set; }

        public bool IsInterpReady => (_interpShiftX != null ) && (_interpShiftY != null);

        private Interpolator2D _interpShiftX;
        private Interpolator2D _interpShiftY;
        private double _squaredWaferRadius;

        public readonly static LengthUnit WaferGridUnit = LengthUnit.Millimeter;
        public readonly static LengthUnit CorrectionUnit = LengthUnit.Micrometer;
        public readonly static Length OuterExtrapoleMargin = 2.Millimeters();

        public void UpdateSquaredRadius()
        {
            _squaredWaferRadius = Math.Pow((WaferCalibrationDiameter.Millimeters * 0.5 + OuterExtrapoleMargin.Millimeters), 2.0);
        }

        public void UpdateInterpolator(Interpolator2D interpShiftX, Interpolator2D interpShiftY)
        {
            _interpShiftX = interpShiftX;
            _interpShiftY = interpShiftY;
        }

        public Tuple<Length, Length> GetInterpolateCorrection(Length xPosition, Length yPosition)
        {
            if(!IsInterpReady)
                throw new Exception("XYCalibrationData interpolation has not been correctly pre-computed");

            double xPos = xPosition.GetValueAs(WaferGridUnit);
            double yPos = yPosition.GetValueAs(WaferGridUnit);

            if (_squaredWaferRadius < (xPos * xPos + yPos * yPos))
            {
                // outside wafer calibratiuon zone
                return new Tuple<Length, Length>(new Length(0.0, CorrectionUnit),
                                                 new Length(0.0, CorrectionUnit));
            }

            double corrX = 0.0;
            double corrY = 0.0;
            if (_interpShiftX == _interpShiftY)
            {      
                _interpShiftX.Interpolate(xPos, yPos, ref corrX, ref corrY);
            }
            else
            {
                corrX = _interpShiftX.Interpolate(xPos, yPos);
                corrY = _interpShiftY.Interpolate(xPos, yPos);
            }

            return new Tuple<Length, Length>(new Length(corrX, CorrectionUnit),  new Length(corrY, CorrectionUnit));
        }
    }

    [DataContract]
    public class Correction
    {
        [DataMember]
        public Length XTheoricalPosition { get; set; }

        [DataMember]
        public Length YTheoricalPosition { get; set; }

        [DataMember]
        public Length ShiftX { get; set; }

        [DataMember]
        public Length ShiftY { get; set; }
    }
}
