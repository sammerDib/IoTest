using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.DataLoader;
using BasicModules.Edition.KlarfEditor;
using BasicModules.KlarfEditor;
using BasicModules.Sizing;

using Format001;


namespace BasicModules.KlarfEditor_Die
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class KlarfEditorDieModule : KlarfEditorModule
    {
        // Par rapport au klarf classic FINEBINNUMBER ET REVIEWSAMPLE sont suprrimés
        private readonly String[] sDieDefectTypeCategories =
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
            "IMAGECOUNT",
            "IMAGELIST"
        };


        public DieLayer mylayer;


        public readonly BoolParameter paramDieOriginToZero;

        //=================================================================
        // Constructeur
        //=================================================================
        public KlarfEditorDieModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramDieOriginToZero = new BoolParameter(this, "Die Origin to zero");
            paramDieOriginToZero.Value = true;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit(); //!\ Sample Plan et DefectTypes list sont initialisé avec les données du klarf standard !!!

            mylayer = (DieLayer)GetLayer(this);
            DataKlarf.DiePitch = new PrmPtFloat((float)mylayer.DiePitchX_µm, (float)mylayer.DiePitchY_µm); // wafer size x/y
            RectangleF dierect_um = mylayer.GetDieMicronRect(0, 0);
            DataKlarf.DieOrigin = new PrmPtFloat(0, 0);
            DataKlarf.DieSize = new PrmPtFloat((float)mylayer.DieSizeX_µm, (float)mylayer.DieSizeY_µm);
            DataKlarf.SampleCenterLocation = new PrmPtFloat(dierect_um.X, dierect_um.Y);
            DataKlarf.NbDies = mylayer.NbDies;
            List<KeyValuePair<int, int>> ListDieIndexes = new List<KeyValuePair<int, int>>();

            DataKlarf.SampleTestPlan = new PrmSampleTestPlan(ListDieIndexes);

            DataKlarf.ResetDefectListType();
            DataKlarf.AddDefectTypes(sDieDefectTypeCategories); // warning defect type should be registered or you will have to declare and defined type by yourself using DataKlarf.AddDefectType(Type p_type, String p_name)

            if (ParamShiftedCoordinates.Value == true)
                DataKlarf.SampleCenterLocation = new PrmPtFloat((float)0.0, (float)0.0); // bottom left corner

            if (paramDieOriginToZero.Value == false)
            {
                DataKlarf.DieOrigin.x = (float)mylayer.DieOriginX_µm;
                DataKlarf.DieOrigin.y = (float)mylayer.DieOriginY_µm;
            }


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
                throw new ApplicationException("Empty cluster : " + obj);

            // CLUSTER INDEX
            if (ParamCompactIndex.Value == true)
            {
                if (cluster.blobList.Count <= 1)   // pour IBM permet de ne pas avoir de trou dans les index de clusters (IBM limité à 32000)
                    cluster.Index = 0;
            }
            int nClusterNum = cluster.Index;

            // ROUGHBIN
            int nClusterRoughBin = 0;
            if (ParamRoughBins.RoughBins.ContainsKey(cluster.DefectClass))
                nClusterRoughBin = ParamRoughBins.RoughBins[cluster.DefectClass].RoughBinNum;

            bool bUseCenterRect = ParamCenteringReport.Value;

            DieLayer mylayer = (DieLayer)GetLayer(this);
            DieImage dieimage = mylayer.GetDieImage(cluster.DieIndexX, cluster.DieIndexY);
            //QuadF micronquadDie = mylayer.PixelToMicron(dieimage.imageRect);
            RectangleF dierect_um = mylayer.GetDieMicronRect(cluster.DieIndexX, cluster.DieIndexY);


            eSizingType sizingType = (eSizingType)cluster.characteristics[SizingCharacteristics.SizingType];
            List<PrmDefect> ltmp = new List<PrmDefect>(Math.Max(1, cluster.blobList.Count));
            foreach (Blob blb in cluster.blobList)
            {
                PrmDefect curDefect = DataKlarf.NewDefect();
                curDefect.SurroundingRectangleMicron = blb.micronQuad.SurroundingRectangle;

                RectangleF rect_um = mylayer.GetRectCoordInDie(cluster, dierect_um, blb.micronQuad.SurroundingRectangle);
                //rect_um.Offset(-dierect_um.Left, -dierect_um.Top);

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

                curDefect.Set("XSIZE", (double)blb.characteristics[SizingCharacteristics.sizeX]);
                curDefect.Set("YSIZE", (double)blb.characteristics[SizingCharacteristics.sizeY]);

                double area = (double)blb.characteristics[SizingCharacteristics.DefectArea];
                curDefect.Set("DEFECTAREA", area);
                curDefect.Set("DSIZE", (double)blb.characteristics[SizingCharacteristics.DSize]);


                curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le rought bon doit être chercher en fonction du type de classification ?
                ltmp.Add(curDefect);

                SendKlarfResult();
            }


            //-------------------------------------------------------------
            // Update Klarf data defect list 
            //-------------------------------------------------------------
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

            if (ltmp.Any())
            {
                using (KlarfCluster klarfCluster = CreateKlarfCluster(ltmp, cluster.DefectClass, nClusterRoughBin, (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize]))
                {
                    ProcessChildren(klarfCluster);
                    SendKlarfResult();
                }
            }

        }

        //=================================================================
        //
        //=================================================================
        protected override void ProcessKlarf()
        {
            // On rempli le sampel test avec les die indexes à la fin car on est sur d'avoir tout reçu
            BuildSampleTestPlan();

            base.ProcessKlarf();
        }

        //=================================================================
        //
        //=================================================================
        private void BuildSampleTestPlan()
        {
            List<ModuleBase> AncestorDieLoadermodule = FindAncestors(mod => mod is DieDataLoaderBase);
            if (AncestorDieLoadermodule.Count == 0)
            {
                throw new ApplicationException("No Dieloader module has been set prior to this module");
            }
            DieDataLoaderBase DirectAncestor = AncestorDieLoadermodule[0] as DieDataLoaderBase;
            DieLayer mydielayer = ((DirectAncestor.Layer) as DieLayer);

            IEnumerable<DieImage> list = mydielayer.GetDieImageList();

            foreach (DieImage dieImage in list)
                DataKlarf.SampleTestPlan.Add(dieImage.DieIndexX, dieImage.DieIndexY);
        }

        //=================================================================
        //
        //=================================================================
        protected LayerBase GetLayer(ModuleBase module)
        {
            List<ModuleBase> loaders = module.FindAncestors(m => m is IDataLoader);
            if (loaders.Count == 0)
                return null;

            IDataLoader loader = (IDataLoader)loaders[0];
            return loader.Layer;
        }
    }
}
