using System.Collections.Generic;
using System.IO;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Data.Ada
{
    public class AdaWriter
    {
        private IniFile _ini;
        private string _filename;
        private int _moduleNbr;

        private int _toolKey;
        private int _chamberKey;

        private Dictionary<int, int> _sectionIDs = new Dictionary<int, int>();
        private Dictionary<int, int> _nbImages = new Dictionary<int, int>();


        public string FileName
        {
            get { return _filename; }
        }

        // Todo internal C

        public AdaWriter(int toolKey, int chamberKey, string filename)
        {
            _toolKey = toolKey;
            _chamberKey = chamberKey;

            _filename = filename;
            _ini = new IniFile(_filename + ".tmp");
            WriteHeader();
        }

        /// <summary>
        /// "Ferme" le fichier ADA, c'est-à-dire qu'on le renomme avec le vrai nom pour que l'ADC le prenne en compte.
        /// </summary>
        public void Close()
        {
            File.Move(_ini.Filename, _filename);
        }

        //=================================================================
        // [HEADER]
        //=================================================================
        private void WriteHeader()
        {
            string section = "HEADER";
            _ini.Write(section, "Version", 10);
            _ini.Write(section, "ModuleNbr", _moduleNbr);
        }

        //=================================================================
        // [INFO WAFER]
        //=================================================================
        public void WriteInfoWafer(string key, object value)
        {
            string section = "INFO WAFER";
            _ini.Write(section, key, value.ToString());
        }

        public void AddFullImage(ResultType resType, PathString image)
        {
            AddImage(resType, image.Directory + Path.DirectorySeparatorChar);

            int extId = resType.GetResultExtensionId();

            string section = $"Module {_sectionIDs[extId]}";
            _nbImages[extId]++;
            _ini.Write(section, "max_input_full_image", _nbImages[extId]);
            _ini.Write(section, $"ResultType_{_nbImages[extId] - 1}", resType);
            _ini.Write(section, $"image_{_nbImages[extId] - 1}", image.Filename);
        }

        public void AddMosaicImage(ResultType resType, int nbColumns, int nbLines, string folder, string basename)
        {
            AddImage(resType, folder);

            int extId = resType.GetResultExtensionId();

            string section = $"Module {_sectionIDs[extId]}";
            _ini.Write(section, "max_input_mosaic_image", 1);
            _ini.Write(section, $"image_mosaic_0", basename);
            _ini.Write(section, "nb_column_0", nbColumns);
            _ini.Write(section, "nb_line_0", nbLines);
        }

        public void AddDieImage(ResultType resType, string folder, string basename, string cutDieConfiguration)
        {

            AddImage(resType, folder);

            int extId = resType.GetResultExtensionId();

            string section = $"Module {_sectionIDs[extId]}";
            _ini.Write(section, "max_input_die_map_image", 1);
            _ini.Write(section, $"image_dies_name_0", basename);
            _ini.Write(section, $"CutDieConfiguration_0", cutDieConfiguration);
        }

        private void AddImage(ResultType resType, string adcInputDataFilePath)
        {
            int extId = resType.GetResultExtensionId(); //Id n'incluant pas les données  category, side & actortype
       
            bool found = _sectionIDs.TryGetValue(extId, out int sectionId);
            if (found)
                return;

            sectionId = _moduleNbr++;

            _sectionIDs[extId] = sectionId;
            _nbImages[extId] = 0;

            _ini.Write("HEADER", "ModuleNbr", _moduleNbr);

            string section = $"Module {sectionId}";
            _ini.Write(section, "ResultType", resType);
            _ini.Write(section, "ToolKey", _toolKey);
            _ini.Write(section, "ChamberKey", _chamberKey);
            _ini.Write(section, "ADCInputDataFilePath", adcInputDataFilePath);
        }

        //=================================================================
        // Matrices
        //=================================================================
        public void WriteRectangularMatrix(ResultType resType, double alignerAngle, bool waferPositionCorrected)
        {
            string section = "INFO WAFER";
            _ini.Write(section, "AlignerAngle", alignerAngle);
            _ini.Write(section, "WaferPositionCorrected", waferPositionCorrected);
        }

        public void WriteRectangularMatrix(ResultType resType, double pixelSizeX, double pixelSizeY, double waferCenterX, double waferCenterY, double angle)
        {
            string section = $"Module {_sectionIDs[resType.GetResultExtensionId()]}";
            _ini.Write(section, "pixel_size_x_0", pixelSizeX);
            _ini.Write(section, "pixel_size_y_0", pixelSizeY);
            _ini.Write(section, "wafer_center_x_0", waferCenterX);
            _ini.Write(section, "wafer_center_y_0", waferCenterY);

            _ini.Write("INFO WAFER", "AlignerAngle", angle);
        }

        public void WriteEdgeMatrix(ResultType resType, double pixelSizeX, double pixelSizeY, int notchY, double startAngle, double radiusPosition, double waferPositionOnChuckX, double waferPositionOnChuckY, int chuckOriginY)
        {
            string section = $"Module {_sectionIDs[resType.GetResultExtensionId()]}";
            _ini.Write(section, "pixel_size_x_0", pixelSizeX);
            _ini.Write(section, "pixel_size_y_0", pixelSizeY);
            _ini.Write(section, "notch_y_0", notchY);
            _ini.Write(section, "start_angle_0", startAngle);
            _ini.Write(section, "RadiusPosition", radiusPosition);
            _ini.Write(section, "chuck_origin_y_0", chuckOriginY);
        }

        public void WriteEdgeMatrix(ResultType resType, double waferPositionOnChuckX, double waferPositionOnChuckY, double alignerAngle, bool waferPositionCorrected)
        {
            string section = "INFO WAFER";
            _ini.Write(section, "AlignerAngle", alignerAngle);
            _ini.Write(section, "WaferPositionOnChuckX", waferPositionOnChuckX);
            _ini.Write(section, "WaferPositionOnChuckY", waferPositionOnChuckY);
            _ini.Write(section, "WaferPositionCorrected", waferPositionCorrected);
        }

        //=================================================================
        // Image meta-data
        //=================================================================
        public void WriteMetaData(ResultType resType, string key, object value)
        {
            string section = $"Module {_sectionIDs[resType.GetResultExtensionId()]}";
            _ini.Write(section, key, value.ToString());
        }
    }
}
