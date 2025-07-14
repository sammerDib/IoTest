using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Data.Ada;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Collection;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class SaveMaskInput : IFlowInput
    {
        [XmlIgnore]
        public ImageData MaskToSave { get; set; }
        
        public Side MaskSide { get; set; }
        
        public string SaveFullPath { get; set; }
        
        [XmlIgnore]
        public object AdaWriterLock { get; set; }
        
        public AdaWriter AdaWriterForSide { get; set; }

        public List<ResultType> RecipeResults { get; set; }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);
            if (RecipeResults.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.Message.Add("Cannot save mask without any ResulType");
            }

            if (AdaWriterForSide is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot save mask without an AdaWriter");
            }

            if (AdaWriterLock is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot save mask without a lock object for the AdaWriter");
            }

            if (MaskToSave is null || MaskToSave.Width == 0 || MaskToSave.Height == 0 ||
                MaskToSave.ByteArray.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.Message.Add("Cannot save mask without a mask image");
            }

            if (SaveFullPath.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.Message.Add("Cannot save mask without a save full path");
            }
            
            return result;
        }
    }
}
