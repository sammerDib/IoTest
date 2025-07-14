using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Led.ViewModel
{
    public class LedVM : ObservableObject, INotifyPropertyChanged
    {
        private LedSupervisor _ledSupervisor;
        private string _deviceID;
        private string _ipAddress;
        private int _portNumber;
        private bool _ledActivated;
        private int _nbModules;

        private TcpClient _client;
        private NetworkStream _stream;

        public LedVM()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                throw new ApplicationException("This constructor is for design mode only.");
        }

        public LedVM(LedSupervisor ledSupervisor, string deviceID)
        {
            _ledSupervisor = ledSupervisor;
            _deviceID = deviceID;
            Responses = new ObservableCollection<string>();
            _ipAddress = "192.168.10.221";
            _portNumber = 1000;
            _nbModules = 0;
            _ledActivated = false;
            ActivateLed = new AutoRelayCommand<bool>(ChangeLedPower);
            SendCommand = new AutoRelayCommand<string>((cmd) => { SendCommandToLeds(cmd); GetLedErrors(); });
            ChangeGLI = new AutoRelayCommand<int>(ChangeGlobalIntensity);
            try
            {
                _client = new TcpClient(IpAddress, PortNumber);
                _stream = _client.GetStream();
                _stream.ReadTimeout = 2000;
                _stream.WriteTimeout = 2000;
            }
            catch (Exception e)
            {
                Responses.Add($"The connexion with LED is not established please do not send any command! Error: {e.Message}");
            }
        }

        public string IpAddress { get => _ipAddress; set { _ipAddress = value; OnPropertyChanged(); } }
        public int PortNumber { get => _portNumber; set { _portNumber = value; OnPropertyChanged(); } }

        public bool LedActivated { get => _ledActivated; set { _ledActivated = value; OnPropertyChanged(); } }

        public int NbModule { get => _nbModules; set { _nbModules = value; OnPropertyChanged(); } }

        public ObservableCollection<string> Responses { get; set; }

        public AutoRelayCommand<string> SendCommand { get; set; }

        public AutoRelayCommand<bool> ActivateLed { get; set; }

        public AutoRelayCommand<int> ChangeGLI { get; set; }

        private void ChangeLedPower(bool activated)
        {
            if (SendCommandToLeds($"GSS={(activated ? 0 : 1)}"))
            {
                LedActivated = !activated;
                // TODO get number of modules
            }
            else
            {
                GetLedErrors();
            }
        }

        private void ChangeGlobalIntensity(int value)
        {
            if (SendCommandToLeds($"GLI={value}"))
            {
                GetLedErrors();
            }
        }

        private bool SendCommandToLeds(string command)
        {
            try
            {
                Responses.Add($"TX:{command}");
                byte[] cmd = Encoding.ASCII.GetBytes(command + (char)0x0D);

                // Send command
                _stream.Write(cmd, 0, cmd.Length);

                // Get response
                using (MemoryStream ms = new MemoryStream())
                {
                    while (_stream.DataAvailable)
                    {
                        byte[] data = new byte[1024];
                        int bytes = _stream.Read(data, 0, data.Length);
                        ms.Write(data, 0, bytes);
                    }

                    ms.Position = 0;
                    string response = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                    Responses.Add($"RX: {response}");
                }

                //stream.Close();
                //client.Close();
            }
            catch (Exception e)
            {
                Responses.Add($"ERR: {e.Message}");
                return false;
            }

            return true;
        }

        private void GetLedErrors()
        {
            // TODO
        }
    }
}
