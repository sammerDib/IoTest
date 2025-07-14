using UnitySC.PM.Shared.UI.Main;

namespace UnitySC.PM.AGS.Modules.Example
{
    public class ExampleVM : IMenuContentViewModel
    {
        public bool IsEnabled => true;

        public bool CanClose()
        {
            return true;
        }

        public void Refresh()
        {
            // Nothing
        }
    }
}
