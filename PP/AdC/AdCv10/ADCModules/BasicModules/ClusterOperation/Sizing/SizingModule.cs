using System;
using System.Drawing;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

namespace BasicModules.Sizing
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    internal class SizingModule : ModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly SizingParameter paramSizing;


        //=================================================================
        // Constructeur
        //=================================================================
        public SizingModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramSizing = new SizingParameter(this, "Sizing");
            ModuleProperty = eModuleProperty.Stage;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("sizing " + obj);
            Cluster cluster = (Cluster)obj;
            Interlocked.Increment(ref nbObjectsIn);

            Measure(cluster, paramSizing);

            double defectMaxSize = (double)cluster.characteristics[SizingCharacteristics.DefectMaxSize];
            double totalDefectSize = (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize];
            logDebug(cluster.ToString() + " DefectMaxSize: " + defectMaxSize + " TotalDefectSize: " + totalDefectSize);

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        public static void Measure(Cluster cluster, SizingParameter paramSizing)
        {
            string defectClass = cluster.DefectClass;
            SizingClass sizingClass = paramSizing.SizingClasses[defectClass];
            cluster.characteristics[SizingCharacteristics.SizingType] = sizingClass.Measure;

            switch (sizingClass.Measure)
            {
                case eSizingType.ByLength:
                    MeasureByLength(cluster);
                    TuneCluster(cluster, sizingClass);
                    break;
                case eSizingType.ByArea:
                    MeasureByArea(cluster);
                    break;
                case eSizingType.ByDiameter:
                    MeasureByDiameter(cluster);
                    TuneCluster(cluster, sizingClass);
                    break;
                case eSizingType.ByPSLLut:
                    MeasureByPSLLut(cluster);
                    break;
                case eSizingType.ByHeight3D:
                    MeasureByHeight3D(cluster);
                    break;
                default:
                    throw new ApplicationException("unknown sizing type:" + sizingClass.Measure);
            }
        }

        //=================================================================
        //
        //=================================================================
        public static void MeasureByLength(Cluster cluster)
        {
            // On recalcule la taille à partir des blobs, c'est plus précis
            // que de prendre la taille du cluster.
            //.............................................................
            RectangleF micronRect = RectangleF.Empty;
            double totalWidth = 0;
            double totalHeight = 0;
            foreach (Blob blob in cluster.blobList)
            {
                RectangleF blobRect = blob.micronQuad.SurroundingRectangle;
                micronRect = RectangleExtension.Union(micronRect, blobRect);
                totalWidth += blobRect.Width;
                totalHeight += blobRect.Height;

                blob.characteristics.Add(SizingCharacteristics.sizeX, (double)blobRect.Width);
                blob.characteristics.Add(SizingCharacteristics.sizeY, (double)blobRect.Height);
                blob.characteristics.Add(SizingCharacteristics.DefectArea, (double)blob.characteristics[BlobCharacteristics.MicronArea]);
                //blob.characteristics.Add(SizingCharacteristics.DSize, Math.Sqrt((double)(blob.characteristics[BlobCharacteristics.MicronArea])));
                double dWidth = Math.Pow((double)blobRect.Width, 2);
                double dHeight = Math.Pow((double)blobRect.Height, 2);
                blob.characteristics.Add(SizingCharacteristics.DSize, Math.Sqrt((double)dWidth + dHeight));

            }

            // Max Length
            //...........
            double maxLength;
            if (micronRect.Width > micronRect.Height)
                maxLength = micronRect.Width;
            else
                maxLength = micronRect.Height;

            cluster.characteristics.Add(SizingCharacteristics.DefectMaxSize, maxLength);

            // Total Length
            //.............
            double totalLength = 0;
            if (totalWidth > totalHeight)
                totalLength = totalWidth;
            else
                totalLength = totalHeight;

            cluster.characteristics.Add(SizingCharacteristics.TotalDefectSize, totalLength);
        }

        //=================================================================
        // 
        //=================================================================
        public static void MeasureByArea(Cluster cluster)
        {
            double totalArea = 0;

            // Sizing/AREA pour DSize=> saturated Sqrt(MicronArea), unsaturated, Sqrt(MicronArea) (et du coup on s'en cogne que ce soit saturé ou pas ... )
            // les autres colonnes, on s'en fout, on met du standard dedans

            if (cluster.characteristics.ContainsKey(ClusterCharacteristics.PSLValue))
            {
                int min_x = 300000, min_y = 300000, max_x = -300000, max_y = -300000;

                foreach (Blob blb in cluster.blobList)
                {
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
                // report basé sur les taille de pixel en µm
                RectangleF blobRect = new RectangleF(min_x, min_y, max_x - min_x, max_y - min_y);

                foreach (Blob blob in cluster.blobList)
                {
                    blob.characteristics.Add(SizingCharacteristics.sizeX, (double)blobRect.Width);
                    blob.characteristics.Add(SizingCharacteristics.sizeY, (double)blobRect.Height);
                    blob.characteristics.Add(SizingCharacteristics.DefectArea, (double)blob.characteristics[BlobCharacteristics.MicronArea]);
                    //blob.characteristics.Add(SizingCharacteristics.DSize, Math.Sqrt((double)(blob.characteristics[BlobCharacteristics.MicronArea])));
                    blob.characteristics.Add(SizingCharacteristics.DSize, (double)blob.characteristics[BlobCharacteristics.MicronArea]);

                    double blobArea = (double)blob.characteristics[BlobCharacteristics.MicronArea];
                    totalArea += blobArea;
                }
            }
            else
            {
                foreach (Blob blob in cluster.blobList)
                //totalArea += blob.micronQuad.Area();
                {
                    RectangleF blobRect = blob.micronQuad.SurroundingRectangle;

                    blob.characteristics.Add(SizingCharacteristics.sizeX, (double)blobRect.Width);
                    blob.characteristics.Add(SizingCharacteristics.sizeY, (double)blobRect.Height);
                    blob.characteristics.Add(SizingCharacteristics.DefectArea, (double)blob.characteristics[BlobCharacteristics.MicronArea]);
                    //blob.characteristics.Add(SizingCharacteristics.DSize, Math.Sqrt((double)(blob.characteristics[BlobCharacteristics.MicronArea])));
                    blob.characteristics.Add(SizingCharacteristics.DSize, (double)blob.characteristics[BlobCharacteristics.MicronArea]);

                    //blob.characteristics.Add(SizingCharacteristics.sizeX, (double)blobRect.Width);
                    //blob.characteristics.Add(SizingCharacteristics.sizeY, (double)blobRect.Height);
                    //blob.characteristics.Add(SizingCharacteristics.DefectArea, (double)blob.characteristics[BlobCharacteristics.MicronArea]);
                    //blob.characteristics.Add(SizingCharacteristics.DSize, Math.Sqrt((double)blob.characteristics[BlobCharacteristics.MicronArea]));
                    double blobArea = (double)blob.characteristics[BlobCharacteristics.MicronArea];
                    totalArea += blobArea;
                }
            }

            // OPI : Données devenues probablement fausses au vu des changements opéré sur la CharacPSL et pas prit en compte dans la spec Post-It ...
            // On verra à l'usage si un client gueule sinon on virera. 
            cluster.characteristics.Add(SizingCharacteristics.DefectMaxSize, (double)cluster.characteristics[ClusterCharacteristics.Area]);
            cluster.characteristics.Add(SizingCharacteristics.TotalDefectSize, totalArea);
        }

        //=================================================================
        // 
        //=================================================================
        public static void MeasureByDiameter(Cluster cluster)
        {
            RectangleF clusterRect = cluster.micronQuad.SurroundingRectangle;
            //double diameter = PointSizeExtension.Distance(clusterRect.TopLeft(), clusterRect.BottomRight());
            double diameter = (double)cluster.characteristics[ClusterCharacteristics.RealDiameter];

            cluster.characteristics.Add(SizingCharacteristics.DefectMaxSize, diameter);
            cluster.characteristics.Add(SizingCharacteristics.TotalDefectSize, diameter);

            RectangleF micronRect = RectangleF.Empty;
            foreach (Blob blob in cluster.blobList)
            {
                RectangleF blobRect = blob.micronQuad.SurroundingRectangle;

                blob.characteristics.Add(SizingCharacteristics.sizeX, (double)blobRect.Width);
                blob.characteristics.Add(SizingCharacteristics.sizeY, (double)blobRect.Height);
                blob.characteristics.Add(SizingCharacteristics.DefectArea, (double)blob.characteristics[BlobCharacteristics.MicronArea]);
                blob.characteristics.Add(SizingCharacteristics.DSize, Math.Sqrt((double)blob.characteristics[BlobCharacteristics.MicronArea]));

            }
        }


        //=================================================================
        // 
        //=================================================================
        public static void MeasureByPSLLut(Cluster cluster)
        {
            // IMPORTANT !!! la donnée MicronArea est déjà caluclé dans la CharacLSE pour une donnée saturé ou non saturé
            // Sizing/PSL => saturated : LSEMAX*NBPIXEL, unsaturated : PSLBYBLOB
            // les SizeX/SizeY ont la valeur des PSL satured or not

            if (!cluster.characteristics.ContainsKey(ClusterCharacteristics.PSLValue))
                throw new ApplicationException("Cluster have no PSL value - PSL Characterization is probably missing");


            foreach (Blob blob in cluster.blobList)
            {
                RectangleF blobRect = blob.micronQuad.SurroundingRectangle;

                blob.characteristics.Add(SizingCharacteristics.sizeX, (double)blobRect.Width);
                blob.characteristics.Add(SizingCharacteristics.sizeY, (double)blobRect.Height);
                //blob.characteristics.Add(SizingCharacteristics.sizeX, blob.characteristics[BlobCharacteristics.MicronArea]);
                //blob.characteristics.Add(SizingCharacteristics.sizeY, blob.characteristics[BlobCharacteristics.MicronArea]);
                //blob.characteristics.Add(SizingCharacteristics.DefectArea, Math.Pow((double)blob.characteristics[BlobCharacteristics.MicronArea],2));
                blob.characteristics.Add(SizingCharacteristics.DefectArea, (double)blob.characteristics[BlobCharacteristics.MicronArea]);
                blob.characteristics.Add(SizingCharacteristics.DSize, (double)blob.characteristics[BlobCharacteristics.pslµsize]);
            }

            // OPI : Données devenues probablement fausses au vu des changements opéré sur la CharacPSL et pas prit en compte dans la spec Post-It ...
            // On verra à l'usage si un client gueule sinon on virera. 
            cluster.characteristics.Add(SizingCharacteristics.DefectMaxSize, cluster.characteristics[ClusterCharacteristics.PSLMaxValue]);
            cluster.characteristics.Add(SizingCharacteristics.TotalDefectSize, cluster.characteristics[ClusterCharacteristics.PSLValue]);
        }

        //=================================================================
        // 
        //=================================================================
        public static void MeasureByHeight3D(Cluster cluster)
        {
            if (!cluster.characteristics.ContainsKey(Cluster3DCharacteristics.HeightAverage) &&
                !cluster.characteristics.ContainsKey(Cluster3DCharacteristics.Height) &&
                !(cluster.characteristics.ContainsKey(Cluster3DCharacteristics.BareHeightAverage) && cluster.characteristics.ContainsKey(Cluster3DCharacteristics.BareHeightMax) && cluster.characteristics.ContainsKey(Cluster3DCharacteristics.BareHeightMin))
                )
                throw new ApplicationException("Cluster have no 3D Height Characteristics");

            if (cluster.characteristics.ContainsKey(Cluster3DCharacteristics.HeightAverage)) // 3D bump Metrology
            {
                cluster.characteristics.Add(SizingCharacteristics.DefectMaxSize, cluster.characteristics[Cluster3DCharacteristics.HeightMax]);
                cluster.characteristics.Add(SizingCharacteristics.TotalDefectSize, cluster.characteristics[Cluster3DCharacteristics.HeightAverage]);

                RectangleF micronRect = RectangleF.Empty;
                foreach (Blob blob in cluster.blobList)
                {
                    RectangleF blobRect = blob.micronQuad.SurroundingRectangle;
                    micronRect = RectangleExtension.Union(micronRect, blobRect);

                    blob.characteristics.Add(SizingCharacteristics.sizeX, (double)blobRect.Width);
                    blob.characteristics.Add(SizingCharacteristics.sizeY, (double)blobRect.Height);
                    blob.characteristics.Add(SizingCharacteristics.DefectArea, (double)blob.characteristics[BlobCharacteristics.MicronArea]);
                    blob.characteristics.Add(SizingCharacteristics.DSize, (double)cluster.characteristics[Cluster3DCharacteristics.HeightAverage]);

                }
            }
            else if (cluster.characteristics.ContainsKey(Cluster3DCharacteristics.BareHeightAverage))
            {
                //Selon le choix utilisateur d'utiliser ou non les hauteurs absolue, les hauteurs sont déjà soustraite de la waleur du background 3d
                // En v8 si HeightAvg >= 0 on retourner le max sinon le Min, ce cas de figure n'est pas stable est précis (vu avec Louise)
                // Donc ici on retourne le heightaverage en totalsize et le maxdefest  la plus grande variation par rapport à l'avgentre le min et le max
                double dHeightAvg = (double)cluster.characteristics[Cluster3DCharacteristics.BareHeightAverage];
                double dHeightMax = (double)cluster.characteristics[Cluster3DCharacteristics.BareHeightMax];
                double dHeightMin = (double)cluster.characteristics[Cluster3DCharacteristics.BareHeightMin];
                if (Math.Abs(dHeightAvg - dHeightMax) < Math.Abs(dHeightAvg - dHeightMin))
                    dHeightMax = dHeightMin;

                cluster.characteristics.Add(SizingCharacteristics.DefectMaxSize, dHeightMax);
                cluster.characteristics.Add(SizingCharacteristics.TotalDefectSize, dHeightAvg);

                RectangleF micronRect = RectangleF.Empty;
                foreach (Blob blob in cluster.blobList)
                {
                    RectangleF blobRect = blob.micronQuad.SurroundingRectangle;
                    micronRect = RectangleExtension.Union(micronRect, blobRect);

                    blob.characteristics.Add(SizingCharacteristics.sizeX, (double)blobRect.Width);
                    blob.characteristics.Add(SizingCharacteristics.sizeY, (double)blobRect.Height);
                    blob.characteristics.Add(SizingCharacteristics.DefectArea, (double)blob.characteristics[BlobCharacteristics.MicronArea]);
                    blob.characteristics.Add(SizingCharacteristics.DSize, (double)dHeightAvg);
                }
            }
            else if (cluster.characteristics.ContainsKey(Cluster3DCharacteristics.Height)) // 3D Bump cluster Metrology (obsolete ?)
            {
                cluster.characteristics.Add(SizingCharacteristics.DefectMaxSize, cluster.characteristics[Cluster3DCharacteristics.Height]);
                cluster.characteristics.Add(SizingCharacteristics.TotalDefectSize, cluster.characteristics[Cluster3DCharacteristics.Height]);

                RectangleF micronRect = RectangleF.Empty;
                foreach (Blob blob in cluster.blobList)
                {
                    RectangleF blobRect = blob.micronQuad.SurroundingRectangle;
                    micronRect = RectangleExtension.Union(micronRect, blobRect);

                    blob.characteristics.Add(SizingCharacteristics.sizeX, (double)blobRect.Width);
                    blob.characteristics.Add(SizingCharacteristics.sizeY, (double)blobRect.Height);
                    blob.characteristics.Add(SizingCharacteristics.DefectArea, (double)blob.characteristics[BlobCharacteristics.MicronArea]);
                    blob.characteristics.Add(SizingCharacteristics.DSize, (double)cluster.characteristics[Cluster3DCharacteristics.Height]);
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        public static void TuneCluster(Cluster cluster, SizingClass sizingClass)
        {
            double defectSize = (double)cluster.characteristics[SizingCharacteristics.DefectMaxSize];
            defectSize = defectSize * sizingClass.TuningMultiplier + sizingClass.TuningOffset;
            cluster.characteristics[SizingCharacteristics.DefectMaxSize] = defectSize;

            double totalDefectSize = (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize];
            totalDefectSize = totalDefectSize * sizingClass.TuningMultiplier + sizingClass.TuningOffset;
            cluster.characteristics[SizingCharacteristics.TotalDefectSize] = totalDefectSize;
        }
        //=================================================================
        // 
        //=================================================================
        public static void TuneBlob(Cluster cluster, SizingClass sizingClass)
        {
            double defectSize;

            foreach (Blob bl in cluster.blobList)
            {
                // X
                defectSize = (double)bl.characteristics[SizingCharacteristics.sizeX];
                bl.characteristics[SizingCharacteristics.sizeX] = defectSize * sizingClass.TuningMultiplier + sizingClass.TuningOffset;
                // Y
                defectSize = (double)bl.characteristics[SizingCharacteristics.sizeY];
                bl.characteristics[SizingCharacteristics.sizeY] = defectSize * sizingClass.TuningMultiplier + sizingClass.TuningOffset;
            }
        }
    }
}
