namespace UnitySC.Shared.UI.Controls.WizardNavigationControl
{
    public interface IWizardNavigationItem
    {
        string Name { get; set; }
        bool IsEnabled { get; set; }
        bool IsMeasure { get; set; }
        bool IsValidated { get; set; }
    }
}
