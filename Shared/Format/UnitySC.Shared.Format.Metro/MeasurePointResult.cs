using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.Format.Metro.PeriodicStruct;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Data.DVID;


namespace UnitySC.Shared.Format.Metro
{
    [Serializable]
    [XmlInclude(typeof(TSVPointResult))]
    [KnownType(typeof(TSVPointResult))]
    [XmlInclude(typeof(NanoTopoPointResult))]
    [KnownType(typeof(NanoTopoPointResult))]
    [XmlInclude(typeof(ThicknessPointResult))]
    [KnownType(typeof(ThicknessPointResult))]
    [XmlInclude(typeof(TopographyPointResult))]
    [KnownType(typeof(TopographyPointResult))]
    [XmlInclude(typeof(TrenchPointResult))]
    [KnownType(typeof(TrenchPointResult))]
    [XmlInclude(typeof(PillarPointResult))]
    [KnownType(typeof(PillarPointResult))]
    [XmlInclude(typeof(PeriodicStructPointResult))]
    [KnownType(typeof(PeriodicStructPointResult))]
    [XmlInclude(typeof(BowPointResult))]
    [KnownType(typeof(BowPointResult))]
    [XmlInclude(typeof(WarpPointResult))]
    [KnownType(typeof(WarpPointResult))]
    [XmlInclude(typeof(StepPointResult))]
    [KnownType(typeof(StepPointResult))]
    [XmlInclude(typeof(EdgeTrimPointResult))]
    [KnownType(typeof(EdgeTrimPointResult))]
    [XmlInclude(typeof(TrenchPointResult))]
    [KnownType(typeof(TrenchPointResult))]

    [DataContract]
    public class MeasurePointResult
    {
        [XmlAttribute("SiteId")]
        [DataMember]
        public int SiteId { get; set; }

        [XmlAttribute("X")]
        [DataMember]
        public double XPosition { get; set; }

        [XmlAttribute("Y")]
        [DataMember]
        public double YPosition { get; set; }

        [XmlAttribute("State")]
        [DataMember]
        public MeasureState State { get; set; }

        [XmlAttribute("Message")]
        [DataMember]
        public string Message { get; set; }

        [XmlElement("Data")]
        [DataMember]
        public List<MeasurePointDataResultBase> Datas { get; set; } = new List<MeasurePointDataResultBase>();

        [XmlIgnore]
        [DataMember]
        public bool IsSubMeasurePoint { get; set; } = false;

        #region Generated Properties

        [XmlIgnore]
        [DataMember]
        public double WaferRelativeXPosition { get; private set; }

        [XmlIgnore]
        [DataMember]
        public double WaferRelativeYPosition { get; private set; }

        [XmlIgnore]
        [DataMember]
        public double QualityScore { get; protected set; }

        #endregion
        
        internal Length Average<TDataType>(List<TDataType> subDatas, Expression<Func<Length>> expr) where TDataType : MeasurePointDataResultBase
        {
            return Average(subDatas, expr.ReturnType.Name);
        }

        internal Length Average<TDataType>(List<TDataType> subDatas, string subDatapropertyName) where TDataType : MeasurePointDataResultBase
        {
            try
            {
                var toComputes = new List<Length>();
                foreach (var data in subDatas.Where(x => x.State != MeasureState.NotMeasured))
                {
                    var propertyValue = data.GetType().GetProperty(subDatapropertyName)?.GetValue(data, null) as Length;
                    if (!(propertyValue is null))
                        toComputes.Add(propertyValue);
                }
                if (toComputes.Any())
                {
                    var unit = toComputes.First().Unit;
                    return new Length(toComputes.Average(x => x.ToUnit(unit).Value), unit);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error during the calculation of average for {subDatapropertyName}", ex);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder($"SiteId: {SiteId} X: {XPosition} Y: {YPosition} State: {State} ");
            foreach (var data in Datas)
            {
                sb.AppendLine(data.ToString());
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// Calculates and stores the positions relative to the wafer of the points.
        /// </summary>
        public void ComputeWaferRelativePosition()
        {
            WaferRelativeXPosition = XPosition;
            WaferRelativeYPosition = YPosition;
        }

        /// <summary>
        /// Calculates and stores the relative wafer positions of the points from the position of a die.
        /// </summary>
        public void ComputeWaferRelativePosition(double dieX, double dieY, double dieHeight)
        {
            double pointX = dieX + XPosition;
            // Subtract die height because the dies are positioned in top left. 
            double pointY = dieY + YPosition - dieHeight;

            WaferRelativeXPosition = pointX;
            WaferRelativeYPosition = pointY;
        }

        public virtual void GenerateStats()
        {
            // By default do nothing
        }

        public virtual void ComputeQualityScoreFromDatas()
        {
            // By default only set Lower QualityScore from datas 
            if (Datas == null || Datas.Count == 0)
                QualityScore = 0.0;
            else
                QualityScore = Datas.Min(x => x?.QualityScore ?? 1.0);
        }
  
        public virtual List<ResultValue> GetResultValues()
        {
            return null;
        }

        public void UpdateIteration(int newIter)
        {
            for(int i=0; i< Datas.Count; i++)
            {
                Datas[i].NewIterInPath(newIter);
            } 
        }
    }
}
