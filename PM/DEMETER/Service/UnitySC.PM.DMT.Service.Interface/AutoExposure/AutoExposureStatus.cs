using System.Runtime.Serialization;

using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.AutoExposure
{
    [DataContract]
    public class AutoExposureStatus : RecipeStatus
    {
        /// <summary>
        /// Front- or back-side
        /// </summary>
        [DataMember]
        public Side Side;

        /// <summary>
        /// Temps d'exposition calculé
        /// </summary>
        [DataMember]
        public double ExposureTimeMs;
    }
}
