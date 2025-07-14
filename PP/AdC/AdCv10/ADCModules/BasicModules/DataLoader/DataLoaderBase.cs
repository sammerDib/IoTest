using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using AcquisitionAdcExchange;

using AdcBasicObjects;
using AdcBasicObjects.Rendering;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using LibProcessing;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Data.Enum;

namespace BasicModules.DataLoader
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base des Modules DataLoader.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public abstract class DataLoaderBase : QueueModuleBase<ImageBase>, IDataLoader
    {
        /// <summary> Couche de Données </summary>
        public LayerBase Layer { get; protected set; }

        /// <summary> Nom de la couche de Données </summary>
        public virtual string LayerName { get { return Factory.ModuleName; } }

        /// <summary>
        /// Type de DataLoader (ActorType)
        /// </summary>
        public abstract ActorType DataLoaderActorType { get; }

        /// <summary>
        /// ResultTypes compatibles avec le dataloader
        /// </summary>
        public abstract IEnumerable<ResultType> CompatibleResultTypes { get; }

        protected InputInfoBase InputInfo { get; private set; }

        /// <summary> Tâche qui recharge les images en mode Reprocess. </summary>
        private System.Threading.Tasks.Task _loadTask;

        protected bool IsReprocessing;

        private static ProcessingClass _processingClassMil = new ProcessingClassMil();
        private static ProcessingClass _processingClassMil3D = new ProcessingClassMil3D();

        //=================================================================
        // Méthodes abstraites
        //=================================================================
        /// <summary> Test si le result type match avec ce DataLoader </summary>
        public abstract bool FilterImage(ResultType restyp, int noColumn = -1);
        /// <summary> Convertit une image au format d'Acquisition en une image au format ADC </summary>
        public abstract ObjectBase ConvertAcquisitionData(AcquisitionDataObject acqDataObject);
        /// <summary> Charge une image d'Acquisition depuis le disque, utile pour rejouer une recette </summary>
        public abstract bool LoadAcquisitionImageFromFile(AcquisitionDataObject acqObject);

        //=================================================================
        // Constructeur
        //=================================================================
        public DataLoaderBase(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // Rendering
        //=================================================================
        private DataLoaderRenderingViewModel _renderingVM;
        public override UserControl RenderingUI
        {
            get
            {
                if (_renderingVM == null)
                    _renderingVM = new DataLoaderRenderingViewModel(this);
                ImageRenderingView renderingView = ImageRenderingView.DefaultInstance;
                renderingView.DataContext = _renderingVM;

                return renderingView;
            }
        }

        //=================================================================
        // Init
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            IsReprocessing = false;

            InitOutputQueue();
            InputInfo = GetInputInfo();
            Recipe.recipeExecutedEvent += RecipeExecutedEventHandler;
        }

        //=================================================================
        // Process
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            CheckMemoryLimit();

            //-------------------------------------------------------------
            // Est-ce une image pour ce DataLoader ?
            //-------------------------------------------------------------
            AcquisitionImageObject acqImageObject = obj as AcquisitionImageObject;
            if (acqImageObject == null)
                return;

            AcquisitionMilImage acqImage = (AcquisitionMilImage)acqImageObject.AcqData;
            bool b = FilterImage(acqImage.ResultType);
            if (!b)
                return;

            // On enregistre l'image dans la recette mergée pour être capable de la rejouer
            //.............................................................................
            if (!IsReprocessing)
            {
                if (InputInfo != null && InputInfo.DataLoaderIdList.Count() > 0 && InputInfo.DataLoaderIdList[0] == Id)
                    ((InspectionInputInfoBase)InputInfo).InputDataList.Add(acqImage);
            }

            //-------------------------------------------------------------
            // Convertit en ImageBase
            //-------------------------------------------------------------
            using (ImageBase adcImage = (ImageBase)ConvertAcquisitionData(acqImageObject))
            {
                // Ajoute l'image à la Layer
                //..........................
                ((ImageLayerBase)Layer).AddImage(adcImage);

                // Queue
                //......
                logDebug("queueing image: " + adcImage);
                outputQueue.Enqueue(adcImage);
                // Ne pas tester le résultat car la queue peut être fermée lors d'un abort
            }
        }

        //=================================================================
        // Stop
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            if (IsReprocessing)
                logDebug("stop delayed, waiting for the end of image loading");
            else // Process online
                base.OnStopping(oldState);
        }

        //=================================================================
        // 
        //=================================================================
        private void RecipeExecutedEventHandler(object sender, EventArgs e)
        {
            if (Layer != null)
                ((ImageLayerBase)Layer).FreeImages();
            Recipe.recipeExecutedEvent -= RecipeExecutedEventHandler;
        }

        //=================================================================
        // Mode Offline
        //=================================================================
        void IDataLoader.StartReprocess()
        {
            IsReprocessing = true;

            _loadTask = Scheduler.StartSingleTask(Name + "-loader", () =>
            {
                try
                {
                    Reprocess();
                }
                catch (Exception e)
                {
                    HandleException(e);
                }
                finally
                {
                    base.OnStopping(State);
                }
            });
        }

        //=================================================================
        // Mode Offline
        //=================================================================
        private void Reprocess()
        {
            //-------------------------------------------------------------
            // Est-ce qu'il y a une InputInfo à traiter ?
            // Attention, une InputInfo peut correspondre à deux DataLoader,
            // et tous chargeront les images :-(
            //-------------------------------------------------------------
            InspectionInputInfoBase inputInfo = null;

            foreach (InspectionInputInfoBase ii in Recipe.InputInfoList)
            {
                if (ii.DataLoaderIdList.Contains(Id))
                {
                    inputInfo = ii;
                    break;
                }
            }

            if (inputInfo == null)
                return;

            //-------------------------------------------------------------
            // Chargement des images
            //-------------------------------------------------------------
            foreach (AcquisitionData acqData in inputInfo.InputDataList)
            {
                if (State == eModuleState.Aborting)
                    break;

                if (Recipe.IsRendering && !acqData.IsEnabled)
                    continue;

                using (AcquisitionImageObject acqObject = new AcquisitionImageObject())
                {
                    acqObject.AcqData = acqData;
                    bool needed = LoadAcquisitionImageFromFile(acqObject);
                    if (needed)
                        Recipe.Feed(acqObject);
                }
            }
        }

        ///=================================================================
        ///<summary>
        /// Retourne la InputInfoBase associée à ce DataLoader
        ///</summary>
        ///=================================================================
        public InputInfoBase GetInputInfo()
        {
            var info = Recipe.InputInfoList.FirstOrDefault(x => x.DataLoaderIdList.Contains(Id));
            if (info == null)
                throw new ApplicationException("no input data");
            return info;
        }

        //=================================================================
        // 
        //=================================================================
        public virtual MatrixBase CreateMatrix(MatrixInfo matrixinfo)
        {
            MatrixBase matrixbase;

            if (!double.IsNaN(matrixinfo.SensorRadiusPosition))
            {
                throw new ApplicationException("Matrice EyeEdge must be computed in a derived class");
            }
            else if (!double.IsNaN(matrixinfo.AlignerAngleRadian) && matrixinfo.AlignerAngleRadian != 0)
            {
                // Matrice Affine
                //...............
                AffineMatrix matrix = new AffineMatrix();

                matrix.PixelWidth = (float)matrixinfo.PixelWidth;
                matrix.PixelHeight = (float)matrixinfo.PixelHeight;
                matrix.WaferCenterX = (float)matrixinfo.WaferCenterX;
                matrix.WaferCenterY = (float)matrixinfo.WaferCenterY;
                matrix.WaferPositionCorrected = matrixinfo.WaferPositionCorrected;
                matrix.Angle = matrixinfo.AlignerAngleRadian;

                matrixbase = matrix;
            }
            else
            {
                // Matrice rectangulaire
                //.....................
                RectangularMatrix matrix = new RectangularMatrix();

                matrix.PixelWidth = (float)matrixinfo.PixelWidth;
                matrix.PixelHeight = (float)matrixinfo.PixelHeight;
                matrix.WaferCenterX = (float)matrixinfo.WaferCenterX;
                matrix.WaferCenterY = (float)matrixinfo.WaferCenterY;
                matrix.WaferPositionCorrected = matrixinfo.WaferPositionCorrected;

                matrixbase = matrix;
            }

            bool valid = matrixbase.Validate();
            if (!valid)
                throw new ApplicationException("Invalid matrix");
            return matrixbase;
        }

        //=================================================================
        // 
        //=================================================================
        protected void SetLayerIds(InspectionInputInfoBase inputInfo)
        {
            ImageLayerBase layer = (ImageLayerBase)Layer;

            // obsolete
            //layer.ActorTypeId = (int)inputInfo.ActorTypeId;
            //layer.ChannelID = inputInfo.ChannelID;
            //layer.ImageID = inputInfo.ImageID;
            //layer.ChamberID = inputInfo.ChamberID;

            layer.ResultType = inputInfo.ResultType;
            layer.ChamberKey = inputInfo.ChamberKey;
            layer.ToolKey = inputInfo.ToolKey;
        }

        //=================================================================
        // 
        //=================================================================
        public static void LoadImageFromFile(string filename, MilImage milImage)
        {
            using (ProcessingImage procimg = new ProcessingImage())
            {
                if (filename.ToUpper().EndsWith(".3DA"))
                    _processingClassMil3D.Load(filename, procimg);
                else
                    _processingClassMil.Load(filename, procimg);

                MIL_ID milId = procimg.GetMilImage().DetachMilId();
                milImage.AttachMilId(milId, transferOnwership: true);
            }
        }

        public List<string> GetInputImageList()
        {
            InspectionInputInfoBase inputInfo = (InspectionInputInfoBase)GetInputInfo();
            List<string> listImages = new List<string>();

            if (inputInfo == null)
                return listImages;

            foreach (AcquisitionDieImage acqData in inputInfo.InputDataList)
            {
                if (!string.IsNullOrEmpty(acqData.Filename))
                    listImages.Add(acqData.Filename);
            }

            return listImages;
        }

    }
}
