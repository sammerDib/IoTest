using System.Reflection;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.Semi.Communication.Abstractions.E37;
using Agileo.Semi.Communication.Abstractions.E4;
using Agileo.Semi.Gem.Abstractions.E30;

using GEM.Common;

using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.HostCommunication
{
    public class HostCommunicationConfiguration : IConfiguration
    {
        static HostCommunicationConfiguration()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SetupPanelResources)));
        }

        public HostCommunicationConfiguration()
        {
            // Setup with default values (From GEM.Common.Constants internal classes)

            // HSMS
            Hsms.LocalIP = "127.0.0.1";
            Hsms.RemoteIP = "127.0.0.1";
            Hsms.LocalPort = 5050;
            Hsms.RemotePort = 5050;
            Hsms.IsActive = false;
            Hsms.T3 = 45;
            Hsms.T5 = 5;
            Hsms.T6 = 5;
            Hsms.T7 = 5;
            Hsms.T8 = 5;
            Hsms.LinkTest = 60;
            Hsms.DeviceID = 10;

            //SECS-I
            Secs1.EntityBehaviour = 0;
            Secs1.SerialPort = 1;
            Secs1.BaudRate = 9600;
            Secs1.T1 = 5;
            Secs1.T2 = 10;
            Secs1.T3 = 45;
            Secs1.T4 = 45;
            Secs1.RetryLimit = 3;
            Secs1.DeviceID = 10;
        }

        public HSMSConfiguration Hsms { get; set; } = new();

        public RS232Configuration Secs1 { get; set; } = new();

        public DefaultStatesConfiguration DefaultStates { get; set; } = new();

        public Secs2LogsConfiguration Secs2Logs { get; set; } = new();

        public bool HsmsActive { get; set; }

        // ReSharper disable once InconsistentNaming
        public string MDLN { get; set; } = Assembly.GetCallingAssembly().GetName().Name;

        // ReSharper disable once InconsistentNaming
        public string SOFTREV { get; set; } = "1.0.0.0";

        public string EqpSerialNum { get; set; }

        public string E30EquipmentSupplier { get; set; } = "Unity-SC";

        public string EqpName { get; set; }

        public int EstablishCommunicationTimeout { get; set; } = 10;

        #region Implementation of IConfiguration

        public string ValidatingParameters() => Hsms.Validate();

        public string ValidatedParameters()
        {
            if (!HsmsActive)
            {
                return IsAnyInvalidSecs1Parameters()
                    ? LocalizationManager.GetString(
                        nameof(SetupPanelResources.SETUP_MESSAGE_HOST_COMMUNICATION_VALUE_WARNING))
                    : string.Empty;
            }

            if (IsAnyInvalidHsmsParameters() || Secs2Logs.LogFileSize <= 0)
            {
                return LocalizationManager.GetString(
                    nameof(SetupPanelResources.SETUP_MESSAGE_HOST_COMMUNICATION_VALUE_WARNING));
            }

            return Hsms.Validate();
        }

        private bool IsAnyInvalidHsmsParameters()
            => IsIntParameterInvalid(0, 65535, Hsms.LocalPort)
               || IsIntParameterInvalid(0, 65535, Hsms.RemotePort)
               || IsIntParameterInvalid(1, 120, Hsms.T3)
               || IsIntParameterInvalid(1, 240, Hsms.T5)
               || IsIntParameterInvalid(1, 240, Hsms.T6)
               || IsIntParameterInvalid(1, 240, Hsms.T7)
               || IsIntParameterInvalid(1, 120, Hsms.T8)
               || IsIntParameterInvalid(1, 120, Hsms.LinkTest);

        private bool IsAnyInvalidSecs1Parameters()
            => IsIntParameterInvalid(300, 9600, Secs1.BaudRate)
               || IsIntParameterInvalid(0, 32767, Secs1.DeviceID)
               || IsDoubleParameterInvalid(0.1, 10, Secs1.T1)
               || IsDoubleParameterInvalid(0.1, 240, Secs1.T2)
               || IsIntParameterInvalid(1, 240, Secs1.T3)
               || IsIntParameterInvalid(1, 240, Secs1.T4)
               || IsIntParameterInvalid(1, 120, Secs1.RetryLimit);

        private static bool IsIntParameterInvalid(int valueMin, int valueMax, int parameterToControl)
            => parameterToControl < valueMin || parameterToControl > valueMax;

        private static bool IsDoubleParameterInvalid(double valueMin, double valueMax, double parameterToControl)
            => parameterToControl < valueMin || parameterToControl > valueMax;

        #endregion
    }

    public class DefaultStatesConfiguration
    {
        // Default values are taken from GEM.Common.Constants internal classes
        public DefaultCommStateType DefaultCommState { get; set; } = DefaultCommStateType.Disabled;

        public DefaultControlStateType DefaultControlState { get; set; } = DefaultControlStateType.HostOffLine;

        public OnLineFailSubstateType OnLineFailSubstate { get; set; } = OnLineFailSubstateType.HostOffLine;

        public DefaultOnLineStateType DefaultOnLineState { get; set; } = DefaultOnLineStateType.Local;

        public byte TimeFormat { get; set; } = 0;
    }

    public class Secs2LogsConfiguration
    {
        // Default values are taken from GEM.Common.Constants internal classes
        public string LogFileName { get; set; } = "Log_";

        public ulong LogFileSize { get; set; } = 1000000;

        public LogMode LogMode { get; set; } = LogMode.All;

        public bool IsHsmsSecsTracerActive { get; set; } = true;
    }
}
