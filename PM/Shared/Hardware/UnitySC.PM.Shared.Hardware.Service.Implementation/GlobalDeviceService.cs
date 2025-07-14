using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Global;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GlobalDeviceService : BaseService, IGlobalDeviceService
    {
        private readonly HardwareManager _hardwareManager;
        private List<GlobalDevice> _currentDevicesState;
        private readonly object _lock = new object();
        private const double RefreshFrequencyMs = 200;
        private System.Timers.Timer _refreshTimer = new System.Timers.Timer();

        /// <summary>
        ///  Subscribed client callbacks
        /// </summary>
        private readonly List<IGlobalCallback> _hardwareServiceCallbackList;

        public GlobalDeviceService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareServiceCallbackList = new List<IGlobalCallback>();
            _hardwareManager = hardwareManager;
            _currentDevicesState = _hardwareManager.GetGlobalDevices();
            _refreshTimer.Interval = RefreshFrequencyMs;
            _refreshTimer.Elapsed += RefreshTimer_Elapsed;
            _refreshTimer.Enabled = true;
        }

        private void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_hardwareServiceCallbackList.Any())
            {
                var newDevicesStates = _hardwareManager.GetGlobalDevices().Clone().ToList();

                if (!DevicesValueEqual(_currentDevicesState, newDevicesStates))
                {
                    _currentDevicesState = newDevicesStates;
                    InvokeCallback(x => x.StatusChanged(newDevicesStates));
                }
            }
        }

        private bool DevicesValueEqual(List<GlobalDevice> devices1, List<GlobalDevice> devices2)
        {
            if (devices1 == devices2)
                return true;

            if (devices1 == null || devices2 == null)
                return false;

            if (devices1.Count != devices2.Count)
                return false;

            for (int i = 0; i < devices2.Count; i++)
            {
                if (devices1[i].Name != devices2[i].Name ||
                    devices1[i].State?.Status != devices2[i].State?.Status
                    || devices1[i].State?.StatusMessage != devices2[i].State?.StatusMessage)
                    return false;
            }

            return true;
        }

        public Response<List<GlobalDevice>> GetDevices()
        {
            return InvokeDataResponse(messageContainer =>
            {
                return _hardwareManager.GetGlobalDevices();
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    var globalCallback = OperationContext.Current.GetCallbackChannel<IGlobalCallback>();
                    _hardwareServiceCallbackList.TryAdd(globalCallback);
                }
            });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    var globalCallback = OperationContext.Current.GetCallbackChannel<IGlobalCallback>();
                    _hardwareServiceCallbackList.Remove(globalCallback);
                }
            });
        }

        private void InvokeCallback(Action<IGlobalCallback> reportOnClient)
        {
            lock (_lock)
            {
                var serviceCallbackToRemove = new List<IGlobalCallback>();
                // Execute callback for each subscribed client
                _hardwareServiceCallbackList.ForEach(callback =>
                {
                    if (((ICommunicationObject)callback).State == CommunicationState.Opened)
                        reportOnClient(callback);
                    else
                        serviceCallbackToRemove.Add(callback);
                });

                // Remove disconnected client callbacks
                foreach (var machineStatusServiceCallback in serviceCallbackToRemove)
                    _hardwareServiceCallbackList.Remove(machineStatusServiceCallback);
            }
        }
    }
}