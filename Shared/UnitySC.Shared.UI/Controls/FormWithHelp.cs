using System;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.Shared.UI.Controls
{
    /// <summary>
    /// Form with help management linked to style
    /// </summary>
    [TemplatePart(Name = "Part_Content", Type = typeof(ContentControl))]
    [TemplatePart(Name = "Part_Help", Type = typeof(HelpDisplay))]
    public class FormWithHelp : ContentControl
    {
        private ContentControl _partContent;
        private HelpDisplay _partHelp;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            _partContent = GetTemplateChild("Part_Content") as ContentControl;
            _partHelp = GetTemplateChild("Part_Help") as HelpDisplay;

            if (_partContent == null || _partHelp == null)
            {
                throw new NullReferenceException("Template parts not available");
            }
            _partContent.GotFocus += partContent_GotFocus;
        }

        private void partContent_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            _partHelp.HelpName = element?.Tag?.ToString();
        }
    }
}