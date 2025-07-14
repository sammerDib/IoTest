using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

using MergeContext.Context;

using UnitySC.Shared.Tools;

namespace BasicModules.BorderRemoval
{
    public class PadFingerRemoveModule : ImageModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly BoolParameter paramRemoveNotch;
        public readonly DoubleParameter paramNotchMargin;
        public readonly BoolParameter paramRemovePads;
        public readonly DoubleParameter paramPadMargin;
        public readonly BoolParameter paramRemoveFingers;
        public readonly DoubleParameter paramFingerMargin;
        public readonly BoolParameter paramDebug;

        //=================================================================
        // Autres membres
        //=================================================================
        private enum Kind { Notch, Pad, Finger, EndOfAcquisition };
        private struct Exclusion
        {
            public Rectangle rect;
            public Kind kind;
        }
        private Dictionary<LayerBase, List<Exclusion>> layerExclusions;
        private const double π = Math.PI;

        //=================================================================
        // Constructeur
        //=================================================================
        public PadFingerRemoveModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramRemoveNotch = new BoolParameter(this, "RemoveNotch");
            paramRemoveNotch.ValueChanged += (b) => { paramNotchMargin.IsEnabled = b; };
            paramNotchMargin = new DoubleParameter(this, "MarginForNotch");
            paramRemovePads = new BoolParameter(this, "RemovePads");
            paramRemovePads.ValueChanged += (b) => { paramPadMargin.IsEnabled = b; };
            paramPadMargin = new DoubleParameter(this, "MarginForPads");
            paramRemoveFingers = new BoolParameter(this, "RemoveFinger");
            paramRemoveFingers.ValueChanged += (b) => { paramFingerMargin.IsEnabled = b; };
            paramFingerMargin = new DoubleParameter(this, "MarginForFingers");
            paramDebug = new BoolParameter(this, "Debug");

            paramNotchMargin.Value = 3000;
            paramPadMargin.Value = 1000;
            paramFingerMargin.Value = 1000;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            layerExclusions = new Dictionary<LayerBase, List<Exclusion>>();
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("EdgeRemove " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ImageBase image = (ImageBase)obj;

            try
            {
                List<Exclusion> exclusions;
                bool found = layerExclusions.TryGetValue(image.Layer, out exclusions);
                if (!found)
                    ComputeExclusionList((MosaicLayer)image.Layer, out exclusions);
                foreach (Exclusion exclu in exclusions)
                    ApplyExclusion(image, exclu);
            }
            catch (Exception ex)
            {
                logError("Get chamber settings error " + ex);
                throw;
            }

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        private void ApplyExclusion(ImageBase image, Exclusion exclusion)
        {
            MilImage milImage = image.CurrentProcessingImage.GetMilImage();

            Rectangle exclusionRect = exclusion.rect;
            exclusionRect.Intersect(image.imageRect);
            if (!exclusionRect.IsEmpty)
            {
                using (MilImage child = new MilImage())
                {
                    var childRect = exclusionRect.NegativeOffset(image.imageRect.TopLeft());
                    child.Child2d(milImage, childRect.ToWinRect());
                    child.Clear();
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void ComputeExclusionList(MosaicLayer layer, out List<Exclusion> exclusions)
        {
            ChamberSettings chamberSettings = layer.GetContextMachine<ChamberSettings>(ConfigurationType.Chamber.ToString());

            exclusions = new List<Exclusion>();
            layerExclusions[layer] = exclusions;

            ComputeNotchExclusion(layer, ref exclusions);
            ComputePadAndFingerExclusions(layer, chamberSettings, ref exclusions);
            ComputeEndOfImageExclusion(layer, ref exclusions);
        }

        //=================================================================
        // 
        //=================================================================
        private void ComputePadAndFingerExclusions(MosaicLayer layer, ChamberSettings chamberSettings, ref List<Exclusion> exclusions)
        {
            NotchWafer wafer = (NotchWafer)Wafer;
            EyeEdgeMatrix matrix = (EyeEdgeMatrix)layer.Matrix;

            foreach (PadFinger pad in chamberSettings.PadFingers)
            {
                double alpha;
                Kind kind;

                // Filtrage des pads/fingers
                //..........................
                switch (pad.ExclusionType)
                {
                    case ExclusionType.Pad:
                        if (!paramRemovePads)
                            continue;   // zone d'exclusion
                        alpha = 2 * Math.Atan((pad.Size + paramPadMargin) / wafer.Diameter);
                        kind = Kind.Pad;
                        break;
                    case ExclusionType.Finger:
                        if (!paramRemoveFingers)
                            continue;
                        alpha = 2 * Math.Atan((pad.Size + paramFingerMargin) / wafer.Diameter);
                        kind = Kind.Finger;
                        break;
                    default:
                        throw new ApplicationException("unknown exclusion type: " + pad.ExclusionType);
                }

                // Calcul de la zone d'exclusion
                //..............................
                double startAngle = pad.Angle - alpha;
                double endAngle = pad.Angle + alpha;

                AddExclusion(layer, startAngle, endAngle, kind, ref exclusions);

                // Gestion du début/fin de l'image
                //................................
                if (startAngle < 0)
                    AddExclusion(layer, startAngle + 2 * π, endAngle + 2 * π, kind, ref exclusions);
                if (endAngle > 2 * π)
                    AddExclusion(layer, startAngle - 2 * π, endAngle - 2 * π, kind, ref exclusions);
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void ComputeEndOfImageExclusion(MosaicLayer layer, ref List<Exclusion> exclusions)
        {
            EyeEdgeMatrix matrix = (EyeEdgeMatrix)layer.Matrix;

            int x0 = 0;
            int y0 = matrix.EndOfAcquisitionY - 100;
            int x1 = layer.FullImageSize.Width;
            int y1 = y0 + 200;  // ça devrait suffire ;-)

            Exclusion exclu = new Exclusion();
            exclu.rect = Rectangle.FromLTRB(x0, y0, x1, y1);
            exclu.kind = Kind.EndOfAcquisition;
            exclusions.Add(exclu);

            // Vérification
            //.............
            int c = exclusions.Last().rect.Top;  // circonference du wafer en pixels
            if (layer.FullImageSize.Height < c)
                logWarning(String.Format("Acquisition image (h: {0} pix) is shorter than one wafer revolution ( {1} pix), pixelsize: {2} µm/pix", layer.FullImageSize.Height, c, ((EyeEdgeMatrix)layer.Matrix).PixelSize.Height));
        }

        //=================================================================
        // 
        //=================================================================
        private void AddExclusion(MosaicLayer layer, double startAngle, double endAngle, Kind kind, ref List<Exclusion> exclusions)
        {
            EyeEdgeMatrix matrix = (EyeEdgeMatrix)layer.Matrix;

            int x0 = 0;
            int y0 = matrix.padAngleToPixel(startAngle);
            int x1 = layer.FullImageSize.Width;
            int y1 = matrix.padAngleToPixel(endAngle);

            Exclusion exclu = new Exclusion();
            exclu.rect = Rectangle.FromLTRB(x0, y0, x1, y1);
            exclu.kind = kind;
            exclusions.Add(exclu);
        }

        //=================================================================
        // 
        //=================================================================
        private void ComputeNotchExclusion(MosaicLayer layer, ref List<Exclusion> exclusions)
        {
            if (!paramRemoveNotch)
                return;

            NotchWafer wafer = (NotchWafer)Wafer;
            EyeEdgeMatrix matrix = (EyeEdgeMatrix)layer.Matrix;

            // Exclusion du Notch
            //...................
            double dsize = (wafer.NotchSize / 2 + paramNotchMargin) / matrix.PixelSize.Height;  // en pixels
            int size = (int)Math.Ceiling(dsize);

            int x0 = 0;
            int y0 = matrix.NotchY - size;
            int x1 = layer.FullImageSize.Width;
            int y1 = matrix.NotchY + size;

            Exclusion exclu = new Exclusion();
            exclu.rect = Rectangle.FromLTRB(x0, y0, x1, y1);
            exclu.kind = Kind.Notch;
            exclusions.Add(exclu);

            // Gestion des cas où le notch est sur le début ou la fin de l'image
            //..................................................................
            int c = (int)(2 * π * matrix.SensorRadiusPosition / matrix.PixelSize.Height + 0.5);  // circonference en pixels
            if (y0 < 0)
            {
                exclu = new Exclusion();
                exclu.rect = Rectangle.FromLTRB(x0, y0 + c, x1, y1 + c);
                exclu.kind = Kind.Notch;
                exclusions.Add(exclu);
            }
            if (y1 > c)
            {
                exclu = new Exclusion();
                exclu.rect = Rectangle.FromLTRB(x0, y0 - c, x1, y1 - c);
                exclu.kind = Kind.Notch;
                exclusions.Add(exclu);
            }
        }

    }
}
