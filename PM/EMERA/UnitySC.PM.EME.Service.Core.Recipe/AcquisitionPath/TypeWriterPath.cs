namespace UnitySC.PM.EME.Service.Core.Recipe.AcquisitionPath
{
    public class TypewriterPath : AcquisitionPath
    {
        public TypewriterPath(double totalAcquisitionSide, double fovSizeX, double fovSizeY, int nbImagesX, int nbImagesY) : base(totalAcquisitionSide, fovSizeX, fovSizeY, nbImagesX, nbImagesY)
        {
        }

        public override AcquisitionPosition NextPosition()
        {
            double centerX = CurrentXID * (FovSizeX - OverlapX) + (FovSizeX / 2) - 1;
            double centerY = CurrentYID * (FovSizeY - OverlapY) + (FovSizeY / 2) - 1;

            var waferPosition = ConvertToWaferCoordinates((centerX, centerY));

            int positionInGridX = CurrentXID;
            int positionInGridY = CurrentYID;

            if (CurrentXID < NbImagesX - 1)
            {
                CurrentXID++;
            }
            else
            {
                CurrentXID = 0;
                CurrentYID++;
            }

            CountTotalImage++;
            return new AcquisitionPosition
            {
                Position = waferPosition,
                PositionInGrid = (positionInGridX, positionInGridY),
            };
        }
    }
}
