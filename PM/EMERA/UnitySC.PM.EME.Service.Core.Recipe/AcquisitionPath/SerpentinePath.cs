namespace UnitySC.PM.EME.Service.Core.Recipe.AcquisitionPath
{
    public class SerpentinePath : AcquisitionPath
    {
        private bool _isMovingRight = true;

        public SerpentinePath(double totalAcquisitionSide, double fovSizeX, double fovSizeY, int nbImagesX,
            int nbImagesY) : base(totalAcquisitionSide, fovSizeX, fovSizeY, nbImagesX, nbImagesY)
        {
        }

        public override AcquisitionPosition NextPosition()
        {
            double centerX = CurrentXID * (FovSizeX - OverlapX) + (FovSizeX / 2) - 1;
            double centerY = CurrentYID * (FovSizeY - OverlapY) + (FovSizeY / 2) - 1;

            var waferPosition = ConvertToWaferCoordinates((centerX, centerY));

            int positionInGridX = CurrentXID;
            int positionInGridY = CurrentYID;

            if (_isMovingRight)
            {
                if (CurrentXID < NbImagesX - 1)
                {
                    CurrentXID++;
                }
                else
                {
                    CurrentYID++;
                    _isMovingRight = false;
                }
            }
            else
            {
                if (CurrentXID > 0)
                {
                    CurrentXID--;
                }
                else
                {
                    CurrentYID++;
                    _isMovingRight = true;
                }
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
