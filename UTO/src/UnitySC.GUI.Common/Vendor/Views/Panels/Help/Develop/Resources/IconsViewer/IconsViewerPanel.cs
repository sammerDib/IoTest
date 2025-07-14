using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Resources.IconsViewer
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of the panel view
    /// </summary>
    public class IconsViewerPanel : BusinessPanel
    {
        #region Properties

        public DataTableSource<KeyValuePair<string, PathGeometry>> DataTableSource { get; set; } = new();
        
        #endregion

        /// <inheritdoc />
        /// <summary>
        /// Initializes a design time instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTable.DataTablePanel" /> class.
        /// </summary>
        public IconsViewerPanel() : this("DesignTime Constructor")
        {
            if (!IsInDesignMode) { throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters."); }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTable.DataTablePanel" /> class.
        /// </summary>
        /// <param name="id">Graphical identifier of the View Model. Can be either a <seealso cref="T:System.String" /> either a localizable resource.</param>
        /// <param name="icon">Optional parameter used to define the representation of the panel inside the application.</param>
        public IconsViewerPanel(string id, IIcon icon = null) : base(id, icon)
        {
            DataTableSource.Search.AddSearchDefinition(new InvariantText("Key"), item => item.Key.ToString(CultureInfo.InvariantCulture), true);
            DataTableSource.Reset(ResourcesHelper.GetAll<PathGeometry>().ToList());
        }

        private ICommand _copyKeyCommand;

        public ICommand CopyKeyCommand => _copyKeyCommand ??= new DelegateCommand<object>(CopyKeyCommandExecute, CopyKeyCommandCanExecute);

        private bool CopyKeyCommandCanExecute(object arg)
        {
            return arg is KeyValuePair<string, PathGeometry>;
        }

        private void CopyKeyCommandExecute(object arg)
        {
            if (arg is KeyValuePair<string, PathGeometry> keyValue)
            {
                Clipboard.SetText(keyValue.Key);
            }
        }

        private ICommand _copyXamlCommand;

        public ICommand CopyXamlCommand
            => _copyXamlCommand ??= new DelegateCommand<object>(CopyXamlCommandExecute, CopyXamlCommandCanExecute);

        private bool CopyXamlCommandCanExecute(object arg)
        {
            return arg is KeyValuePair<string, PathGeometry>;
        }

        private void CopyXamlCommandExecute(object arg)
        {
            if (arg is KeyValuePair<string, PathGeometry> keyValue)
            {
                Clipboard.SetText($"<controls:Icon Data=\"{{StaticResource {keyValue.Key}}}\" />");
            }
        }
    }
}

