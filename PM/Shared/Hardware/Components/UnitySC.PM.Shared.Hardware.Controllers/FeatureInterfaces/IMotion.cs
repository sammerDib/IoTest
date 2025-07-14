using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;

namespace UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces
{
    public interface IMotion
    {
        void Move(params PMAxisMove[] moves);

        void RelativeMove(params PMAxisMove[] moves);
    }
}
