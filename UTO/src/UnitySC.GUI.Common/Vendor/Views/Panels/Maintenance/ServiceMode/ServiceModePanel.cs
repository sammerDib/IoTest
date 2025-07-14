using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.MarkDownViewer;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.LogViewer;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode
{
    public class ServiceModePanel : BusinessPanel
    {
        #region Fields

        private readonly Agileo.EquipmentModeling.Equipment _equipment;

        private readonly Dictionary<DeviceContainer, DeviceViewModel> _deviceViewModels = new();

        private readonly LogViewerCollection _logViewers = new();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceModePanel" /> class ONLY FOR the DESIGN MODE.
        /// </summary>
        public ServiceModePanel() : this(null, "DesignTime Constructor")
        {
            if (!IsInDesignMode) { throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters."); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceModePanel" /> class.
        /// </summary>
        public ServiceModePanel(Agileo.EquipmentModeling.Equipment equipment, string id, IIcon icon = null) : base(id, icon)
        {
            _equipment = equipment;
            DataTreeSource = new DataTreeSource<DeviceContainer>(device => device.Devices);
            GetMarkdownToolTipFunc = MarkDownToolTip;
        }

        #region Properties
        
        public Func<DeviceContainer, MarkDownViewerViewModel> GetMarkdownToolTipFunc { get; }

        public DataTreeSource<DeviceContainer> DataTreeSource { get; set; }

        private DeviceContainer _selectedValue;

        public DeviceContainer SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                SetAndRaiseIfChanged(ref _selectedValue, value);
                ShowViewer(SelectedValue);
            }
        }

        private DeviceViewModel _deviceViewModel;

        public DeviceViewModel DeviceViewModel
        {
            get { return _deviceViewModel;}
            set
            {
                SetAndRaiseIfChanged(ref _deviceViewModel, value);
            }
        }

        private bool _areCommandsEnabled;

        public bool AreCommandsEnabled
        {
            get => _areCommandsEnabled;
            set => SetAndRaiseIfChanged(ref _areCommandsEnabled, value);
        }
        
        #endregion

        #region Methods

        public static MarkDownViewerViewModel MarkDownToolTip(DeviceContainer deviceContainer)
        {
            if (deviceContainer is not Agileo.EquipmentModeling.Device device)
            {
                return null;
            }

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(device.DeviceType.DocumentationAsMarkdown))
            {
                sb.AppendLine(device.DeviceType.DocumentationAsMarkdown);
            }

            var interlocksSb = new StringBuilder();
            foreach (var superType in device.DeviceType.GetSuperTypesHierarchy())
            {
                foreach (var docMarkdown in superType.Interlocks.Select(interlock => interlock.DocumentationAsMarkdown))
                {
                    if (!string.IsNullOrEmpty(docMarkdown))
                    {
                        interlocksSb.Append("* ");
                        interlocksSb.AppendLine(docMarkdown);
                    }
                }
            }

            foreach (var docMarkdown in device.DeviceType.Interlocks.Select(interlock => interlock.DocumentationAsMarkdown))
            {
                if (!string.IsNullOrEmpty(docMarkdown))
                {
                    interlocksSb.Append("* ");
                    interlocksSb.AppendLine(docMarkdown);
                }
            }

            if (interlocksSb.Length > 0)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("---");
                    sb.AppendLine();
                }

                sb.AppendLine("#### interlocks:");
                sb.AppendLine(interlocksSb.ToString());
            }

            return sb.Length > 0 ? new MarkDownViewerViewModel(sb.ToString()) : null;
        }

        private void ShowViewer(DeviceContainer container)
        {
            if (container is Agileo.EquipmentModeling.Device device)
            {
                if (_deviceViewModels.TryGetValue(device, out var deviceViewModel))
                {
                    DeviceViewModel = deviceViewModel;
                }
                else
                {
                    var logViewer = _logViewers.GetLogViewer(device);
                    DeviceViewModel = new DeviceViewModel(device, logViewer);
                    _deviceViewModels.Add(device, DeviceViewModel);
                }
            }
            else
            {
                DeviceViewModel = null;
            }
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void OnSetup()
        {
            if (_equipment.Container is Package pkg)
            {
                DataTreeSource.Reset(pkg.AllEquipments());
                DataTreeSource.ExpandAll();

                foreach (var device in pkg.AllDevices())
                {
                    _logViewers.CreateLogViewer(device);
                }
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var deviceViewModel in _deviceViewModels.Values)
                {
                    deviceViewModel.Dispose();
                }

                _deviceViewModels.Clear();
                _logViewers.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
