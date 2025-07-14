using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Execution
{
    /// <summary>
    /// Measure of ANALYSE recipe which is currently running.
    /// </summary>
    [DataContract]
    public class MeasurePointInfo
    {
        [DataMember]
        public string MeasureName { get; set; }

                
        [DataMember]
        public DieIndex Die { get; set; }
        
        [DataMember]
        public XYPosition Position { get; set; }

        /// <summary>
        /// Index of the measured point among all points to measure in the recipe.
        /// </summary>
        [DataMember]
        public int PointDataIndex { get; set; }

        /// <summary>
        /// Index of the measured point among all points to measure in the recipe.
        /// </summary>
        [DataMember]
        public int? RepeatIndex { get; set; }

        /// <summary>
        /// Index of the measured point among all points to measure in the recipe.
        /// </summary>
        [DataMember]
        public int? NbOfRepeat { get; set; }

        /// <summary>
        /// Index of the measured point among all points to measure in the recipe.
        /// </summary>
        //[DataMember]
        //public int? SequenceNumber { get; set; }
   
        public override string ToString()
        {   
             return $"{MeasureName}  Die Row: {Die?.Row} Die Collumn: {Die?.Column}  Index:{PointDataIndex}";
        }
    }
}
