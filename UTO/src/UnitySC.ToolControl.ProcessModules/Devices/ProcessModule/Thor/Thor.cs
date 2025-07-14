using UnitySC.Shared.Data.Enum;

namespace UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.Thor
{
    public partial class Thor
    {
        #region Setup

        private void InstanceInitialization()
        {
            ActorType = ActorType.Thor;
        }

        #endregion

        public override string GetMessagesConfigurationPath(string path)
        {
            return System.IO.Path.Combine(
                path,
                $".\\Devices\\{nameof(ProcessModule)}\\{nameof(Thor)}\\Resources\\MessagesConfiguration.xml");
        }
    }
}
