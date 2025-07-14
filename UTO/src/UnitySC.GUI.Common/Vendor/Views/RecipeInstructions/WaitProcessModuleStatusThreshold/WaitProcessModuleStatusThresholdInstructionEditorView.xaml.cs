using System;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.WaitProcessModuleStatusThreshold
{
    /// <summary>
    /// Logique d'interaction pour WaitStatusThresholdInstructionEditorView.xaml
    /// </summary>
    public partial class WaitProcessModuleStatusThresholdInstructionEditorView
    {
        private WaitProcessModuleStatusThresholdInstructionEditor ViewModel => DataContext as WaitProcessModuleStatusThresholdInstructionEditor;

        public WaitProcessModuleStatusThresholdInstructionEditorView()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel?.UpdateLabel();
        }

        private void UnitsNet_OnValueChanged(object sender, EventArgs e)
        {
            ViewModel?.UpdateLabel();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel?.UpdateLabel();
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel?.UpdateLabel();
        }
    }
}
