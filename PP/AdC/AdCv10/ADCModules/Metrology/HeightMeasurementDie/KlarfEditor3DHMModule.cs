using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.KlarfEditor_Die;

using Format001;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class KlarfEditor3DHMModule : KlarfEditorDieModule
    {
        //public readonly BoolParameter paramShiftedCoordinates;
        //=================================================================
        // Constructeur
        //=================================================================
        public KlarfEditor3DHMModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            //paramShiftedCoordinates = new BoolParameter(this, "CoordinatesShifted");
        }
        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit(); //!\ Sample Plan et DefectTypes list sont initialisé avec les données du klarf standard !!!

            if (ParamShiftedCoordinates.Value == true)
                DataKlarf.SampleCenterLocation = new PrmPtFloat((float)0.0, (float)0.0); // bottom left corner
            else DataKlarf.SampleCenterLocation = new PrmPtFloat((float)DataKlarf.SampleSize.waferDiameter_mm * 500.0f, (float)DataKlarf.SampleSize.waferDiameter_mm * 500.0f); // wafer size x/y  * 2
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
            Cluster3DDieHM cluster = (Cluster3DDieHM)obj;
            if (cluster == null || cluster.blobList == null || cluster.blobList.Count == 0)
                throw new ApplicationException("Empty cluster : " + obj);

            // CLUSTER INDEX
            int nClusterNum = cluster.Index;

            // ROUGHBIN
            int nClusterRoughBin = 0;
            int nClusterFineBin = 0;
            bool bUseCenterRect = ParamCenteringReport.Value;

            DieLayer mylayer = (DieLayer)GetLayer(this);
            DieImage dieimage = mylayer.GetDieImage(cluster.DieIndexX, cluster.DieIndexY);
            QuadF micronquadDie = mylayer.Matrix.pixelToMicron(dieimage.imageRect);
            RectangleF dierect_um = mylayer.GetDieMicronRect(cluster.DieIndexX, cluster.DieIndexY);

            List<PrmDefect> ltmp = new List<PrmDefect>(Math.Max(1, cluster.blobList.Count));

            if (cluster.characteristics.ContainsKey(Cluster3DCharacteristics.isHeightFailure) && (bool)cluster.characteristics[Cluster3DCharacteristics.isHeightFailure])
            {
                String sDieFailureLabel = "Die Average Height out of tolerance"; // should be the same in HeightMeasurementAnalysisModule.UpdateParams
                nClusterRoughBin = ParamRoughBins.RoughBins[sDieFailureLabel].RoughBinNum;
                nClusterFineBin = ParamRoughBins.RoughBins[sDieFailureLabel].FineBinNum;
                PrmDefect curDefect = DataKlarf.NewDefect();

                RectangleF rect_um = cluster.micronQuad.SurroundingRectangle;
                rect_um.Offset(-dierect_um.Left, -dierect_um.Top);
                double area = Math.Abs(rect_um.Area());

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
                curDefect.Set("XINDEX", cluster.DieIndexX);
                curDefect.Set("YINDEX", cluster.DieIndexY);
                //
                // END - SPECIFIC DIE KLARF
                //

                curDefect.Set("XSIZE", (double)rect_um.Width);
                curDefect.Set("YSIZE", (double)rect_um.Height);

                curDefect.Set("DEFECTAREA", area);
                curDefect.Set("DSIZE", cluster.characteristics[Cluster3DCharacteristics.HeightAverage]);

                curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le rought bon doit être chercher en fonction du type de classification ?
                //curDefect.Set("FINEBINNUMBER", nClusterFineBin);
                ltmp.Add(curDefect);

            }
            else if (cluster.characteristics.ContainsKey(Cluster3DCharacteristics.isCoplaFailure) && (bool)cluster.characteristics[Cluster3DCharacteristics.isCoplaFailure])
            {
                String sDieFailureLabel = "Die Coplanarity out of tolerance"; // should be the same in HeightMeasurementAnalysisModule.UpdateParams
                nClusterRoughBin = ParamRoughBins.RoughBins[sDieFailureLabel].RoughBinNum;
                nClusterFineBin = ParamRoughBins.RoughBins[sDieFailureLabel].FineBinNum;
                PrmDefect curDefect = DataKlarf.NewDefect();
                RectangleF rect_um = cluster.micronQuad.SurroundingRectangle;
                rect_um.Offset(-dierect_um.Left, -dierect_um.Top);

                double area = Math.Abs(rect_um.Area());

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
                curDefect.Set("XINDEX", cluster.DieIndexX);
                curDefect.Set("YINDEX", cluster.DieIndexY);
                //
                // END - SPECIFIC DIE KLARF
                //

                curDefect.Set("XSIZE", (double)rect_um.Width);
                curDefect.Set("YSIZE", (double)rect_um.Height);

                curDefect.Set("DEFECTAREA", area);
                curDefect.Set("DSIZE", cluster.characteristics[Cluster3DCharacteristics.HeightAverage]);

                curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le rought bon doit être chercher en fonction du type de classification ?
                //curDefect.Set("FINEBINNUMBER", nClusterFineBin);
                ltmp.Add(curDefect);

            }
            else if (cluster.characteristics.ContainsKey(Cluster3DCharacteristics.isSubCoplaFailure) && (bool)cluster.characteristics[Cluster3DCharacteristics.isSubCoplaFailure])
            {
                String sDieFailureLabel = "Die Substrate Coplanarity out of tolerance"; // should be the same in HeightMeasurementAnalysisModule.UpdateParams
                nClusterRoughBin = ParamRoughBins.RoughBins[sDieFailureLabel].RoughBinNum;
                nClusterFineBin = ParamRoughBins.RoughBins[sDieFailureLabel].FineBinNum;
                PrmDefect curDefect = DataKlarf.NewDefect();
                RectangleF rect_um = cluster.micronQuad.SurroundingRectangle;
                rect_um.Offset(-dierect_um.Left, -dierect_um.Top);

                double area = Math.Abs(rect_um.Area());

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
                curDefect.Set("XINDEX", cluster.DieIndexX);
                curDefect.Set("YINDEX", cluster.DieIndexY);
                //
                // END - SPECIFIC DIE KLARF
                //

                curDefect.Set("XSIZE", (double)rect_um.Width);
                curDefect.Set("YSIZE", (double)rect_um.Height);

                curDefect.Set("DEFECTAREA", area);
                curDefect.Set("DSIZE", cluster.characteristics[Cluster3DCharacteristics.HeightAverage]);

                curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le rought bon doit être chercher en fonction du type de classification ?
                //curDefect.Set("FINEBINNUMBER", nClusterFineBin);
                ltmp.Add(curDefect);

            }
            // on passe en revue les measures
            else
            {

                //double MissingFailureType = 1.0;  //"Missing";
                //double BadHeightFailureType = 2.0; // "BadHeight";
                String[] ArFailureLabel = new String[] { String.Empty, "Missing Measure", "Height Measure out of tolerance" };
                double maxHeight = -300000.0;

                Blob selectBlob = null;
                bool bsuccess = false;

                foreach (Blob blb in cluster.blobList)
                {
                    if (!blb.characteristics.ContainsKey(Blob3DCharacteristics.FailureType))
                        continue; // analysis module should be missing or not have been correctly updated

                    int nFailureType = (int)((double)blb.characteristics[Blob3DCharacteristics.FailureType]);
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

                    if ((double)blb.characteristics[Blob3DCharacteristics.HeightMicron] > maxHeight)
                    {
                        maxHeight = (double)blb.characteristics[Blob3DCharacteristics.HeightMicron];
                        selectBlob = blb;
                        bsuccess = true; //one of them put flag to true
                    }

                }

                if (bsuccess)
                {
                    PrmDefect curDefect = DataKlarf.NewDefect();

                    RectangleF rect_um = cluster.micronQuad.SurroundingRectangle;

                    //rect_um.Offset(_dataKlarf.SampleCenterLocation.x, _dataKlarf.SampleCenterLocation.y);
                    rect_um.Offset(-dierect_um.Left, -dierect_um.Top);

                    double area = Math.Abs(rect_um.Area());

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
                    curDefect.Set("XINDEX", cluster.DieIndexX);
                    curDefect.Set("YINDEX", cluster.DieIndexY);
                    //
                    // END - SPECIFIC DIE KLARF
                    //

                    curDefect.Set("XSIZE", (double)rect_um.Width);
                    curDefect.Set("YSIZE", (double)rect_um.Height);

                    curDefect.Set("DEFECTAREA", area);
                    //curDefect.Set("DSIZE", selectBlob.characteristics[Blob3DCharacteristics.HeightMicron]);
                    curDefect.Set("DSIZE", cluster.characteristics[Cluster3DCharacteristics.HeightAverage]);

                    curDefect.Set("CLASSNUMBER", 0);
                    curDefect.Set("TEST", 1);
                    curDefect.Set("CLUSTERNUMBER", nClusterNum);

                    curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le rought bon doit être chercher en fonction du type de classification ?
                                                                       //curDefect.Set("FINEBINNUMBER", nClusterFineBin);
                    if (bsuccess)
                        ltmp.Add(curDefect);
                }

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
