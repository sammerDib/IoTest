using CommunityToolkit.Mvvm.Messaging;

namespace ADC.BusinessServices
{
    public class BusinessServiceBase
    {
        private IMessenger _messenger = null;
        public IMessenger Messenger { get { return _messenger; } }


        public BusinessServiceBase(IMessenger messenger)
        {
            _messenger = messenger;
        }


    }
}
