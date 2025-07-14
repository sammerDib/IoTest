using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Controls;

using AdcBasicObjects;
using AdcBasicObjects.Rendering;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

namespace AdvancedModules.StitchingImage
{
    public class StitchingImageModule : QueueModuleBase<ImageBase>
    {
        // Internal data
        //..............
        private List<ImageBase> ImagesList = new List<ImageBase>();

        //=================================================================
        // Paramètres du XML
        //=================================================================

        //=================================================================
        // Constructeur
        //=================================================================
        public StitchingImageModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // Rendering
        //=================================================================
        private ImageRenderingViewModel _renderingVM;
        private ImageRenderingView _renderingView;
        public override UserControl RenderingUI
        {
            get
            {
                if (_renderingVM == null)
                    _renderingVM = new ImageRenderingViewModel(this);
                if (_renderingView == null)
                {
                    _renderingView = new ImageRenderingView();
                    _renderingView.DataContext = _renderingVM;
                }

                return _renderingView;
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();


            // Récupération de la Layer
            //.........................
            //List<ModuleBase> ancestors = FindAncestors(m => m is IDataLoader);
            //if (ancestors.Count == 0)
            //    throw new ApplicationException("Can't find DataLoader");
            //if (ancestors.Count > 1)
            //    throw new ApplicationException("Stitching more than one layer");
            //IDataLoader dataloader = (IDataLoader)ancestors[0];

            //MosaicLayer layer = dataloader.Layer as MosaicLayer;
            //if (layer == null)
            //    throw new ApplicationException("Stitching a layer that is not a mosaic");

           

            //-------------------------------------------------------------
            // Autres inits
            //-------------------------------------------------------------
            InitOutputQueue();
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
                logDebug("process " + obj);
                Interlocked.Increment(ref nbObjectsIn);
                ImageBase image = (ImageBase)obj;

                obj.AddRef();
                ImagesList.Add(image);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("ProcessImageStitching", () => ProcessImageStitching());
            base.OnStopping(oldState);
        }

        //=================================================================
        //
        //=================================================================
        public override void Abort()
        {
            logDebug("abort");
            // On abort pas la queue maintenant car les parents peuvent encore pousser des données.
            // La queue sera fermée quand les parents seront stoppés.
            SetState(eModuleState.Aborting);
        }

        //=================================================================
        //
        //=================================================================
        public override void GetStats(out int nbIn, out int nbOut)
        {
            nbIn = nbObjectsIn;
            nbOut = nbObjectsOut;
        }

        //=================================================================
        // 
        //=================================================================

        //=================================================================
        // 
        //=================================================================
        private void ProcessImageStitching()
        {
            //logDebug("Starting Process Stitching Images , nb_Imagess: " + nbObjectsIn + "  nb_clusters_on_borders: " + nbCandidates + "  stitchs: " + nb_stitches);

            //-------------------------------------------------------------
            // Debug
            //-------------------------------------------------------------

            //-------------------------------------------------------------
            //On met les images dans la queue
            //-------------------------------------------------------------
            foreach (ImageBase image in ImagesList)
            {
                if (State != eModuleState.Aborting)
                {
                    outputQueue.Enqueue(image);
                }
                image.DelRef();
            }

            //-------------------------------------------------------------
            // Puis on arrête la queue
            //-------------------------------------------------------------
            if (State == eModuleState.Aborting)
                outputQueue.AbortQueue();

            logDebug("end");
        }

        //=================================================================
        // 
        //=================================================================
        protected override void ProcessQueueElement(ImageBase image)
        {
            if (State != eModuleState.Aborting)
            {

                ProcessChildren(image);
            }
        }

        


    }
}
