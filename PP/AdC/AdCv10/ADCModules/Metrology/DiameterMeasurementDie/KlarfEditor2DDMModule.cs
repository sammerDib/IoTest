using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.KlarfEditor_Die;
//using System.Windows.Controls;
using Format001;

namespace DiameterMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class KlarfEditor2DDMModule : KlarfEditorDieModule
    {
        //public readonly BoolParameter paramShiftedCoordinates;
        //=================================================================
        // Constructeur
        //=================================================================
        public KlarfEditor2DDMModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            //  paramShiftedCoordinates = new BoolParameter(this, "CoordinatesShifted");
        }
        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit(); //!\ Sample Plan et DefectTypes list sont initialisé avec les données du klarf standard !!!

            if (ParamShiftedCoordinates.Value == true)
                DataKlarf.SampleCenterLocation = new PrmPtFloat((float)0.0, (float)0.0); // bottom left corner
            //else _dataKlarf.SampleCenterLocation = new PrmPtFloat((float)_dataKlarf.SampleSize.waferDiameter_mm * 500.0f, (float)_dataKlarf.SampleSize.waferDiameter_mm * 500.0f); // wafer size x/y  * 2
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
            Cluster2DDieDM cluster = (Cluster2DDieDM)obj;
            if (cluster == null || cluster.blobList == null || cluster.blobList.Count == 0)
                throw new ApplicationException("Empty cluster : " + obj);
            DieLayer layer = (DieLayer)GetLayer(this);

            // CLUSTER INDEX
            int nClusterNum = cluster.Index;
            int nDieIdx_X = cluster.DieIndexX;
            int nDieIdx_Y = cluster.DieIndexY;

            // ROUGHBIN
            int nClusterRoughBin = 0;
            int nClusterFineBin = 0;
            bool bUseCenterRect = ParamCenteringReport.Value;

            DieLayer mylayer = (DieLayer)GetLayer(this);
            DieImage dieimage = mylayer.GetDieImage(cluster.DieIndexX, cluster.DieIndexY);
            QuadF micronquadDie = mylayer.Matrix.pixelToMicron(dieimage.imageRect);
            RectangleF dierect_um = layer.GetDieMicronRect(cluster.DieIndexX, cluster.DieIndexY);

            List<PrmDefect> ltmp = new List<PrmDefect>(Math.Max(1, cluster.blobList.Count));

            PrmDefect curDefect = DataKlarf.NewDefect();
            double area = 0.0;

            if (cluster.characteristics.ContainsKey(Cluster2DCharacteristics.isDiameterFailure) && (bool)cluster.characteristics[Cluster2DCharacteristics.isDiameterFailure])
            {
                String sDieFailureLabel = "Die Average Diameter out of tolerance"; // should be the same in DiametertMeasurementAnalysisModule.UpdateParams
                nClusterRoughBin = ParamRoughBins.RoughBins[sDieFailureLabel].RoughBinNum;
                nClusterFineBin = ParamRoughBins.RoughBins[sDieFailureLabel].FineBinNum;

                RectangleF rect_um = cluster.micronQuad.SurroundingRectangle;
                rect_um.Offset(-dierect_um.X, -dierect_um.Y);
                area = Math.Abs(rect_um.Area());

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

                //
                // SPECIFIC DIE KLARF CLUSTER
                //
                curDefect.Set("XINDEX", nDieIdx_X);
                curDefect.Set("YINDEX", nDieIdx_Y);
                //
                // END - SPECIFIC DIE KLARF
                //

                curDefect.Set("XSIZE", (double)rect_um.Width);
                curDefect.Set("YSIZE", (double)rect_um.Height);

                curDefect.Set("DEFECTAREA", area);
                curDefect.Set("DSIZE", cluster.characteristics[Cluster2DCharacteristics.DiameterAverage]);

                curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le rought bon doit être chercher en fonction du type de classification ?
                //curDefect.Set("FINEBINNUMBER", nClusterFineBin);
                ltmp.Add(curDefect);

            }
            else
            {
                // on passe en revue les measures

                // double MissingFailureType = 1.0;  //"Missing";
                // double BadDiameterFailureType = 2.0; // "BadDiameter";
                // double BadOffsetFailureType = 3.0; // "BadOffset";

                String[] ArFailureLabel = new String[] { String.Empty, "Missing Measure", "Diameter Measure out of tolerance", "Offset Measure out of tolerance" };

                Blob localBlob = new Blob(nClusterNum, cluster);
                int min_x = 300000, min_y = 300000, max_x = -300000, max_y = -300000;
                double sizeX = 0, sizeY = 0;

                int nFailureType = 0;

                foreach (Blob blb in cluster.blobList)
                {
                    if (!blb.characteristics.ContainsKey(Blob2DCharacteristics.FailureType))
                        continue; // analysis module should be missing or not have been correctly updated

                    nFailureType = (int)((double)blb.characteristics[Blob2DCharacteristics.FailureType]);
                    if (nFailureType == 0)
                        continue; // not a defect

                    if (ParamRoughBins.RoughBins.ContainsKey(ArFailureLabel[nFailureType]))
                    {
                        nClusterRoughBin = ParamRoughBins.RoughBins[ArFailureLabel[nFailureType]].RoughBinNum;
                        nClusterFineBin = ParamRoughBins.RoughBins[ArFailureLabel[nFailureType]].FineBinNum;
                    }
                    else
                    {
                        nClusterRoughBin = 0;
                        nClusterFineBin = 0;
                    }

                    area += (double)blb.characteristics[BlobCharacteristics.MicronArea];
                    sizeX += (double)blb.characteristics[ClusterCharacteristics.RealWidth];
                    sizeY += (double)blb.characteristics[ClusterCharacteristics.RealHeight];

                    if (min_x > (int)blb.micronQuad.Xmin)
                        min_x = (int)blb.micronQuad.Xmin;
                    if (max_x < (int)blb.micronQuad.Xmax)
                        max_x = (int)blb.micronQuad.Xmax;
                    //
                    if (min_y > (int)blb.micronQuad.Ymin)
                        min_y = (int)blb.micronQuad.Ymin;
                    if (max_y < (int)blb.micronQuad.Ymax)
                        max_y = (int)blb.micronQuad.Ymax;
                }

                RectangleF blobRect = new RectangleF(min_x, min_y, max_x - min_x, max_y - min_y);

                //rect_um.Offset(_dataKlarf.SampleCenterLocation.x, _dataKlarf.SampleCenterLocation.y);
                blobRect.Offset(-dierect_um.X, -dierect_um.Y);

                area = Math.Abs(blobRect.Area());

                //-- Orign blob is CENTER of rect -- In viewer defect rect is defined be its BOTTOM LEFT origin
                if (bUseCenterRect)
                {
                    PointF Mid = blobRect.Middle();
                    curDefect.Set("XREL", (double)Mid.X);
                    curDefect.Set("YREL", (double)Mid.Y);
                }
                else
                {
                    //-- Orign blob BOTTOM LEFT of rect -- In viewer rect is defined like that
                    curDefect.Set("XREL", (double)blobRect.Left);
                    curDefect.Set("YREL", (double)blobRect.Bottom);
                }

                //
                // SPECIFIC DIE KLARF CLUSTER
                //
                curDefect.Set("XINDEX", nDieIdx_X);
                curDefect.Set("YINDEX", nDieIdx_Y);
                //
                // END - SPECIFIC DIE KLARF
                //

                curDefect.Set("XSIZE", (double)blobRect.Width);
                curDefect.Set("YSIZE", (double)blobRect.Height);

                curDefect.Set("DEFECTAREA", area);

                if (nFailureType == 1)
                    curDefect.Set("DSIZE", 0.0);
                else
                    curDefect.Set("DSIZE", Math.Sqrt(area));

                curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le rought bon doit être chercher en fonction du type de classification ?
                //curDefect.Set("FINEBINNUMBER", nClusterFineBin);
                ltmp.Add(curDefect);
            }

            //-------------------------------------------------------------
            // Update Klarf data defect list 
            //-------------------------------------------------------------
            if (ltmp.Count != 0)
            {
                lock (Syncklarf)
                {
                    int nLastDefectId = DataKlarf.NDEFECT;

                    // Warning ici on aura pas tous les dies surtout si ya pas de defautls on ne verra rien passé
                    //_dataKlarf.SampleTestPlan.TryAdd(cluster.DieIndexX, cluster.DieIndexY);

                    PrmImageData imgData = new PrmImageData();
                    if (ParamMultiTiff.Value)
                        SaveThumbnails(cluster, imgData);

                    for (int i = 0; i < ltmp.Count; i++)
                    {
                        ltmp[i].Set("DEFECTID", nLastDefectId + i + 1);
                        ltmp[i].Set("IMAGECOUNT", imgData.List.Count);
                        ltmp[i].Set("IMAGELIST", imgData);
                        DataKlarf.AddDefect(ltmp[i]);
                    }
                }
            }

            Interlocked.Increment(ref nbObjectsOut);
        }

    }
}
