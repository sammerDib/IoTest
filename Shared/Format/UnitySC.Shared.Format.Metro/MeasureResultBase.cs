using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.Format.Metro.PeriodicStruct;
using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Format.XYCalibration;

namespace UnitySC.Shared.Format.Metro
{
    [Serializable]
    [XmlInclude(typeof(TSVResult))]
    [KnownType(typeof(TSVResult))]
    [XmlInclude(typeof(NanoTopoResult))]
    [KnownType(typeof(NanoTopoResult))]
    [XmlInclude(typeof(ThicknessResult))]
    [KnownType(typeof(ThicknessResult))]
    [XmlInclude(typeof(TopographyResult))]
    [KnownType(typeof(TopographyResult))]
    [XmlInclude(typeof(StepResult))]
    [KnownType(typeof(StepResult))]
    [XmlInclude(typeof(EdgeTrimResult))]
    [KnownType(typeof(EdgeTrimResult))]
    [XmlInclude(typeof(TrenchResult))]
    [KnownType(typeof(TrenchResult))]
    [XmlInclude(typeof(PillarResult))]
    [KnownType(typeof(PillarResult))]
    [XmlInclude(typeof(PeriodicStructResult))]
    [KnownType(typeof(PeriodicStructResult))]
    [XmlInclude(typeof(BowResult))]
    [KnownType(typeof(BowResult))]
    [XmlInclude(typeof(WarpResult))]
    [KnownType(typeof(WarpResult))]
    [XmlInclude(typeof(EdgeTrimResult))]
    [KnownType(typeof(EdgeTrimResult))]
    [XmlInclude(typeof(XYCalibrationResult))]
    [KnownType(typeof(XYCalibrationResult))]
    [DataContract]
    public abstract class MeasureResultBase
    {
        // Note : /!\ for retro compatibility we should keep "AutomationInfo" as property name
        [XmlElement("AutomationInfo")]
        [DataMember]
        public RemoteProductionResultInfo AutomationInfo { get; set; }         //Note: could be null in case of Engineering/Local Run 

        [DataMember]
        public WaferDimensionalCharacteristic Wafer { get; set; }

        [DataMember]
        public WaferMap DiesMap { get; set; }

        [XmlAttribute("Name")]
        [DataMember]
        public string Name { get; set; }

        [XmlAttribute("Info")]
        [DataMember]
        public string Information { get; set; }

        [XmlAttribute("State")]
        [DataMember]
        public GlobalState State { get; set; }

        [XmlArrayItem("Point")]
        [DataMember]
        public List<MeasurePointResult> Points { get; set; } // Non Patterné

        [XmlArrayItem("Die")]
        [DataMember]
        public List<MeasureDieResult> Dies { get; set; } // Patterné

        [XmlIgnore]
        [DataMember]
        public double QualityScore { get; protected set; }

        public object InternalTableToUpdate(object table)
        {
            throw new NotImplementedException();
        }

        public virtual List<ResultDataStats> GenerateStatisticsValues(long dbResId)
        {
            // Return empty list by default.
            return new List<ResultDataStats>();
        }

        public virtual void FillNonSerializedProperties(bool generateState, bool generateStatistics)
        {
            // Do nothing default.
        }

        public virtual byte[] ToCsv()
        {
            // Return empty CSV by default.
            return new byte[1];
        }

        public List<MeasurePointResult> GetAllPoints()
        {
            var points = new List<MeasurePointResult>();

            if (Points != null)
            {
                points.AddRange(Points.OrderBy(point => point.State));
            }

            if (Dies != null)
            {
                points.AddRange(Dies.SelectMany(die => die.Points).OrderBy(point => point.State));
            }

            return points;
        }

        public void UpdateIteration(int newIter)
        {

            if (Points != null)
            {
                foreach (var point in Points)
                    point?.UpdateIteration(newIter);
            }

            if (Dies != null)
            {
                foreach (var die in Dies)
                {
                    if (die.Points != null)
                    {
                        foreach (var point in die.Points)
                            point?.UpdateIteration(newIter);
                    }
                }
            }

        }

        protected void AppendCSVAutomInfo(ref CSVStringBuilder sbCSV)
        { 
            if(AutomationInfo == null || sbCSV == null)
                return;

            sbCSV.AppendLine("#");
            sbCSV.AppendLine("#", "Recipe", AutomationInfo.DFRecipeName);
            sbCSV.AppendLine("#", "LotID", AutomationInfo.LotID);
            sbCSV.AppendLine("#", "CarrierID", AutomationInfo.CarrierID);
            sbCSV.AppendLine("#", "ProcessJobID", AutomationInfo.ProcessJobID);
            sbCSV.AppendLine("#", "Slot", AutomationInfo.SlotID.ToString()); 
            sbCSV.AppendLine("#", "ANALYSE Rcp", AutomationInfo.PMRecipeName);
            sbCSV.AppendLine("#", "Start Time", AutomationInfo.StartRecipeTime.ToString("yyyy_MM_dd-HH:mm:ss.fff"));
            sbCSV.AppendLine("#");
        }

        protected void AppendStateAndPositionHeader(ref CSVStringBuilder sbCSV)
        {
            sbCSV.Append("No", "State", "X (mm)", "Y (mm)");
        }

        protected void AppendDieStateAndPositionHeader(ref CSVStringBuilder sbCSV)
        {
            sbCSV.Append("No", "State", "Die IndexRepeta X (mm)", "Die IndexRepeta Y (mm)", "X (mm)", "Y (mm)", "Die X (mm)", "Die Y (mm)");
        }

        protected void AppendHeader(string outputName, bool repeta, ref CSVStringBuilder sbCSV, string unit = "")
        {
            if (repeta)
            {
                sbCSV.Append($"{outputName} Avg{unit}", $"{outputName} 3sig{unit}", $"{outputName} Min{unit}", $"{outputName} Max{unit}");
            }
            else
            {
                sbCSV.Append($"{outputName}{unit}");
            }
        }

        protected void AppendStatsNanometers(MetroStatsContainer container, bool repeta, ref CSVStringBuilder sbCSV)
        {
            sbCSV.Append($"{container?.Mean?.Nanometers}");
            if (repeta)
            {
                sbCSV.Append($"{container?.Sigma3?.Nanometers}", $"{container?.Min?.Nanometers}", $"{container?.Max?.Nanometers}");
            }
        }

        protected void AppendStatsMicrometers(MetroStatsContainer container, bool repeta, ref CSVStringBuilder sbCSV)
        {
            sbCSV.Append($"{container?.Mean?.Micrometers}");
            if (repeta)
            {
                sbCSV.Append($"{container?.Sigma3?.Micrometers}", $"{container?.Min?.Micrometers}", $"{container?.Max?.Micrometers}");
            }
        }

        protected void AppendStats(MetroDoubleStatsContainer container, bool repeta, ref CSVStringBuilder sbCSV)
        {
            sbCSV.Append($"{container?.Mean}");
            if (repeta)
            {
                sbCSV.Append($"{container?.Sigma3}", $"{container?.Min}", $"{container?.Max}");
            }
        }
    }
}
