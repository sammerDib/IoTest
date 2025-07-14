using Agileo.Common.Access;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Rights
{
    public abstract class RightsViewModelBase : Notifier
    {
        public DelegateCommand ResetCommand { get; protected set; }

        public RightViewModel EnabledRight { get; protected set; }
        public RightViewModel VisibilityRight { get; protected set; }

        public abstract GraphicalElement GraphicalElement { get; }

        public AccessLevel HighestLevel => EnabledRight.Level > VisibilityRight.Level ? EnabledRight.Level : VisibilityRight.Level;
    }
}
