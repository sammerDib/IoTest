using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{

    [DataContract]
    public enum XYCalibDirection
    {
        [EnumMember]
        TopBottomThenLeftRight = 0, // Top -> Bottom and Left -> Right -aka - Rows Then Columns

        [EnumMember]
        LeftRightThenTopBottom = 1, // Left -> Right and Top -> Bottom -aka - Columns Then Rows 

        [EnumMember]
        BottomTopThenRightLeft = 2, //  Bottom -> Top and  Right -> Left -aka - End Rows Then Ends Columns 
      
        [EnumMember]
        RightLeftThenBottomTop = 3, //  Right -> Left and Bottom -> Top -aka - End Columns  Then Ends Rows 
    }

    [DataContract]
    public class XYCalibrationInput
    {
        /// <summary>
        /// For serialize
        /// </summary>
        public XYCalibrationInput()
        {
        }

        public XYCalibrationInput(bool useAutoFocus, TopImageAcquisitionContext imageAcquisitionContext, string recipeName, int everyNbDie=1, XYCalibDirection scanDirection = XYCalibDirection.LeftRightThenTopBottom)
        {
            UseAutoFocus = useAutoFocus;
            ImageAcquisitionContext = imageAcquisitionContext;
            RecipeName = recipeName;
            EveryNbDie = everyNbDie;
            ScanDirection = scanDirection;
        }

        [DataMember]
        public bool UseAutoFocus { get; set; }

        [DataMember]
        public string RecipeName { get; set; }

        [DataMember]
        public int EveryNbDie { get; set; }

        [DataMember]
        public XYCalibDirection ScanDirection { get; set; } = XYCalibDirection.TopBottomThenLeftRight;

        [DataMember]
        public Length InitialShiftCenterX { get; set; } = 0.Millimeters();

        [DataMember]
        public Length InitialShiftCenterY { get; set; } = 0.Millimeters();

        [DataMember]
        public TopImageAcquisitionContext ImageAcquisitionContext { get; set; }

        [XmlIgnore]
        [DataMember]
        public CalibrationFlag CalibFlag { get; set; } = CalibrationFlag.Calib;
    }
}
