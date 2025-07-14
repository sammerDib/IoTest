using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Proxy.Light
{
    public class LightBench : ObservableRecipient
    {
        private readonly IEMELightService _lightsSupervisor;
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly IKeyboardMouseHook _keyboardMouseHook;        
        public ObservableCollection<LightVM> Lights { get; private set; }        
        private LightVM _selectedLight;
        public LightVM SelectedLight
        {
            get { return _selectedLight; }
            set
            {
                if (_selectedLight != value)
                {                    
                    if (_selectedLight != null && _selectedLight.IsTurnedOn)
                    {
                        _selectedLight.Switch.Execute(null);
                    }                   
                    _selectedLight = value;
                    OnPropertyChanged();
                }
                SetProperty(ref _selectedLight, value);
            }
        }       

        public LightBench(IEMELightService lightsSupervisor, IKeyboardMouseHook keyboardMouseHook, ILogger logger, IMessenger messenger)
        {
            _lightsSupervisor = lightsSupervisor;
            _logger = logger;
            _messenger = messenger;
            _keyboardMouseHook = keyboardMouseHook;

            Init();
            _messenger.Register<LightSourceChangedMessage>(this, (_, m) => UpdateLight(m));
            _keyboardMouseHook.KeyPressedEvent += KeyboardMouseHook_KeyEvent;
        }

        private void UpdateLight(LightSourceChangedMessage lightMessage)
        {
            var light = Lights.FirstOrDefault(x => x.DeviceID == lightMessage.LightID);
            light?.Update(lightMessage);
        }

        private void Init()
        {
            try
            {
                var lightsConfig = _lightsSupervisor.GetLightsConfig().Result;
                if (lightsConfig == null || lightsConfig.Count == 0)
                {
                    throw new NoLightsDefinedException("No light configuration is defined for this tool.");
                }

                var lights = lightsConfig.Select(item => new LightVM(item, _lightsSupervisor)).ToList();
                Lights = new ObservableCollection<LightVM>(lights);
                SelectedLight = lights.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Lights Init Failed");
                _messenger.Send(new Message(MessageLevel.Error, ex.Message));
            }
        }
        
        public void KeyboardMouseHook_KeyEvent(object sender, KeyGlobalEventArgs e)
        {
            switch (e.CurrentKey)
            {
                case Key.F7:
                    {
                        double newPower = Math.Max(0, Lights[0].Power - 1);
                        Lights[0].Power = newPower;
                        Lights[0].ChangePower.Execute(newPower);
                        break;
                    }
                case Key.F8:
                    {
                        double newPower = Math.Min(100, Lights[0].Power + 1);
                        Lights[0].Power = newPower;
                        Lights[0].ChangePower.Execute(newPower);
                        break;
                    }
            }
        }
        
        protected override void OnDeactivated()
        {
            _messenger.Unregister<LightSourceChangedMessage>(this);
            _keyboardMouseHook.KeyPressedEvent -= KeyboardMouseHook_KeyEvent;
            base.OnDeactivated();
        }
    }
}
