namespace UnitySC.DataAccess.Service.Interface
{
    public interface IRegisterResultServer
    {
        //
        // event CallBack for ResultService
        //
        event ResultAddedEventHandler ResultAdded;

        event ResultAcqAddedEventHandler ResultAcquistionAdded;

        event StateUpdateEventHandler StateChanged;
    }
}
