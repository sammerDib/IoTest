using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.Shared.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Recipe
{
    [DataContract(Namespace = "")]
    [XmlInclude(typeof(BrightFieldMeasure))]
    [XmlInclude(typeof(DeflectometryMeasure))]
    [XmlInclude(typeof(HighAngleDarkFieldMeasure))]
    [XmlInclude(typeof(BackLightMeasure))]
    [XmlRoot("Recipe")]
    [Serializable]
    public class DMTRecipe : PmRecipe
    {
        public static string CurrentFileVersion = "1.0.0";
        
        [DataMember]
        public string FileVersion { get; set; } = CurrentFileVersion;

        [DataMember]
        public bool IsFSPerspectiveCalibrationUsed { get; set; }

        [DataMember]
        public bool IsBSPerspectiveCalibrationUsed { get; set; }

        [DataMember]
        public bool AreAcquisitionsSavedInDatabase { get; set; }

        [DataMember]
        public List<MeasureBase> Measures { get; set; }

        [DataMember]
        [XmlIgnore]
        public Step Step { get; set; }

        public bool GetIsPerspectiveCalibrationUsed(Side side)
        {
            if (side == Side.Front)
                return IsFSPerspectiveCalibrationUsed;

            return IsBSPerspectiveCalibrationUsed;
        }

        public void SetIsPerspectiveCalibrationUsed(Side side, bool isPerspectiveCalibrationUsed)
        {
            if (side == Side.Front)
                IsFSPerspectiveCalibrationUsed = isPerspectiveCalibrationUsed;
            else
                IsBSPerspectiveCalibrationUsed = isPerspectiveCalibrationUsed;
        }
    }
}
