using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;

using AdcBasicObjects;
using AdcBasicObjects.Rendering;

using ADCEngine;

using AdcTools;

using BasicModules.MilClusterizer;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace BasicModules
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public abstract class ClusterizerModuleBase : QueueModuleBase<Cluster>, IClusterizerModule
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum eClusterizationAlgorithm
        {
            [Description("By Blob")]
            Blob,
            [Description("By Surrounding Rectangle")]
            SurroundingRectangle,
        }

        /// <summary>
        /// Index du module de clusteurisation pour générer des numéros de cluster uniques
        /// </summary>
        protected int ClusterizerIndex;
        /// <summary>
        /// Nombre de modules de clusteurisation, pour générer des numéros de cluster uniques
        /// </summary>
        protected int NbClusterizers;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramClusterizationStep;
        public readonly ConditionalIntParameter paramVignetteOversize;
        public readonly ConditionalIntParameter paramDefectCountLimit;

        //=================================================================
        // Constructeur
        //=================================================================
        public ClusterizerModuleBase(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramClusterizationStep = new IntParameter(this, "ClusterizationStep");
            paramClusterizationStep.Value = 10;

            paramVignetteOversize = new ConditionalIntParameter(this, "VignetteOversize", min: 2);

            paramDefectCountLimit = new ConditionalIntParameter(this, "DefectCountLimit");
            paramDefectCountLimit.Value = 20000;
            paramDefectCountLimit.IsUsed = true;

            ModuleProperty = eModuleProperty.Stage;
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string error = base.Validate();
            if (error != null)
                return error;

            if (paramVignetteOversize.IsUsed && paramVignetteOversize.Value < 2)
                return paramVignetteOversize.Name + "must be >=2";

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            InitOutputQueue();

            _hasRenderingChildren = Children.Any(m => m.IsRendering);

            var list = Recipe.ModuleList.Select(kvp => kvp.Value).OfType<IClusterizerModule>().ToList();
            ClusterizerIndex = list.IndexOf(this);
            NbClusterizers = list.Count();
            if (ClusterizerIndex < 0)
                throw new ApplicationException("Can't find Clusterizer Modules");
        }

        /// <summary>
        /// Crée un numéro de cluster unique
        /// </summary>
        /// <param name="index">index du cluster dans l'image</param>
        protected int CreateClusterNumber(ImageBase image, int index)
        {
            int number = ClusterizerIndex + NbClusterizers * (image.ImageIndex + image.Layer.MaxDataIndex * index);
            return number;
        }

        //=================================================================
        // Envoi des clusters dans la queue
        //=================================================================
        protected void QueueClusters(ImageBase image, IEnumerable<Cluster> clusterList)
        {
            foreach (Cluster cluster in clusterList)
            {
                cluster.blobList.Clear();
                if (State != eModuleState.Aborting)
                {
                    if (cluster.OriginalProcessingImage.GetMilImage().MilId == 0)
                    {
                        AllocateVignette(image, cluster);
                        CopyVignette(image, cluster);
                        cluster.micronQuad = image.Layer.Matrix.pixelToMicron(cluster.pixelRect);
                    }
                    outputQueue.Enqueue(cluster);
                }
                cluster.DelRef();
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected void AllocateVignette(ImageBase image, Cluster cluster)
        {
            // Taille de la vignette
            //......................
            int oversize = 2;
            if (paramVignetteOversize.IsUsed)
                oversize = paramVignetteOversize;

            cluster.imageRect = cluster.pixelRect;
            cluster.imageRect.Inflate(oversize, oversize);
            cluster.imageRect.Intersect(image.imageRect);

            // Allocation de l'image Originale
            //................................
            MilImage sourceImage = image.OriginalProcessingImage.GetMilImage();
            MilImage milImage = cluster.OriginalProcessingImage.GetMilImage();
            milImage.Alloc2d(cluster.imageRect.Width, cluster.imageRect.Height, sourceImage.Type, sourceImage.Attribute);

            // Allocation de l'image Courrante
            //................................
            sourceImage = image.ResultProcessingImage.GetMilImage();
            milImage = cluster.ResultProcessingImage.GetMilImage();
            milImage.Alloc2d(cluster.imageRect.Width, cluster.imageRect.Height, sourceImage.Type, sourceImage.Attribute);
        }

        //=================================================================
        // 
        //=================================================================
        protected void CopyVignette(ImageBase image, Cluster cluster)
        {
            // Position du cluster dans l'image
            //.................................
            Rectangle vignetteRect = cluster.imageRect.NegativeOffset(image.imageRect.TopLeft());

            // Copie des vignettes
            //....................
            MilImage clusterImage = cluster.OriginalProcessingImage.GetMilImage();
            MilImage sourceImage = image.OriginalProcessingImage.GetMilImage();
            MilImage.CopyColor2d(sourceImage, clusterImage,    //src, dest
                 MIL.M_ALL_BAND, vignetteRect.X, vignetteRect.Y,   // src
                 MIL.M_ALL_BAND, 0, 0,   // dest
                 vignetteRect.Width, vignetteRect.Height
                 );

            clusterImage = cluster.ResultProcessingImage.GetMilImage();
            sourceImage = image.ResultProcessingImage.GetMilImage();
            MilImage.CopyColor2d(sourceImage, clusterImage,    //src, dest
                 MIL.M_ALL_BAND, vignetteRect.X, vignetteRect.Y,   // src
                 MIL.M_ALL_BAND, 0, 0,   // dest
                 vignetteRect.Width, vignetteRect.Height
                 );

            // Indique que l'image de travail est l'image originale
            //.....................................................
            cluster.CurrentIsOriginal = true;
        }

        //=================================================================
        // 
        //=================================================================
        public virtual bool MergeClusters(Cluster cl1, Cluster cl2)
        {
            string str = "merging clusters " + cl1 + " " + cl2;

            cl1.Index = Math.Min(cl1.Index, cl2.Index);
            cl1.pixelRect = RectangleExtension.Union(cl1.pixelRect, cl2.pixelRect);
            cl1.blobList.AddRange(cl2.blobList);

            cl2.DelRef();

            logDebug(str + " rect=" + cl1.pixelRect);
            return true;
        }

        //=================================================================
        // 
        //=================================================================
        public Rectangle GetClusterRectangle(Cluster cl)
        {
            return cl.pixelRect;
        }

        //=================================================================
        // Regroupement des clusters par rectangle englobant.
        // NB: 
        // Si les clusters ont une classe, on peut les regrouper séparément
        // pour chaque classe. La gestion des classes doit être faite par la
        // classe dérivée.
        // Par défaut, on ne tient pas compte de la classe.
        //=================================================================

        /// <summary> Retourne le nombre de classes de clusters </summary>
        protected virtual int GetNbClasses() { return 1; }
        /// <summary> Retourne le numéro de la classe du cluster </summary>
        protected virtual int GetClusterClass(Cluster cluster) { return 0; }

        public List<Cluster> GroupBlobsBySurroundingRectangles(ImageBase image, List<Cluster> clusterList)
        {
            //-------------------------------------------------------------
            // Création des grilles, une par classe
            //-------------------------------------------------------------
            int nbClasses = GetNbClasses();
            List<MergeGrid<Cluster>> grids = new List<MergeGrid<Cluster>>();
            grids.Resize(nbClasses);
            for (int i = 0; i < nbClasses; i++)
            {
                MergeGrid<Cluster> grid = new MergeGrid<Cluster>(MergeClusters, GetClusterRectangle);
                grid.Init(image.imageRect, step: 300);
                grid.mergeNeighbourDistance = paramClusterizationStep;
                grids[i] = grid;
            }

            //-------------------------------------------------------------
            // Merge des clusters
            //-------------------------------------------------------------
            foreach (Cluster cluster in clusterList)
            {
                int _class = GetClusterClass(cluster);
                MergeGrid<Cluster> grid = grids[_class];
                grid.Merge(cluster);
            }

            //-------------------------------------------------------------
            // Résultat
            //-------------------------------------------------------------
            List<Cluster> list;
            if (nbClasses == 1)
            {
                list = grids[0].GetAll();
            }
            else
            {
                list = new List<Cluster>();
                foreach (MergeGrid<Cluster> grid in grids)
                {
                    List<Cluster> sublist = grid.GetAll();
                    list.AddRange(sublist);
                }
            }

            return list;
        }

        //=================================================================
        // Rendering
        //=================================================================
        private bool _hasRenderingChildren;
        public ConcurrentBag<Cluster> RenderingClusters = new ConcurrentBag<Cluster>();

        protected override void StoreRenderingObject(ObjectBase obj)
        {
            if (Recipe.IsRendering && obj != null)
            {
                if (obj is Cluster)
                {
                    obj.AddRef();
                    RenderingClusters.Add((Cluster)obj);
                }
                else
                {
                    base.StoreRenderingObject(obj);
                }
            }
        }

        public override void ClearRenderingObjects()
        {
            foreach (Cluster cl in RenderingClusters)
                cl.DelRef();
            //TODO FDE nettoyer
            RenderingClusters = new ConcurrentBag<Cluster>();

            base.ClearRenderingObjects();
        }

        private ClusterizerRenderingViewModel _renderingVM;
        public override UserControl RenderingUI
        {
            get
            {
                if (_renderingVM == null)
                    _renderingVM = new ClusterizerRenderingViewModel(this);
                ImageRenderingView renderingView = ImageRenderingView.DefaultInstance;
                renderingView.DataContext = _renderingVM;

                return renderingView;
            }
        }

        //=================================================================
        // for debug 
        //=================================================================
        protected void DrawClusterList(ImageBase image, List<Cluster> clusterList)
        {
            using (MilGraphicsContext gc = new MilGraphicsContext())
            {
                gc.Image = image.CurrentProcessingImage.GetMilImage();
                gc.Alloc(Mil.Instance.HostSystem);
                gc.Color = MIL.M_COLOR_WHITE;

                gc.Image.Clear();

                foreach (Cluster cluster in clusterList)
                {
                    Rectangle r = cluster.pixelRect.NegativeOffset(image.imageRect.TopLeft());
                    gc.RectFill(r);
                }
            }
        }

    }
}
