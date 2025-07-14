using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Proxy.Light
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class LightsSupervisor : ILightService, ILightServiceCallback
    {
        private TaskCompletionSource<bool> _taskSetLightIntensity;
        private InstanceContext _instanceContext;
        private DuplexServiceInvoker<ILightService> _lightService;
        private ILogger _logger;
        private IMessenger _messenger;
        public List<LightVM> Lights { get; private set; }

        private LightContext _lightBeingSet;

        public LightsSupervisor(ILogger<LightsSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            _lightService = new DuplexServiceInvoker<ILightService>(_instanceContext, "HARDWARELightService", ClassLocator.Default.GetInstance<SerilogLogger<ILightService>>(), messenger, x => x.SubscribeToLightChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.HardwareControl));
            _logger = logger;
            _messenger = messenger;
            FillLightList();
            ServiceLocator.KeyboardMouseHook.KeyPressedEvent += KeyboardMouseHook_KeyEvent;
        }

        private void KeyboardMouseHook_KeyEvent(object sender, KeyGlobalEventArgs e)
        {
            if (e.CurrentKey == Key.F7)
            {
                // Decrease light intensity by 1 %
                if (Lights[0].Intensity - 1 < 0)
                    Lights[0].Intensity = 0;
                else
                    Lights[0].Intensity--;
                return;
            }
            if (e.CurrentKey == Key.F8)
            {
                // Increase light intensity by 1 %
                if (Lights[0].Intensity + 1 > 100)
                    Lights[0].Intensity = 100;
                else
                    Lights[0].Intensity++;
                return;
            }
        }

        public Response<double> GetLightIntensity(string lightID)
        {
            return _lightService.TryInvokeAndGetMessages(l => l.GetLightIntensity(lightID));
        }

        public Response<VoidResult> SetLightIntensity(string lightID, double intensity)
        {
            lock (this)
            {
                return _lightService.TryInvokeAndGetMessages(l => l.SetLightIntensity(lightID, intensity));
            }
        }

        public async Task<bool> SetLightIntensityAsync(string lightID, double intensity)
        {
            Console.WriteLine($"SetLightIntesityAsync LightID : {lightID} Intensity {intensity}");

            if (!(_taskSetLightIntensity is null))
            {
                if (!SpinWait.SpinUntil(() => _taskSetLightIntensity is null, 15000))
                {
                    _logger.Error($"Failed to set light intensity LightID : {lightID} Intensity {intensity}");
                    return false;
                }
            }
            _lightBeingSet = new LightContext(lightID, intensity);
            _taskSetLightIntensity = new TaskCompletionSource<bool>();
            SetLightIntensity(lightID, intensity);

            var task = _taskSetLightIntensity.Task;
            if (await Task.WhenAny(task, Task.Delay(10000)) == task)
            {
                _taskSetLightIntensity = null;
                Console.WriteLine($"SetLightIntesityAsync Done LightID : {lightID} Intensity {intensity}");
                return task.Result;
            }
            else
            {
                _taskSetLightIntensity = null;
                Console.WriteLine($"SetLightIntesityAsync Failed LightID : {lightID} Intensity {intensity}");
                return false;
            }
        }

        public void LightIntensityChangedCallback(string lightID, double intensity)
        {
            var light = Lights.FirstOrDefault(x => x.DeviceID == lightID);

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (light != null)
                    light.SetIntensityFromHardware(intensity);
                LightsChangedEvent?.Invoke(lightID, intensity);

                if (!(_taskSetLightIntensity is null))
                {
                    if (_lightBeingSet.DeviceID == lightID && _lightBeingSet.Intensity == intensity)
                    {
                        _taskSetLightIntensity.TrySetResult(true);
                    }
                }
            }));
        }

        public Response<VoidResult> SubscribeToLightChanges()
        {
            return _lightService.TryInvokeAndGetMessages(l => l.SubscribeToLightChanges());
        }

        public Response<VoidResult> UnsubscribeToLightChanges()
        {
            return _lightService.TryInvokeAndGetMessages(l => l.UnsubscribeToLightChanges());
        }

        private void FillLightList()
        {
            if (Lights == null)
                Lights = new List<LightVM>();

            foreach (var probe in ServiceLocator.ProbesSupervisor.Probes)
            {
                if (probe.Configuration is ISingleProbeConfig)
                {
                    foreach (var light in (probe.Configuration as ISingleProbeConfig).Lights.Where(x => x.IsEnabled))
                    {
                        if (!Lights.Any((l) => l.DeviceID == light.DeviceID))
                            Lights.Add(new LightVM(light, this));
                    }
                }
            }
        }

        public delegate void LightsChangedHandler(string lightID, double intensity);

        public event LightsChangedHandler LightsChangedEvent;

        // All the lights in the UP Modules
        public List<LightVM> LightsUp { get => Lights.Where(x => x.Position == PM.Shared.Hardware.Service.Interface.ModulePositions.Up).ToList(); }

        // All the lights in the DOWN Modules
        public List<LightVM> LightsDown { get => Lights.Where(x => x.Position == PM.Shared.Hardware.Service.Interface.ModulePositions.Down).ToList(); }

        public LightVM GetMainLight()
        {
            var light = LightsUp.SingleOrDefault(x => x.IsMainLight);
            if (light == null)
                light = LightsDown.SingleOrDefault(x => x.IsMainLight);
            if (light == null)
            {
                _logger.Error("There is no main light defined");
                throw new Exception("There is no main light defined");
            }
            return light;
        }

        public bool LightsAreLocked
        {
            get
            {
                // If all the lights are Locked
                return !Lights.Any(l => !l.IsLocked);
            }

            set
            {
                foreach (var light in Lights)
                {
                    light.IsLocked = value;
                }
            }
        }
    }
}
