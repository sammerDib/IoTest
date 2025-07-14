using System.Collections.Generic;

using AdcBasicObjects;

using AdcTools;

namespace DiameterMeasurementDieModule
{
    public class Cluster2DDieDM : Cluster
    {

        public uint _NbMeasuresPass = 0;
        public uint _NbMeasuresMissing = 0;
        public uint _NbMeasuresBadDiameter = 0;
        public uint _NbMeasuresBadOffset = 0;

        public double _SumMeasuresDiameter = 0.0;
        public double _SumMeasuresOffset = 0.0;

        public double _MinMeasureDiameter = 0.0;
        public double _MaxMeasureDiameter = 0.0;
        public double _MeanMeasureDiameter = 0.0;
        public double _StdDevMeasureDiameter = 0.0;

        public double _MinMeasureOffset = 0.0;
        public double _MaxMeasureOffset = 0.0;
        public double _MeanMeasureOffset = 0.0;
        public double _StdDevMeasureOffset = 0.0;

        public List<double> _StatsDiameter = null;
        public List<double> _StatsOffset = null;

        override public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                Name = "CLU2Ddie-" + _index;
            }
        }

        public Cluster2DDieDM(int index, ImageBase image)
            : base(index, image.Layer)
        {

            OriginalProcessingImage.SetMilImage(image.CurrentProcessingImage.GetMilImage());

            // Indique que l'image de travail est l'image originale
            //.....................................................
            CurrentIsOriginal = true;

            pixelRect = image.imageRect;
            micronQuad = image.Layer.Matrix.pixelToMicron(pixelRect);
            imageRect = pixelRect;

            // Info spécifiques
            //.................
            if (image is DieImage)
            {
                DieImage dieimg = (DieImage)image;
                DieIndexX = dieimg.DieIndexX;
                DieIndexY = dieimg.DieIndexY;
                DieOffsetImage = dieimg.imageRect.TopLeft();

            }

            if (image is MosaicImage)
            {
                MosaicImage mosaicimg = (MosaicImage)image;
                Line = mosaicimg.Line;
                Column = mosaicimg.Column;
            }
        }

        public double ComputeDieMeasuresVariance(double dva)
        {
            //to do rti 
            return 0.0;
        }
    }
}
