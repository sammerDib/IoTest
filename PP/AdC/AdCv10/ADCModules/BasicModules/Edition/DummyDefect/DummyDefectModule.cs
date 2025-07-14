using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.Sizing;

using UnitySC.Shared.LibMIL;

using LibProcessing;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace BasicModules.Edition.DummyDefect
{
    public class DummyDefectModule : ImageModuleBase 
    {
        private const int AngleTextStep = 10; // Pour l'affichage des degrés dans l'image edge
        public const string DefectClassName = "DummyDefect";
        public readonly IntParameter ScaleXFactor;
        public readonly IntParameter ScaleYFactor;
        public Cluster ClusterResult;
        private float _radius;
        private object _lockConcatImage = new object();

        public readonly BoolParameter SaveImageResult;
        private ProcessingClassMil _processClass = new ProcessingClassMil();
        private ProcessingImage _concatImage = null;

        private Size _mosaicImageSize;
        private ImageLayerBase _layer;
        private bool _isEdge;
        internal bool ChildrenMustBeProcess = true;

        public DummyDefectModule(IModuleFactory factory, int id, Recipe recipe) : base(factory, id, recipe)
        {
            ScaleXFactor = new IntParameter(this, "ScaleXFactor", min: 1);
            ScaleYFactor = new IntParameter(this, "ScaleYFactor", min: 1);
            SaveImageResult = new BoolParameter(this, "SaveImageResult");
        }

        protected override void OnInit()
        {
            Recipe.recipeExecutedEvent += RecipeExecutedEventHandler;
            _radius = Wafer.SurroundingRectangle.Height / 2;
            base.OnInit();
        }

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("DummyDefect " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            IImage image = (IImage)obj;
            _layer = (ImageLayerBase)image.Layer;

            lock (_lockConcatImage)
            {
                if (_concatImage == null)
                    _isEdge = _layer.ResultType.GetActorType() == ActorType.Edge;

                if (image is MosaicImage)
                {
                    MosaicImage mosaicImage = (MosaicImage)image;
                    if (_concatImage == null)
                    {
                        _concatImage = new ProcessingImage();
                        MilImage milImage = mosaicImage.CurrentProcessingImage.GetMilImage();
                        MosaicLayer layer = ((MosaicLayer)mosaicImage.Layer);
                        int nbColumns = layer.NbColumns;
                        int nbLines = layer.NbLines;
                        _mosaicImageSize = layer.MosaicImageSize;
                        using (MilImage concatMilImage = new MilImage())
                        {
                            if (_isEdge)
                                concatMilImage.Alloc2d(nbColumns * _mosaicImageSize.Width + 100 * ScaleXFactor, nbLines * _mosaicImageSize.Height, milImage.Type, milImage.Attribute);
                            else
                                concatMilImage.Alloc2d(nbColumns * _mosaicImageSize.Width, nbLines * _mosaicImageSize.Height, milImage.Type, milImage.Attribute);

                            _concatImage.SetMilImage(concatMilImage);
                        }
                    }

                    _processClass.SetSubImage(mosaicImage.CurrentProcessingImage, _concatImage, mosaicImage.Column * _mosaicImageSize.Width, mosaicImage.Line * _mosaicImageSize.Height);

                }
                else if (image is FullImage)
                {
                    _concatImage = (ProcessingImage)image.CurrentProcessingImage.DeepClone();
                }
            }

            if (ChildrenMustBeProcess)
                ProcessChildren(obj);
        }

        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("parent stopped, starting processing task");
            Scheduler.StartSingleTask("ProcessDummyDefect", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                        ProcessDummyDefect();
                    else if (oldState == eModuleState.Aborting)
                        PurgeDummyDefect();
                    else
                        throw new ApplicationException("invalid state");
                }
                catch (Exception ex)
                {
                    logDebug("Dummy defect Error : " + ex.Message);

                }
                finally
                {
                    base.OnStopping(oldState);
                }
            });
        }

        private void RecipeExecutedEventHandler(object sender, EventArgs e)
        {
            if (ClusterResult != null)
                ClusterResult.Dispose();
            Recipe.recipeExecutedEvent -= RecipeExecutedEventHandler;
        }

        private void PurgeDummyDefect()
        {
            logDebug("Dummy defect generation aborted");
            if (_concatImage != null)
            {
                _concatImage.Dispose();
                _concatImage = null;
            }
        }

        private void ProcessDummyDefect()
        {
            log("Creating dummy defect: ");
            if (_concatImage != null)
            {
                using (ProcessingImage resizeImage = _processClass.Resize(_concatImage, ScaleXFactor, ScaleYFactor))
                {
                    String baseName = Recipe.Wafer.Basename;
                    if (_isEdge)
                    {
                        //AddDegreeText(resizeImage);
                        using (ProcessingImage rotationImage = _processClass.Rotate(resizeImage))
                        {
                            CreateClusterResult(rotationImage);
                        }
                    }
                    else
                    {
                        CreateClusterResult(resizeImage);
                    }

                    _concatImage.Dispose();
                    _concatImage = null;
                }
            }

            Interlocked.Increment(ref nbObjectsOut);
        }


        private void CreateClusterResult(ProcessingImage imageResult)
        {
            ClusterResult = new Cluster(-1, _layer);
            ClusterResult.defectClassList = new List<string>() { DefectClassName }; ;

            int notchSize = 0;
            if (Wafer is NotchWafer)
                notchSize = Convert.ToInt32(((NotchWafer)Wafer).NotchSize);

            // dummy defect added on notch position
            RectangleF micronRec = new RectangleF(0, -_radius, notchSize, notchSize);
            ClusterResult.micronQuad = new QuadF(micronRec);
            ClusterResult.pixelRect = _layer.Matrix.micronToPixel(micronRec);
            ClusterResult.imageRect = ClusterResult.pixelRect;
            ClusterResult.characteristics.Add(SizingCharacteristics.DefectMaxSize, (double)Math.Max(micronRec.Width, micronRec.Height));
            ClusterResult.characteristics.Add(SizingCharacteristics.TotalDefectSize, (double)Math.Max(micronRec.Width, micronRec.Height));
            ClusterResult.characteristics.Add(SizingCharacteristics.SizingType, eSizingType.ByLength);
            ClusterResult.characteristics.Add(ClusterCharacteristics.AbsolutePosition, ClusterResult.micronQuad.SurroundingRectangle);

            ClusterResult.OriginalProcessingImage.SetMilImage(imageResult.GetMilImage());
        }

        private void AddDegreeText(ProcessingImage imageResult)
        {
            using (MilGraphicsContext gc = new MilGraphicsContext())
            {
                gc.Image = imageResult.GetMilImage();
                gc.Alloc(Mil.Instance.HostSystem);


                for (int i = 0; i < 360; i += AngleTextStep)
                {
                    double radAngle = (i - 90) * (Math.PI / 180);
                    PointF pointMicron = new PointF(
                        _radius * (float)Math.Cos(radAngle),
                        _radius * (float)Math.Sin(radAngle));

                    Point pointPixel = _layer.Matrix.micronToPixel(pointMicron);
                    if (i != 0)
                        gc.AddText(pointPixel.X / ScaleXFactor, pointPixel.Y / ScaleYFactor, "--- " + i + " Deg. ---");
                    else
                        gc.AddText(pointPixel.X / ScaleXFactor, pointPixel.Y / ScaleYFactor, "---  NOTCH  ---");
                }
            }

        }

    }
}
