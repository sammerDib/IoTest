using UnitySC.Shared.Data;

namespace UnitySC.PM.Shared.UI.Proxy
{
    public delegate void UserChangedEventHandler(UnifiedUser user);

    public interface IUserSupervisor
    {
        UnifiedUser CurrentUser { get; }

        UnifiedUser Connect(string user, string password);
        
        void Disconnect();

        event UserChangedEventHandler UserChanged;

    }
}
