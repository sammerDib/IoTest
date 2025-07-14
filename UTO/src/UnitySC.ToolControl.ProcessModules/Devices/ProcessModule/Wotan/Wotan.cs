using UnitySC.Shared.Data.Enum;

namespace UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.Wotan
{
    public partial class Wotan
    {
        #region Setup

        private void InstanceInitialization()
        {
            ActorType = ActorType.Wotan;
        }

        #endregion

        public override string GetMessagesConfigurationPath(string path)
        {
            return System.IO.Path.Combine(
                path,
                $".\\Devices\\{nameof(ProcessModule)}\\{nameof(Wotan)}\\Resources\\MessagesConfiguration.xml");
        }
    }
}
