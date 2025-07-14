using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.Edition.KlarfEditor;
using BasicModules.KlarfEditor;

using Format001;

using UnitySC.Shared.Data.Enum;


namespace BasicModules.Edition.KlarfEditorEdge
{
    public class KlarfEditorEdgeModule : KlarfEditorModule
    {
        // Par rapport au klarf classic GRADE ZABS ZONEID sont ajouté  ET REVIEWSAMPLE est suprrimés
        private readonly String[] sEdgeDefectTypeCategories =
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
            "GRADE",
            "ZABS",
            "ZONEID",
            "CLASSNUMBER",
            "ROUGHBINNUMBER",
            "FINEBINNUMBER",
            "TEST",
            "CLUSTERNUMBER",
            "IMAGECOUNT",
            "IMAGELIST"
        };

        //=================================================================
        // Constructeur
        //=================================================================
        public KlarfEditorEdgeModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {



        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();  //!\ Sample Plan et DefectTypes list sont initialisé avec les données du klarf standard !!!

            DataKlarf.ResetDefectListType();
            DataKlarf.AddDefectTypes(sEdgeDefectTypeCategories); // warning defect type should be registered or you will have to declare and defined type by yourself using DataKlarf.AddDefectType(Type p_type, String p_name)
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
                throw new ApplicationException("Empty cluster");

            int nClusterNum = cluster.Index;
            int nClusterRoughBin = 0;
            int nClusterFineBin = 0;
            if (ParamRoughBins.RoughBins.ContainsKey(cluster.DefectClass))
            {
                nClusterRoughBin = ParamRoughBins.RoughBins[cluster.DefectClass].RoughBinNum;
                nClusterFineBin = ParamRoughBins.RoughBins[cluster.DefectClass].FineBinNum;
            }

            ImageLayerBase mylayer = cluster.Layer as ImageLayerBase;
            if ((mylayer.ResultType.GetActorType() != ActorType.Edge))
            {
                throw new ApplicationException($"Wrong ActorTypeId ({mylayer.ResultType.GetActorType()})");
            }

            //TODO Orientation et repère pour le positionnement et orientation du Notch/Flat - to get
            double dNotchAngle_dg = 0.0;  // orient en bas sens trigonométique (+ ds le sens anticlockwise);
            int iZoneID = 0;


#pragma warning disable CS0162 // Unreachable code detected // to remove when the edge module result type will be handle   

            switch (mylayer.ResultType)
            {
               //case (int)eChannelID.EyeEdge_Up: iZoneID = 8; break; //eChannelID.EyeEdge_Up //0
               //case (int)eChannelID.EyeEdge_UpBevel: iZoneID = 16; break;//EyeEdge_UpBevel //1
               //case (int)eChannelID.EyeEdge_Apex: iZoneID = 32; break; //EyeEdge_Apex //2
               //case (int)eChannelID.EyeEdge_BottomBevel: iZoneID = 64; break; //EyeEdge_BottomBevel //3
               //case (int)eChannelID.EyeEdge_Bottom: iZoneID = 128; break;//EyeEdge_Bottom //4

                default:
                    throw new NotImplementedException($"ResultType for {ActorType.Edge} are ot yet implemented in USP ADC");
                   // throw new ApplicationException(String.Format("Unknown ResultType (\"{0}\")", mylayer.ResultType));
            }

            bool bUseCenterRect = ParamCenteringReport.Value;

            List<PrmDefect> ltmp = new List<PrmDefect>(Math.Max(1, cluster.blobList.Count));

            foreach (Blob blb in cluster.blobList)
            {
                PrmDefect curDefect = DataKlarf.NewDefect();
                curDefect.SurroundingRectangleMicron = blb.micronQuad.SurroundingRectangle;

                RectangleF rect_um = blb.micronQuad.SurroundingRectangle;
                rect_um.Offset(DataKlarf.SampleCenterLocation.x, DataKlarf.SampleCenterLocation.y);

                double dX_um = 0.0;
                double dY_um = 0.0;
                //-- Orign blob is CENTER of rect -- In viewer defect rect is defined be its BOTTOM LEFT origin
                if (bUseCenterRect)
                {
                    PointF Mid = rect_um.Middle();
                    curDefect.Set("XREL", (double)Mid.X);
                    curDefect.Set("YREL", (double)Mid.Y);

                    dX_um = Mid.X;
                    dY_um = Mid.Y;
                }
                else
                {
                    //-- Orign blob BOTTOM LEFT of rect -- In viewer rect is defined like that
                    curDefect.Set("XREL", (double)rect_um.Left);
                    curDefect.Set("YREL", (double)rect_um.Bottom);
                    dX_um = rect_um.Left;
                    dY_um = rect_um.Bottom;
                }

                curDefect.Set("XSIZE", (double)blb.characteristics[SizingCharacteristics.sizeX]);
                curDefect.Set("YSIZE", (double)blb.characteristics[SizingCharacteristics.sizeY]);

                //
                // SPECIFIC EDGE KLARF CLUSTER
                //

                // Compute Angle and Radius position

                double dAngle_dg = Math.Atan2(dY_um, dX_um) * (180.0 / Math.PI); // de [-pi; pi] sur [0.0 360.0[ 
                if (dAngle_dg < 0.0)
                {
                    dAngle_dg += 360.0; ;
                }

                //TODO Orientation et repère pour le positionnement et orientation du Notch/Flat - to validate
                // par défaut on considère le notch en bas on doit donc ajouter 90° à l'angle ainsi calculer
                dAngle_dg += 90.0 - dNotchAngle_dg;
                curDefect.Set("GRADE", dAngle_dg);

                double dRadius_um = Math.Sqrt(dX_um * dX_um + dY_um * dY_um);
                curDefect.Set("ZABS", dRadius_um);

                curDefect.Set("ZONEID", iZoneID);

                //
                // END - SPECIFIC EDGE KLARF
                //

                curDefect.Set("DEFECTAREA", (double)blb.characteristics[SizingCharacteristics.DefectArea]);
                curDefect.Set("DSIZE", (double)blb.characteristics[SizingCharacteristics.DSize]);

                //curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le rought bon doit être chercher en fonction du type de classification ?
                curDefect.Set("FINEBINNUMBER", nClusterFineBin);

                curDefect.Set("TEST", 1);

                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                ltmp.Add(curDefect);
            }

            //-------------------------------------------------------------
            // Update Klarf data defect list 
            //-------------------------------------------------------------
            lock (Syncklarf)
            {
                int nLastDefectId = DataKlarf.NDEFECT;

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
                using (KlarfCluster klarfBlob = CreateKlarfCluster(ltmp, cluster.DefectClass, nClusterRoughBin, (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize]))
                {
                    ProcessChildren(klarfBlob);
                }
                SendKlarfResult();
            }
        }
    }

}
