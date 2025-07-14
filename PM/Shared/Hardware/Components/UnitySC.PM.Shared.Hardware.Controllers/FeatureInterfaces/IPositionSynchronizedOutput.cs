namespace UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces
{
    public interface IPositionSynchronizedOutput
    {
        void DisablePSO();
        
        void SetPSOInFixedWindowMode(double beginPosition, double endPosition, double pixelDegSize);
        
        double GetNearestPSOPixelSize(double PixelSizeTarget, double WaferDiameter);
    }
}
