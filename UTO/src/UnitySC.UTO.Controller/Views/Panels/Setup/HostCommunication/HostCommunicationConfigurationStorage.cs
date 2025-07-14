using System;

using Agileo.Common.Configuration;
using Agileo.Common.Tracing;
using Agileo.Semi.Communication.Abstractions.E37;
using Agileo.Semi.Communication.Abstractions.E4;
using Agileo.Semi.Gem.Abstractions.E30;

using GEM.Common;

using Newtonsoft.Json;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.HostCommunication
{
    /// <summary>
    /// This class is used to act as a bridge between the <see cref="HostCommunicationConfiguration"/> instance and the internal configuration of the application <see cref="Remote.GemController"/> component.
    /// It is given to a <see cref="ConfigManager{T}"/> as a storage strategy.
    /// </summary>
    public class HostCommunicationConfigurationStorage : IStorage
    {
        // Convert the Application Host Communication Parameters to an HostCommunicationConfiguration instance
        public T Load<T>(out string errors) where T : IConfiguration
        {
            var hostConfiguration = new HostCommunicationConfiguration();

            errors = string.Empty;
            if (App.ControllerInstance.GemController.E30Std.Connection.Configuration is HSMSConfiguration)
            {
                hostConfiguration.Hsms = Clone(App.ControllerInstance.GemController.E30Std.Connection.Configuration as HSMSConfiguration, out errors);
                hostConfiguration.HsmsActive = true;
            }
            else if (App.ControllerInstance.GemController.E30Std.Connection.Configuration is RS232Configuration)
            {
                hostConfiguration.Secs1 = Clone(App.ControllerInstance.GemController.E30Std.Connection.Configuration as RS232Configuration, out errors);
                hostConfiguration.HsmsActive = false;
            }

            SafeGetValue<string>(nameof(HostCommunicationConfiguration.MDLN), v => hostConfiguration.MDLN = v, true);
            SafeGetValue<string>(nameof(HostCommunicationConfiguration.SOFTREV), v => hostConfiguration.SOFTREV = v, true);
            SafeGetValue<string>(nameof(HostCommunicationConfiguration.EqpSerialNum), v => hostConfiguration.EqpSerialNum = v, true);
            SafeGetValue<string>(nameof(HostCommunicationConfiguration.E30EquipmentSupplier), v => hostConfiguration.E30EquipmentSupplier = v, true);
            SafeGetValue<string>(nameof(HostCommunicationConfiguration.EqpName), v => hostConfiguration.EqpName = v);

            SafeGetValue<int>(nameof(HostCommunicationConfiguration.EstablishCommunicationTimeout), v => hostConfiguration.EstablishCommunicationTimeout = v);

            SafeGetValue<DefaultCommStateType>(nameof(DefaultStatesConfiguration.DefaultCommState), v => hostConfiguration.DefaultStates.DefaultCommState = v);
            SafeGetValue<DefaultControlStateType>(nameof(DefaultStatesConfiguration.DefaultControlState), v => hostConfiguration.DefaultStates.DefaultControlState = v);
            SafeGetValue<OnLineFailSubstateType>(nameof(DefaultStatesConfiguration.OnLineFailSubstate), v => hostConfiguration.DefaultStates.OnLineFailSubstate = v);
            SafeGetValue<DefaultOnLineStateType>(nameof(DefaultStatesConfiguration.DefaultOnLineState), v => hostConfiguration.DefaultStates.DefaultOnLineState = v);
            SafeGetValue<byte>(nameof(DefaultStatesConfiguration.TimeFormat), v => hostConfiguration.DefaultStates.TimeFormat = v);

            SafeGetValue<string>(nameof(Secs2LogsConfiguration.LogFileName), v => hostConfiguration.Secs2Logs.LogFileName = v);
            SafeGetValue<ulong>(nameof(Secs2LogsConfiguration.LogFileSize), v => hostConfiguration.Secs2Logs.LogFileSize = v);
            SafeGetValue<LogMode>(nameof(Secs2LogsConfiguration.LogMode), v => hostConfiguration.Secs2Logs.LogMode = v);
            SafeGetValue<bool>(nameof(Secs2LogsConfiguration.IsHsmsSecsTracerActive), v => hostConfiguration.Secs2Logs.IsHsmsSecsTracerActive = v);

            try
            {
                return (T)Convert.ChangeType(hostConfiguration, typeof(T));
            }
            catch (InvalidCastException e)
            {
                errors = e.Message;
                return default(T);
            }
        }

        // Apply the modified HostCommunicationConfiguration instance to the Application
        public string Save<T>(T configuration) where T : IConfiguration
        {
            var errors = string.Empty;
            var hostConfiguration = configuration as HostCommunicationConfiguration;

            if (hostConfiguration != null)
            {
                if (hostConfiguration.HsmsActive)
                {
                    App.ControllerInstance.GemController.E30Std.Connection.ChangeConfiguration(Clone(hostConfiguration.Hsms, out errors));
                }
                else
                {
                    App.ControllerInstance.GemController.E30Std.Connection.ChangeConfiguration(Clone(hostConfiguration.Secs1, out errors));
                }

                SafeSetValue(nameof(HostCommunicationConfiguration.MDLN), hostConfiguration.MDLN, true);
                SafeSetValue(nameof(HostCommunicationConfiguration.SOFTREV), hostConfiguration.SOFTREV, true);
                SafeSetValue(nameof(HostCommunicationConfiguration.EqpSerialNum), hostConfiguration.EqpSerialNum, true);
                SafeSetValue(nameof(HostCommunicationConfiguration.E30EquipmentSupplier), hostConfiguration.E30EquipmentSupplier, true);
                SafeSetValue(nameof(HostCommunicationConfiguration.EqpName), hostConfiguration.EqpName);
                SafeSetValue(nameof(HostCommunicationConfiguration.EstablishCommunicationTimeout), hostConfiguration.EstablishCommunicationTimeout);

                SafeSetValue(nameof(DefaultStatesConfiguration.DefaultCommState), hostConfiguration.DefaultStates.DefaultCommState);
                SafeSetValue(nameof(DefaultStatesConfiguration.DefaultControlState), hostConfiguration.DefaultStates.DefaultControlState);
                SafeSetValue(nameof(DefaultStatesConfiguration.OnLineFailSubstate), hostConfiguration.DefaultStates.OnLineFailSubstate);
                SafeSetValue(nameof(DefaultStatesConfiguration.DefaultOnLineState), hostConfiguration.DefaultStates.DefaultOnLineState);
                SafeSetValue(nameof(DefaultStatesConfiguration.TimeFormat), hostConfiguration.DefaultStates.TimeFormat);

                SafeSetValue(nameof(Secs2LogsConfiguration.LogFileName), hostConfiguration.Secs2Logs.LogFileName);
                SafeSetValue(nameof(Secs2LogsConfiguration.LogFileSize), hostConfiguration.Secs2Logs.LogFileSize);
                SafeSetValue(nameof(Secs2LogsConfiguration.LogMode), hostConfiguration.Secs2Logs.LogMode);
                SafeSetValue(nameof(Secs2LogsConfiguration.IsHsmsSecsTracerActive), hostConfiguration.Secs2Logs.IsHsmsSecsTracerActive);
            }
            return errors;
        }

        #region Privates

        private static void SafeGetValue<T>(string wellKnownName, Action<T> assignAction, bool isVariable = false) where T : IConvertible
        {
            if (isVariable)
            {
                try
                {
                    var e30Variable = App.ControllerInstance.GemController.E30Std.DataServices.GetVariableByWellKnownName(wellKnownName);
                    var typedValue = e30Variable.ValueTo<T>();
                    assignAction(typedValue);
                }
                catch (Exception e)
                {
                    TraceManager.Instance().Trace(nameof(HostCommunicationSetupPanel), TraceLevelType.Warning, $"Unable to retrieve the value of the SV named '{wellKnownName}'",
                        new TraceParam { StringAttachment = e.ToString() });
                }
            }
            else
            {
                try
                {
                    var value = App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices.GetValueByWellKnownName(wellKnownName);
                    var typedValue = value.ValueTo<T>();
                    assignAction(typedValue);
                }
                catch (Exception e)
                {
                    TraceManager.Instance().Trace(nameof(HostCommunicationSetupPanel), TraceLevelType.Warning, $"Unable to retrieve the value of the EC named '{wellKnownName}'",
                        new TraceParam { StringAttachment = e.ToString() });
                }
            }
        }

        private static void SafeSetValue<T>(string wellKnownName, T value, bool isVariable = false)
        {
            if (isVariable)
            {
                try
                {
                    var e30Variable = App.ControllerInstance.GemController.E30Std.DataServices.GetVariableByWellKnownName(wellKnownName);
                    e30Variable.SetValue(value);
                }
                catch (Exception e)
                {
                    TraceManager.Instance()
                        .Trace(nameof(HostCommunicationSetupPanel), TraceLevelType.Warning, $"Unable to set the value of the SV named '{wellKnownName}'",
                            new TraceParam { StringAttachment = e.ToString() });
                }
            }
            else
            {
                try
                {
                    var eac = App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices.SetValueByWellKnownName(wellKnownName, value);
                    if (eac.ValueTo<int>() != 0)
                    {
                        TraceManager.Instance().Trace(nameof(HostCommunicationSetupPanel), TraceLevelType.Warning, $"Unable to set the value of the EC named '{wellKnownName}' : {eac.Description}");
                    }
                }
                catch (Exception e)
                {
                    TraceManager.Instance()
                        .Trace(nameof(HostCommunicationSetupPanel), TraceLevelType.Warning, $"Unable to set the value of the EC named '{wellKnownName}'",
                            new TraceParam { StringAttachment = e.ToString() });
                }
            }
        }

        /// <summary>
        /// Allows to clone the configuration instance so as not to impact the active configuration instance in the application <see cref="Remote.GemController"/> component.
        /// </summary>
        private static T Clone<T>(T configuration, out string errors)
        {
            errors = string.Empty;
            try
            {
                var serializedValue = JsonConvert.SerializeObject(configuration);
                return JsonConvert.DeserializeObject<T>(serializedValue);
            }
            catch (Exception ex)
            {
                errors = ex.ToString();
                return default(T);
            }
        }

        #endregion
    }
}
