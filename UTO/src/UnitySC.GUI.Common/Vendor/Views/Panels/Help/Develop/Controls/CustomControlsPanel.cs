using System;
using System.Collections.ObjectModel;

using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitsNet;
using UnitsNet.Units;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Controls
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of the panel view
    /// </summary>
    public class CustomControlsPanel : BusinessPanel
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a design time instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Controls.CustomControlsPanel" /> class.
        /// </summary>
        public CustomControlsPanel() : this("DesignTime Constructor")
        {
            if (!IsInDesignMode) { throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters."); }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Controls.CustomControlsPanel" /> class.
        /// </summary>
        /// <param name="id">Graphical identifier of the View Model. Can be either a <seealso cref="T:System.String" /> either a localizable resource.</param>
        /// <param name="icon">Optional parameter used to define the representation of the panel inside the application.</param>
        public CustomControlsPanel(string id, IIcon icon = null) : base(id, icon)
        {
        }

        public ObservableCollection<string> SelectedIntStrings { get; } = new();
        public ObservableCollection<string> SelectedColorStrings { get; } = new();


        private Frequency _frequency = new(100, FrequencyUnit.Hertz);

        public Frequency Frequency
        {
            get => _frequency;
            set => SetAndRaiseIfChanged(ref _frequency, value);
        }
    }
}

