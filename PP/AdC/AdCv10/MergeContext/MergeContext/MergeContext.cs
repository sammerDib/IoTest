using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;

using AcquisitionAdcExchange;
using AdcBasicObjects;
using ADCEngine;
using ADCEngine.Wafers;

using AdcRobotExchange;
using AdcTools;
using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.LibMIL;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using Dto = UnitySC.DataAccess.Dto;

namespace MergeContext
{
    ///////////////////////////////////////////////////////////////////////
    // Merge des info de l'acquisition dans la recette ADC
    ///////////////////////////////////////////////////////////////////////
    public class MergeContext
    {
        //.................................................................
        // Propriétés publiques
        //.................................................................
        public Recipe Recipe;
        public WaferInfo WaferInfo => _recipeData?.WaferInfo;
        public XmlDocument Xmldoc;
        public string ErrorMessage;
        /// <summary> Si le chemin n'est pas défini on utilise la recette défini dans l'ADA </summary>
        public string LocalRecipePath;

        //.................................................................
        // Propriétés privées
        //.................................................................
        /// <summary> Répertoire des recettes quand on n'utilise pas la base de données </summary>
        private static PathString s_recipefolder = ConfigurationManager.AppSettings["Editor.RecipeFolder"]; // 

        private List<DataLoaderBase> _dataLoaderList = new List<DataLoaderBase>();
     
        private RecipeData _recipeData;

        //=================================================================
        // Constructeur
        //=================================================================
        static MergeContext()
        {
            AcquisitionMilImage.MilSystemId = Mil.Instance.HostSystem;
        }

        public MergeContext(RecipeData data)
        {
            _recipeData = data;
        }

        public MergeContext(Recipe recipe, RecipeData data)
        {
            Recipe = recipe;
            _recipeData = data;
        }

        //=================================================================
        //
        //=================================================================
        public void Merge()
        {
            bool isDatabaseConnected = true;
            try
            {
                //-------------------------------------------------------------
                // Chargement du fichier recette XML
                //-------------------------------------------------------------
                if (Recipe == null)
                    LoadRecipe();

                //-------------------------------------------------------------
                // Merge
                //-------------------------------------------------------------
                CreateWafer();
                MergeLayers();
                ReadToolName();

                // Not needed anymore machine context is no longer within database
                //MergeContextMachine();

                Recipe.OutputDir = Recipe.Wafer.GetWaferInfo(eWaferInfo.ADCOutputDataFilePath);
            }

            catch (SqlException ex)
            {
                ErrorMessage = ex.Message;
                isDatabaseConnected = false;
                throw;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                throw;
            }
            finally
            {
                if (Recipe.Toolname == null)
                {
                    SendDataBaseStatusToRobot(isDatabaseConnected);
                    SendWaferReportToRobot(_recipeData.WaferInfo);
                }
            }
        }

        //=================================================================
        //
        //=================================================================
        public void LoadRecipe()
        {
            Recipe = new Recipe();
            Xmldoc = new XmlDocument();
            PathString path;

            // Utilisation d'une recette local
            if (!string.IsNullOrEmpty(LocalRecipePath))
            {
                path = LocalRecipePath;
            }
            // Utilisisation de la recette définit dans l'ADA
            else
            {
                bool useDatabaseToGetRecipes = Convert.ToBoolean(ConfigurationManager.AppSettings["DatabaseConfig.UseDatabaseToGetRecipes"]);
                // Récupération de la recette en base de données
                if (useDatabaseToGetRecipes)
                {
                    Recipe = GetRecipeFromDataBase();
                }
                // Récupération de la recette dans le repertoire des recettes.
                else
                {
                    path = s_recipefolder / _recipeData.ADCRecipeFileName;
                    Xmldoc.Load(path);
                    Recipe.Load(Xmldoc);
                    Recipe.SetInputDir(path);
                }
            }

            
        }

        /// <summary>
        /// Obtient une recette de la base de données et la copie dans le cache si nécessaire
        /// </summary>
        /// <returns>Chemin de la recette dans le cache </returns>
        private Recipe GetRecipeFromDataBase()
        {

            Recipe recipe=new Recipe();

            string recipePath = string.Empty;

            // Recupére la derniére version d'une recette dans la base de données
            var dbRecipeProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();

            //Dto.Recipe recipe = dbRecipeProxy.GetLastRecipe(recipeName, true);
#warning ** USP **  Use a temporary constant GUIDkey fro recipe key in GetRecipeFromDataBase

            Guid guidkey = new Guid(_recipeData.ADCRecipeGuid);
            int version = 0;
            if(!int.TryParse(_recipeData.ADCRecipeVersion, out version))
                version = 0;

            Dto.Recipe dbRecipe = null;
            if (version == 0)
                dbRecipe = dbRecipeProxy.GetLastRecipe(guidkey, true);
            else
                //recipe = dbRecipeProxy.GetRecipeId(guidkey, version);
                throw new NotImplementedException("Can not use a specific recipe version");
            
             if (dbRecipe == null)
                throw new InvalidOperationException(string.Format("Cannot find the recipe ({0}) in DataBase", _recipeData.ADCRecipeGuid));

            //TODO FDS is cache usefull ? it is removed for the moment

            //string recipeName = recipe.Name;
            //string pathRecipeDataBaseCache = ConfigurationManager.AppSettings["DatabaseConfig.RecipeCache"];

            //if (!Directory.Exists(pathRecipeDataBaseCache))
            //{
            //    Directory.CreateDirectory(pathRecipeDataBaseCache);
            //}

            //string outputRecipeFolder = Path.Combine(pathRecipeDataBaseCache, string.Format("{0}v{1}", Path.GetFileNameWithoutExtension(recipe.Name), recipe.Version));

            //bool cacheIsUpToDate = false;
            //if (Directory.Exists(outputRecipeFolder))
            //{
            //    recipePath = Path.Combine(outputRecipeFolder, recipe.Name);

            //    // Suppression dans le cache si version plus récente dans la base de données
            //    if (DateTime.Compare(Directory.GetCreationTime(outputRecipeFolder), recipe.Created) < 0 || !File.Exists(recipePath))
            //    {
            //        Directory.Delete(outputRecipeFolder, true);
            //    }
            //    else
            //    {
            //        cacheIsUpToDate = true;
            //    }
            //}

            //// Mise à jour du cache
            //if (!cacheIsUpToDate)
            //{
            //    string sqlRecipeFileDirectory = ConfigurationManager.AppSettings["DatabaseConfig.AdditionnalRecipeFiles.ServerDirectory"];

            //    // Enregistrement de la recette et des fichiers externes dans le repertoire de cache

            //    /// Export the recipe and external files
            //    /// 
            //    //recipePath = RecipeService.ExportRecipeAndExternalFiles(recipe, outputRecipeFolder, true, sqlRecipeFileDirectory);
            //    // TODO RTI voir avec francis FDS
            //    recipePath = null;
            //        int nres = dbRecipeProxy.SetRecipe(new Dto.Recipe(), false);

            //}


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(dbRecipe.XmlContent);

            XmlNode xmlNode = xmldoc;

            try
            {
                recipe.Load(xmlNode);
                recipe.Name = dbRecipe.Name;
                recipe.Comment = dbRecipe.Comment;
                recipe.Created = dbRecipe.Created;
                recipe.Key = dbRecipe.KeyForAllVersion;
                recipe.Version = dbRecipe.Version;
                recipe.ActorType = (ActorType)dbRecipe.ActorType;
                recipe.CreatorChamberId = dbRecipe.CreatorChamberId;
                recipe.StepId = dbRecipe.StepId;
                recipe.IsShared = dbRecipe.IsShared;
            }
            catch (Exception)
            {

                
            }


       
            return recipe;
        }
  
        //=================================================================
        // 
        //=================================================================
        public void SetAcquitionImages(List<AcquisitionData> acquisitionImageList)
        {
            foreach (AcquisitionData acqImage in acquisitionImageList)
            {
                foreach (InspectionInputInfoBase inputInfo in Recipe.InputInfoList)
                {
                    if (inputInfo.FilterImage(acqImage))
                    {
                        inputInfo.InputDataList.Add(acqImage);
                        break;
                    }
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        public bool FeedImage(AcquisitionMilImage acqImage)
        {
            if (Recipe.HasError)
                return false;
            using (AcquisitionImageObject imageObject = new AcquisitionImageObject())
            {
                // Convertit l'Acquisition image en object ADC
                // et transfert la responsabilité du buffer MIL
                //.............................................
                imageObject.AcqData = acqImage;
                imageObject.MilImage.AttachMilId(acqImage.MilBufId, transferOnwership: true);
                acqImage.MilBufId = 0;
                acqImage.MilData = null;

                // Envoie à la recette
                //....................
                Recipe.Feed(imageObject);
                return true;
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void CreateWafer()
        {
            // Récupération du WaferType depuis la Base de Données
            //....................................................



            string waferTypeName = _recipeData.WaferInfo.dico[eWaferInfo.WaferType];

            // TODO pour l'usp le product id va pointer vers le wafer category id qui contient ces information
            // l'info ne vient plus de l'_ada  

#warning ** USP ** need to update wafertype in wafercategory - use temprorary constant natch wafer 300mm     
            Recipe.Wafer = WaferFactory.CreateWafer(waferTypeName);

            /*
            IWaferTypeService waferTypeService = ClassLocator.Default.GetInstance<IWaferTypeService>();
            waferType = waferTypeService.Get(waferTypeName);
            if (waferType == null)
                throw new ApplicationException("Unknown wafer type: " + waferTypeName);

            // Création du Wafer
            //..................
            switch (waferType.Shape)
            {
                case OldDto.WaferShape.Notch:
                    {
                        NotchWafer wafer = new NotchWafer();
                        wafer.Diameter = waferType.Diameter.Value;
                        wafer.NotchSize = waferType.NotchSize.Value;
                        Recipe.Wafer = wafer;
                        break;
                    }
                case OldDto.WaferShape.Rectangular:
                    {
                        RectangularWafer wafer = new RectangularWafer();
                        wafer.Init((float)waferType.SizeX.Value, (float)waferType.SizeY.Value);
                        Recipe.Wafer = wafer;
                        break;
                    }
                case OldDto.WaferShape.Flat:
                case OldDto.WaferShape.DoubleFlat:
                    {
                        FlatWafer wafer = new FlatWafer();
                        wafer.Diameter = waferType.Diameter.Value;
                        if (waferType.FlatVerticalX.HasValue)
                            wafer.FlatVerticalX = (float)waferType.FlatVerticalX.Value;
                        else
                            wafer.FlatVerticalX = Double.NaN;

                        if (waferType.FlatHorizontalY.HasValue)
                            wafer.FlatHorizontalY = (float)waferType.FlatHorizontalY.Value;
                        else
                            wafer.FlatHorizontalY = Double.NaN;
                        Recipe.Wafer = wafer;
                        break;
                    }
                default:
                    throw new ApplicationException("unknown wafer type: " + waferType.Name);
            }*/

            // Copie du waferInfo
            //...................
            Recipe.Wafer.waferInfo = new CustomExceptionDictionary<eWaferInfo, string>(_recipeData.WaferInfo.dico);
        }

        //=================================================================
        // Mise à jour des DataLoaders
        //=================================================================
        private void MergeLayers()
        {
            Recipe.InputInfoList = new List<InputInfoBase>();

            _dataLoaderList = Recipe.Root.Children.OfType<DataLoaderBase>().ToList();

            foreach (AcquisitionLayerInfoBase layerinfo in _recipeData.Layers)
            {
                InputInfoBase inputInfo = CreateInputInfo(layerinfo);
                Recipe.InputInfoList.Add(inputInfo);

                List<DataLoaderBase> loaders = FindMatchingDataLoaders(layerinfo);
                foreach (DataLoaderBase loader in loaders)
                    inputInfo.DataLoaderIdList.Add(loader.Id);
            }
        }

        //=================================================================
        // 
        //=================================================================
        private InputInfoBase CreateInputInfo(AcquisitionLayerInfoBase layerInfo)
        {
            //-------------------------------------------------------------
            // Création du InputInfo
            //-------------------------------------------------------------
            InspectionInputInfoBase input;


            switch (layerInfo)
            {
                case AcquisitionFullWithMaskLayerInfo layerWithMaskInfo:
                    var maskInput = new FullImageWithMaskInputInfo();
                    input = maskInput;
                    maskInput.MaskFilePath = layerWithMaskInfo.MaskFilePath;
                    break;
                case AcquisitionFullLayerInfo _:
                    input = new FullImageInputInfo();
                    break;
                case AcquisitionMosaicLayerInfo mosaicLayerInfo:
                    var mosaicInput = new MosaicInputInfo();
                    input = mosaicInput;
                    mosaicInput.Basename = mosaicLayerInfo.Basename;
                    mosaicInput.NbData = mosaicLayerInfo.NbData;
                    mosaicInput.NbLines = mosaicLayerInfo.NbLines;
                    mosaicInput.NbColumns = mosaicLayerInfo.NbColumns;
                    mosaicInput.MosaicImageWidth = mosaicLayerInfo.MosaicImageWidth;
                    mosaicInput.MosaicImageHeight = mosaicLayerInfo.MosaicImageHeight;
                    break;
                case AcquisitionDieLayerInfo dieLayerInfo:
                    var dieInput = new DieInputInfo();
                    input = dieInput;
                    dieInput.Basename = dieLayerInfo.Basename;
                    dieInput.diesxml = dieLayerInfo.DieXMLFile;
                    dieInput.MinIndexX = dieLayerInfo.MinIndexX;
                    dieInput.MaxIndexX = dieLayerInfo.MaxIndexX;
                    dieInput.MinIndexY = dieLayerInfo.MinIndexY;
                    dieInput.MaxIndexY = dieLayerInfo.MaxIndexY;

                    dieInput.DiePitchX_µm = dieLayerInfo.DiePitchX_µm;
                    dieInput.DiePicthY_µm = dieLayerInfo.DiePitchY_µm;
                    dieInput.DieOriginX_µm = dieLayerInfo.DieOriginX_µm;
                    dieInput.DieOriginY_µm = dieLayerInfo.DieOriginY_µm;
                    dieInput.DieSizeX_µm = dieLayerInfo.DieSizeX_µm;
                    dieInput.DieSizeY_µm = dieLayerInfo.DieSizeY_µm;
                    break;
                default:
                    throw new ApplicationException("unknown Layer type: " + layerInfo.GetType());
            }

            //-------------------------------------------------------------
            // IDs
            //-------------------------------------------------------------
            input.ResultType = layerInfo.ResultType;
            input.ChamberKey = layerInfo.ChamberKey;
            input.ToolKey = layerInfo.ToolKey;

            input.NbData = layerInfo.NbData;
            input.Folder = layerInfo.Folder;

            //-------------------------------------------------------------
            // Matrice
            //-------------------------------------------------------------
            input.MatrixInfo = layerInfo.MatrixInfo;
            input.MetaData.AddRange(layerInfo.MetaData);

            return input;
        }

        //=================================================================
        // 
        //=================================================================
        private List<DataLoaderBase> FindMatchingDataLoaders(AcquisitionLayerInfoBase layerinfo)
        {
            List<DataLoaderBase> list = new List<DataLoaderBase>();

            foreach (DataLoaderBase loader in _dataLoaderList)
            {
                bool b = loader.FilterImage(layerinfo.ResultType);
                if (b)
                    list.Add(loader);
            }

            return list;
        }

        //=================================================================
        // 
        //=================================================================
        [Obsolete ("No more configuration contexte are within database")]
        private void MergeContextMachine()
        {
           /* IConfigurationService confsrv = ClassLocator.Default.GetInstance<IConfigurationService>();

            for (int i = 0; i < _recipeData.Layers.Count(); i++)
            {
                AcquisitionLayerInfoBase layerinfo = _recipeData.Layers[i];

                // Récupération du Contexte Machine depuis la Base de Données
                //...........................................................
                int ChamberID = layerinfo.ChamberID;
                Dto.WaferContext waferConfig = confsrv.GetWaferContext(ChamberID, waferType.Id);

                // Merge dans les inputs de la recette
                //....................................
                Recipe.InputInfoList[i].ContextMachineList.Clear();

                foreach (Dto.Configuration config in waferConfig.Configurations)
                {
                    ContextMachine ctx = new ContextMachine()
                    {
                        Type = config.ConfigurationType.ToString(),
                        Configuration = config.ContentObject,
                    };
                    Recipe.InputInfoList[i].ContextMachineList.Add(ctx);
                }

                // Ajout de la configuration de la chambre
                Dto.ChamberConfiguration chamberConfiguration = confsrv.GetChamberConfiguration(ChamberID);
                if (chamberConfiguration != null)
                {
                    ContextMachine ctx = new ContextMachine()
                    {
                        Type = chamberConfiguration.ConfigurationType.ToString(),
                        Configuration = chamberConfiguration.ContentObject,
                    };
                    Recipe.InputInfoList[i].ContextMachineList.Add(ctx);
                }
            }*/
        }

        //=================================================================
        // 
        //=================================================================
        private void SendWaferReportToRobot(WaferInfo waferInfo)
        {
            //-------------------------------------------------------------
            // Construction de la structure
            //-------------------------------------------------------------
            WaferReport report = new WaferReport();
            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.LotID, out report.LotID);

            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.WaferID, out report.WaferID);
            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.WaferGUID, out report.WaferGUID);
            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.SlotID, out report.SlotID);
            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.LoadPortID, out report.LoadPortID);
            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.StartProcess, out report.ProcessStartTime);
            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.JobID, out report.JobID);
            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.JobStartTime, out report.JobStartTime);
            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.AdaFilename, out report.LaunchingFileName);
            waferInfo.dico.TryGetValue(AcquisitionAdcExchange.eWaferInfo.ADCOutputDataFilePath, out report.OutputDirectory);

            if (ErrorMessage != null)
            {
                report.WaferStatus = WaferReport.eWaferStatus.Error;
                report.ErrorMessage = ErrorMessage;
                report.FaultyModule = "MergeContext";
            }
            else
            {
                report.WaferStatus = WaferReport.eWaferStatus.Processing;
            }

            //-------------------------------------------------------------
            // Envoi
            //-------------------------------------------------------------
            ADC.Instance.TransferToRobotStub.TransferWaferReport(Recipe.Toolname, $"{report.JobID}{waferInfo.Basename}", report);
        }

        //=================================================================
        // 
        //=================================================================
        private void SendDataBaseStatusToRobot(bool connected)
        {
            ADC.Instance.TransferToRobotStub.TransferDataBaseStatus(Recipe.Toolname, eDataBaseType.ConfigurationDataBase, connected);
        }

        //=================================================================
        // 
        //=================================================================
        private void ReadToolName()
        {
            // NB: here we assume taht we have only one Tool Source and one Chamber Source
            // Note Rti : todo later (if needed) : handle multiple chamber source from only one tool source
            //          (what if we had multiple tool source - not in the scop right now)
            if (_recipeData.Layers.Count > 0)
            {
                var dbToolProxy = ClassLocator.Default.GetInstance<DbToolServiceProxy>();
                int toolKey = _recipeData.Layers.First().ToolKey;
                Dto.Tool tool = dbToolProxy.GetTool(toolKey);
                if (tool == null)
                    throw new ApplicationException($"No Tool found with toolKey = {toolKey}");
                Recipe.Toolname = tool.Name;
            }
        }
    }
}
