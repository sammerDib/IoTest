using System.Threading.Tasks;

namespace UnitySC.Shared.UI.Controls.WizardNavigationControl
{
    public interface INavigable
    {
        Task PrepareToDisplay();

        bool CanLeave(INavigable nextPage, bool forceClose = false);
    }
}
