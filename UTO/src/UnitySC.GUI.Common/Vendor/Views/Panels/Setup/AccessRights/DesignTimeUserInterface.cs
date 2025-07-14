using Agileo.Common.Access;
using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.AccessRights;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Configuration;
using Agileo.GUI.Services.Icons;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights
{
    public class DesignTimeUserInterface : UserInterface
    {
        private static readonly AccessManager AccessManager = new();

        public DesignTimeUserInterface() : base(new AccessManager(), LocalizationManager.Instance, Agileo.Common.Logging.Logger.GetLogger("DesignTimeLogger"))
        {
        }

        public override AgileoGuiConfiguration GetCurrentConfiguration()
        {
            return null;
        }

        protected override void CreatePanels()
        {
            Navigation.RootMenu.Items.Add(CreatePanel("Main", AccessLevel.Visibility));
            Navigation.RootMenu.Items.Add(new Menu("System")
            {
                AccessRights = CreateAccessRights(AccessLevel.Visibility),
                Items =
                {
                    CreatePanel("Process", AccessLevel.Visibility),
                }
            });
            Navigation.RootMenu.Items.Add(new Menu("Setup")
            {
                AccessRights = CreateAccessRights(AccessLevel.Level5),
                Items =
                {
                    CreatePanel("AccessRights", AccessLevel.Level5),
                    CreatePanel("Diagnostics", AccessLevel.Level5)
                }
            });
            Navigation.RootMenu.Items.Add(CreatePanel("Traces", AccessLevel.Level2));
            Navigation.RootMenu.Items.Add(new Menu("Help")
            {
                AccessRights = CreateAccessRights(AccessLevel.Level1),
                Items =
                {
                    CreatePanel("Help", AccessLevel.Level3)
                }
            });
        }

        protected override Agileo.GUI.Components.TitlePanel CreateTitlePanel()
        {
            return null;
        }

        private BusinessPanel CreatePanel(string name, AccessLevel level)
        {
            return new DesignTimeBusinessPanel(name)
            {
                AccessRights = CreateAccessRights(level)
            };
        }

        public static GraphicalAccessRights CreateAccessRights(AccessLevel level)
        {
            return new GraphicalAccessRights(
                new RightData("IsVisible", AccessLevel.Visibility),
                new RightData("IsEnabled", level),
                AccessManager);
        }
    }

    public class DesignTimeBusinessPanel : BusinessPanel
    {
        public DesignTimeBusinessPanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            Commands.Add(new BusinessPanelCommand("Command A", new DelegateCommand(() => {}))
            {
                AccessRights = DesignTimeUserInterface.CreateAccessRights(AccessLevel.Level3)
            });
        }
    }
}
