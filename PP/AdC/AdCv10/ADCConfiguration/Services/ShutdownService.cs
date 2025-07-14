using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.LibMIL;

namespace ADCConfiguration.Services
{
    public class ShutdownService
    {
        public IMessenger Messenger { get; private set; }

        public ShutdownService(IMessenger messenger)
        {
            Messenger = messenger;
        }

        private bool shutdowning = false;

        public void ShutdownApp()
        {
            if (shutdowning) return;

            //controle si tt a était sauvegarder.
            // si ce n'est pas le cas, peut etre demander une confirm de sortie.
            if (true)
            {
                Mil.Instance.Free();

                // arret autorisé
                shutdowning = true;
                var currentUser = Services.Instance.AuthentificationService.CurrentUser;
#warning ** USP ** Disconnect to do
                //   if (currentUser != null)
                //       ClassLocator.Default.GetInstance<ILogService>().Disconnect(Services.Instance.MapperService.Mapper.Map<Database.Service.Dto.User>(currentUser));
                App.Current.Shutdown();

            }
            else
            {
                // arret refusé.
            }

        }
    }
}
