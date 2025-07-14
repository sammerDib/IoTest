using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Execution
{
    /// <summary>
    /// Progress state of an ANALYSE recipe execution.
    /// </summary>
    [DataContract]
    public class RecipeProgress
    {
        [DataMember]
        public TimeSpan RemainingTime { get; set; }

        /// <summary>
        /// Number of points not already measured.
        /// </summary>
        [DataMember]
        public int NbRemainingPoints { get; set; }

        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Status of the execution.
        /// </summary>
        [DataMember]
        public RecipeProgressState RecipeProgressState { get; set; }

        [DataMember]
        public string ResultFolderPath { get; set; }

        /// <summary>
        /// Currently running measure.
        /// </summary>
        [DataMember]
        public MeasurePointInfo PointMeasureStarted { get; set; }

        [DataMember]
        public RecipeInfo RunningRecipeInfo { get; set; }


        [DataMember]
        public RemoteProductionInfo RemoteInfo { get; set; }

        public override string ToString()
        {
            return $"[{RecipeProgressState}] {Message} : RemainingTime {RemainingTime} NbRemainingPoints {NbRemainingPoints} PointStarted {PointMeasureStarted} ";
        }
    }
}
