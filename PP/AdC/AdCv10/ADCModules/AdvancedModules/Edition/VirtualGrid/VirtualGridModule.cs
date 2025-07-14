using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace AdvancedModules.Edition.VirtualGrid
{
    internal class VirtualGridModule : QueueModuleBase<Cluster>
    {
        private struct XY
        {
            public int X, Y;
            public XY(int x, int y) { X = x; Y = y; }
        }

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly DoubleParameter paramOffsetX;
        public readonly DoubleParameter paramOffsetY;
        public readonly DoubleParameter paramCutStepX;
        public readonly DoubleParameter paramCutStepY;
        public readonly DoubleParameter paramDieSizeX;
        public readonly DoubleParameter paramDieSizeY;

        //=================================================================
        // Données internes
        //=================================================================
        private Dictionary<XY, Cluster> virtualClusters;
        private MilBuffer1d milLut;

        //=================================================================
        // Constructeur
        //=================================================================
        public VirtualGridModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramOffsetX = new DoubleParameter(this, "OffsetX");
            paramOffsetY = new DoubleParameter(this, "OffsetY");
            paramCutStepX = new DoubleParameter(this, "CutStepX");
            paramCutStepY = new DoubleParameter(this, "CutStepY");
            paramDieSizeX = new DoubleParameter(this, "DieSizeX");
            paramDieSizeY = new DoubleParameter(this, "DieSizeY");
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            bool valid = (paramCutStepX > 0 && paramCutStepY > 0 && paramDieSizeX > 0 && paramDieSizeY > 0);
            if (!valid)
                return "Invalid die definition";

            valid = (paramCutStepX >= paramDieSizeX && paramCutStepY >= paramDieSizeY);
            if (!valid)
                return "CutStep must be greater or equal to DieSize";

            return base.Validate();
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            InitOutputQueue();

            virtualClusters = new Dictionary<XY, Cluster>();

            // Création de la LUT
            //...................
            milLut = new MilBuffer1d();
            milLut.Alloc1d(256, 32 + MIL.M_SIGNED, MIL.M_LUT);
            int[] lut = new int[256];
            lut[0] = 0;
            for (int i = 1; i < 256; i++)
                lut[i] = 1;
            milLut.Put(lut);
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("from Id = " + parent.Id + " - cluster " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            Cluster cluster = (Cluster)obj;

            Rectangle dieIndexes = GetDieIndexes(cluster);
            for (int indexX = dieIndexes.Left; indexX <= dieIndexes.Right; indexX++)
            {
                for (int indexY = dieIndexes.Top; indexY <= dieIndexes.Bottom; indexY++)
                {
                    XY index = new XY(indexX, indexY);
                    bool intersect = CheckDieClusterIntersection(index, cluster);
                    if (intersect)
                        AddClusterToVirtualCluster(index, cluster);
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("VirtualGrid",
                () =>
                {
                    try
                    {
                        QueueVirtualClusters();
                    }
                    catch (Exception ex)
                    {
                        string msg = "virtual grid generation failed: " + ex.Message;
                        HandleException(new ApplicationException(msg, ex));
                    }
                    finally
                    {
                        PurgeVirtualClusters();
                        base.OnStopping(oldState);
                    }
                });
        }

        //=================================================================
        // 
        //=================================================================
        private Rectangle GetDieIndexes(Cluster cluster)
        {
            int xmin = (int)Math.Floor((cluster.micronQuad.Xmin - paramOffsetX) / paramCutStepX);
            int xmax = (int)Math.Ceiling((cluster.micronQuad.Xmax - paramOffsetX) / paramCutStepX);
            int ymin = (int)Math.Floor((cluster.micronQuad.Ymin - paramOffsetY) / paramCutStepY);
            int ymax = (int)Math.Ceiling((cluster.micronQuad.Ymax - paramOffsetY) / paramCutStepY);

            return Rectangle.FromLTRB(xmin, ymin, xmax, ymax);
        }

        //=================================================================
        // 
        //=================================================================
        private RectangleF GetDieRectangle(XY index)
        {
            double x = paramOffsetX + index.X * paramCutStepX;
            double y = paramOffsetY + index.Y * paramCutStepY;
            RectangleF dieMicronRect = new RectangleF((float)x, (float)y, (float)paramDieSizeX, (float)paramDieSizeY);
            return dieMicronRect;
        }

        //=================================================================
        // 
        //=================================================================
        private bool CheckDieClusterIntersection(XY index, Cluster cluster)
        {
            RectangleF dieMicronRect = GetDieRectangle(index);
            Rectangle diePixelRect = cluster.Layer.Matrix.micronToPixel(dieMicronRect);

            Rectangle r = diePixelRect;
            r.Intersect(cluster.pixelRect);

            if (r.Width == 0 || r.Height == 0)
                return false;

            using (MilImage milChild = new MilImage())
            using (var milStat = new MilImageResult())
            {
                MilImage milImage = cluster.ResultProcessingImage.GetMilImage();
                r = r.NegativeOffset(cluster.imageRect.TopLeft());
                milChild.Child2d(milImage, r.ToWinRect());


                milStat.AllocResult(milImage.OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);
                milStat.Stat(milChild, MIL.M_STAT_MAX);
                double m = milStat.GetResult(MIL.M_STAT_MAX);
                return m != 0;
            }
        }

        //=================================================================
        // 
        //=================================================================
        private int clusterIndex;
        private object mutex = new object();
        private void AddClusterToVirtualCluster(XY index, Cluster orginalCluster)
        {
            lock (mutex)
            {
                Cluster virtualCluster;

                bool found = virtualClusters.TryGetValue(index, out virtualCluster);
                if (!found)
                {
                    int clusterIndex = Interlocked.Increment(ref this.clusterIndex);
                    virtualCluster = new Cluster(clusterIndex, orginalCluster.Layer);
                    virtualClusters.Add(index, virtualCluster);

                    virtualCluster.DieIndexX = index.X;
                    virtualCluster.DieIndexY = index.Y;
                    virtualCluster.micronQuad = GetDieRectangle(index).ToQuadF();
                    virtualCluster.pixelRect = orginalCluster.Layer.Matrix.micronToPixel(virtualCluster.micronQuad.SurroundingRectangle);
                    virtualCluster.imageRect = orginalCluster.imageRect;
                    virtualCluster.characteristics.AddRange(orginalCluster.characteristics);
                    virtualCluster.defectClassList = new List<string>(orginalCluster.defectClassList);
                    virtualCluster.DieOffsetImage = new Point(0, 0);
                    virtualCluster.OriginalProcessingImage.SetMilImage(orginalCluster.OriginalProcessingImage.GetMilImage());
                    virtualCluster.ResultProcessingImage.SetMilImage(orginalCluster.ResultProcessingImage.GetMilImage());

                    // On crée un seul blob de la taille du die
                    virtualCluster.blobList = new List<Blob>();
                    Blob blob = new Blob(virtualCluster.Index, parentCluster: virtualCluster);
                    blob.characteristics[BlobCharacteristics.MicronArea] = orginalCluster.micronQuad.SurroundingRectangle.Area();
                    blob.micronQuad = virtualCluster.micronQuad;
                    blob.pixelRect = virtualCluster.pixelRect;
                    blob.characteristics.Add(ClusterCharacteristics.RealWidth, (double)blob.micronQuad.SurroundingRectangle.Width);
                    blob.characteristics.Add(ClusterCharacteristics.RealHeight, (double)blob.micronQuad.SurroundingRectangle.Height);
                    // pas de carac dans ce blob
                    virtualCluster.blobList.Add(blob);
                }
                else
                {
                    Blob blob = virtualCluster.blobList[0];
                    double area = (double)blob.characteristics[BlobCharacteristics.MicronArea];
                    area += orginalCluster.micronQuad.SurroundingRectangle.Area();
                    blob.characteristics[BlobCharacteristics.MicronArea] = area;
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void QueueVirtualClusters()
        {
            foreach (Cluster cl in virtualClusters.Values)
                outputQueue.Enqueue(cl);
        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeVirtualClusters()
        {
            foreach (Cluster cl in virtualClusters.Values)
                cl.DelRef();
            virtualClusters.Clear();

            if (milLut != null)
            {
                milLut.Dispose();
                milLut = null;
            }
        }

    }
}
