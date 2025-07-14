using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public interface IAcquisitionImageResult
    {
        string FolderName { get; }
        string BaseName { get; }
        ResultType ResultType { get; }
        Length PixelSize { get; }
        int WaferCenter { get; }
        string AcquisitionLabel { get; }
    }
}
