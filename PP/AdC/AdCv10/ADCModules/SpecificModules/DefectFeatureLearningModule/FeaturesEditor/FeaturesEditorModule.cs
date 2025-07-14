using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.Edition.DataBase;

using LibProcessing;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace DefectFeatureLearning.FeaturesEditor
{
    public class FeaturesEditorModule : DatabaseEditionModule
    {
        private Classification classification = new Classification();
        private static ProcessingClass _processClass = new ProcessingClassMil();
        private static ProcessingClass _processClass3D = new ProcessingClassMil3D();
        private PathString _folderThumbnail;
        private List<int> _registeredResultTypes;

        //=================================================================
        // Constructeur
        //=================================================================
        public FeaturesEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        protected override List<int> RegisteredResultTypes()
        {
            if (_registeredResultTypes.IsNullOrEmpty())
            {
                _registeredResultTypes = new List<int> { (int)ResultTypeFile.Cluster_Precarac_ASE };
            }
            return _registeredResultTypes;
        }

        protected override void OnInit()
        {
            base.OnInit();

            //-------------------------------------------------------------
            // Répertoires
            //-------------------------------------------------------------

            PathString uniqueID = new PathString(Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.Basename));
            _folderThumbnail = uniqueID;

            Directory.CreateDirectory(DestinationDirectory / _folderThumbnail);
            classification.WaferData = CreateWafer(Wafer);
        }

        private WaferDataBase CreateWafer(WaferBase waferBase)
        {
            WaferDataBase res = null;
            if (waferBase is NotchWafer)
                res = new NotchWaferData() { Diameter = ((NotchWafer)waferBase).Diameter };

            else if (waferBase is RectangularWafer)
                res = new RectangularWaferData() { Height = ((RectangularWafer)waferBase).Height, Width = ((RectangularWafer)waferBase).Width };

            else if (waferBase is FlatWafer)
                res = new FlatWaferData() { FlatVerticalX = ((FlatWafer)waferBase).FlatVerticalX, FlatHorizontalY = ((FlatWafer)waferBase).FlatHorizontalY };
            else
                throw new InvalidOperationException("Unknow wafer type");

            res.WaferInfos = waferBase.waferInfo?.Select(x => new WaferInfo() { Key = x.Key.ToString(), Value = x.Value }).ToList();
            return res;
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            Defect defect = new Defect();
            classification.Defects.Add(defect);
            defect.Class = "Unclassified";

            if (obj is Cluster)
            {
                Cluster cluster = (Cluster)obj;
                AddCluster(defect, cluster);
            }
            else if (obj is CMC)
            {
                CMC cmc = (CMC)obj;
                foreach (Cluster cluster in cmc.GetClusters())
                    AddCluster(defect, cluster);
            }
            else
            {
                throw new ApplicationException("unknown object type: " + obj);
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("parent stopped, starting processing task");

            Scheduler.StartSingleTask("ProcessAse", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                        ProcessAse();
                }
                catch (Exception ex)
                {
                    string msg = "ASE generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    PurgeAse();
                    base.OnStopping(oldState);
                }
            });
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessAse()
        {
            //-------------------------------------------------------------
            // Ecriture du fichier ASE
            //-------------------------------------------------------------
            PathString path = DestinationDirectory / Recipe.Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.Basename) + ".ASE";
            log("Creating ase file " + path);
            classification.Serialize(path);

            logDebug("ase generated: " + path);

            //-------------------------------------------------------------
            // Stop processing
            //-------------------------------------------------------------
            nbObjectsOut = classification.Defects.Count;
        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeAse()
        {
            // Purge de la liste interne de defects
            //......................................
            classification.Defects.Clear();
        }

        //=================================================================
        // 
        //=================================================================
        private void AddCluster(Defect defect, Cluster cluster)
        {
            Layer layer = CreateLayer(defect, cluster);
            defect.MicronRect = cluster.micronQuad.SurroundingRectangle;
            ListFeatures(layer, cluster);
            SaveThumbnails(layer, cluster);
        }

        //=================================================================
        // 
        //=================================================================
        private Layer CreateLayer(Defect defect, Cluster cluster)
        {
            Layer layer = new Layer();
            layer.Name = cluster.Layer.name;
            layer.ClusterNumber = cluster.Index;
            defect.Layers.Add(layer);
            return layer;
        }

        //=================================================================
        // 
        //=================================================================
        private Layer ListFeatures(Layer layer, Cluster cluster)
        {
            foreach (var carac in cluster.characteristics)
            {
                Feature feature = new Feature();
                layer.Features.Add(feature);
                feature.Name = carac.Key.Name;
                if (carac.Key.Type == typeof(double))
                    feature.Value = (double)carac.Value;
                else if (carac.Key.Type == typeof(bool))
                    feature.Value = (bool)carac.Value ? 1 : 0;
                // else on ignore la carac
            }

            return layer;
        }

        //=================================================================
        // 
        //=================================================================
        private void SaveThumbnails(Layer layer, Cluster cluster)
        {
            PathString filename = GetThubnailFilename(cluster, bw: false);
            SaveImage(DestinationDirectory / filename, cluster.OriginalProcessingImage);
            layer.OriginalImage = filename;

            filename = GetThubnailFilename(cluster, bw: true);
            SaveImage(DestinationDirectory / filename, cluster.ResultProcessingImage);
            layer.BinaryImage = filename;
        }

        //=================================================================
        // 
        //=================================================================
        protected PathString GetThubnailFilename(Cluster cluster, bool bw)
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

            PathString filename = _folderThumbnail / cluster.Name + "-" + cluster.Layer.name + type + ext;
            return filename;
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


    }
}
