using System.Windows;

using UnitySC.Shared.ResultUI.Metro.Utilities;

namespace UnitySC.Shared.ResultUI.Metro.AttachedProperties
{
    /// <summary>
    /// Attached property allowing to store the information concerning the fact that the column was generated automatically or not.
    /// This allows the <see cref="ColumnGenerator"/> to delete the generated columns while keeping the columns defined in the xaml during column generation.
    /// </summary>
    public static class GridViewColumnAttachedProperties
    {
        public static readonly DependencyProperty IsGeneratedProperty = DependencyProperty.RegisterAttached(
            "IsGenerated", typeof(bool), typeof(GridViewColumnAttachedProperties), new PropertyMetadata(default(bool)));

        public static void SetIsGenerated(DependencyObject element, bool value)
        {
            element.SetValue(IsGeneratedProperty, value);
        }

        public static bool GetIsGenerated(DependencyObject element)
        {
            return (bool)element.GetValue(IsGeneratedProperty);
        }
    }
}
