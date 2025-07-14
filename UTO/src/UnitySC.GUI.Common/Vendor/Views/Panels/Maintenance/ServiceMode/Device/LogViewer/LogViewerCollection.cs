using System;
using System.Collections.Generic;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.LogViewer
{
    public sealed class LogViewerCollection : IDisposable
    {
        private readonly Dictionary<Agileo.EquipmentModeling.Device, LogViewerViewModel> _logViewers = new();

        public void CreateLogViewer(Agileo.EquipmentModeling.Device device)
        {
            _logViewers.Add(device, new LogViewerViewModel(device));
        }

        public LogViewerViewModel GetLogViewer(Agileo.EquipmentModeling.Device device)
        {
            if (!_logViewers.TryGetValue(device, out var result))
            {
                throw new InvalidOperationException($"No log viewer found for device {device.Name}");
            }

            return result;
        }

        public void Dispose()
        {
            foreach (var logViewerViewModel in _logViewers)
            {
                logViewerViewModel.Value.Dispose();
            }

            _logViewers.Clear();
        }
    }
}
