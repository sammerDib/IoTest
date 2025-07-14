using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using AcquisitionAdcExchange;

using ADCEngine;

using LibProcessing;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace MergeContext
{
    ///////////////////////////////////////////////////////////////////////
    // Parsing du fichier .ADA
    // Le but est de créer:
    // - les structures AcquisitionAdcExchange équivalentes au contenu de l'_ada
    // - la liste des images d'aquisition (AcquisitionData)
    ///////////////////////////////////////////////////////////////////////
    public class AdaLoader
    {
        private IniFile _ada;            // Fichier ADA (format INI)

        public RecipeData RecipeData = new RecipeData();
        public List<AcquisitionData> AcqImageList = new List<AcquisitionData>();
        public bool IsOfflineMode { get; set; }

        private int _moduleNbr;

        private ResultType _resultType;
        private int _toolKey;
        private int _chamberKey;

        private static ProcessingClass s_processingClassMil = new ProcessingClassMil();
        private static ProcessingClass s_processingClassMil3D = new ProcessingClassMil3D();

        //=================================================================
        // Constructeur
        //=================================================================
        public AdaLoader(string ada)
        {
            _ada = new IniFile(ada);
            RecipeData.Layers = new List<AcquisitionLayerInfoBase>();
        }

        //=================================================================
        // Lecture de l'ADA
        //=================================================================
        public void LoadAda()
        {
            if (!File.Exists(_ada.Filename))
                throw new ApplicationException("ADA file doesn't exists: " + _ada.Filename);

            LoadHeader();
            LoadInfoWafer();

            for (int i = 0; i < _moduleNbr; i++)
                LoadModule(i);
        }

        //=================================================================
        // [HEADER]
        //=================================================================
        protected void LoadHeader()
        {
            string section = "HEADER";
            _moduleNbr = _ada.GetInt(section, "ModuleNbr");
        }

        //=================================================================
        // [INFO WAFER]
        //=================================================================
        protected void LoadInfoWafer()
        {
            string key;

            //-------------------------------------------------------------
            // Création d'un nouveau Wafer
            //-------------------------------------------------------------
            WaferInfo waferInfo = new WaferInfo();
            RecipeData.WaferInfo = waferInfo;

            Dictionary<eWaferInfo, string> info = waferInfo.dico;

            //-------------------------------------------------------------
            // Lecture des infos wafer
            //-------------------------------------------------------------
            string section = "INFO WAFER";

            var list = Enum.GetValues(typeof(eWaferInfo)).Cast<eWaferInfo>();
            foreach (var e in list)
            {
                key = e.ToString();
                string value = _ada.GetString(section, key, "");
                info.Add(e, value);
            }
            info[eWaferInfo.AdaFilename] = _ada.Filename;
            
            //-------------------------------------------------------------
            // Basename
            //-------------------------------------------------------------
            if (String.IsNullOrWhiteSpace(info[eWaferInfo.Basename]))
                info[eWaferInfo.Basename] = CreateBaseNameFromInfo(info);

            //-------------------------------------------------------------
            // Recipe Identity (Name, GUID, Version)
            //-------------------------------------------------------------
            string ADCRecipeFileName = info[eWaferInfo.ADCRecipeFileName];
            string ADCRecipeGuid = info[eWaferInfo.ADCRecipeGUID];
            string ADCRecipeVersion = info[eWaferInfo.ADCRecipeVersion];

            if (String.IsNullOrEmpty(ADCRecipeGuid) || String.IsNullOrWhiteSpace(ADCRecipeGuid))
            {
                if (!IsOfflineMode)
                {
                    throw new ApplicationException("Missing ADCRecipeGuid in ADA file: " + _ada.Filename);
                }               
            }                

            if (String.IsNullOrEmpty(ADCRecipeVersion) || String.IsNullOrWhiteSpace(ADCRecipeVersion))
                ADCRecipeVersion = "0";

            RecipeData.ADCRecipeFileName = ADCRecipeFileName;
            RecipeData.ADCRecipeGuid = ADCRecipeGuid;
            RecipeData.ADCRecipeVersion = ADCRecipeVersion;
        }


        protected String CreateBaseNameFromInfo(Dictionary<eWaferInfo, string> info)
        {
            // Base Name Format is like
            // ECLIPSE_35669_D03012017_EDGE_LBTKOSMI-U-8180-Final1_W_LP2_S1
            // <ToolName>_<UNiqueId>_D<DateTime{ddMMyyyy}>_<LotID>_<WaferIDOCR>_LP<LoadPort>_S<SlotId>

            StringBuilder sb = new StringBuilder(150);
            if (!String.IsNullOrEmpty(info[eWaferInfo.EquipmentID])) // il est où le toolName (eclispe bla bla)
                sb.Append(info[eWaferInfo.EquipmentID]);
            else
                sb.Append("4See");
            sb.Append("_");

            if (!String.IsNullOrEmpty(info[eWaferInfo.StartProcess]))
            {
                string sDt = info[eWaferInfo.StartProcess]; //03-01-2017  17:00:28
                try
                {
                    DateTime myDate = DateTime.ParseExact(sDt.TrimDuplicateSpaces(), "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    sb.Append(String.Format("D{0:ddMMyyyy}_", myDate));
                }
                catch (Exception)
                {
                    try
                    {
                        DateTime myDate = DateTime.ParseExact(sDt.TrimDuplicateSpaces(), "M-d-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        sb.Append(String.Format("D{0:ddMMyyyy}_", myDate));
                    }
                    catch
                    {
                        sb.Append(String.Format("D{0:ddMMyyyy}_", DateTime.Now));
                    }

                }
            }
            else
                sb.Append(String.Format("D{0:ddMMyyyy}_", DateTime.Now));

            if (!String.IsNullOrEmpty(info[eWaferInfo.LotID]))
                sb.Append(info[eWaferInfo.LotID]);
            else
                sb.Append("AutoLotID");
            sb.Append("_");

            if (!String.IsNullOrEmpty(info[eWaferInfo.WaferID]))
                sb.Append(info[eWaferInfo.WaferID]);
            else
                sb.Append("W");
            sb.Append("_");

            sb.Append("LP");
            if (!String.IsNullOrEmpty(info[eWaferInfo.LoadPortID]))
                sb.Append(info[eWaferInfo.LoadPortID]);
            else
                sb.Append("0");
            sb.Append("_");

            sb.Append("S");
            if (!String.IsNullOrEmpty(info[eWaferInfo.SlotID]))
                sb.Append(info[eWaferInfo.SlotID]);
            else
                sb.Append("1");

            return sb.ToString();
        }

        [Obsolete("ADCv9 IBM specific -not referenced")]
        protected String CreateBaseNameFromInfo_IBM(Dictionary<eWaferInfo, string> info)
        {
            //     Database.Service.Dto.Tool tool = ClassLocator.Default.GetInstance<Database.Service.IToolService>().GetTool(ChamberID);
            //     
            //     // Base Name Format is like
            //     // ECLIPSE_35669_D03012017_EDGE_LBTKOSMI-U-8180-Final1_W_LP2_S1
            //     // <ToolName>_<UNiqueId>_D<DateTime{ddMMyyyy}>_<LotID>_<WaferIDOCR>_LP<LoadPort>_S<SlotId>
            // 
            StringBuilder sb = new StringBuilder(80);
            //     if (!String.IsNullOrEmpty(info[eWaferInfo.EquipmentID])) // il est où le toolName (eclispe bla bla)
            //     {
            //         if (info[eWaferInfo.EquipmentID] == "DEFAULT_EQUIPMENT_ID")
            //         {
            //             if (tool != null)
            //                 sb.Append(tool.Name);
            //             else
            //             {
            //                 info[eWaferInfo.EquipmentID] = "CSM103";
            //                 sb.Append("CSM103");
            //             }
            //         }
            //         else
            //         {
            //             sb.Append(info[eWaferInfo.EquipmentID]);
            //         }
            //     }                
            //     else
            //         sb.Append("4See");
            //     sb.Append("_");
            // 
            //     if (!String.IsNullOrEmpty(info[eWaferInfo.UniqueID]))
            //         sb.Append(info[eWaferInfo.UniqueID]);
            //     else
            //         sb.Append("999999");
            //     sb.Append("_");
            // 
            //     //if (!String.IsNullOrEmpty(info[eWaferInfo.WaferID]))
            //     //    sb.Append(info[eWaferInfo.WaferID]);
            //     //else
            //     //    sb.Append("W");
            //     //sb.Append("_");
            // 
            //     if (!String.IsNullOrEmpty(info[eWaferInfo.StartProcess]))
            //     {                
            //         string sDt = info[eWaferInfo.StartProcess]; //03-01-2017  17:00:28
            //         try
            //         {
            //             DateTime myDate = DateTime.ParseExact(sDt.TrimDuplicateSpaces(), "M-d-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            //             sb.Append(String.Format("D{0:ddMMyyyy}_", myDate));
            //         }
            //         catch (Exception)
            //         {
            //             sb.Append(String.Format("D{0:ddMMyyyy}_", DateTime.Now));
            //         }
            //     }
            //     else
            //         sb.Append(String.Format("D{0:ddMMyyyy}_", DateTime.Now));
            // 
            //     if (!String.IsNullOrEmpty(info[eWaferInfo.ToolRecipe]))
            //         sb.Append(info[eWaferInfo.ToolRecipe]);
            //     else
            //         sb.Append("UnknowRecipe");
            //     sb.Append("_");
            // 
            //     if (!String.IsNullOrEmpty(info[eWaferInfo.LotID]))
            //         sb.Append(info[eWaferInfo.LotID]);
            //     else
            //         sb.Append("Unknow");
            //     sb.Append("_");            
            // 
            //     //sb.Append("LP");
            //     //if (!String.IsNullOrEmpty(info[eWaferInfo.LoadPortID]))
            //     //    sb.Append(info[eWaferInfo.LoadPortID]);
            //     //else
            //     //    sb.Append("0");
            // 
            return sb.ToString();
        }

        //=================================================================
        // [Module X]
        // NB: moduleIndex est l'index du module dans le fichier _ada, et non
        //     le ActorTypeId 
        //=================================================================
        protected void LoadModule(int moduleIndex)
        {
            string section = "Module " + moduleIndex;

            // Lecture des IDs
            //................

            _resultType = _ada.GetEnum<ResultType>(section, "ResultType");
            _toolKey = _ada.GetInt(section, "ToolKey");
            _chamberKey = _ada.GetInt(section, "ChamberKey");

            // Création des layers
            //....................
            int max_input_full_image = _ada.GetInt(section, "max_input_full_image", -1);
            for (int imageID = 0; imageID < max_input_full_image; imageID++)
                LoadFullImageModule(moduleIndex, imageID);

            int max_input_mosaic_image = _ada.GetInt(section, "max_input_mosaic_image", -1);
            for (int imageID = 0; imageID < max_input_mosaic_image; imageID++)
                LoadMosaicModule(moduleIndex, imageID);

            int max_input_die_map_image = _ada.GetInt(section, "max_input_die_map_image", -1);
            for (int imageID = 0; imageID < max_input_die_map_image; imageID++)
                LoadDieModule(moduleIndex, imageID);
        }

        //=================================================================
        // Full Image
        //=================================================================
        protected void LoadFullImageModule(int moduleIndex, int imageId)
        {
            //-------------------------------------------------------------
            // Lecture ADA
            //-------------------------------------------------------------
            string section = "Module " + moduleIndex;

            PathString image = _ada.GetString(section, "image_" + imageId);
            PathString ADCInputDataFilePath = _ada.GetString(section, "ADCInputDataFilePath");
            PathString path = ADCInputDataFilePath / image;

            //-------------------------------------------------------------
            // Création d'une image acquisition
            //-------------------------------------------------------------
            AcquisitionFullImage acqimage = new AcquisitionFullImage();
            SetLayerIds(acqimage);
            acqimage.Filename = path;

            var imgIdResType = acqimage.ResultType;
            try
            {
                imgIdResType = _ada.GetEnum<ResultType>(section, "ResultType_" + imageId);
            }
            catch { } // silent catch
            finally
            {
                acqimage.ResultType = imgIdResType;
            }
            AcqImageList.Add(acqimage);

            //-------------------------------------------------------------
            // Création de la LayerInfo
            //-------------------------------------------------------------
            PathString maskFileName = _ada.GetString(section, "MaskFileName", null);
            
            var layerInfo = maskFileName.Basename is null ? new AcquisitionFullLayerInfo() : new AcquisitionFullWithMaskLayerInfo();
            SetLayerIds(layerInfo);
            layerInfo.ResultType = acqimage.ResultType;

            layerInfo.Folder = ADCInputDataFilePath;
            ImageDiskInquire(path, out layerInfo.TotalImageWidth, out layerInfo.TotalImageHeight);
            layerInfo.NbData = 1;

            RecipeData.Layers.Add(layerInfo);

            //-------------------------------------------------------------
            // Matrice et MetaData
            //-------------------------------------------------------------
            layerInfo.MetaData = LoadModuleMetaData(moduleIndex);
            layerInfo.MatrixInfo = LoadMatrix(moduleIndex,
                                              defaultWaferCenterX: layerInfo.TotalImageWidth / 2,
                                              defaultWaferCenterY: layerInfo.TotalImageHeight / 2
                                              );
            
            //-------------------------------------------------------------
            // Handling of mask coming from DMT, if any
            //-------------------------------------------------------------
            if (layerInfo is AcquisitionFullWithMaskLayerInfo layerWithMaskInfo)
            {
                PathString maskFilePath = ADCInputDataFilePath / maskFileName;
                layerWithMaskInfo.MaskFilePath = maskFilePath;
            }
        }

        //=================================================================
        // Mosaïque
        //=================================================================
        protected void LoadMosaicModule(int moduleIndex, int imageId)
        {
            //-------------------------------------------------------------
            // Lecture ADA
            //-------------------------------------------------------------
            string section = "Module " + moduleIndex;

            int nb_line = _ada.GetInt(section, "nb_line_" + imageId);
            int nb_column = _ada.GetInt(section, "nb_column_" + imageId);
            string basename = _ada.GetString(section, "image_mosaic_" + imageId);
            PathString ADCInputDataFilePath = _ada.GetString(section, "ADCInputDataFilePath");

            //-------------------------------------------------------------
            // Images
            //-------------------------------------------------------------
            List<AcquisitionMosaicImage> mosaicList = CreateMosaicList(ADCInputDataFilePath, basename);
            if (mosaicList.Count == 0)
                throw new ApplicationException("no mosaic in folder: \"" + ADCInputDataFilePath + "\" basename: " + basename);
            AcqImageList.AddRange(mosaicList);

            //-------------------------------------------------------------
            // Création de la LayerInfo
            //-------------------------------------------------------------
            AcquisitionMosaicLayerInfo layerInfo = new AcquisitionMosaicLayerInfo();

            SetLayerIds(layerInfo);
            layerInfo.Folder = ADCInputDataFilePath;
            layerInfo.Basename = basename;
            layerInfo.NbLines = nb_line;
            layerInfo.NbColumns = nb_column;

            AcquisitionMosaicImage image = (AcquisitionMosaicImage)mosaicList[0];
            PathString path = ADCInputDataFilePath / image.Filename;
            ImageDiskInquire(path, out layerInfo.MosaicImageWidth, out layerInfo.MosaicImageHeight);
            layerInfo.TotalImageWidth = layerInfo.MosaicImageWidth * layerInfo.NbColumns;
            layerInfo.TotalImageHeight = layerInfo.MosaicImageHeight * layerInfo.NbLines;

            RecipeData.Layers.Add(layerInfo);

            //-------------------------------------------------------------
            // Matrice et MetaData
            //-------------------------------------------------------------
            layerInfo.MetaData = LoadModuleMetaData(moduleIndex);
            layerInfo.MatrixInfo = LoadMatrix(moduleIndex,
                                              defaultWaferCenterX: layerInfo.TotalImageWidth / 2,
                                              defaultWaferCenterY: layerInfo.TotalImageHeight / 2
                                              );
        }


        //=================================================================
        // Dies
        //=================================================================
        protected void LoadDieModule(int moduleIndex, int imageId)
        {
            //-------------------------------------------------------------
            // Lecture ADA
            //-------------------------------------------------------------
            string section = "Module " + moduleIndex;

            PathString ADCInputDataFilePath = _ada.GetString(section, "ADCInputDataFilePath");
            string basename = _ada.GetString(section, "image_dies_name_" + imageId);
            PathString diesxml = _ada.GetString(section, "CutDieConfiguration_" + imageId);
            diesxml = ADCInputDataFilePath / diesxml;

            //-------------------------------------------------------------
            // Images
            //-------------------------------------------------------------
            DieXmlLoader dieXmlLoader = new DieXmlLoader(_resultType, _toolKey, _chamberKey);
            dieXmlLoader.ReadDieList(diesxml, basename, (_resultType.GetActorType() == ActorType.BrightField3D) ? ".3DA" : ".BMP");

            // On ajoute les dies à la liste d'images gloable
            AcqImageList.AddRange(dieXmlLoader.DieList);

            //-------------------------------------------------------------
            // Création de la LayerInfo
            //-------------------------------------------------------------
            AcquisitionDieLayerInfo layerInfo = new AcquisitionDieLayerInfo();

            SetLayerIds(layerInfo);
            layerInfo.Folder = ADCInputDataFilePath;
            layerInfo.Basename = basename;
            layerInfo.DieXMLFile = diesxml;
            layerInfo.MinIndexX = dieXmlLoader.DieIndexes.X;
            layerInfo.MinIndexY = dieXmlLoader.DieIndexes.Y;
            layerInfo.MaxIndexX = dieXmlLoader.DieIndexes.Right;
            layerInfo.MaxIndexY = dieXmlLoader.DieIndexes.Bottom;
            layerInfo.NbData = dieXmlLoader.DieList.Count;

            layerInfo.TotalImageWidth = dieXmlLoader.TotalImageSizeX;
            layerInfo.TotalImageHeight = dieXmlLoader.TotalImageSizeY;

            layerInfo.DiePitchX_µm = dieXmlLoader.CutStepX;
            layerInfo.DiePitchY_µm = dieXmlLoader.CutStepY;

            layerInfo.DieOriginX_µm = dieXmlLoader.Die00ToWaferCenterX;
            layerInfo.DieOriginY_µm = dieXmlLoader.Die00ToWaferCenterY;

            layerInfo.DieSizeX_µm = dieXmlLoader.DieSizeX;
            layerInfo.DieSizeY_µm = dieXmlLoader.DieSizeY;

            RecipeData.Layers.Add(layerInfo);

            //-------------------------------------------------------------
            // Matrice et MetaData
            //-------------------------------------------------------------
            layerInfo.MetaData = LoadModuleMetaData(moduleIndex);
            layerInfo.MatrixInfo = LoadMatrix(moduleIndex,
                                              dieXmlLoader.WaferCenterX,
                                              dieXmlLoader.WaferCenterY,
                                              dieXmlLoader.PixelSizeX,
                                              dieXmlLoader.PixelSizeY
                                              );
        }

        //=================================================================
        // Matrice
        //=================================================================
        protected MatrixInfo LoadMatrix(int moduleIndex, double defaultWaferCenterX, double defaultWaferCenterY, double defaultPixelSizeX = double.NaN, double defaultPixelSizeY = double.NaN)
        {
            string section = "Module " + moduleIndex;

            MatrixInfo matrixInfo = new MatrixInfo();
            matrixInfo.PixelWidth = _ada.GetDouble(section, "pixel_size_x_0", defaultPixelSizeX);
            matrixInfo.PixelHeight = _ada.GetDouble(section, "pixel_size_y_0", defaultPixelSizeY);
            matrixInfo.WaferCenterX = _ada.GetDouble(section, "wafer_center_x_0", defaultWaferCenterX);
            matrixInfo.WaferCenterY = _ada.GetDouble(section, "wafer_center_y_0", defaultWaferCenterY);
            matrixInfo.WaferPositionCorrected = _ada.GetBool("INFO WAFER", "WaferPositionCorrected", false);

            matrixInfo.AlignerAngleDegree = _ada.GetDouble("INFO WAFER", "AlignerAngle", Double.NaN);


            //-------------------------------------------------------------
            // Matrice Edge
            //-------------------------------------------------------------
            int NotchY = _ada.GetInt(section, "notch_y_0", -1);
            if (NotchY >= 0)
            {
                //matrixInfo.AcquisitionStartAngleDegree = _ada.GetDouble(section, "start_angle_" + ImageID);
                matrixInfo.AcquisitionStartAngleDegree = _ada.GetDouble(section, "SensorAngle");
                matrixInfo.SensorRadiusPosition = _ada.GetDouble(section, "RadiusPosition") * 1000;    //conversion mm -> µm
                matrixInfo.WaferPositionOnChuckX = _ada.GetDouble("INFO WAFER", "WaferPositionOnChuckX");
                matrixInfo.WaferPositionOnChuckY = _ada.GetDouble("INFO WAFER", "WaferPositionOnChuckY");
                matrixInfo.ChuckOriginY = _ada.GetInt(section, "chuck_origin_y_0", -1);
                matrixInfo.NotchY = NotchY;
            }

            return matrixInfo;
        }

        //=================================================================
        // Module meta-data
        //=================================================================
        protected Dictionary<LayerMetaData, string> LoadModuleMetaData(int moduleIndex)
        {
            // Chargement des méta data
            //.........................
            Dictionary<LayerMetaData, string> metadata = new Dictionary<LayerMetaData, string>();
            string section = "Module " + moduleIndex;

            var list = Enum.GetValues(typeof(LayerMetaData)).Cast<LayerMetaData>();
            foreach (var e in list)
            {
                string key = e.ToString();
                string value = _ada.GetString(section, key, null);
                if (value != null)
                    metadata.Add(e, value);
            }

            // APC (c'est bien bidouille mais ça vient de la V8)
            //..................................................
            string apcfilename = GetApcFilename(moduleIndex);
            if (apcfilename != null)
            {
                if (File.Exists(apcfilename))
                    metadata[LayerMetaData.apcfile] = apcfilename;
            }

            return metadata;
        }

        //=================================================================
        // APC (c'est bien bidouille mais ça vient de la V8)
        //=================================================================
        protected string GetApcFilename(int moduleIndex)
        {
            string section = "Module " + moduleIndex;

            PathString ADCInputDataFilePath = _ada.GetString(section, "ADCInputDataFilePath");
            PathString KlarfFileName = _ada.GetString("INFO WAFER", "KlarfFileName", null);
            if (KlarfFileName.path == null)
                return null;

            string basename = KlarfFileName.Basename;

            if (_resultType.GetActorType() == ActorType.DEMETER)
            {
                switch (_resultType)
                {
                    case ResultType.DMT_CurvatureX_Front:
                    case ResultType.DMT_CurvatureY_Front:
                    case ResultType.DMT_AmplitudeX_Front:
                    case ResultType.DMT_AmplitudeY_Front:
                    case ResultType.DMT_CurvatureY_Back:
                    case ResultType.DMT_CurvatureX_Back:
                    case ResultType.DMT_AmplitudeX_Back:
                    case ResultType.DMT_AmplitudeY_Back:
                        basename += "_Deflectivity";
                        break;
                    case ResultType.DMT_Brightfield_Front:
                    case ResultType.DMT_Brightfield_Back:
                        basename += "_Brightfield";
                        break;
                    case ResultType.DMT_ObliqueLight_Front:
                    case ResultType.DMT_ObliqueLight_Back:
                        basename += "_ObliqueLight";
                        break;
                    default:
                        return null;
                }
            }

            PathString apcfilename = ADCInputDataFilePath / basename + ".XML";
            return apcfilename;
        }

        //=================================================================
        // 
        //=================================================================
        protected void SetLayerIds(AcquisitionLayerInfoBase layerInfo)
        {
            layerInfo.ResultType = _resultType;
            layerInfo.ChamberKey = _chamberKey;
            layerInfo.ToolKey = _toolKey;
        }

        protected void SetLayerIds(AcquisitionData acqdata)
        {
            acqdata.ResultType = _resultType;
            acqdata.ToolKey = _toolKey;
            acqdata.ChamberKey = _chamberKey;
        }


        //=================================================================
        // Lecture de la liste des images mosaïques sur le disque
        //=================================================================
        public List<AcquisitionMosaicImage> CreateMosaicList(PathString folder, string basename)
        {
            List<AcquisitionMosaicImage> mosaicList = new List<AcquisitionMosaicImage>();

            //-------------------------------------------------------------
            // Lecture de la liste des images sur le disque
            //-------------------------------------------------------------
            bool Is3d = (_resultType.GetActorType() == ActorType.BrightField3D);
            string ext = GetFileExtensionForModule(_resultType.GetActorType());
            string pattern = Is3d ? basename + "_*" + ext : basename + "_C*_L*" + ext;
            var filenames = Directory.EnumerateFiles(folder, pattern, SearchOption.AllDirectories);

            //-------------------------------------------------------------
            // Parsing du nom des fichiers images 
            //-------------------------------------------------------------
            foreach (PathString filename in filenames)
            {
                int line = 0, column = 0;

                bool bOk = true;    // Si pas OK, le fichier n'est pas une image mosaïque
                string[] items = filename.Basename.Split('_');

                // Une seule ligne en 3d.
                if (Is3d)
                {
                    bOk = bOk && (items.Count() >= 1);
                    bOk = bOk && int.TryParse(items.Last(), out column);
                }
                else
                {
                    bOk = bOk && (items.Count() >= 2);
                    bOk = bOk && int.TryParse(items[items.Length - 2].Substring(1), out column);
                    bOk = bOk && int.TryParse(items.Last().Substring(1), out line);
                }

                if (bOk)
                {
                    AcquisitionMosaicImage acqImage = new AcquisitionMosaicImage();
                    SetLayerIds(acqImage);
                    acqImage.Column = column;
                    acqImage.Line = line;
                    acqImage.Filename = filename;

                    mosaicList.Add(acqImage);
                }
            }

            return mosaicList;
        }


        //=================================================================
        /// <summary>
        /// Retourne l'extension des fichiers image en fonction du type de module.
        /// </summary>
        //=================================================================
        private string GetFileExtensionForModule(ActorType actorType)
        {
            string ext;
            switch (actorType)
            {
                case ActorType.BrightField3D:
                    ext = ".3DA";
                    break;
                case ActorType.EMERA:  // Emera c'est PL !
                    ext = ".tiff";
                    break;
                default:
                    ext = ".BMP";
                    break;
            }
            return ext;
        }

        //=================================================================
        //
        //=================================================================
        private void ImageDiskInquire(String filename, out int width, out int height)
        {
            int depth;  //unused
            if (filename.ToUpper().EndsWith(".3DA"))
                s_processingClassMil3D.ImageDiskInquire(filename, out width, out height, out depth);
            else
                s_processingClassMil.ImageDiskInquire(filename, out width, out height, out depth);
        }

        public void CompleteAda(string adcRecipeName, Guid adcRecipeGuid, ADCRemoteProductionInfo remoteProductionInfo)
        {
            _ada.Write("INFO WAFER", "ADCRecipeName", adcRecipeName);
            _ada.Write("INFO WAFER", "ADCRecipeGUID", adcRecipeGuid);
            _ada.Write("INFO WAFER", "WaferID", remoteProductionInfo.WaferID);
            _ada.Write("INFO WAFER", "JobID", remoteProductionInfo.JobID);
            _ada.Write("INFO WAFER", "LotID", remoteProductionInfo.LotID);
            _ada.Write("INFO WAFER", "SlotID", remoteProductionInfo.SlotID);
        }

        public bool ContainsRemoteProductionInfo()
        {
            // We suppose that if there is no WaferID, the remote production info are missing
            var waferID = _ada.GetString("INFO WAFER", "WaferID", "");
            return !string.IsNullOrEmpty(waferID);
        }
        public void ApplyCurrentRecipe(Recipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            RecipeData.ADCRecipeGuid = recipe.Key.ToString();
            RecipeData.WaferInfo.dico[eWaferInfo.ADCRecipeGUID] = recipe.Key.ToString();
            RecipeData.WaferInfo.dico[eWaferInfo.ADCRecipeName] = recipe.Name;
            RecipeData.WaferInfo.dico[eWaferInfo.ADCRecipeVersion] = recipe.Version.ToString();
        }
    }
}
