namespace UnitySC.PM.Shared.Hardware.Service.Interface.Common
{
    public interface IHarwareSimulator
    {
        void OpenCloseDoorSlit(bool open);

        void MoveInLoadingUnloadingPosition();
    }
}