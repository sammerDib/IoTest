using System;
using System.Runtime.Serialization;

namespace AdcRobotExchange
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: Structure contenant le status d'un wafer
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class WaferReport
    {
        public enum eWaferStatus
        {
            Unprocessed = 0,
            Processing,
            Complete,
            Error
        }

        public enum eAnalysisStatus
        {
            Unanalyzed = 0,
            Aborted,
            Success,

            /// <summary>
            /// Too much defects
            /// </summary>
            Partial,

            /// <summary>
            /// Grading classified the wafer as reject
            /// </summary>
            GradingReject,

            /// <summary>
            /// Grading classified the wafer as rework
            /// </summary>
            GradingRework
        }

        [DataMember] public eWaferStatus WaferStatus;
        [DataMember] public eAnalysisStatus AnalysisStatus;

        [DataMember] public string WaferID;
        [DataMember] public string WaferGUID;
        [DataMember] public string LotID;
        [DataMember] public string SlotID;
        [DataMember] public string LoadPortID;

        /// <summary> Début du traitement par l'ADC </summary>
        [DataMember] public string ProcessStartTime;
        /// <summary> Début du traitement du foup par le robot </summary>
        [DataMember] public String JobStartTime;
        [DataMember] public String JobID;
        ///<summary> chemin et nom fichier recette ADC </summary>
        [DataMember] public string LaunchingFileName;

        [DataMember] public string KlarfFilename;
        [DataMember] public string OutputDirectory;

        [DataMember] public string ErrorMessage;
        [DataMember] public string FaultyModule;

        [DataMember] public string defectCount_tot;
    }
}
