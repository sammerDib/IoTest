using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;

namespace UnitySC.Equipment.Abstractions.Vendor
{
    public class EquipmentManager : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentManager"/> class.
        /// </summary>
        /// <param name="equipmentFilePath">The equipment file path.</param>
        /// <param name="setup">Setup callback, called after load of equipment</param>
        public EquipmentManager(string equipmentFilePath, Action<Agileo.EquipmentModeling.Equipment> setup = null)
        {
            Logger = Agileo.Common.Logging.Logger.GetLogger(GetType().Name);
            Equipment = LoadEquipment(equipmentFilePath);

            setup?.Invoke(Equipment);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="EquipmentManager"/> class.
        /// </summary>
        ~EquipmentManager()
        {
            Dispose(false);
        }

        #endregion Constructors

        #region Properties

        protected ILogger Logger { get; }

        public ActivityManager ActivityManager { get; set; }

        #endregion

        #region Generic Devices Accessors

        /// <summary>
        /// Gets the equipment.
        /// </summary>
        public Agileo.EquipmentModeling.Equipment Equipment { get; }

        /// <summary>
        /// Gets all the instance of <see cref="CommunicatingDevice"/> present inside the loaded equipment.
        /// </summary>
        public IEnumerable<CommunicatingDevice> CommunicatingDevices => Equipment.AllDevices<CommunicatingDevice>();

        #endregion Generic Devices Accessors

        #region Equipment Helper Methods

        /// <summary>
        /// Loads the equipment.
        /// </summary>
        /// <param name="equipmentFilePath">The equipment file path.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Failed to load equipment file</exception>
        private Agileo.EquipmentModeling.Equipment LoadEquipment(string equipmentFilePath)
        {
            Logger.Info("Load the Equipment Model {EquipmentFilePath}", equipmentFilePath);

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyPath = Path.GetDirectoryName(assembly.Location);
            var equipment = Agileo.EquipmentModeling.Equipment.LoadFromFile(equipmentFilePath, assemblyPath);
            if (equipment == null)
            {
                throw new InvalidOperationException($"Failed to load equipment file: {equipmentFilePath}");
            }

            return equipment;
        }

        #endregion Equipment Helper Methods

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Equipment?.Dispose();
                foreach (var device in Equipment.AllDevices())
                {
                    DisposeSubDevices(device);
                }
            }
        }

        #endregion IDisposable

        #region Private Methods

        private void DisposeSubDevices(Device device)
        {
            foreach (var subDevice in device.AllDevices())
            {
                if (subDevice.Devices.Count > 0)
                {
                    DisposeSubDevices(subDevice);
                }

                subDevice.Dispose();
            }
        }


        #endregion
    }
}
