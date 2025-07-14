namespace UnitySC.PM.EME.Service.Core.Recipe.AcquisitionPath
{
    public abstract class AcquisitionPath
    {
        private double TotalAcquisitionSide { get; }
        protected double FovSizeX { get; }
        protected double FovSizeY { get; }
        public int NbImagesX { get; }
        public int NbImagesY { get; }
        protected int CurrentXID { get; set; }
        protected int CurrentYID { get; set; }
        public double OverlapX { get; set; }
        public double OverlapY { get; set; }
        protected int CountTotalImage { get; set; }

        protected AcquisitionPath(double totalAcquisitionSide, double fovSizeX, double fovSizeY, int nbImagesX, int nbImagesY)
        {
            TotalAcquisitionSide = totalAcquisitionSide;
            FovSizeX = fovSizeX;
            FovSizeY = fovSizeY;
            NbImagesX = nbImagesX;
            NbImagesY = nbImagesY;
            OverlapX = (NbImagesX * FovSizeX - totalAcquisitionSide) / (NbImagesX);
            OverlapY = (NbImagesY * FovSizeY - totalAcquisitionSide) / (NbImagesY);
        }

        public abstract AcquisitionPosition NextPosition();

        protected (double, double) ConvertToWaferCoordinates((double, double) coordinates)
        {
            double x = coordinates.Item1 - TotalAcquisitionSide / 2;
            double y = -coordinates.Item2 + TotalAcquisitionSide / 2;

            var convertedCoordinates = (x, y);
            return convertedCoordinates;
        }

        public bool IsLastPosition() => CountTotalImage == NbImagesX * NbImagesY;
    }

    public class AcquisitionPosition
    {
        public (double, double) Position { get; set; }
        public (int, int) PositionInGrid { get; set; }
    }
}
