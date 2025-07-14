using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.DataMonitoring;
using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Editors.DataSourceEditor
{
    public class EquipmentDataSourceEditor : Notifier
    {
        private readonly DataCollectionPlan _dataCollectionPlan;

        /// <summary>
        /// Build a new instance of <see cref="EquipmentDataSourceEditor"/>
        /// </summary>
        /// <param name="equipment">Equipment available to give data to a <see cref="DataCollectionPlan"/>.</param>
        /// <param name="dataCollectionPlan">The <see cref="DataCollectionPlan"/> on which to add sources.</param>
        public EquipmentDataSourceEditor(Agileo.EquipmentModeling.Equipment equipment, DataCollectionPlan dataCollectionPlan)
        {
            Equipment = equipment;
            _dataCollectionPlan = dataCollectionPlan;
        }

        public Agileo.EquipmentModeling.Equipment Equipment { get; }

        private Device _selectedDevice;

        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                if (_selectedDevice == value) return;
                _selectedDevice = value;
                OnPropertyChanged();
                UpdateSelectedDeviceStatus(_selectedDevice);
            }
        }

        private ICollection<DeviceStatus> _deviceStatus;
        public ICollection<DeviceStatus> DeviceStatus
        {
            get
            {
                return _deviceStatus;

            }
            private set
            {
                _deviceStatus = value;
                OnPropertyChanged();
            }
        }

        private void UpdateSelectedDeviceStatus(Device device)
        {
            var statuses = GetRelatedStatuses(device);
            DeviceStatus = statuses.Where(status
                => !_dataCollectionPlan.DataSources.Any(
                    source => source.Information.SourceName.Equals(status.Name))).ToList();
        }

        private IEnumerable<DeviceStatus> GetRelatedStatuses(Device device)
        {
            if (device == null) return new Collection<DeviceStatus>();
            var statuses = new Collection<DeviceStatus>();
            device.DeviceType.Statuses.ToList().ForEach(status =>
            {

                if (status.Unit == null)
                {
                    var cSharpType = (status.Type as CSharpType);
                    var isInterpretable = cSharpType != null;
                    if (!isInterpretable) return;

                    var isEnum = cSharpType.PlatformType.IsEnum;
                    var isPrimitive = cSharpType.PlatformType.IsPrimitive;
                    if (!isEnum && isPrimitive)
                    {
                        statuses.Add(status);
                    }
                }
                else
                {
                    statuses.Add(status);
                }
            });
            device.DeviceType.SuperType?.Statuses.ToList().ForEach(status => { if (IsQuantityStatus(status)) statuses.Add(status); });
            return statuses;
        }

        private DeviceStatus _selectedStatus;

        public DeviceStatus SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                if (_selectedStatus == value) return;
                _selectedStatus = value;
                OnPropertyChanged();
            }
        }

        private bool IsQuantityStatus(DeviceStatus status)
        {
            try
            {
                var quantityTypes = Quantity.Types.Select(quantityType => Quantity.GetInfo(quantityType).UnitType).ToList();
                var type = status.Unit?.GetType();
                return quantityTypes.Contains(type);
            }
            catch
            {
                throw new InvalidOperationException($"An error occurred in {nameof(EquipmentDataSourceEditor)}.{nameof(IsQuantityStatus)}");
            }
        }
    }
}
