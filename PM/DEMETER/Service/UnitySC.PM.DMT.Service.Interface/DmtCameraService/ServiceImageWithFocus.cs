using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using UnitySC.Shared.Image;

namespace UnitySC.PM.DMT.Service.Interface
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SubImageProperties
    {
        public UInt32 SubImageNumber;
        public UInt32 SubImagePositionY, SubImagePositionX;
        public UInt32 SubImageHeight, SubImageWidth;
        public double ComputedFocusQuality;
    }

    [DataContract]
    public class ServiceImageWithFocus : ServiceImage
    {
        [DataMember]
        public SubImageProperties[] SubImagesProperties { get; set; }

        public ServiceImageWithFocus(ServiceImage image, SubImageProperties[] subImagesProperties)
        {
            Data = image.Data;
            DataHeight = image.DataHeight;
            DataWidth = image.DataWidth;
            Type = image.Type;
            SubImagesProperties = subImagesProperties;
        }
    }
}
