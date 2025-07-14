using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.DataLoader;
using BasicModules.Edition.DataBase;
using BasicModules.Edition.DummyDefect;
using BasicModules.Edition.Rendering;
using BasicModules.Edition.Rendering.Message;

using CommunityToolkit.Mvvm.Messaging;

using Format001;

using LibProcessing;

using UnitySC.Shared.Tools;

using Point = System.Drawing.Point;

namespace BasicModules.AsoEditor
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    internal class AsoEditorModule : DatabaseEditionModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly BoolParameter paramSaveThumbnails;
        public readonly AsoEditorParameter paramDefectClasses;
        [ExportableParameter(false)]
        public readonly EnumParameter<PrmOrientationInstruction.oiType> ParamOrientationInstruction;

        //=================================================================
        // Autres Champs
        //=================================================================
        private List<Cluster> _clusterList = new List<Cluster>();
        public List<Cluster> ClusterList { get => _clusterList; set => _clusterList = value; }

        private PathString _foldername;
        private PathString _folderThumbnail;


        private static ProcessingClass _processClass = new ProcessingClassMil();
        private static ProcessingClass _processClass3D = new ProcessingClassMil3D();
        private MatrixBase viewerMatrix;

        private CustomExceptionDictionary<string, CategoryStatistic> PerCategoryStatisticMap;
        private List<ModuleBase> VidModules;

        //=================================================================
        // Database results registration
        //=================================================================
        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.Cluster_ASO);
            return Rtypes;
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public AsoEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramSaveThumbnails = new BoolParameter(this, "SaveThumbnails");
            paramDefectClasses = new AsoEditorParameter(this, "DefectClasses");

            ParamOrientationInstruction = new EnumParameter<PrmOrientationInstruction.oiType>(this, "OrientationInstruction");

            ParamOrientationInstruction.ValueChanged += ChangeLabelSuffixForOrientation;
            ParamResultLabelSuffix.String = ParamResultLabelSuffix.String.Insert(0, GetSuffixLead(ParamOrientationInstruction.Value));

            if (!(Application.Current is null))
            {
                // Application.Current is null when running in AdaToAdc mode, visibility needn't be changed as there is no UI
                // Necessary for correct result handling in DB, setting must be hidden from UI
                ParamResultIndex.ParameterUI.Visibility = Visibility.Collapsed;
            }
            ParamResultIndex.IsUsed = true;
            ParamOrientationInstruction.ValueChanged += ChangeResultIndexForOrientation;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            //-------------------------------------------------------------
            // Répertoires
            //-------------------------------------------------------------
            _foldername = DestinationDirectory;
           
            _folderThumbnail = BuildThumbnailPath(ParamOrientationInstruction.Value);
            //-------------------------------------------------------------
            // Création de la Map des catégories
            //-------------------------------------------------------------
            PerCategoryStatisticMap = new CustomExceptionDictionary<string, CategoryStatistic>(exceptionKeyName: "Defect Category");
            VidModules = FindVidModules();

            foreach (AsoDefectClass defectClass in paramDefectClasses.DefectClassToCategoryMap.Values)
            {
                bool exists = PerCategoryStatisticMap.ContainsKey(defectClass.DefectLabel);
                if (!exists)
                {
                    CategoryStatistic stat = new CategoryStatistic();
                    stat.DefectCategory = defectClass.DefectCategory;
                    stat.Color = defectClass.Color;
                    stat.SaveThumbnails = defectClass.SaveThumbnails;
                    PerCategoryStatisticMap[stat.DefectCategory] = stat;
                }
            }

            foreach (AsoDefectVidCategory defectCategory in paramDefectClasses.VidToCategoryMap.Values)
            {
                bool exists = PerCategoryStatisticMap.ContainsKey(defectCategory.DefectCategory);
                if (!exists)
                {
                    CategoryStatistic stat = new CategoryStatistic();
                    stat.DefectCategory = defectCategory.DefectCategory;
                    stat.Color = defectCategory.Color;
                    stat.SaveThumbnails = defectCategory.SaveThumbnails;
                    PerCategoryStatisticMap[stat.DefectCategory] = stat;
                }
            }

            //-------------------------------------------------------------
            // Création du répertoire de vignettes
            //-------------------------------------------------------------
            bool bCreateVignetteFolder = paramSaveThumbnails;

            foreach (CategoryStatistic stat in PerCategoryStatisticMap.Values)
                bCreateVignetteFolder = bCreateVignetteFolder | stat.SaveThumbnails;

            if (bCreateVignetteFolder)
                Directory.CreateDirectory(_foldername / _folderThumbnail);
        }

        //=================================================================
        // 
        //=================================================================
        protected PathString GetClusterFilename(Cluster cluster, bool bw)
        {
            string type;
            string ext;

            if (!bw && cluster.OriginalProcessingImage.Format == ProcessingImage.eImageFormat.Height3D)
            {
                ext = ".3DA";
                type = "";
            }
            else if (bw)
            {
                ext = ".bmp";
                type = "-bw";
            }
            else
            {
                ext = ".bmp";
                type = "-grey";
            }

            PathString filename = _folderThumbnail / cluster.Name + type + ext;
            return filename;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            Cluster cluster = (Cluster)obj;

            //-------------------------------------------------------------
            // Calcul des statistiques par catégorie de défauts
            //-------------------------------------------------------------
            string category = GetClusterCategory(cluster);
            if (category == null)
            {
                Interlocked.Increment(ref nbObjectsOut);
                return;
            }

            CategoryStatistic stat = PerCategoryStatisticMap[category];

            if (cluster.blobList == null)
                throw new ApplicationException("Blob List is null or empty in Cluster " + obj);

            int nNbBlobDefect = cluster.blobList.Count;
            double size = (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize];

            BasicModules.Sizing.eSizingType sizingType = (BasicModules.Sizing.eSizingType)cluster.characteristics[SizingCharacteristics.SizingType];
            switch (sizingType)
            {
                case BasicModules.Sizing.eSizingType.ByLength:
                case BasicModules.Sizing.eSizingType.ByDiameter:
                case BasicModules.Sizing.eSizingType.ByHeight3D:
                    //pas besoin de convertir => µm
                    break;
                case BasicModules.Sizing.eSizingType.ByPSLLut:
                    size /= 1000; //==> nm
                    break;
                case BasicModules.Sizing.eSizingType.ByArea:
                    size /= 1000000.0; //==> mm²
                    break;
                default:
                    throw new ApplicationException("unknown sizing type: " + sizingType);
            }

            lock (stat)
            {
                stat.DefectCount += 1;
                stat.TotalDefectSize += (double)size;
            }

            //-------------------------------------------------------------
            // Ecriture des vignettes
            //-------------------------------------------------------------
            if (paramSaveThumbnails || stat.SaveThumbnails)
            {
                PathString filename = _foldername / GetClusterFilename(cluster, bw: true);
                SaveImage(filename, cluster.ResultProcessingImage);

                filename = _foldername / GetClusterFilename(cluster, bw: false);
                SaveImage(filename, cluster.OriginalProcessingImage);
            }

            //-------------------------------------------------------------
            // Stockage des clusters
            //-------------------------------------------------------------
            cluster.AddRef();
            lock (ClusterList)
                ClusterList.Add(cluster);

            // Notification pour affichage des résultats
            ClassLocator.Default.GetInstance<IMessenger>().Send(new AsoResultMessage() { Module = this });
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("ProcessAso", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                    {
                        ProcessAso();
                        ResultState resstate = ResultState.Ok; // TO DO -- check grading reject , rework if exist, or partial result
                        if (State == eModuleState.Aborting)
                            resstate = ResultState.Error;
                        RegisterResultInDatabase(ResultTypeFile.Cluster_ASO, resstate);
                    }
                    else if (oldState == eModuleState.Aborting)
                    {
                        PurgeAso();
                        RegisterResultInDatabase(ResultTypeFile.Cluster_ASO, ResultState.Error);
                    }
                    else
                        throw new ApplicationException("invalid state");
                }
                catch (Exception ex)
                {
                    RegisterResultInDatabase(ResultTypeFile.Cluster_ASO, ResultState.Error);
                    string msg = "ASO generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    base.OnStopping(oldState);
                }
            });
        }

        private void ChangeResultIndexForOrientation(PrmOrientationInstruction.oiType enumValue)
        {
            switch (enumValue)
            {
                case PrmOrientationInstruction.oiType.FRONT:
                    ParamResultIndex.Value = 0;
                    break;
                case PrmOrientationInstruction.oiType.BACK:
                    ParamResultIndex.Value = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeLabelSuffixForOrientation(PrmOrientationInstruction.oiType enumValue)
        {
            if (ParamResultLabelSuffix.String.StartsWith("-FS") || ParamResultLabelSuffix.String.StartsWith("-BS"))
            {
                ParamResultLabelSuffix.String = ParamResultLabelSuffix.String.Remove(0, 3).Insert(0, GetSuffixLead(enumValue));
            }
        }

        private static string GetSuffixLead(PrmOrientationInstruction.oiType enumValue)
        {
            switch (enumValue)
            {
                case PrmOrientationInstruction.oiType.FRONT:
                    return "-FS";
                case PrmOrientationInstruction.oiType.BACK:
                    return "-BS";
                case PrmOrientationInstruction.oiType.BEVEL:
                    return "-BV";
                default:
                    return "";
            }
        }
        private PathString BuildThumbnailPath(PrmOrientationInstruction.oiType orientation)
        {
            PathString uniqueID = new PathString(Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.Basename));
            string orientationFolder = orientation.ToString();

            PathString finalPath = uniqueID / orientationFolder / $"Run_{RunIter}";

            return finalPath;
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessAso()
        {
            //-------------------------------------------------------------
            // Create a matrix to transfrom from microns to viewer pixels
            //-------------------------------------------------------------
            const int viewerWaferSize = 3000; // Fixed, the viewer displays a 3100x3100 image
            const int viewerMargin = 50;
            float asoPixelSize = Math.Max(Wafer.SurroundingRectangle.Width, Wafer.SurroundingRectangle.Height) / viewerWaferSize;
            int center = (viewerWaferSize / 2) + viewerMargin;
            Point waferCenter = new Point(center, center);

            RectangularMatrix rmatrix = new RectangularMatrix();
            rmatrix.Init(waferCenter, new SizeF(asoPixelSize, asoPixelSize));
            viewerMatrix = rmatrix;

            //-------------------------------------------------------------
            // Write ASO file
            //-------------------------------------------------------------
            PathString path = GetResultFullPathName(ResultTypeFile.Cluster_ASO);
            if (!Directory.Exists(path.Directory))
            {
                log("Creating aso file " + path);
                Directory.CreateDirectory(path.Directory);
            }
            using (StreamWriter aso = new StreamWriter(path))
            {
                // Header
                //.......
                WriteHeader(aso);

                // Write each cluster
                //...................
                foreach (Cluster cluster in ClusterList.OrderBy(cl => cl.Index))
                {
                    if (State != eModuleState.Aborting)
                        WriteCluster(aso, cluster);
                    cluster.DelRef();
                    Interlocked.Increment(ref nbObjectsOut);
                }

                // Add dummy defects
                List<Cluster> dummyClusters = FindAncestors(x => x is DummyDefectModule).OfType<DummyDefectModule>().Where(x => x.ClusterResult != null).Select(x => x.ClusterResult).ToList();

                int lastClusterIndex = ClusterList.Any() ? ClusterList.Max(x => x.Index) : 0;
                foreach (Cluster cluster in dummyClusters)
                {
                    lastClusterIndex++;
                    cluster.Index = lastClusterIndex;
                    WriteCluster(aso, cluster);
                    if (paramSaveThumbnails)
                    {
                        string filename = _foldername / GetClusterFilename(cluster, bw: false);
                        SaveImage(filename, cluster.OriginalProcessingImage);
                    }
                }

                ClusterList.Clear();

                // Close file
                //...........
                if (State == eModuleState.Aborting)
                    aso.WriteLine("Aborted");
            }
            logDebug("aso generated: " + path);
        }

        //=================================================================
        // 
        //=================================================================
        private void WriteHeader(StreamWriter aso)
        {
            //-------------------------------------------------------------
            // REPORT_GLOBAL;bloc_count;cluster_count;defect_count;recipe_file;orientation_mark #
            // REPORT_HEADER;WaferID;SlotID;RecipeID;LotID;ToolsName;isSquareWafer;valueWaferSizeX_mm;valueWaferSizeY_mm #
            // REPORT_DIEGRID;DieOriginX;DieOriginY;DiePitchX;DiePitchY#
            //-------------------------------------------------------------
            uint NbDefect_Count_Global = 0;
            foreach (CategoryStatistic stat in PerCategoryStatisticMap.Values)
            {
                NbDefect_Count_Global += (uint)stat.DefectCount;
            }
            string recipe_file = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.ADCRecipeFileName);
            //REPORT_GLOBAL;bloc_count;cluster_count;defect_count;recipe_file;orientation_mark #
            aso.WriteLine("REPORT_GLOBAL;" + Recipe.NbLayers + ";" + ClusterList.Count + ";" + NbDefect_Count_Global + ";" + recipe_file + ";" + GetWaferOrientationMark() + "#");

            string UniqueId = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.Basename);
            string SlotID = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.SlotID);
            string ToolRecipe = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.ToolRecipe);
            string LotID = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.LotID);
            string ToolsName = Recipe.Toolname;
            bool isSquareWafer = Wafer is RectangularWafer;
            int valueWaferSizeX_mm = (int)(Wafer.SurroundingRectangle.Width / 1000.0f);
            int valueWaferSizeY_mm = (int)(Wafer.SurroundingRectangle.Height / 1000.0f);

            //REPORT_HEADER;WaferID;SlotID;RecipeID;LotID;ToolsName;isSquareWafer;valueWaferSizeX_mm;valueWaferSizeY_mm#
            aso.WriteLine("REPORT_HEADER;" + UniqueId + ";" + SlotID + ";" + ToolRecipe + ";" + LotID + ";" + ToolsName + ";" + isSquareWafer + ";" + valueWaferSizeX_mm + ";" + valueWaferSizeY_mm + "#");

            // retreived direct dataloader
            List<ModuleBase> AncestorLoadermodule = FindAncestors(mod => mod is DataLoaderBase);
            if (AncestorLoadermodule.Count == 0)
            {
                throw new ApplicationException("No loader module has been set prior to this module");
            }

            float lfDieOriginX_µm = 0.0f;
            float lfDieOriginY_µm = 0.0f;
            float lfDiePitchX_µm = 0.0f;
            float lfDiePitchY_µm = 0.0f;
            if (AncestorLoadermodule[0] is DieDataLoaderBase)
            {
                DieDataLoaderBase DirectAncestor = AncestorLoadermodule[0] as DieDataLoaderBase;
                DieLayer dielayer = ((DirectAncestor.Layer) as DieLayer);
                lfDieOriginX_µm = (float)dielayer.DieOriginX_µm;
                lfDieOriginY_µm = (float)dielayer.DieOriginY_µm;
                lfDiePitchX_µm = (float)dielayer.DiePitchX_µm;
                lfDiePitchY_µm = (float)dielayer.DiePitchY_µm;
            }
            //REPORT_DIEGRID; DieOriginX; DieOriginY; DiePitchX; DiePitchY#
            aso.WriteLine("REPORT_DIEGRID;" + lfDieOriginX_µm + ";" + lfDieOriginY_µm + ";" + lfDiePitchX_µm + ";" + lfDiePitchY_µm + "#");

            //-------------------------------------------------------------
            // REPORT_DETAIL;Label;Count;Size répété N fois (pour chaque type classe de defaut)
            //-------------------------------------------------------------
            foreach (CategoryStatistic stat in PerCategoryStatisticMap.Values)
            {
                aso.WriteLine("REPORT_DETAIL;" + stat.DefectCategory + ";" + stat.DefectCount + ";" + stat.TotalDefectSize + "#");
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected void WriteCluster(StreamWriter aso, Cluster cluster)
        {
            RectangleF clusterRect = cluster.micronQuad.SurroundingRectangle;
            RectangleF micronRect = clusterRect;
            micronRect.X += Wafer.SurroundingRectangle.Width / 2;
            micronRect.Y = (Wafer.SurroundingRectangle.Height / 2) + micronRect.Y;

            Rectangle pixelRect = viewerMatrix.micronToPixel(clusterRect);
            Rectangle pictureRect = GetPictureRectangle(cluster);

            string category = GetClusterCategory(cluster);
            CategoryStatistic stat = PerCategoryStatisticMap[category];

            double maxclustersize = 0;
            double totalclustersize = 0;
            string unit = "";
            string typereportsize = "";

            maxclustersize = (double)cluster.characteristics[SizingCharacteristics.DefectMaxSize];
            totalclustersize = (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize];

            BasicModules.Sizing.eSizingType sizingType = (BasicModules.Sizing.eSizingType)cluster.characteristics[SizingCharacteristics.SizingType];
            switch (sizingType)
            {
                case BasicModules.Sizing.eSizingType.ByLength:
                    unit = "µm";
                    typereportsize = "Length";
                    break;
                case BasicModules.Sizing.eSizingType.ByArea:
                    // Note Rti : HERE we currently pass in mm² if we have at least 0.1mm²
                    // maybe a typo if we want to pass when 1mm²is reached then add an extra 0 to the comparsion below
                    if (maxclustersize >= 100000)
                    {
                        maxclustersize /= 1000000;
                        totalclustersize /= 1000000;
                        unit = "mm²";
                    }
                    else unit = "µm²";
                    typereportsize = "Area";
                    break;
                case BasicModules.Sizing.eSizingType.ByDiameter:
                    unit = "µm";
                    typereportsize = "Diameter";
                    break;
                case BasicModules.Sizing.eSizingType.ByPSLLut:
                    unit = "µm";
                    typereportsize = "LUT";
                    break;
                case BasicModules.Sizing.eSizingType.ByHeight3D:
                    unit = "µm";
                    typereportsize = "Height3D";
                    break;
                default:
                    throw new ApplicationException("unknown sizing type: " + sizingType);
            }

            //CLUSTER_DESCR;num_cluster; 
            // num_bloc;BlocSelect;userlabel;nb_defect;
            // totalclustersize; maxclustersize;
            // micronposx; micronposy;
            // micronsizex; micronsizey; unit; 
            // pixelposx; pixelposy; pictureposx; pictureposy;
            // pixelsizex; pixelsizey;
            // thumbnailgreylevel; thumbnailbinary;
            // columnnumber; linenumber; virtualblocnumber; colorname; 
            // diex; diey; customerlabel; typereportsize; 
            //iskilling; 
            // [allcaract]#
            StringBuilder sb = new StringBuilder(2048);

            int nBlocNum = cluster.Layer.Index;
            int nBlocSelected = cluster.Layer.Index; // a changer ! -- en post charac on associe l'algo et l'image sur laquel on va faire tourner la post charac
            int virtualblocnumber = 0; // les bloc virtuelle sont il toujours existant
            int nKilling_Defect = 0; // fait dans le yield 

            // OPI : Modification local pour AXT. Le transfert de grading se fera via la nouvelle data base mais pour AXT, le temps nous manque. On oinscrit le resultat
            // dans le fichier ASO qui servira à reconstruire la liste des wafers nok via la Dll de sorting
            // ATTENTION Bidouille !!!
            if ((Recipe.GradingMark == Recipe.Grading.Reject) || (Recipe.GradingMark == Recipe.Grading.Rework))
                nKilling_Defect = 1;

            //sb.Append("CLUSTER_DESCR;" + cluster.Index +
            //            ";" + nBlocNum + ";" + nBlocSelected + ";" + category + ";" + cluster.blobList.Count +
            //            ";" + totalclustersize + ";" + maxclustersize + ";" + (int)micronRect.X + ";" + (int)micronRect.Y + ";" + micronRect.Width + ";" + micronRect.Height + ";" + unit +
            //            ";" + pixelRect.X + ";" + pixelRect.Y + ";" + pictureRect.X + ";" + pictureRect.Y + ";" + pixelRect.Width + ";" + pixelRect.Height +
            //            ";" + GetClusterFilename(cluster, bw: false) + ";" + GetClusterFilename(cluster, bw: true) +
            //            ";" + cluster.Column + ";" + cluster.Line + ";" + virtualblocnumber + ";" + stat.Color +
            //            ";" + cluster.DieIndexX + ";" + cluster.DieIndexY + ";" + cluster.DefectClass + ";" + typereportsize + ";" + nKilling_Defect+";");


            sb.Append("CLUSTER_DESCR;"); sb.Append(cluster.Index); sb.Append(";");
            sb.Append(nBlocNum); sb.Append(";"); sb.Append(nBlocSelected); sb.Append(";"); sb.Append(category); sb.Append(";"); sb.Append(cluster.blobList.Count); sb.Append(";");
            sb.Append(totalclustersize); sb.Append(";"); sb.Append(maxclustersize); sb.Append(";");
            sb.Append((int)micronRect.X); sb.Append(";"); sb.Append((int)micronRect.Y); sb.Append(";");
            sb.Append(micronRect.Width); sb.Append(";"); sb.Append(micronRect.Height); sb.Append(";"); sb.Append(unit); sb.Append(";");
            sb.Append(pixelRect.X); sb.Append(";"); sb.Append(pixelRect.Y); sb.Append(";"); sb.Append(pictureRect.X); sb.Append(";"); sb.Append(pictureRect.Y); sb.Append(";");
            sb.Append(pixelRect.Width); sb.Append(";"); sb.Append(pixelRect.Height); sb.Append(";");
            sb.Append(GetClusterFilename(cluster, bw: false)); sb.Append(";"); sb.Append(GetClusterFilename(cluster, bw: true)); sb.Append(";");
            sb.Append(cluster.Column); sb.Append(";"); sb.Append(cluster.Line); sb.Append(";"); sb.Append(virtualblocnumber); sb.Append(";"); sb.Append(stat.Color); sb.Append(";");
            sb.Append(cluster.DieIndexX); sb.Append(";"); sb.Append(cluster.DieIndexY); sb.Append(";"); sb.Append(cluster.DefectClass); sb.Append(";"); sb.Append(typereportsize); sb.Append(";");
            sb.Append(nKilling_Defect); /*sb.Append(";");*/

            String ss = (sb.ToString()).Substring(sb.ToString().Length - 30);
            foreach (var carac in cluster.characteristics)
            {
                sb.Append(";");
                sb.Append(carac.Key.Name);
                sb.Append("|");
                sb.Append(carac.Value);

                ss = (sb.ToString()).Substring(sb.ToString().Length - 30);
            }
            sb.Append(";Layer|");
            sb.Append(cluster.Layer.name);
            sb.Append("#");

            aso.WriteLine(sb.ToString());
        }

        //=================================================================
        // 
        //=================================================================
        public int GetWaferOrientationMark()
        {
            WaferBase wafer = Recipe.Wafer;
            int type = 0; // Notch==0, Flat==1, doubleFlat==2
            if (Wafer is NotchWafer)
                type = 0;
            else if (Wafer is FlatWafer)
            {
                FlatWafer flatwafer = (FlatWafer)wafer;
                if (flatwafer.IsDoubleFlat)
                    type = 2;
                else
                    type = 1;
            }
            //else if (Wafer is RectangularWafer)
            //{
            //    /// ??
            //}
            return type;
        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeAso()
        {
            // Purge de la liste interne de clusters
            //......................................
            foreach (Cluster cluster in ClusterList)
                cluster.DelRef();
            ClusterList.Clear();
        }

        //=================================================================
        // 
        //=================================================================
        private Rectangle GetPictureRectangle(Cluster cluster)
        {
            QuadF micronPicQ = cluster.Layer.Matrix.pixelToMicron(cluster.imageRect);
            Rectangle rect = viewerMatrix.micronToPixel(micronPicQ.SurroundingRectangle);
            return rect;
        }

        //=================================================================
        //
        //=================================================================
        public string GetClusterCategory(Cluster cluster)
        {
            if (VidModules.Count == 0)
            {
                // Sans VID
                //.........

                if (paramDefectClasses.DefectClassToCategoryMap.ContainsKey(cluster.DefectClass))
                {
                    AsoDefectClass defectClass = paramDefectClasses.DefectClassToCategoryMap[cluster.DefectClass];
                    return defectClass.DefectCategory;
                }
                else
                {
                    // specific case for metrology, this is a Godd Cluster (without defect so don't write it)
                    // should we need to write cluster even if there are good ?
                    return null;
                }

            }
            else
            {
                // Avec VIDs
                //..........
                foreach (BasicModules.VidReport.VidReportModule vidmodule in VidModules)
                {
                    VidReport.ReportClass vidReportClass;
                    bool found = vidmodule.paramCategories.ReportClasses.TryGetValue(cluster.DefectClass, out vidReportClass);
                    if (found)
                    {
                        int vid = vidReportClass.VID;
                        AsoDefectVidCategory cat = paramDefectClasses.VidToCategoryMap[vid];
                        return cat.DefectCategory;
                    }
                }
                if (cluster.DefectClass != Cluster.DefectClassNone)
                    throw new ApplicationException("Can't find VID for defect class: " + cluster.DefectClass);
                else
                    return null;
            }
        }

        //=================================================================
        //
        //=================================================================
        private List<ModuleBase> FindVidModules()
        {
            List<ModuleBase> vidmodules = FindAncestors(mod => mod is BasicModules.VidReport.VidReportModule);
            return vidmodules;
        }

        //=================================================================
        //
        //=================================================================
        private void SaveImage(string filename, ProcessingImage processingImage)
        {
            if (processingImage.Format == ProcessingImage.eImageFormat.Height3D)
                _processClass3D.Save(filename, processingImage);
            else
                _processClass.Save(filename, processingImage);
        }

        //=================================================================
        // Usercontrol et ViewModel
        //=================================================================
        private static AsoRenderingViewModel _asoRenderingVm = null;
        private static UserControl _asoRenderingControl = null;

        public override UserControl GetUI()
        {
            if (_asoRenderingVm == null)
            {
                _asoRenderingVm = new AsoRenderingViewModel(this);
                _asoRenderingControl = new ResultView();
                _asoRenderingControl.DataContext = _asoRenderingVm;
            }
            else if (_asoRenderingVm.Module != this)
            {
                _asoRenderingVm.Module = this;
            }

            return _asoRenderingControl;
        }

        public override UserControl RenderingUI
        {
            get
            {
                return GetUI();
            }
        }

        public override void ClearRenderingObjects()
        {
            base.ClearRenderingObjects();
            if (_asoRenderingVm != null)
            {
                _asoRenderingVm.Clean();
                _asoRenderingVm = null;
                _asoRenderingControl = null;
            }
        }
    }
}
