using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.Classification;
using BasicModules.Sizing;

using Format001;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

namespace BasicModules.PLSEditor
{


    ///////////////////////////////////////////////////////////////////////
    //PLS Editor : Prior Level Subtraction - IBM
    ///////////////////////////////////////////////////////////////////////
    [Obsolete("IBM acv9 specific, need to refacto full Feature within ADC USP when we have time")]
    // outils permettant de comparer le dernier resultat d'un klar d'un meme foup / wafer slot et d'en deduire ce sui a changé (add, remove) ou pas  (common)
    // note de RTI : il faut le penser comme un nouveau type de resulat klarf ! ainsi l'on pourra comparer des recettes avec des klarf classifié et des klarf PLS (add, common, remove)
    // necéssité que dataaccess puisse etre en pesure de retourné le chemein du dernier resulat d'un foup/lot/recipe pour un meme slot
    // si le resulat PLS précédent n'exitse pas on ne fait pas de comparaison.
    // Original Module Author: fréderic Dulou
    // Historique : module préalablement déstiné à etre un module edition mais contrepierd il s'est retourvé avant un editeur klarf classique
    // resumé : le module va  chercher le dernier klarf de cette recette et de ce foup.lot.slot (qui ne soit pas un klarf PLS), load le klarf et générer des cluster de defect (préjob)
    // puis compare avec les cluster de la recette courante en fin d'execution (post job)
    // le klarf qui suit le module de PLSeditor aura un suffixe (_PLS), seul les ADD, COMMON o REMOVE defect sont inscrit (selon les parametres, car on ne peux vouloir voir que les add para exemple)

    public class PLSEditorModule : QueueModuleBase<Cluster>
    {

        private DataKlarf _preKlarfData;         // Donnée du klarf pré job
        private string _csNamePreKlarfFile;      // Nom du fichier klarf pré job
        private List<PrmDefect> _preData;        // list des defects  du klarf pré job
        private List<PrmDefect> _postData;       // list des defects  du klarf post job
        private List<PrmDefect> _addedData;
        private List<PrmDefect> _commonData;
        private List<PrmDefect> _removedData;
        private eSizingType _sizingType;

        private LayerBase _mylayer;

        //private static UserControl _klarfRenderingcontrol;
        private DataKlarf _dataKlarf;

        public DefectClass DefectAdders = new DefectClass();
        public DefectClass DefectCommons = new DefectClass();
        public DefectClass DefectRemoved = new DefectClass();
        public List<string> ListDefectLabel = new List<string>();

        private RangeComparator _comparatorAdders = new RangeComparator();
        private RangeComparator _comparatorCommons = new RangeComparator();
        private RangeComparator _comparatorRemoved = new RangeComparator();

        public readonly DoubleParameter ParamfProximity_um;
        public readonly BoolParameter ParamCenteringReport;

        [System.Reflection.Obfuscation(Exclude = true)]
        public enum ePLSChoice
        {
            [Description("All")]
            All,
            [Description("Adders")]
            Adders,
            [Description("Commons")]
            Commons,
            [Description("Removed")]
            Removed,
        }

        public EnumParameter<ePLSChoice> paramPlsChoice;

        public int RunIter { get; private set; }
        public PathString PreKlarfDirectory { get; private set; }

        private LayerBase _layer;


        protected readonly String[] sDefectTypeCategories =
        {
            "DEFECTID",
            "XREL",
            "YREL",
            "XINDEX",
            "YINDEX",
            "XSIZE",
            "YSIZE",
            "DEFECTAREA",
            "DSIZE",
            "CLASSNUMBER",
            "TEST",
            "CLUSTERNUMBER",
            "ROUGHBINNUMBER",
            "FINEBINNUMBER",
            "REVIEWSAMPLE",
            "IMAGECOUNT",
            "IMAGELIST"
        };

        private List<bool> _flagIndexForAddedList = new List<bool>();

        private List<Cluster> _listePostCluster = new List<Cluster>();

        //=================================================================
        // Constructeur
        //=================================================================
        public PLSEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ParamfProximity_um = new DoubleParameter(this, "Proximity um ", 0);
            ParamCenteringReport = new BoolParameter(this, "CenteringReport");
            paramPlsChoice = new EnumParameter<ePLSChoice>(this, "Defects choice");
            //paramRoughBins = new KlarfEditorRoughBinParameter(this, "DefectRoughBins");
            string SERVERNAME = ConfigurationManager.AppSettings["DatabaseResults.ServerName"];
            if (String.IsNullOrEmpty(SERVERNAME) || String.IsNullOrWhiteSpace(SERVERNAME))
                throw new Exception("Empty DatabaseResults.ServerName, check app.config.xml file");

            //DataBaseAccess = (UseUnityControlDataAccess() ? (IDBResult)new DBResultsUnityControl(SERVERNAME) : (IDBResult)new DBAdcResultsv3(SERVERNAME));
            //DataBaseAccess.OnDatabaseLog_Debug += new DatabaseResults.DelegLog(logDebug);
            //DataBaseAccess.OnDatabaseLog_Info += new DatabaseResults.DelegLog(log);
            //DataBaseAccess.OnDatabaseLog_Warning += new DatabaseResults.DelegLog(logWarning);
            //DataBaseAccess.OnDatabaseLog_Error += new DatabaseResults.DelegLog(logError);

            // Create Defect classes

            DefectAdders.label = "Pls-Adders";
            _comparatorAdders.Min = 0;
            DefectAdders.compartorList.Add(_comparatorAdders);

            DefectCommons.label = "Pls-Common";
            _comparatorCommons.Min = 0;
            DefectCommons.compartorList.Add(_comparatorCommons);

            DefectRemoved.label = "Pls-Remove";
            _comparatorRemoved.Min = 0;
            DefectRemoved.compartorList.Add(_comparatorRemoved);

            if ((paramPlsChoice == ePLSChoice.Adders) || (paramPlsChoice == ePLSChoice.All))
                ListDefectLabel.Add(DefectAdders.label);
            if ((paramPlsChoice == ePLSChoice.Commons) || (paramPlsChoice == ePLSChoice.All))
                ListDefectLabel.Add(DefectCommons.label);
            if ((paramPlsChoice == ePLSChoice.Removed) || (paramPlsChoice == ePLSChoice.All))
                ListDefectLabel.Add(DefectRemoved.label);
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

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            string sErrMsg;

            base.OnInit();

            if (!UseDatabase())
            {
                RunIter = 0;
                PreKlarfDirectory = Recipe.OutputDir;
                DateTime dateklarf = DateTime.MinValue;

                _csNamePreKlarfFile = GetlastKlarfFile("", out dateklarf);
            }
            else
            {
                // perform pre registering and update DestinationDirectory & ResultFileBaseName & RunIter & nInternalResID 
                // InitDestinationDirectoryDatabase();
            }

            DateTime dtNow = DateTime.Now;
            _mylayer = GetLayer(this);

            _dataKlarf = new DataKlarf();
            string ADCRecipeFileName = Wafer.GetWaferInfo(eWaferInfo.ADCRecipeFileName);

            //var dbRecipeServiceProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
            //var recipe = dbRecipeServiceProxy.GetLastRecipe(Guidkey);
            UnitySC.DataAccess.Dto.Recipe recipe = null;
            if (recipe != null)
            {
                _dataKlarf.SetupID = new PrmSetupId(Wafer.GetWaferInfo(eWaferInfo.ToolRecipe), recipe.Created);
            }
            else
            {
                _dataKlarf.SetupID = new PrmSetupId(Wafer.GetWaferInfo(eWaferInfo.ToolRecipe), dtNow);
            }

            _dataKlarf.ResultTimeStamp = dtNow;

            // "UNITY-SC" "ToolCategory" "ToolName"
            _dataKlarf.InspectionStationID = String.Format("\"{0}\" \"{1}\" \"{2}\"", "UNITYSC", "4See", Recipe.Toolname);

            _dataKlarf.LotID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.LotID));
            _dataKlarf.EquipmentID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.EquipmentID));
            _dataKlarf.DeviceID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.DeviceID));
            _dataKlarf.WaferID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.WaferID));
            _dataKlarf.StepID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.StepID));

            int nSlotID = -1;
            if (!int.TryParse(Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.SlotID), NumberStyles.Integer, CultureInfo.InvariantCulture, out nSlotID))
            {
                sErrMsg = String.Format("Error Unable to Parse Wafer SlotId (\"{0}\")", Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.SlotID));
                logError(sErrMsg);
                throw new Exception("Init Exception : " + sErrMsg);
            }
            _dataKlarf.SlotID = nSlotID;

            int nSampleSize_mm = 0;
            if (Wafer is NotchWafer)
            {
                nSampleSize_mm = (int)((Wafer as NotchWafer).Diameter / 1000.0);
            }
            else if (Wafer is FlatWafer)
            {
                nSampleSize_mm = (int)((Wafer as FlatWafer).Diameter / 1000.0);

            }
            else if (Wafer is RectangularWafer)
            {
                float width_mm = (Wafer as RectangularWafer).Width / 1000.0f;
                float Height_mm = (Wafer as RectangularWafer).Height / 1000.0f;
                nSampleSize_mm = (int)(Math.Sqrt(width_mm * width_mm + Height_mm * Height_mm));
            }

            _dataKlarf.SampleSize = new PrmSampleSize(nSampleSize_mm);

            _dataKlarf.DiePitch = new PrmPtFloat((float)nSampleSize_mm * 1000, (float)nSampleSize_mm * 1000); // wafer size x/y


            HashSet<ModuleBase> subtreeNodes = GetAllDescendants();
            bool paramShiftedCoordinates = false;

            foreach (ModuleBase child in subtreeNodes)
            {
                if (child.DisplayName.Contains("Klarf Edition"))
                {

                    paramShiftedCoordinates = ((ParameterTemplate<bool>)child.ParameterList.Find(p => p.Name == "CoordinatesShifted")).Value;
                    break;
                }
            }

            if (paramShiftedCoordinates == true)
                _dataKlarf.SampleCenterLocation = new PrmPtFloat((float)0.0, (float)0.0); // bottom left corner
            else _dataKlarf.SampleCenterLocation = new PrmPtFloat((float)_dataKlarf.SampleSize.waferDiameter_mm * 500.0f, (float)_dataKlarf.SampleSize.waferDiameter_mm * 500.0f); // wafer size x/y  * 2

            _dataKlarf.CoordinatesCentered = new PrmYesNo(ParamCenteringReport.Value);

            _dataKlarf.SampleTestPlan = new PrmSampleTestPlan(0, 0);

            _dataKlarf.AddDefectTypes(sDefectTypeCategories); // warning defect type should be registered or you will have to declare and defined type by yourself using DataKlarf.AddDefectType(Type p_type, String p_name)


            // = GetlastKlarfFile(out preKlarfData);
            _preKlarfData = KlarfFile.Read(_csNamePreKlarfFile, out sErrMsg);
            log("PLS Klarf pre job : " + _csNamePreKlarfFile);
            if (_preKlarfData != null)
            {
                log("PLS Klarf pre job defects count : " + _preKlarfData.DefectList.Count);
            }
            else
                throw new ApplicationException("NO Klarf pre job File");
            _preData = new List<PrmDefect>();
            _postData = new List<PrmDefect>();
            _addedData = new List<PrmDefect>();
            _commonData = new List<PrmDefect>();
            _removedData = new List<PrmDefect>();
            _listePostCluster = new List<Cluster>();
            InitOutputQueue();
        }

        private LayerBase GetLayer(PLSEditorModule pLSEditorModule)
        {
            List<ModuleBase> loaders = FindAncestors(m => m is IDataLoader);
            if (loaders.Count == 0)
                return null;

            IDataLoader loader = (IDataLoader)loaders[0];
            return loader.Layer;
        }

        public string GetKlarfExt()
        {
            PathString Ext;
            Ext = ConfigurationManager.AppSettings["KlarfExtension"];
            if (string.IsNullOrEmpty(Ext))
                Ext = ".001";
            return Ext;
        }


        private string GetlastKlarfFile(string recipeRobot, out DateTime lastWriteTime)
        {

            DirectoryInfo dir = new DirectoryInfo(PreKlarfDirectory / recipeRobot);

            IEnumerable<FileInfo> listFiles = dir.GetFiles("*" + GetKlarfExt());
            DataKlarf fileKlarfData;
            string sErrMsg;
            string WaferID = Wafer.GetWaferInfo(eWaferInfo.WaferID);

            List<FileInfo> ListFileInfo = (from file in listFiles
                                           let dateWrite = file.LastWriteTime
                                           orderby dateWrite descending
                                           select file).ToList();

            foreach (FileInfo file in ListFileInfo)
            {
                if (file.Name.Contains("_PLS" + file.Extension) == false)
                {
                    fileKlarfData = KlarfFile.Read(file.FullName, out sErrMsg);
                    if (fileKlarfData.WaferID == WaferID)
                    {
                        _preKlarfData = fileKlarfData;
                        lastWriteTime = file.LastWriteTime;
                        return file.FullName;
                    }
                }
            }
            lastWriteTime = DateTime.MinValue;
            return null;

        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            //-------------------------------------------------------------
            // Récuperation du cluster
            //-------------------------------------------------------------
            Cluster cluster = (Cluster)obj;

            if (cluster == null || cluster.blobList == null || cluster.blobList.Count == 0)
                throw new ApplicationException("Pls : Empty cluster");

            _sizingType = (BasicModules.Sizing.eSizingType)cluster.characteristics[SizingCharacteristics.SizingType];

            _listePostCluster.Add((Cluster)cluster.DeepClone());

            List<PrmDefect> ltmp = CreatePrmDefectList(cluster);

            int nLastDefectId = _postData.Count + 1;

            PrmImageData imgData = new PrmImageData();
            for (int i = 0; i < ltmp.Count; i++)
            {
                ltmp[i].Set("DEFECTID", nLastDefectId + i);
                ltmp[i].Set("IMAGECOUNT", imgData.List.Count);
                ltmp[i].Set("IMAGELIST", imgData);
                _postData.Add(ltmp[i]);
                _flagIndexForAddedList.Add(false);
            }

        }

        private void ProcessPls()
        {
            if (State != eModuleState.Aborting)
            {
                ComputeComparison((float)ParamfProximity_um);
            }
        }

        protected override void ProcessQueueElement(Cluster cluster)
        {
            if (State != eModuleState.Aborting)
            {
                ProcessChildren(cluster);
            }
        }

        /// <summary>
        /// Crée la liste des PrmDefect à partir des blobs du cluster
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        private List<PrmDefect> CreatePrmDefectList(Cluster cluster)
        {
            int nClusterNum = cluster.Index;
            int nClusterClassNumber = 0;

            BasicModules.Sizing.eSizingType sizingType = (BasicModules.Sizing.eSizingType)cluster.characteristics[SizingCharacteristics.SizingType];
            bool UseHeight3D = false;
            double totalclusterSizeHeight3D_um = 0.0;
            if (sizingType == BasicModules.Sizing.eSizingType.ByHeight3D)
            {
                UseHeight3D = cluster.characteristics.ContainsKey(SizingCharacteristics.TotalDefectSize);
                if (UseHeight3D)
                    totalclusterSizeHeight3D_um = (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize]; // aka BareHeightAverage
            }

            bool bUseCenterRect = ParamCenteringReport.Value;

            List<PrmDefect> ltmp = new List<PrmDefect>(Math.Max(1, cluster.blobList.Count));
            foreach (Blob blb in cluster.blobList)
            {
                PrmDefect curDefect = _dataKlarf.NewDefect();
                curDefect.SurroundingRectangleMicron = blb.micronQuad.SurroundingRectangle;

                RectangleF rect_um = blb.micronQuad.SurroundingRectangle;
                rect_um.Offset(_dataKlarf.SampleCenterLocation.x, _dataKlarf.SampleCenterLocation.y);

                //-- Orign blob is CENTER of rect -- In viewer defect rect is defined be its BOTTOM LEFT origin
                if (bUseCenterRect)
                {
                    PointF Mid = rect_um.Middle();
                    curDefect.Set("XREL", (double)Mid.X);
                    curDefect.Set("YREL", (double)Mid.Y);
                }
                else
                {
                    //-- Orign blob BOTTOM LEFT of rect -- In viewer rect is defined like that
                    curDefect.Set("XREL", (double)rect_um.Left);
                    curDefect.Set("YREL", (double)rect_um.Bottom);
                }

                curDefect.Set("XSIZE", (double)blb.characteristics[SizingCharacteristics.sizeX]);
                curDefect.Set("YSIZE", (double)blb.characteristics[SizingCharacteristics.sizeY]);

                double area = (double)blb.characteristics[SizingCharacteristics.DefectArea];
                curDefect.Set("DEFECTAREA", area);
                curDefect.Set("DSIZE", (double)blb.characteristics[SizingCharacteristics.DSize]);

                curDefect.Set("CLASSNUMBER", nClusterClassNumber);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                ltmp.Add(curDefect);
            }
            return ltmp;
        }

        /// <summary>
        /// Retourne une liste de cluster issue de la liste de PrmDefect
        /// </summary>
        /// <param name="lPrmDefect"></param>
        /// <returns></returns>
        private List<Cluster> CreateClusterListFromDefect(List<PrmDefect> lPrmDefect)
        {
            int nClusterNum;
            int nClusterRoughBin = 0;
            int nClusterFineBin = 0;
            int nClusterClassNumber = 0;
            List<Cluster> lCluster = new List<Cluster>();
            Cluster cluster = null;
            int blobId = 0;
            Blob blb;

            // Classer les defects par cluster
            foreach (PrmDefect defect in lPrmDefect)
            {
                nClusterNum = (int)defect.Get("CLUSTERNUMBER");
                // recherche si on a déjà ce cluster
                if (lCluster.Count > 0)
                {
                    List<Cluster> l = lCluster.Where(cl => cl.Index == nClusterNum).ToList();
                    if (l.Count > 0)
                        cluster = l.First();
                    else
                        cluster = null;
                }
                if (cluster == null) // pas encore créé.
                {
                    cluster = new Cluster(nClusterNum, _layer);
                    cluster.blobList = new List<Blob>(Math.Max(1, lPrmDefect.Count));
                    lCluster.Add(cluster);
                }
                // On complète le cluster
                nClusterRoughBin = (int)defect.Get("ROUGHBINNUMBER"); // le roughbin bon doit être cherché en fonction du type de classification ?
                if (_layer.GetType().Name != "DieLayer")
                    nClusterFineBin = (int)defect.Get("FINEBINNUMBER");
                nClusterClassNumber = (int)defect.Get("CLASSNUMBER");
                //cluster.defectClassList.Add(paramRoughBins.RoughBins.FirstOrDefault(x => x.Value.ClassNumber == nClusterClassNumber && x.Value.RoughBinNum == nClusterRoughBin && x.Value.FineBinNum == nClusterFineBin).Key);
                cluster.characteristics[SizingCharacteristics.SizingType] = _sizingType;
                cluster.characteristics[SizingCharacteristics.TotalDefectSize] = defect.Size;
                bool bUseCenterRect = ParamCenteringReport.Value;


                blb = new Blob(blobId, cluster);

                blb.micronQuad = new QuadF(defect.SurroundingRectangleMicron);

                //RectangleF rect_um = blb.micronQuad.SurroundingRectangle;
                //rect_um.Offset(_dataKlarf.SampleCenterLocation.x, _dataKlarf.SampleCenterLocation.y);

                //--Orign blob is CENTER of rect-- In viewer defect rect is defined be its BOTTOM LEFT origin
                //if (bUseCenterRect)
                //{
                //    PointF Mid = rect_um.Middle();
                //    curDefect.Set("XREL", (double)Mid.X);

                //    curDefect.Set("YREL", (double)Mid.Y);
                //}
                //else
                //{
                //    //-- Orign blob BOTTOM LEFT of rect -- In viewer rect is defined like that
                //    rect_um.Left = (float)defect.Get("XREL");
                //    curDefect.Set("YREL", (double)rect_um.Bottom);
                //}

                blb.characteristics[SizingCharacteristics.sizeX] = defect.Get("XSIZE");
                blb.characteristics[SizingCharacteristics.sizeY] = defect.Get("YSIZE");


                blb.characteristics[SizingCharacteristics.DefectArea] = defect.Get("DEFECTAREA");
                blb.characteristics[SizingCharacteristics.DSize] = defect.Get("DSIZE");

                cluster.blobList.Add(blb);
            }
            return lCluster;
        }

        private void ComputeComparison(float fProximity_um)
        {
            try
            {
                log("PLS Compute Comparaison");
                _preData = _preKlarfData.DefectList;
                _layer = _listePostCluster[0].Layer;
                log("  preData :" + _preData.Count + "postData :" + _postData.Count);

                bool bUseCenterRect = ParamCenteringReport.Value;

                SizeF inflateSize = new SizeF(fProximity_um, fProximity_um);
                for (int i = 0; i < _preData.Count; i++)
                {
                    PrmDefect predefect = _preData[i];
                    // Calculer le SurroundingRectangleMicron du preDefect
                    if (predefect.SurroundingRectangleMicron.Width == 0)
                    {
                        RectangleF rect_um = new RectangleF(0, 0, (float)(double)predefect.Get("XSIZE"), (float)(double)predefect.Get("YSIZE"));
                        //rect_um.Width = (float)(double)predefect.Get("XSIZE");
                        //rect_um.Height = (float)(double)predefect.Get("YSIZE");


                        if (bUseCenterRect)
                        {
                            PointF Mid = new PointF();
                            Mid.X = (float)(double)predefect.Get("XREL");
                            Mid.Y = (float)(double)predefect.Get("YREL");
                            rect_um.X = Mid.X - rect_um.Width / 2;
                            rect_um.Y = Mid.Y + rect_um.Height / 2;
                        }
                        else
                        {
                            //-- Orign blob BOTTOM LEFT of rect -- In viewer rect is defined like that
                            rect_um.X = (float)(double)predefect.Get("XREL");
                            rect_um.Y = (float)(double)predefect.Get("YREL") - rect_um.Height;
                        }
                        rect_um.Offset(-_dataKlarf.SampleCenterLocation.x, -_dataKlarf.SampleCenterLocation.y);
                        predefect.SurroundingRectangleMicron = rect_um;
                    }

                    bool foundOne = false;

                    for (int j = 0; j < _postData.Count; j++)
                    {
                        if (DefectMatch(_postData[j], predefect, inflateSize))
                        {

                            if (paramPlsChoice == ePLSChoice.Commons || (paramPlsChoice == ePLSChoice.All))
                                _commonData.Add(_postData[j]);

                            _flagIndexForAddedList[j] = true;
                            _postData.RemoveAt(j);  // to do a confirmer !!
                            j--;
                            foundOne = true;
                        }
                        if (State == eModuleState.Aborting)
                            return;
                    }
                    if (State == eModuleState.Aborting)
                        return;
                    if (foundOne)
                    {
                        _preData.RemoveAt(i); // purge de defaut trouvé en common pour les removes
                        i--;
                    }
                }


                log("PLS Compute Comparaison executed");



                if ((paramPlsChoice == ePLSChoice.Commons || (paramPlsChoice == ePLSChoice.All)) && _commonData.Count > 0)
                {
                    log("PLS common :" + _commonData.Count);
                    _dataKlarf.AddListDefect(_commonData);
                }


                if (paramPlsChoice == ePLSChoice.Adders || (paramPlsChoice == ePLSChoice.All))
                {
                    // on met dans les Added que ceux qui n'ont pas eu de copain (flag à false)
                    //for (int j = 0; j < postData.Count; j++)
                    {

                        //if (flagIndexForAddedList[j] == false)
                        //    addedData.Add(postData[j]);
                        _addedData.AddRange(_postData);
                    }
                    log("PLS added :" + _addedData.Count);
                    _dataKlarf.AddListDefect(_addedData);
                }

                // ce qu'il reste dans m_PreData = remove
                if (paramPlsChoice == ePLSChoice.Removed || (paramPlsChoice == ePLSChoice.All))
                {
                    _removedData.AddRange(_preData);
                    log("PLS removed :" + _removedData.Count);
                }

                // On met à jour les vid
                //((RangeComparator)(DefectAll.compartorList[0])).Min = removedData.Count + addedData.Count + commonData.Count;
                ((RangeComparator)(DefectAdders.compartorList[0])).Min = _addedData.Count;
                ((RangeComparator)(DefectCommons.compartorList[0])).Min = _commonData.Count;
                ((RangeComparator)(DefectRemoved.compartorList[0])).Min = _removedData.Count;

                // on met en queue  les clusters demandés 
                //
                List<int> lIdClusterToEnqueu = new List<int>();
                Cluster cluster = null;

                if ((paramPlsChoice == ePLSChoice.Adders) || (paramPlsChoice == ePLSChoice.All))
                {
                    for (int i = 0; i < _addedData.Count; i++)
                    {
                        cluster = GetCluster(_addedData[i]);
                        lIdClusterToEnqueu.Add(cluster.Index);
                        cluster.defectClassList.Insert(0, DefectAdders.label);
                        outputQueue.Enqueue(cluster);
                    }
                }

                if ((paramPlsChoice == ePLSChoice.Commons) || (paramPlsChoice == ePLSChoice.All))
                {
                    for (int i = 0; i < _commonData.Count; i++)
                    {
                        cluster = GetCluster(_commonData[i]);
                        if (!lIdClusterToEnqueu.Contains(cluster.Index))
                        {
                            cluster.defectClassList.Insert(0, DefectCommons.label);

                            lIdClusterToEnqueu.Add(cluster.Index);
                            outputQueue.Enqueue(cluster);

                        }
                    }
                }

                if ((paramPlsChoice == ePLSChoice.Removed) || (paramPlsChoice == ePLSChoice.All))
                {  // removed defects
                   // Généger les clusters du pré job
                    List<Cluster> lremoveCluster = CreateClusterListFromDefect(_removedData);

                    foreach (Cluster cl in lremoveCluster)
                    {
                        cl.defectClassList.Insert(0, DefectRemoved.label);

                        outputQueue.Enqueue(cl);
                    }
                }


                foreach (Cluster cl in _listePostCluster)
                {
                    cl.DelRef();
                }
            }
            catch (Exception ex)
            {
                log("PLS error :" + ex.ToString());
            }
            finally
            {
                if (State == eModuleState.Aborting)
                    outputQueue.AbortQueue();
                else
                    outputQueue.CloseQueue();

                logDebug("end pls");
            }
        }

        /// <summary>
        /// Compare les 2 defauts pour évaluer s'il sont concordants
        /// </summary>
        /// <param name="postDefect"></param>
        /// <param name="predefect"></param>
        /// <param name="inflateSize"></param>
        /// <returns></returns>
        private bool DefectMatch(PrmDefect postDefect, PrmDefect predefect, SizeF inflateSize)
        {
            RectangleF eRectPreProc = predefect.SurroundingRectangleMicron;
            eRectPreProc.Inflate(inflateSize);

            RectangleF eRectPostProc = postDefect.SurroundingRectangleMicron;
            eRectPostProc.Inflate(inflateSize);

            if (eRectPreProc.IntersectsWith(eRectPostProc))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Récupère l'index du cluster et vérifie que le cluster existe.
        /// </summary>
        /// <param name="defect"></param>
        /// <returns></returns>
        private Cluster GetCluster(PrmDefect defect)
        {
            int clId = (int)defect.Get("CLUSTERNUMBER");
            Cluster cluster = _listePostCluster.Where(cl => cl.Index == clId).First();
            if (cluster == null)
            {
                throw new ApplicationException("No Cluster with this index : " + clId);
            }
            return cluster;
        }

        public override UserControl RenderingUI
        {
            get
            {
                return GetUI();
            }
        }


        protected override void OnStopping(eModuleState oldState)
        {
            log("Starting Process PLS");
            Task task = Scheduler.StartSingleTask("ProcessPls", () =>
            {
                log("Task Process PLS");
                ProcessPls();
                base.OnStopping(oldState);
            });
        }

    }
}
