using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace AcquisitionAdcExchange
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base pour les infos sur une "couche de données"
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    [KnownType(typeof(AcquisitionFullLayerInfo))]
    [KnownType(typeof(AcquisitionMosaicLayerInfo))]
    [KnownType(typeof(AcquisitionDieLayerInfo))]
    public abstract class AcquisitionLayerInfoBase
    {
        //[Obsolete][DataMember] public eModuleID ModuleID; // obsolete - todo repalce actortype / resultype
        //[Obsolete][DataMember] public int ChannelID; // obsolete - todo repalce actortype / resultype
        //[Obsolete][DataMember] public int? ChamberID; // obsolete - todo replace toolkey / chamberkey
        //[DataMember] public int ImageID;

        [DataMember] public ResultType ResultType;
        [DataMember] public int ToolKey; 
        [DataMember] public int ChamberKey; 

        [DataMember] public string Folder;
        [DataMember] public MatrixInfo MatrixInfo;

        [DataMember] public int NbData;

        /// <summary>
        /// Taille de la layer, en pixels.
        /// Pour une mosaïque, c'est la taille de l'image mosaïque complète.
        /// Pour les dies, c'est la tailles de l'image ayant servie à tailler les dies.
        /// </summary>
        [DataMember] public int TotalImageWidth;
        [DataMember] public int TotalImageHeight;

        [DataMember] public Dictionary<LayerMetaData, string> MetaData;
    }

    public enum LayerMetaData
    {
        lseMin_nm,
        lseMax_nm,
        hazeMin_ppmCal,
        hazeMax_ppmCal,
        apcfile,
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC: Layer pour les Images pleine plaque
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class AcquisitionFullLayerInfo : AcquisitionLayerInfoBase
    {
    }

    [DataContract]
    public class AcquisitionFullWithMaskLayerInfo : AcquisitionFullLayerInfo
    {
        [DataMember] public string MaskFilePath;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC: Layer pour les Images Mosaïques
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class AcquisitionMosaicLayerInfo : AcquisitionLayerInfoBase
    {
        [DataMember] public string Basename;
        [DataMember] public int NbColumns;
        [DataMember] public int NbLines;
        [DataMember] public int MosaicImageWidth;    // en pixels, taille d'un élément de la mosaïque
        [DataMember] public int MosaicImageHeight;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC: Layer pour les Die
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class AcquisitionDieLayerInfo : AcquisitionLayerInfoBase
    {
        [DataMember] public string DieXMLFile;
        [DataMember] public string Basename;

        // Index min/max des dies
        [DataMember] public int MinIndexX;
        [DataMember] public int MinIndexY;
        [DataMember] public int MaxIndexX;
        [DataMember] public int MaxIndexY;


        [DataMember] public double DiePitchX_µm;
        [DataMember] public double DiePitchY_µm;

        [DataMember] public double DieOriginX_µm;  // décalage en X du centre du DieOrigin (0,0) par au centre du Wafer
        [DataMember] public double DieOriginY_µm;  // décalage en X du centre du DieOrigin (0,0) par au centre du Wafer

        [DataMember] public double DieSizeX_µm;
        [DataMember] public double DieSizeY_µm;
    }


}
