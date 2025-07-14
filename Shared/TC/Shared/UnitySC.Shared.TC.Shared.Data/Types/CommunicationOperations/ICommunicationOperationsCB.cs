namespace UnitySC.Shared.TC.Shared.Data
{
    public interface ICommunicationOperationsCB
    {
        void CommunicationEstablished();

        void CommunicationInterrupted();

        void CommunicationCheck();
    }
}
