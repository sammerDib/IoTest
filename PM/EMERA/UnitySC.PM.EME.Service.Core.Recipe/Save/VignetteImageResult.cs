using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public class VignetteImageResult : IAcquisitionImageResult
    {
        public VignetteImageResult(string folderName, string baseName, int nbLines, int nbColumns,
            ResultType resultType, Length pixelSize, Length waferDiameter, string acquisitionLabel)
        {
            FolderName = folderName;
            BaseName = baseName;
            NbLines = nbLines;
            NbColumns = nbColumns;
            ResultType = resultType;
            PixelSize = pixelSize;
            AcquisitionLabel = acquisitionLabel;
            WaferCenter = (int)(waferDiameter.Millimeters / 2 / PixelSize.Millimeters);
        }
        public string FolderName { get; }
        public string BaseName { get; }
        public int NbLines { get; }
        public int NbColumns { get; }
        public ResultType ResultType { get; }
        public Length PixelSize { get; }
        public int WaferCenter { get; }    
        public string AcquisitionLabel { get;}
    }
}
