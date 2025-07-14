using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.HLS.Service.Interface.Acquisition
{
    [DataContract]
     public class RecipeStatus
    {
        [DataContract]
        public enum RecipeState
        {
            [EnumMember] Success,
            [EnumMember] Running,
            [EnumMember] Aborted,
            [EnumMember] Failed
        };

        [DataMember]
        public RecipeState State;

        /// <summary>
        /// Nombre total d'étapes pour exécuter la recette
        /// </summary>
        [DataMember]
        public int TotalSteps;

        /// <summary>
        /// Numéro de l'étape courrante
        /// </summary>
        [DataMember]
        public int CurrentStep;

        /// <summary>
        /// Info textuelle sur l'avancement de la recette
        /// </summary>
        [DataMember]
        public string Message;
    }
}
