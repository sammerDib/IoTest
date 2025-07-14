using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public class FullImageResult : IAcquisitionImageResult
    {
        public FullImageResult(string folderName, string baseName,
         ResultType resultType, Length pixelSize, Length waferDiameter, string acquisitionLabel)
        {
            FolderName = folderName;
            BaseName = baseName;
            ResultType = resultType;
            PixelSize = pixelSize;
            AcquisitionLabel = acquisitionLabel;
            WaferCenter = (int)(waferDiameter.Millimeters / 2 / PixelSize.Millimeters);
        }
        public string FolderName { get; }
        public string BaseName { get; }
        public ResultType ResultType { get; }
        public Length PixelSize { get; }
        public int WaferCenter { get; }
        public string AcquisitionLabel { get; }
    }
}
