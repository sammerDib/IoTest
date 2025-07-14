using CV = Emgu.CV;

namespace UnitySC.PM.WOTAN.Common
{
    public class LaneImage
    {        
        private float _pixelSizeX, _pixelSizeY; // µm/px
        private GrabDirection _grabDirection;
        private Side _side;
        private int _laneNumber;

        private CV.Mat _image;

        public float PixelSizeX { get => _pixelSizeX; set => _pixelSizeX = value; }
        public float PixelSizeY { get => _pixelSizeY; set => _pixelSizeY = value; }
        public GrabDirection GrabDirection { get => _grabDirection; set => _grabDirection = value; }
        public Side Side { get => _side; set => _side = value; }
        public int LaneNumber { get => _laneNumber; set => _laneNumber = value; }
        public CV.Mat Image { get => _image; set => _image = value; }

        public float PixelSize { get => (_pixelSizeX + _pixelSizeY) / 2; }

        public double ToPixels(double um)
        {
            return um / PixelSize;
        }

        public double ToUm(double px)
        {
            return px * PixelSize;
        }
    }
}
