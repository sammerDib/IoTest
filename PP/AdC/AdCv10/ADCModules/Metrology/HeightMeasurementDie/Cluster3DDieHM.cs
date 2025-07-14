using AdcBasicObjects;

using AdcTools;

using FormatAHM;

namespace HeightMeasurementDieModule
{
    public class Cluster3DDieHM : Cluster
    {
        public DieHMResults _dieHMresult = null;

        public uint _NbMeasuresPass = 0;
        public uint _NbMeasuresMissing = 0;
        public uint _NbMeasuresBadHeight = 0;
        public double _SumMeasuresHeight = 0.0;

        override public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                Name = "CLU3Ddie-" + _index;
            }
        }

        public Cluster3DDieHM(int index, ImageBase image)
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

                _dieHMresult = new DieHMResults(DieIndexX, DieIndexY, pixelRect.Left, pixelRect.Top, pixelRect.Width, pixelRect.Height);
            }

            if (image is MosaicImage)
            {
                MosaicImage mosaicimg = (MosaicImage)image;
                Line = mosaicimg.Line;
                Column = mosaicimg.Column;
            }
        }
    }
}
