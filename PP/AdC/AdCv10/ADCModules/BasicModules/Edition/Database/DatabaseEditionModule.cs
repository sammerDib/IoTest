// decomment below if we want to use directly Result Index in case of registering issue with several PM or PP
#define USE_RESULTINDEX_PARAM   

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;
using ADCEngine.Parameters;

using AdcRobotExchange;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

namespace BasicModules.Edition.DataBase
{
    public abstract class DatabaseEditionModule : ModuleBase
    {
        protected enum ResultTypeFile
        {
            Defect_001 = 0,
            Cluster_Precarac_ASE,
            Cluster_ASO,
            Haze_AZE,   ///OBSOLETE
            Crown_CRW,
            YieldMap_YLD,
            EyeEdge_EDG,
            Globaltopo_GTR,
            HeightMeasurement_AHM,
            HazeLS_HAZE,
            StatusLot,

        };

        protected static readonly string[] Extentions = {
            ".001",     //Defect_001 = 0,
            ".ase",     //Cluster_Precarac_ASE,
            ".aso",     //Cluster_Postcarac_ASO,
            ".aze",     //Haze_AZE, //Darkfield ones ///OBSOLETE
            ".crw",     //Crown_CRW,
            ".yld",     //YieldMap_YLD,
            ".edg",     //EyeEdge_EDG,
            ".gtr",     //Globaltopo_GTR,
            ".ahm",     //HeightMeasurement_AHM
            ".haze",    //HazeLS_HAZE
            ".trf"      //StatusLot  
        };

        protected enum ResultState  // check result status ADC should process => initial status == NotProcess -- should be coherent with IDBResult.eResultState
        {
            Error = -2,      //résultat NON procéssé par l’ADC suite à une erreur ou abort => Croix Rouge par-dessus rond wafer dans vue lot, wafer non consultable
            NotProcess = -1, //le résultat a été préregister, l’ADC n’a pas encore finalisé et enregistré le fichier (vignette « engrenage » dans la vue lot, la vue wafer N’est PAS consultable)
            Ok = 0,          //resultat procéssé par l’ADC, le resultat est OK, pas de bordure
            Partiel,         //résultat procéssé par l’ADC, le résultat n’est pas complet car a été interrompu suite à un trop grand nombre de défaut par exemple(bordure pointillé Jaune)
            Rework,          //Résultat procéssé par l’ADC, le grading adc signifie que le wafer doit être rework (bordure Orange)/
            Reject,          //Résultat procéssé par l’ADC, le grading adc signifie que le wafer doit être reject (bordure Rouge)
        };

        static protected ResultType Convert(ResultTypeFile resultTypeFile)
        {
            ResultType res = ResultType.NotDefined;
            switch (resultTypeFile)
            {
                case ResultTypeFile.Defect_001: res = ResultType.ADC_Klarf; break;
                case ResultTypeFile.Cluster_Precarac_ASE: res = ResultType.ADC_ASE; break;
                case ResultTypeFile.Cluster_ASO: res = ResultType.ADC_ASO; break;
                case ResultTypeFile.Haze_AZE: res = ResultType.ADC_DFHaze; break; //OBSoLETE
                case ResultTypeFile.HazeLS_HAZE: res = ResultType.ADC_Haze; break;
                case ResultTypeFile.Crown_CRW: res = ResultType.ADC_Crown; break;
                case ResultTypeFile.YieldMap_YLD: res = ResultType.ADC_YieldMap; break;
                case ResultTypeFile.EyeEdge_EDG: res = ResultType.ADC_EyeEdge; break;
                case ResultTypeFile.Globaltopo_GTR: res = ResultType.ADC_GlobalTopo; break;
                case ResultTypeFile.HeightMeasurement_AHM: res = ResultType.ADC_HeightMes; break;
                default:
                    break;
            }
            return res;
        }

        static protected ResultType ConvertResType(int resultTypeFile)
        {
            if (!Enum.IsDefined(typeof(ResultTypeFile), resultTypeFile))
                return ResultType.NotDefined;
            return Convert((ResultTypeFile)resultTypeFile);
        }

        //=================================================================
        // Propriétés
        //=================================================================
        public bool UseManualSorting { get; set; }
        public int RunIter { get; private set; }
        public PathString DestinationDirectory { get; private set; }
        protected String ResultFileBaseName { get; private set; }
        private long InternalParentResID { get; set; } = -1;
        private long InternalResItemID { get; set; } = -1;
        /// <summary>
        /// By default, for modules who has some results to register (Editor module, will return list of their reegistered results type (cf Basic Modules)
        /// </summary>
        /// <returns></returns>
        protected abstract List<int> RegisteredResultTypes();

        protected List<ResultType> GetRegisteredResultTypes()
        {
            return RegisteredResultTypes().Select(x => ConvertResType(x)).ToList();
        }


        [ExportableParameter(false)]
        public readonly SeparatorParameter ParamDBSeparator;
        [ExportableParameter(false)]
        public readonly ReadOnlyDisplayParameter ParamResultLabelPreview;
        [ExportableParameter(false)]
        public readonly StringParameter ParamResultLabelSuffix;
#if USE_RESULTINDEX_PARAM
        [ExportableParameter(false)]
        public readonly ConditionalIntParameter ParamResultIndex;
#endif

        private String _lotId = String.Empty;
        private String _recipeId = String.Empty;
        private int _slotID = -1;

        private byte _resultIdx = 0;

        // protected bool _PlsAncestor = false;

        //=================================================================
        // Constructeur
        //=================================================================
        public DatabaseEditionModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ParamDBSeparator = new SeparatorParameter(this, "Database");
            ParamDBSeparator.IsEnabled = false;
            ParamResultLabelPreview = new ReadOnlyDisplayParameter(this, "ResultLabelPreview");
            ParamResultLabelPreview.IsEnabled = false;
            ParamResultLabelSuffix = new StringParameter(this, "ResultLabelSuffix");
            ParamResultLabelSuffix.ValueChanged += (s) => {
                ParamResultLabelPreview.DisplayString = GetDBResultLabelName(false);
            };

#if USE_RESULTINDEX_PARAM
            ParamResultIndex = new ConditionalIntParameter(this, "ResultIndex", 0, 255);
            ParamResultIndex.IsUsed = false;
            ParamResultIndex.UseChanged += (b) => { ParamResultLabelPreview.DisplayString = GetDBResultLabelName(false); };
            ParamResultIndex.ValueChanged += (s) => { ParamResultLabelPreview.DisplayString = GetDBResultLabelName(false); };
            ParamResultIndex.Value = 0;
#endif

            UseManualSorting = false;

            RunIter = -1;
            DestinationDirectory = null;


        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            RetreiveRecipeParams();

            if (!UseDatabase())
            {
                RunIter = 0;
                DestinationDirectory = Recipe.OutputDir;
                PathString filename = String.Format("{0}_{1}", Wafer.Basename, RunIter);
                ResultFileBaseName = filename.RemoveInvalidFilePathCharacters("");
                InternalParentResID = 0;
                InternalResItemID = 0;
            }
            else
            {
                // perform pre registering and update DestinationDirectory & ResultFileBaseName & RunIter & InternalParentResID && InternalResItemID
                // update result lot name
                InitResultItemsDatabase();
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void RetreiveRecipeParams()
        {
            // Recupération des données recette
            //
            _lotId = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.LotID);
            _recipeId = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.ToolRecipe);

            int nSlotID = -1;
            if (!int.TryParse(Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.SlotID), NumberStyles.Integer, CultureInfo.InvariantCulture, out nSlotID))
            {
                String sErrMsg = String.Format("Error Unable to Parse Wafer SlotId (\"{0}\")", Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.SlotID));
                logError(sErrMsg);
                throw new Exception("Init Exception : " + sErrMsg);
            }
            _slotID = nSlotID;
        }

        protected string GetDBResultLabelName(bool useADASuffix = true)
        {
            ResultType restype = ConvertResType((RegisteredResultTypes()?.FirstOrDefault()) ?? 1);
            byte index = 0; // TODO update de index
#if USE_RESULTINDEX_PARAM
            if (ParamResultIndex.IsUsed)
                index = (byte)ParamResultIndex.Value;
#endif
            var defaultlabel = restype.DefaultLabelName(index);
            var resultSuffix = ParamResultLabelSuffix.Value;
            var resultADASuffix = Wafer?.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.ResultSuffix) ?? String.Empty;

            if (string.IsNullOrEmpty(resultSuffix) || string.IsNullOrWhiteSpace(resultSuffix))
                resultSuffix = String.Empty;

            return useADASuffix ? $"{defaultlabel}{resultSuffix}{resultADASuffix}" : $"{defaultlabel}{resultSuffix}";
        }

        /*        //
                // si on veut automatiser la creation des result index d'un meme recette
                // known issue :  en cas d'utilisation d'une meme recette sur un front et back mesurement les result index ne sont pas correcte et doivent demarré != 0
                protected byte FindResultIndex()
                {
                    byte resIdx = 0;

                    // retreveive all edition module of the same type and return resindx relative to ID
                    var sameEditionModules = GetAllSameEditionModules();
                    var ll = sameEditionModules.ToList();
                    //sameEditionModules = sameEditionModules.OrderBy(m => m.Id);
                    foreach (var dbEdit in sameEditionModules.OrderBy(m => m.Id))
                    {
                        if(dbEdit.Id == Id)
                            break;
                        ++resIdx;
                    }
                    return resIdx;
                }
                public HashSet<DatabaseEditionModule> GetAllSameEditionModules()
                {
                    HashSet<DatabaseEditionModule> set = new HashSet<DatabaseEditionModule>();
                    GetAllSameEditionModule(Recipe.Root, set);
                    return set;
                }
                private bool HasSameRegistredResultTypes(DatabaseEditionModule otherModule)
                {
                    var myresTypes = GetRegisteredResultTypes();
                    var resTypes = otherModule.GetRegisteredResultTypes(); 
                    if (resTypes.Count != myresTypes.Count)
                        return false;

                    foreach (var myrestype in myresTypes)
                    {
                        if(!resTypes.Contains(myrestype))
                            return false;
                    }
                    return true;
                }
                private void GetAllSameEditionModule(ModuleBase mod , HashSet<DatabaseEditionModule> set)
                {
                    if (mod == null)
                        return;

                    if (mod is DatabaseEditionModule)
                    {
                        var dbeditmodule = (mod as DatabaseEditionModule);
                        if (HasSameRegistredResultTypes(dbeditmodule))
                            set.Add(dbeditmodule);
                    }

                    foreach (var childmodule in mod.Children)
                    {
                        GetAllSameEditionModule(childmodule, set);
                    }
                }*/

        private void InitResultItemsDatabase()
        {
            bool isResultDatabaseConnectionSuccesful = true;
            try
            {
                var dbRegister = ClassLocator.Default.GetInstance<DbRegisterResultServiceProxy>();

                ResultType restype = ConvertResType((RegisteredResultTypes()?.FirstOrDefault()) ?? 1);

                var resultLabelName = GetDBResultLabelName(useADASuffix: true);

                byte resIdx = 0; // count how many result of same type we have
#if USE_RESULTINDEX_PARAM       // or we can let user decide (the used should be only for label name purpose)
                resIdx = (byte)ParamResultIndex.Value;
#endif
                _resultIdx = resIdx;

                ResultFilterTag tag = ResultFilterTag.None; // None or Engineering

                // Récupération de la Layer
                //.........................
                List<ModuleBase> ancestors = FindAncestors(m => m is IDataLoader);
                if (ancestors.Count == 0)
                    throw new ApplicationException("Can't find DataLoader");
                
                List<IDataLoader> dataLoaders = ancestors.Cast<IDataLoader>().ToList();
                IDataLoader dataloader = dataLoaders.FirstOrDefault(dataldr => dataldr.Layer != null);
                if (dataloader == null)
                    throw new ApplicationException("Can't find DataLoader with layer");

                var imglayer = dataloader.Layer as ImageLayerBase;
                int sourcetoolKey = imglayer.ToolKey;
                int sourceChamberKey = imglayer.ChamberKey;

                var recipeInfo = new RecipeInfo()
                {
                    ActorType = ActorType.ADC,  // needed for result check not vital
                    Name = Recipe.Name,         // not vital, for information and debug logs
                    Key = Recipe.Key,           // VITAL - GUID recover from DB or given by ada via dataflow
                    Version = Recipe.Version,   // specific version of this guid recipe or 0 in case of lastest version
                };


                string processStartTime = Wafer.GetWaferInfo(eWaferInfo.StartProcess);
                DateTime dtStartTime = DateTime.Now;

                // TODO RTI je pense que cette approche pose problème, on peut mal interpréter la date
                try
                {
                    dtStartTime = DateTime.ParseExact(processStartTime.TrimDuplicateSpaces(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                catch
                {
                    try
                    {
                        dtStartTime = DateTime.ParseExact(processStartTime.TrimDuplicateSpaces(), "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        // use datetime.now
                    }
                }

                var remoteInfo = new RemoteProductionInfo()
                {
                    ProcessedMaterial = new Material()
                    {
                        SlotID = _slotID,
                        LotID = _lotId,
                        ProcessJobID = Wafer.GetWaferInfo(eWaferInfo.JobID),
                        SubstrateID = Wafer.Basename,   // BaseName  (Without Path and extension),
                    },

                    DFRecipeName = Wafer.GetWaferInfo(eWaferInfo.ToolRecipe), //Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.ToolRecipe);
                    ModuleRecipeName = recipeInfo.Name,
                    ModuleStartRecipeTime = dtStartTime,

                };

                var outPreRegister = dbRegister.PreRegisterResult(sourcetoolKey, sourceChamberKey, recipeInfo, remoteInfo, restype, resIdx, tag, resultLabelName);
                if (outPreRegister == null)
                {
                    throw new Exception($"Impossible to Pre register Result <{restype}> in database - Check DataAccess / DataBase Connection status");
                }

                DestinationDirectory = outPreRegister.ResultPathRoot;
                ResultFileBaseName = outPreRegister.ResultFileName;
                RunIter = outPreRegister.RunIter;
                InternalParentResID = outPreRegister.InternalDBResId;
                InternalResItemID = outPreRegister.InternalDBResItemId;

                string sErr;
                log("DestinationDirectory : " + DestinationDirectory);
                if (!DestinationDirectory.OptimNetworkPath(out sErr))
                {
                    logWarning("Unable To optimize network path : " + sErr);
                }
                Directory.CreateDirectory(DestinationDirectory);

            }
            catch (Exception e)
            {
                var msg = e.Message;
                isResultDatabaseConnectionSuccesful = false;
                throw;
            }
            finally
            {
                ADC.Instance.TransferToRobotStub.TransferDataBaseStatus(Recipe.Toolname, eDataBaseType.ResultDataBase, isResultDatabaseConnectionSuccesful);
            }
        }

        public virtual bool UseDatabase()
        {
            bool bRes = true;
            string sVal = ConfigurationManager.AppSettings["DatabaseResults.Use"];
            if (Boolean.TryParse(sVal, out bRes))
                return bRes;
            else
                return false;
        }

        // TO DO handle klarfextension variation -- Noth this way since ResultTypeFile ar deprecated
        // in case custommer want another extension fo klarf (IBM)
        public string GetKlarfExt()
        {
            PathString Ext;
            Ext = ConfigurationManager.AppSettings["KlarfExtension"];
            if (String.IsNullOrEmpty(Ext))
                Ext = Extentions[(int)ResultTypeFile.Defect_001];
            return Ext;
        }

        protected virtual PathString GetResultFullPathName(ResultTypeFile p_Typ)
        {
            if (DestinationDirectory == null)
                throw new ApplicationException("EditionDBRegister Init has not been called !");
            PathString Ext;
            if (p_Typ == ResultTypeFile.Defect_001)
            {
                Ext = ConfigurationManager.AppSettings["KlarfExtension"];
                if (String.IsNullOrEmpty(Ext))
                    Ext = Extentions[(int)p_Typ];
            }
            else
            {
                Ext = Extentions[(int)p_Typ];
            }

            if (!Ext.path.StartsWith("."))
                Ext = new PathString("." + Ext.path);

            string sFileName = $"{ResultFileBaseName}{Ext}";
            return (DestinationDirectory / sFileName);
        }

        protected virtual PathString GetlotStatusFileName(string orientation)
        {
            if (DestinationDirectory == null)
                throw new ApplicationException("EditionDBRegister Init has not been called !");
            string dt = DateTime.Now.ToString("MMddyyyy");
            string sOrientation_Txt = "";
            switch (orientation.ToLower())
            {
                case "front":
                    sOrientation_Txt = "_FS";
                    break;
                case "back":
                    sOrientation_Txt = "_BS";
                    break;
                case "bevel":
                    sOrientation_Txt = "_BV";
                    break;
            }

            PathString sFileName = $"Lotend-{_lotId}-{dt}{sOrientation_Txt}_{_resultIdx}_{RunIter}{Extentions[(int)ResultTypeFile.StatusLot]}";
            ResultFileBaseName = sFileName.RemoveInvalidFilePathCharacters("");
            return (DestinationDirectory / ResultFileBaseName);
        }

        protected virtual bool RegisterResultInDatabase(ResultTypeFile p_Typ, ResultState state)
        {
            if (!UseDatabase())
                return true;

            bool bSuccess = false;

            var dbRegister = ClassLocator.Default.GetInstance<DbRegisterResultServiceProxy>();

            // cas où les enum adc et db sont en cohérence -- Deprecated
            UnitySC.DataAccess.Dto.ModelDto.Enum.ResultState dbstate;
            switch (state)
            {
                case ResultState.Error:
                    dbstate = UnitySC.DataAccess.Dto.ModelDto.Enum.ResultState.Error; break;
                case ResultState.NotProcess:
                    dbstate = UnitySC.DataAccess.Dto.ModelDto.Enum.ResultState.NotProcess; break;
                case ResultState.Ok:
                    dbstate = UnitySC.DataAccess.Dto.ModelDto.Enum.ResultState.Ok; break;
                case ResultState.Partiel:
                    dbstate = UnitySC.DataAccess.Dto.ModelDto.Enum.ResultState.Partial; break;
                case ResultState.Rework:
                    dbstate = UnitySC.DataAccess.Dto.ModelDto.Enum.ResultState.Partial; break;
                case ResultState.Reject:
                    dbstate = UnitySC.DataAccess.Dto.ModelDto.Enum.ResultState.Reject; break;
                default:
                    dbstate = UnitySC.DataAccess.Dto.ModelDto.Enum.ResultState.Error; break;
            }

            if (InternalParentResID > 0 || InternalResItemID > -1)
            {
                bSuccess = dbRegister.UpdateResultState(InternalResItemID, dbstate) ?? false;
            }
            else
            {
                logWarning("Cannot update result as pre-registering failed");
            }
            return bSuccess;
        }
    }
}
