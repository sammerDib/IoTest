using System.Linq;
using System.Windows;

using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.Equipment.Abstractions.Devices.ProcessModule;

namespace UnitySC.GUI.Common.Views.Panels.Sample
{
    /// <summary>
    /// Logique d'interaction pour SamplePanelView.xaml
    /// </summary>
    public partial class SamplePanelView
    {
        public SamplePanelView()
        {
            InitializeComponent();
        }

        private void CodeBehindButton_OnClick(object sender, RoutedEventArgs e)
            => App.Instance.EquipmentManager.Equipment.AllDevices<ProcessModule>().FirstOrDefault()?.StartCommunication();

        private void PopupButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = new Popup(
                nameof(SamplePanelResources.BP_SAMPLE_POPUP_TITLE),
                nameof(SamplePanelResources.BP_SAMPLE_POPUP_TITLE)) { SeverityLevel = MessageLevel.Info };
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));
            popup.Commands.Add(
                new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_OK), new DelegateCommand(ExecuteMethod)));

            App.Instance.UserInterface.Popups.Show(popup);
        }

        private void ExecuteMethod()
        {
            // custom code here
        }

        private void UserMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var message = new UserMessage(MessageLevel.Warning, nameof(SamplePanelResources.CUSTOM_USER_MESSAGE));
            (DataContext as SamplePanel)?.Messages.Show(message);
        }

        private void AppMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var message = new UserMessage(MessageLevel.Error, nameof(SamplePanelResources.CUSTOM_ERROR_USER_MESSAGE));
            App.Instance.UserInterface.Messages.Show(message);
        }
    }
}
