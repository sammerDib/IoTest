using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AcquisitionAdcExchange
{
    //=================================================================
    /// <summary>
    // Infos client sur le Wafer
    /// </summary>
    //=================================================================
    public enum eWaferInfo
    {
        Basename,
        ADCOutputDataFilePath,
        KlarfFileName,
        CarrierStatus,
        SlotID,
        LoadPortID,
        WaferGUID,
        WaferID,
        StepID,
        DeviceID,
        ToolRecipe,
        LotID,
        /// <summary> Date de début du traitement du wafer par le robot </summary>
        StartProcess,
        ADCRecipeFileName,
        EquipmentID,
        WaferType,
        AdaFilename,
        /// <summary> Date de début du traitement du foup par le robot </summary>
        JobStartTime,
        JobID,
        GradingRecipeFilePath,
        TotalDefectCount,
        WaferAngleDegrees,
        WaferXShiftum,
        WaferYShiftum,
        CorrectorsEnabled,

        /// USP - new parameters 
        /// <summary> ADC recipe guid (KeyAllVersion in Database) (added by Dataflow) [Mandatory]</summary>
        ADCRecipeGUID,
        /// <summary> ADC recipe version (added by Dataflow) [Optional] if not present use the latest one</summary>
        ADCRecipeVersion,
        /// <summary> Suffix string append to Result Type Item name in database (to distinguish front and back result for example) [Optional]</summary>
        ResultSuffix,
        ADCRecipeName
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe contenant les infos client sur le Wafer
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class WaferInfo
    {
        [DataMember] public Dictionary<eWaferInfo, string> dico = new Dictionary<eWaferInfo, string>();
        public string Basename { get { return dico[eWaferInfo.Basename]; } }
    }
}
