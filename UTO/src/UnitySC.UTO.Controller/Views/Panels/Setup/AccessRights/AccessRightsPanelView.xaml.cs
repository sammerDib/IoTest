using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using UnitySC.GUI.Common.Vendor.UIComponents.Extensions;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.AccessRights
{
    /// <summary>
    /// Interaction logic for AccessRightsPanelView.xaml
    /// </summary>
    public partial class AccessRightsPanelView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRightsPanelView"/> class.
        /// </summary>
        public AccessRightsPanelView()
        {
            InitializeComponent();

            CommandsItemsControl.ItemContainerGenerator.StatusChanged += CommandsItemContainerGeneratorOnStatusChanged;
            ToolsItemsControl.ItemContainerGenerator.StatusChanged += ToolsItemContainerGeneratorOnStatusChanged;
        }
        
        private void CommandsItemContainerGeneratorOnStatusChanged(object sender, EventArgs e)
        {
            if (CommandsItemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                var virtualizingStackPanel = CommandsItemsControl.GetChildren<VirtualizingStackPanel>().FirstOrDefault();
                CommandsItemsControlWidth = virtualizingStackPanel?.ActualWidth ?? double.NaN;
            }
        }

        private void ToolsItemContainerGeneratorOnStatusChanged(object sender, EventArgs e)
        {
            if (ToolsItemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                var virtualizingStackPanel = ToolsItemsControl.GetChildren<VirtualizingStackPanel>().FirstOrDefault();
                ToolsItemControlWidth = virtualizingStackPanel?.ActualWidth ?? double.NaN;
            }
        }

        public static readonly DependencyProperty CommandsItemsControlWidthProperty = DependencyProperty.Register(
            nameof(CommandsItemsControlWidth),
            typeof(double),
            typeof(AccessRightsPanelView),
            new PropertyMetadata(default(double)));

        public double CommandsItemsControlWidth
        {
            get { return (double)GetValue(CommandsItemsControlWidthProperty); }
            set { SetValue(CommandsItemsControlWidthProperty, value); }
        }

        public static readonly DependencyProperty ToolsItemControlWidthProperty = DependencyProperty.Register(
            nameof(ToolsItemControlWidth),
            typeof(double),
            typeof(AccessRightsPanelView),
            new PropertyMetadata(default(double)));

        public double ToolsItemControlWidth
        {
            get { return (double)GetValue(ToolsItemControlWidthProperty); }
            set { SetValue(ToolsItemControlWidthProperty, value); }
        }
    }
}
