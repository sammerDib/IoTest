using System.IO;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.Shared;

namespace UnitySC.PM.DMT.Service.Host
{
    public class DMTServiceConfigurationManager : PMServiceConfigurationManager, IDMTServiceConfigurationManager
    {
        private const string CalibrationInputFolderName = "Input";
        private const string CalibrationOuputFolderName = "Output";
        private const string CalibrationOuputBackupFolderName = "OutputBackup";
        private const string CurvatureCalibrationFolderName = "CurvatureCalibrationData";

        public DMTServiceConfigurationManager(string[] args) : base(args)
        {
            // DEMETER-specific calibration
            CalibrationInputFolderPath = Path.Combine(CalibrationFolderPath, CalibrationInputFolderName);
            CalibrationOutputFolderPath = Path.Combine(CalibrationFolderPath, CalibrationOuputFolderName);
            CalibrationOutputBackupFolderPath = Path.Combine(CalibrationFolderPath, CalibrationOuputBackupFolderName);
            CurvatureCalibrationFolderPath = Path.Combine(CalibrationOutputFolderPath, CurvatureCalibrationFolderName);
        }

        public string CalibrationInputFolderPath { get; private set; }

        public string CalibrationOutputFolderPath { get; private set; }

        public string CalibrationOutputBackupFolderPath { get; private set; }

        public string AlgorithmConfigurationFilePath { get; private set; }

        public string CurvatureCalibrationFolderPath { get; private set; }

        public string FringeConfigurationFilePath { get; private set; }
    }
}

