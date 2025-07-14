using System.Collections.Generic;
using System.Runtime.Serialization;

using AcquisitionAdcExchange;

using ADCEngine;

using UnitySC.Shared.Data.Enum;

namespace BasicModules.DataLoader
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base pour définir les inputs de la recette provenant de
    /// l'Acquisition "standard".
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public abstract class InspectionInputInfoBase : InputInfoBase
    {
        // obsolete
        //public eModuleID ActorTypeId;
        //public int ChannelID;
        //public int ImageID;
        //public int? ChamberID;

        public ResultType ResultType;
        public int ToolKey;
        public int ChamberKey;

        ///<summary> répertoire contenant le(s) fichier(s) image</summary>
        public string Folder;

        public MatrixInfo MatrixInfo;

        /// <summary> Liste des "images" d'une couche de l'acquisition </summary>
        public List<AcquisitionData> InputDataList = new List<AcquisitionData>();

        /// <summary> Nombre d'"images" d'une couche de l'acquisition </summary>
        public int NbData;

        ///=================================================================<summary>
        /// Teste si l'image correspond à cette input, en comparant les
        /// ActorTypeId/ChannelID/ImageID.
        ///</summary>=================================================================
        public bool FilterImage(AcquisitionData acqData)
        {
            bool b = true;
            b = b && (acqData.ResultType == ResultType);
            return b;
        }
    }
}
