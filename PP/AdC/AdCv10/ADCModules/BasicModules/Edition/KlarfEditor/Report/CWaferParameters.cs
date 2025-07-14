using System;
using System.Collections.Generic;
using System.Drawing;

namespace BasicModules.KlarfEditor
{
    public enum enumTypeField { tfString = 0, tfBoolean, tfInteger, tfDouble }
    public class CWaferParameters
    {
        public static int NbParameters = 17;
        // From wafer or manual form
        public String FileVersion = "";
        public String FileTimeStamp = "";
        public String TiffFileName = "";
        public String InspectionStationID = "";
        public String SampleType = "";
        public String ResultTimeStamp = "";
        public String LotID = "";
        public String SampleSize = "";
        public String DeviceID = "";
        public String SetupID = "";
        public String StepID = "";
        public String SampleOrientationMarkType = "";
        public String OrientationMarkLocation = "";
        public String WaferID = "";
        public String SlotID = "";
        public String OrientationInstructions = "";
        public String InspectionOrientation = "";
        public String SampleTestPlan = "";
        public String DefectRecordSpec;
        public PointF DiePitch;
        public PointF DieOrigin;

        public List<String> ColumnNameList = new List<String>();
        public List<Point> DiesIndexes = new List<Point>();

        public static String[] WafersParametersName = new String[]{
            "FileVersion",
            "FileTimeStamp",
            "TiffFileName",
            "InspectionStationID",
            "SampleType",
            "ResultTimestamp",
            "LotID",
            "SampleSize",
            "DeviceID",
            "SetupID",
            "StepID",
            "SampleOrientationMarkType",
            "OrientationMarkLocation",
            "WaferID",
            "Slot",
            "OrientationInstructions",
            "InspectionOrientation",
            "SampleTestPlan",
            "DefectRecordSpec",
            "DiePitch",
            "DieOrigin"
        };
        public static enumTypeField[] WafersParametersType = new enumTypeField[]{
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString,
            enumTypeField.tfString
        };

        public CWaferParameters()
        {
            FileVersion = "";
            FileTimeStamp = "";
            TiffFileName = "";
            InspectionStationID = "";
            SampleType = "";
            ResultTimeStamp = "";
            LotID = "";
            SampleSize = "";
            DeviceID = "";
            SetupID = "";
            StepID = "";
            SampleOrientationMarkType = "";
            OrientationMarkLocation = "";
            WaferID = "";
            SlotID = "";
            OrientationInstructions = "";
            InspectionOrientation = "";
            SampleTestPlan = "";
            DefectRecordSpec = "";
            DiePitch = new PointF(0, 0);
            DieOrigin = new PointF(0, 0);
        }
    }
}
