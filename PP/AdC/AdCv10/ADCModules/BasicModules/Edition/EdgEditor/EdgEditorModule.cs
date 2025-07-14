using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.Edition.DataBase;
using BasicModules.Edition.EdgEditor;
using BasicModules.Edition.Rendering.Message;
using BasicModules.Sizing;

using CommunityToolkit.Mvvm.Messaging;

using LibProcessing;

using UnitySC.Shared.Tools;

namespace BasicModules.EdgEditor
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    internal class EdgEditorModule : DatabaseEditionModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly BoolParameter paramSaveThumbnails;

        //=================================================================
        // Autres Champs
        //=================================================================
        private List<Cluster> _clusterList = new List<Cluster>();
        public List<Cluster> ClusterList { get => _clusterList; set => _clusterList = value; }

        private PathString _foldername;
        private PathString _folderThumbnail;

        private static ProcessingClass _processClass = new ProcessingClassMil();

        protected String m_sReportTxt = "";

        //=================================================================
        // Database results registration
        //=================================================================
        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.EyeEdge_EDG);
            return Rtypes;
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public EdgEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramSaveThumbnails = new BoolParameter(this, "SaveThumbnails");
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

            PathString uniqueID = new PathString(Wafer.GetWaferInfo(eWaferInfo.Basename));
            _folderThumbnail = uniqueID / String.Format("Run_{0}", RunIter);

            //-------------------------------------------------------------
            // Création du répertoire de vignettes
            //-------------------------------------------------------------
            bool bCreateVignetteFolder = paramSaveThumbnails;

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
                // ERROR ca exitse pas encore les 3da eye edge !!!

                throw new ApplicationException("EDG editor could not handle 3da thumbnail in Cluster " + cluster);
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

            if (cluster.blobList == null)
                throw new ApplicationException("Blob List is null or empty in Cluster " + obj);

            int nNbBlobDefect = cluster.blobList.Count;
            double size = (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize];

            eSizingType sizingType = (eSizingType)cluster.characteristics[SizingCharacteristics.SizingType];
            switch (sizingType)
            {
                case eSizingType.ByLength:
                case eSizingType.ByDiameter:
                case eSizingType.ByHeight3D:
                    //pas besoin de convertir => µm
                    break;
                case eSizingType.ByPSLLut:
                    size /= 1000; //==> nm
                    break;
                case BasicModules.Sizing.eSizingType.ByArea:
                    size /= 1000000.0; //==> mm²
                    break;
                default:
                    throw new ApplicationException("unknown sizing type: " + sizingType);
            }

            //-------------------------------------------------------------
            // Ecriture des vignettes
            //-------------------------------------------------------------
            if (paramSaveThumbnails)
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

            // Notification pour affichage des résultats - rendering
            ClassLocator.Default.GetInstance<IMessenger>().Send(new EdgResultMessage() { Module = this });
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("ProcessEdg", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                    {
                        ProcessEdg();
                        ResultState resstate = ResultState.Ok;
                        if (State == eModuleState.Aborting)
                            resstate = ResultState.Error;
                        RegisterResultInDatabase(ResultTypeFile.EyeEdge_EDG, resstate);
                    }
                    else if (oldState == eModuleState.Aborting)
                    {
                        PurgeEdg();
                        RegisterResultInDatabase(ResultTypeFile.EyeEdge_EDG, ResultState.Error);
                    }
                    else
                        throw new ApplicationException("invalid state");
                }
                catch (Exception ex)
                {
                    RegisterResultInDatabase(ResultTypeFile.EyeEdge_EDG, ResultState.Error);
                    string msg = "EDG generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    base.OnStopping(oldState);
                }
            });
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessEdg()
        {

            //#warning TODO Edg editor

            //-------------------------------------------------------------
            // Create a matrix to transfrom from microns to viewer pixels
            //-------------------------------------------------------------

            //-------------------------------------------------------------
            // Write EDG file
            //-------------------------------------------------------------
            PathString path = GetResultFullPathName(ResultTypeFile.EyeEdge_EDG);           
            if (!Directory.Exists(path.Directory))
            {
                log("Creating EDG file " + path);
                Directory.CreateDirectory(path.Directory);
            }

            using (StreamWriter swEdg = new StreamWriter(path))
            {
                // Header
                //.......
                WriteHeader(swEdg);

                // Write each cluster
                //...................
                foreach (Cluster cluster in ClusterList.OrderBy(cl => cl.Index))
                {
                    if (State != eModuleState.Aborting)
                        WriteCluster(swEdg, cluster);
                    cluster.DelRef();
                    Interlocked.Increment(ref nbObjectsOut);
                }

                ClusterList.Clear();

                // Close file
                //...........
                if (State == eModuleState.Aborting)
                    swEdg.WriteLine("Aborted");
            }
            logDebug("edg generated: " + path);
        }

        //=================================================================
        // Ecrititure des infos de l'entete du fichier 
        //=================================================================
        private void WriteHeader(StreamWriter sw)
        {
            // Récupération des infos du header 
            string waferID = "WaferID = " + Wafer.GetWaferInfo(eWaferInfo.WaferID);
            string loadPort = "LoadPort = " + Wafer.GetWaferInfo(eWaferInfo.LoadPortID);
            string lotId = "LotID = " + Wafer.GetWaferInfo(eWaferInfo.LotID);
            string slotID = "SlotID = " + Wafer.GetWaferInfo(eWaferInfo.SlotID);
            string recipeID = "RecipeID = " + Wafer.GetWaferInfo(eWaferInfo.ToolRecipe);
            string stepID = "StepID = " + Wafer.GetWaferInfo(eWaferInfo.StepID);
            string deviceID = "DeviceID = " + Wafer.GetWaferInfo(eWaferInfo.DeviceID);
            string sensorsNamesList = "";
            //To verify: S'assurer de la notation du type de wafer 
            string waferType = "WaferType = " + Wafer.GetWaferInfo(eWaferInfo.WaferType);
            string waferSize = "WaferSize = " + (Wafer.SurroundingRectangle.Width / 1000.0f);
            string toolsName = "ToolsName = " + Recipe.Toolname;

            // Récuperation du nombre des clusters et d'autres infos pour chaque sonde 
            var layerParams = ClusterList.GroupBy(cluster => ((ImageLayerBase)cluster.Layer).ResultType)
                                         .Select(grp =>
                                         new
                                         {
                                             ResultTypeID = grp.Key,
                                             SensorName = grp.First().Layer.name,
                                             NocthY = ((EyeEdgeMatrix)grp.First().Layer.Matrix).NotchY,
                                             PixelSize = ((EyeEdgeMatrix)grp.First().Layer.Matrix).PixelSize,
                                             NbClusters = grp.Count()
                                         });

            // Récupération de la liste des sondes 

            /// ic ya un bug dans l eviwer à corriger
            var sensorList = layerParams.Select(query => query.SensorName);
            List<KeyValuePair<int, string>> Ssensors = new List<KeyValuePair<int, string>>(5);
            foreach (string sensorName in sensorList)
            {
                string sensor_elt = GetEdgeSensorName(sensorName, out int nKeyOrder);
                Ssensors.Add(new KeyValuePair<int, string>(nKeyOrder, sensor_elt));
            }
            Ssensors.Sort((x, y) => x.Key.CompareTo(y.Key));

            foreach (var kvp in Ssensors)
            {
                sensorsNamesList += kvp.Value + ";";
            }
            sensorsNamesList = "SensorList = " + sensorsNamesList;

            // Ecriture des données du header dans le fichier Edge 
            sw.WriteLine("[REPORT_HEADER]");
            sw.WriteLine(waferID);
            sw.WriteLine(loadPort);
            sw.WriteLine(lotId);
            sw.WriteLine(slotID);
            sw.WriteLine(recipeID);
            sw.WriteLine(stepID);
            sw.WriteLine(deviceID);
            sw.WriteLine(waferType);
            sw.WriteLine(waferSize);
            sw.WriteLine(toolsName);
            sw.WriteLine(sensorsNamesList);
            sw.WriteLine();

            foreach (var item in layerParams)
            {
                string sensorName = GetEdgeSensorName(item.SensorName, out int nKey);
                string edgeExposureTime = "EdgeExposureTime_" + sensorName + " = " + "0"; // Default value 
                string clusterNumber = "ClusterNumber_" + sensorName + " = " + item.NbClusters;
                string notchPosition = "NotchPosition_" + sensorName + " = " + item.NocthY;
                string pixelSize = "PixelSize_µm_" + sensorName + " = " + item.PixelSize.Height; // Recuperer le heigth ou widh ?
                sw.WriteLine(edgeExposureTime);
                sw.WriteLine(clusterNumber);
                sw.WriteLine(notchPosition);
                sw.WriteLine(pixelSize);
                sw.WriteLine();
            }

        }

        //=================================================================
        //  Ecrititure des infos du cluster dans le fichier 
        //=================================================================
        protected void WriteCluster(StreamWriter sw, Cluster cluster)
        {

            // Récupération des infos du cluster 
            string clusterName = "[CLUSTER_" + cluster.Index + "]";
            string sensorType = "Sensor_Type = " + GetEdgeSensorName(cluster.Layer.name, out int nkerord);
            string vignetteNumber = "vignette_number = " + cluster.Line; // match the line index in acquisition image folder

            // pour cohérence avec viewer : ANGLE EN degrés !!
            // 0° en bas au niveau Notch, deplacement trigonométrique +  (soit anto clockwise)
            string vignetteAngle = "vignette_angle = " + (cluster.micronQuad.SurroundingRectangle.Middle().Angle() * 180 / Math.PI + 90.0).ToString();

            string total_cluster_size = "total_cluster_size = " + (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize];
            string vignetteBin = "vignette_bin = " + @".\" + GetClusterFilename(cluster, bw: true);
            string vignetteNdg = "vignette_ndg = " + @".\" + GetClusterFilename(cluster, bw: false); ;
            string DefectClass = "Defect_class = " + cluster.DefectClass;
            string UnitClusterSize = "";
            eSizingType sizingType = (eSizingType)cluster.characteristics[SizingCharacteristics.SizingType];
            switch (sizingType)
            {
                case eSizingType.ByLength:
                    UnitClusterSize = "µm";
                    break;
                case eSizingType.ByArea:
                    UnitClusterSize = "mm²";
                    break;
                case eSizingType.ByDiameter:
                    UnitClusterSize = "µm";
                    break;
                case eSizingType.ByPSLLut:
                    UnitClusterSize = "nm";
                    break;
                case eSizingType.ByHeight3D:
                    UnitClusterSize = "µm";
                    break;
                default:
                    throw new ApplicationException("unknown sizing type: " + sizingType);
            }
            UnitClusterSize = "unit_cluster_size = " + UnitClusterSize;
            string positionX = "Position_X = " + cluster.pixelRect.Middle().X;
            string positionY = "Position_Y = " + cluster.pixelRect.Middle().Y;
            string lengthX = "Length_X = " + cluster.pixelRect.Width;
            string lengthY = "Length_Y = " + cluster.pixelRect.Height;
            string realPosLeft = "RealPos_Left = " + cluster.pixelRect.TopLeft().X;
            string realPosRigth = "RealPos_Right = " + cluster.pixelRect.TopRight().X;
            string outboundLeft = "Outbound_Left = " + "0";
            string outboundRight = "Outbound_Right = " + "0";

            // Ecriture des données du cluster dans le fichier edge 
            sw.WriteLine(clusterName);
            sw.WriteLine(sensorType);
            sw.WriteLine(vignetteNumber);
            sw.WriteLine(vignetteAngle);
            sw.WriteLine(total_cluster_size);
            sw.WriteLine(UnitClusterSize);
            sw.WriteLine(vignetteBin);
            sw.WriteLine(vignetteNdg);
            sw.WriteLine(DefectClass);
            sw.WriteLine(positionX);
            sw.WriteLine(positionY);
            sw.WriteLine(lengthX);
            sw.WriteLine(lengthY);
            sw.WriteLine(realPosLeft);
            sw.WriteLine(realPosRigth);
            sw.WriteLine(outboundLeft);
            sw.WriteLine(outboundRight);
            sw.WriteLine();

        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeEdg()
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
        private void SaveImage(string filename, ProcessingImage processingImage)
        {
            if (processingImage.Format != ProcessingImage.eImageFormat.Height3D)
                _processClass.Save(filename, processingImage);
        }

        //=================================================================
        // Usercontrol et ViewModel
        //=================================================================
        //#warning TODO Edg editor Rendering ?
        /*static EdgRenderingViewModel _edgRenderingVm = null;
		static UserControl _edgRenderingControl = null;

		public override UserControl GetUI()
		{
            if (_edgRenderingVm == null)
            {
                _edgRenderingVm = new AsoRenderingViewModel(this);
                _edgRenderingControl = new ResultView();
                _edgRenderingControl.DataContext = _edgRenderingVm;
            }
            else if (_edgRenderingVm.Module != this)
            {
                _edgRenderingVm.Module = this;
            }

            return _edgRenderingControl;
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
            if (_edgRenderingVm != null)
            {
                _edgRenderingVm.Clean();
                _edgRenderingVm = null;
                _edgRenderingControl = null;
            }
        }*/
        protected string GetEdgeSensorName(string pOriginName, out int nKeyOrder)
        {
            string edgeSensorName = "";

            switch (pOriginName)
            {
                case "TopSensor":
                    edgeSensorName = Enum.GetName(typeof(eEdgeModuleID), eEdgeModuleID.Up);
                    nKeyOrder = 1;
                    break;
                case "TopBevelSensor":
                    edgeSensorName = Enum.GetName(typeof(eEdgeModuleID), eEdgeModuleID.UpBev);
                    nKeyOrder = 2;
                    break;
                case "ApexSensor":
                    edgeSensorName = Enum.GetName(typeof(eEdgeModuleID), eEdgeModuleID.Side);
                    nKeyOrder = 3;
                    break;
                case "BottomSensor":
                    edgeSensorName = Enum.GetName(typeof(eEdgeModuleID), eEdgeModuleID.Down);
                    nKeyOrder = 5;
                    break;
                case "BottomBevelSensor":
                    edgeSensorName = Enum.GetName(typeof(eEdgeModuleID), eEdgeModuleID.DownBev);
                    nKeyOrder = 4;
                    break;

                default:
                    throw new Exception("Sensor name not found !");

            }

            return edgeSensorName;
        }
    }
}
