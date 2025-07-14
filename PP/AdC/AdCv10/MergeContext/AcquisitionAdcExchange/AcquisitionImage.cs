using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Data.Enum;

namespace AcquisitionAdcExchange
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base des données échangées entre l'Aquisition et l'ADC
    /// NB: ce n'est pas forcément une image
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    [KnownType(typeof(AcquisitionData))]
    [KnownType(typeof(AcquisitionMilImage))]
    [KnownType(typeof(AcquisitionFullImage))]
    [KnownType(typeof(AcquisitionMosaicImage))]
    [KnownType(typeof(AcquisitionDieImage))]
    [XmlInclude(typeof(AcquisitionMilImage))]
    [XmlInclude(typeof(AcquisitionFullImage))]
    [XmlInclude(typeof(AcquisitionMosaicImage))]
    [XmlInclude(typeof(AcquisitionDieImage))]
    public abstract class AcquisitionData
    {
        [DataMember] public string Filename;
        
        [DataMember] public ResultType ResultType;
        [DataMember] public int ToolKey;
        [DataMember] public int ChamberKey;

        /// <summary> Indique si l'image doit être chargée </summary>
        [XmlIgnore] public bool IsEnabled = true;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC:
    /// Classe de base pour les images MIL
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public abstract class AcquisitionMilImage : AcquisitionData
    {
        public static MIL_ID MilSystemId;

        /// <summary>
        /// ID de buffer MIL.
        /// La proprité du buffer est transférée de l'acquoisition 
        /// à l'ADC qui libérera le buffer.
        /// </summary>
        public MIL_ID MilBufId
        {
            get { return _milBufId; }
            set
            {
                _milBufId = value;
                if (_milBufId != 0)
                {
                    Width = (int)MIL.MbufInquire(_milBufId, MIL.M_SIZE_X);
                    Height = (int)MIL.MbufInquire(_milBufId, MIL.M_SIZE_Y);
                    MilType = (int)MIL.MbufInquire(_milBufId, MIL.M_TYPE);
                }
                else
                {
                    Width = Height = 0;
                    MilType = 0;
                }
            }
        }
        private MIL_ID _milBufId;

        /// <summary> Largeur de l'image en pixel </summary>
        [DataMember] public int Width { get; set; }
        /// <summary> Hauteur de l'image en pixel </summary>
        [DataMember] public int Height { get; set; }
        /// <summary> Type d'image (8/32-bit...) </summary>
        [DataMember] public int MilType { get; set; }

        /// <summary>
        /// Mécanisme pour le tranfert WCF de l'image.
        /// WCF ne sait pas transférer des buffer MIL (ni des tableux à deux 
        /// dimensions :-) , donc on transforme le buffer MIL en tableau de byte.
        /// NB: si WCF n'est pas utilisé, MilData n'est pas utilisé non plus,
        /// donc il n'y a pas d'overhead.
        /// </summary>
        [DataMember(Order = 1)]
        public byte[] MilData
        {
            get
            {
                if (MilBufId == 0 || MilType != 8)
                    return null;
                if (_milData == null)
                {
                    _milData = new byte[Width * Height];
                    MIL.MbufGet(MilBufId, _milData);
                }
                return _milData;
            }
            set
            {
                if (_milData == value)
                    return;
                _milData = value;

                if (_milData != null)
                {
                    if (MilBufId != MIL.M_NULL)
                        throw new ApplicationException("Trying to reuse an AcquisitionImage");
                    MIL.MbufAlloc2d(MilSystemId, Width, Height, MilType + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref _milBufId);
                    MIL.MbufPut(MilBufId, _milData);
                }
            }
        }
        private byte[] _milData;

    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC: Image pleine plaque
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class AcquisitionFullImage : AcquisitionMilImage
    {
        public override string ToString()
        {
            return "FullImage";
        }
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC: Image Mosaïque
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class AcquisitionMosaicImage : AcquisitionMilImage
    {
        [DataMember] public int Line;
        [DataMember] public int Column;

        public override string ToString()
        {
            return "Mosaic-L" + Line + "-C" + Column;
        }
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC: Image d'un Die
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class AcquisitionDieImage : AcquisitionMilImage
    {
        [DataMember] public int IndexX;
        [DataMember] public int IndexY;

        /// <summary> en pixel </summary>
        [DataMember] public int X;
        /// <summary> en pixel </summary>
        [DataMember] public int Y;

        public override string ToString()
        {
            return "Die-IndexX" + IndexX + "-Y" + IndexY;
        }
    }


}
