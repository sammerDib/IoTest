using UnitySC.PM.Shared;

namespace UnitySC.PM.DMT.Service.Interface
{
    public interface IDMTServiceConfigurationManager : IPMServiceConfigurationManager
    {
        string CalibrationInputFolderPath { get; }

        string CalibrationOutputFolderPath { get; }

        string CalibrationOutputBackupFolderPath { get; }

        string CurvatureCalibrationFolderPath { get; }
    }
}
