using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public interface ISubDevice
    {
        /// <summary>
        /// Device name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Device state
        /// </summary>
        DeviceState State { get; set; }

        /// <summary>
        /// Device family
        /// </summary>
        String DeviceGroupName { get; }

        /// <summary>
        /// Device ID
        /// </summary>
        string DeviceID { get; set; }

        /// <summary>
        /// Event to notifiy a modification of status
        /// </summary>
        event StateChangedEventHandler OnStatusChanged;
    }

   
}
