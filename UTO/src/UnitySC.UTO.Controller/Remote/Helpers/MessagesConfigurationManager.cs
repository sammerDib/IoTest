using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using GEM;

using SECS2;

using UnitySC.UTO.Controller.Remote.Constants;

namespace UnitySC.UTO.Controller.Remote.Helpers
{
    internal class MessagesConfigurationManager
    {
        #region Fields

        private XDocument _configurationDocument;
        private readonly object _configLock = new();
        private readonly XmlWriterSettings _writerSettings = new() { Indent = true };

        #endregion

        #region Properties

        #region SV

        private Dictionary<string, Variable> _svDictionary;

        public Dictionary<string, Variable> SvDictionary
        {
            get
            {
                if (_svDictionary != null)
                {
                    return _svDictionary;
                }

                var variablesElement = _configurationDocument.Root?.Element(ConfigurationXNames.Variables);
                _svDictionary = ReadSVVariablesSettings(variablesElement);

                return _svDictionary;
            }
        }

        #endregion

        #region DV

        private Dictionary<string, Variable> _dvDictionary;

        public Dictionary<string, Variable> DvDictionary
        {
            get
            {
                if (_dvDictionary != null)
                {
                    return _dvDictionary;
                }

                var variablesElement = _configurationDocument.Root?.Element(ConfigurationXNames.Variables);
                _dvDictionary = ReadDVVariablesSettings(variablesElement);

                return _dvDictionary;
            }
        }

        #endregion


        #region EC

        private Dictionary<string, EC> _ecDictionary;

        public Dictionary<string, EC> EcDictionary
        {
            get
            {
                if (_ecDictionary != null)
                {
                    return _ecDictionary;
                }

                var variablesElement = _configurationDocument.Root?.Element(ConfigurationXNames.Variables);
                _ecDictionary = ReadECVariablesSettings(variablesElement);

                return _ecDictionary;
            }
        }

        #endregion


        #region HostConfig

        private Dictionary<string, EC> _hostConfigDictionary;

        public Dictionary<string, EC> HostConfigDictionary
        {
            get
            {
                if (_hostConfigDictionary != null)
                {
                    return _hostConfigDictionary;
                }

                _hostConfigDictionary = ReadHostConfigSettings(_configurationDocument.Root);

                return _hostConfigDictionary;
            }
        }

        #endregion

        #region Secs2DataItems

        private Dictionary<string, Secs2DataItem> _secs2DataItemDictionary;

        public Dictionary<string, Secs2DataItem> Secs2DataItemDictionary
        {
            get
            {
                if (_secs2DataItemDictionary != null)
                {
                    return _secs2DataItemDictionary;
                }

                lock (_configLock)
                {
                    _secs2DataItemDictionary = ReadSecs2DataItemsSettings(_configurationDocument.Root);
                }

                return _secs2DataItemDictionary;
            }
        }

        #endregion

        #region CE

        private Dictionary<string, CE> _ceDictionary;

        public Dictionary<string, CE> CeDictionary
        {
            get
            {
                if (_ceDictionary != null)
                {
                    return _ceDictionary;
                }

                _ceDictionary = ReadCeSettings(_configurationDocument.Root);

                return _ceDictionary;
            }
        }

        #endregion

        #region Alarms

        private Dictionary<string, Alarm> _alarmsDictionary;

        public Dictionary<string, Alarm> AlarmsDictionary
        {
            get
            {
                if (_alarmsDictionary != null)
                {
                    return _alarmsDictionary;
                }

                _alarmsDictionary = ReadAlarmsSettings(_configurationDocument.Root);

                return _alarmsDictionary;
            }
        }

        #endregion

        #region AllowedOutMessages

        private List<PipingMessage> _allowedOutMessages;

        public List<PipingMessage> AllowedOutMessages
        {
            get
            {
                if (_allowedOutMessages != null)
                {
                    return _allowedOutMessages;
                }

                var cpElement = _configurationDocument.Root?.Element(ConfigurationXNames.CommunicationPiping);
                if (cpElement != null)
                {
                    _allowedOutMessages = ReadMessagesSettings(cpElement.Element(ConfigurationXNames.CPIsAllowedOut));
                }

                return _allowedOutMessages;
            }
        }

        #endregion

        #region AllowedInMessages

        private List<PipingMessage> _allowedInMessages;

        public List<PipingMessage> AllowedInMessages
        {
            get
            {
                if (_allowedInMessages != null)
                {
                    return _allowedInMessages;
                }

                var cpElement = _configurationDocument.Root?.Element(ConfigurationXNames.CommunicationPiping);
                if (cpElement != null)
                {
                    _allowedInMessages = ReadMessagesSettings(cpElement.Element(ConfigurationXNames.CPIsAllowedIn));
                }

                return _allowedInMessages;
            }
        }

        #endregion

        #endregion

        private MessagesConfigurationManager()
        {
           
        }

        public static MessagesConfigurationManager Load(string messagesConfigPath)
        {
            MessagesConfigurationManager configManager = new();
            configManager.LoadConfiguration(messagesConfigPath);
            return configManager;
        }

        private void LoadConfiguration(string messagesConfigPath)
        {
            if (!File.Exists(messagesConfigPath))
            {
                throw new ArgumentException(
                    $"Configuration file '{messagesConfigPath}' does not exist",
                    nameof(messagesConfigPath));
            }

            lock (_configLock)
            {
                List<string> errors = new();

                // Get embedded MessagesConfiguration XSD
                var gemAssembly = Assembly.GetExecutingAssembly();
                using var schemaStream =
                    gemAssembly.GetManifestResourceStream("UnitySC.UTO.Controller.Configuration.XSD.MessagesConfiguration.xsd");

                if (schemaStream is null)
                {
                    throw new InvalidOperationException(
                        "Embedded MessagesConfiguration.xsd resources stream is null");
                }

                using var schemaReader = XmlReader.Create(schemaStream);
                var schema = XmlSchema.Read(schemaReader, null);

                // Configure XML reader settings with schema validation
                var settings = new XmlReaderSettings();
                settings.Schemas.Add(schema);
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationEventHandler += (_, args) => errors.Add(args.Message);
                settings.IgnoreWhitespace = true;

                // Load the MessagesConfiguration XML file
                using var stream = new FileStream(messagesConfigPath, FileMode.Open);
                using var reader = XmlReader.Create(stream, settings);
                _configurationDocument = XDocument.Load(reader, LoadOptions.None);

                if (errors.Count == 0 && _configurationDocument?.Root is null)
                {
                    errors.Add($"Configuration file '{messagesConfigPath}' is empty.");
                }

                if (errors.Count == 0)
                {
                    return;
                }

                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new InvalidOperationException(
                    $"Configuration file contains some errors : {errorMessage}");
            }
        }

        public void AppendConfiguration(string messagesConfigPath, int instanceId, string processModuleName)
        {
            lock (_configLock)
            {
                if (_configurationDocument.Root == null)
                {
                    throw new InvalidOperationException(
                        "No MessagesConfiguration loaded in memory. Please first call LoadConfiguration to load a first MessagesConfiguration before calling AppendConfiguration method.");
                }

                List<string> errors = new();

                // Get embedded MessagesConfiguration XSD
                var gemAssembly = Assembly.GetExecutingAssembly();
                using var schemaStream =
                    gemAssembly.GetManifestResourceStream("UnitySC.UTO.Controller.Configuration.XSD.MessagesConfiguration.xsd");

                if (schemaStream is null)
                {
                    throw new InvalidOperationException(
                        "Embedded MessagesConfiguration.xsd resources stream is null");
                }

                using var schemaReader = XmlReader.Create(schemaStream);
                var schema = XmlSchema.Read(schemaReader, null);

                // Configure XML reader settings with schema validation
                var settings = new XmlReaderSettings();
                settings.Schemas.Add(schema);
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationEventHandler += (_, args) => errors.Add(args.Message);
                settings.IgnoreWhitespace = true;

                // Load the MessagesConfiguration XML file
                using var stream = new FileStream(messagesConfigPath, FileMode.Open);
                using var reader = XmlReader.Create(stream, settings);
                var appendConfiguration = XDocument.Load(reader, LoadOptions.None);

                if (errors.Count == 0 && _configurationDocument?.Root is null)
                {
                    errors.Add($"Configuration file '{messagesConfigPath}' is empty.");
                }

                if (errors.Count == 0)
                {
                    AddVariables(appendConfiguration, instanceId, processModuleName);
                    AddCollectionEvents(appendConfiguration, instanceId, processModuleName);
                    AddReports(appendConfiguration, instanceId, processModuleName);
                    AddAlarm(appendConfiguration, instanceId, processModuleName);

                    return;
                }

                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new InvalidOperationException(
                    $"Configuration file contains some errors : {errorMessage}");
            }
        }

        #region Private
        private void AddVariables(XDocument configToAppend, int instanceId, string processModuleName)
        {
            var sourceVariablesNode = configToAppend.Root?.Element(ConfigurationXNames.Variables);
            if (sourceVariablesNode == null)
            {
                throw new InvalidOperationException(
                    $"Variables node cannot be found in source file.");
            }

            var variablesNode = _configurationDocument.Root?.Element(ConfigurationXNames.Variables);
            if (variablesNode == null)
            {
                throw new InvalidOperationException(
                    $"Variables node cannot be found in destination file.");
            }

            //Add new items if does not exist
            foreach (var element in sourceVariablesNode.Elements())
            {
                if (element.Name == ConfigurationXNames.SV)
                {
                    var lastSv = variablesNode.Elements().LastOrDefault(e => e.Name == ConfigurationXNames.SV);
                    lastSv?.AddAfterSelf(element);
                }
                else if (element.Name == ConfigurationXNames.DV)
                {
                    var lastDv = variablesNode.Elements().LastOrDefault(e => e.Name == ConfigurationXNames.DV);
                    lastDv?.AddAfterSelf(element);
                }
                else if (element.Name == ConfigurationXNames.EC)
                {
                    var lastEc = variablesNode.Elements().LastOrDefault(e => e.Name == ConfigurationXNames.EC);
                    lastEc?.AddAfterSelf(element);
                }
            }
        }

        private void AddCollectionEvents(XDocument configToAppend, int instanceId, string processModuleName)
        {
            var sourceCollectionEventsNode =
                configToAppend.Root?.Element(ConfigurationXNames.CollectionEvents);
            if (sourceCollectionEventsNode == null)
            {
                throw new InvalidOperationException(
                    $"CollectionEvents node cannot be found in source file.");
            }

            var collectionEventsNode =
                _configurationDocument.Root?.Element(ConfigurationXNames.CollectionEvents);
            if (collectionEventsNode == null)
            {
                throw new InvalidOperationException(
                    $"CollectionEvents node cannot be found in destination file.");
            }

            foreach (var element in sourceCollectionEventsNode.Elements())
            {
                collectionEventsNode.Add(element);
            }
        }

        private void AddReports(XDocument configToAppend, int instanceId, string processModuleName)
        {
            var sourceReportsNode = configToAppend.Root?.Element(ConfigurationXNames.Reports);
            if (sourceReportsNode == null)
            {
                throw new InvalidOperationException(
                    $"Reports node cannot be found in source file.");
            }

            var reportsNode = _configurationDocument.Root?.Element(ConfigurationXNames.Reports);
            if (reportsNode == null)
            {
                throw new InvalidOperationException(
                    $"Reports node cannot be found in destination file.");
            }

            foreach (var element in sourceReportsNode.Elements())
            {
                reportsNode.Add(element);
            }
        }

        private void AddAlarm(XDocument configToAppend, int instanceId, string processModuleName)
        {
            var sourceAlarmsNode = configToAppend.Root?.Element(ConfigurationXNames.Alarms);
            if (sourceAlarmsNode == null)
            {
                throw new InvalidOperationException($"Alarms node cannot be found in source file.");
            }

            var alarmsNode = _configurationDocument.Root?.Element(ConfigurationXNames.Alarms);
            if (alarmsNode == null)
            {
                throw new InvalidOperationException(
                    $"Alarms node cannot be found in destination file.");
            }

            foreach (var element in sourceAlarmsNode.Elements())
            {
                alarmsNode.Add(element);
            }
        }


        private Dictionary<string, Variable> ReadSVVariablesSettings(
            XContainer variablesElement)
        {
            var svDictionary = new Dictionary<string, Variable>();
            if (variablesElement is null)
            {
                return svDictionary;
            }

            foreach (var svElement in variablesElement.Elements(ConfigurationXNames.SV))
            {
                var vid = (string)svElement.Element(ConfigurationXNames.VID);
                var vname = (string)svElement.Element(ConfigurationXNames.VName);
                var units = (string)svElement.Element(ConfigurationXNames.VUnits);
                var type = (ItemType)Enum.Parse(
                    typeof(ItemType),
                    (string)svElement.Element(ConfigurationXNames.VType));
                var wellKnownName = (string)svElement.Element(ConfigurationXNames.VWellKnowName);

                svDictionary.Add(vid, new Variable()
                {
                    Vid = vid,
                    Name = vname,
                    Units = units,
                    Type = type,
                    WellKnownName = wellKnownName
                });
            }

            return svDictionary;
        }

        private Dictionary<string, Variable> ReadDVVariablesSettings(
            XContainer variablesElement)
        {
            var dvDictionary = new Dictionary<string, Variable>();
            if (variablesElement is null)
            {
                return dvDictionary;
            }

            foreach (var svElement in variablesElement.Elements(ConfigurationXNames.DV))
            {
                var vid = (string)svElement.Element(ConfigurationXNames.VID);
                var vname = (string)svElement.Element(ConfigurationXNames.VName);
                var units = (string)svElement.Element(ConfigurationXNames.VUnits);
                var type = (ItemType)Enum.Parse(
                    typeof(ItemType),
                    (string)svElement.Element(ConfigurationXNames.VType));
                var wellKnownName = (string)svElement.Element(ConfigurationXNames.VWellKnowName);

                dvDictionary.Add(vid, new Variable()
                {
                    Vid = vid,
                    Name = vname,
                    Units = units,
                    Type = type,
                    WellKnownName = wellKnownName
                });
            }

            return dvDictionary;
        }

        private Dictionary<string, EC> ReadECVariablesSettings(
            XContainer variablesElement)
        {
            var ecDictionary = new Dictionary<string, EC>();
            if (variablesElement is null)
            {
                return ecDictionary;
            }

            foreach (var ecElement in variablesElement.Elements(ConfigurationXNames.EC))
            {
                var vid = (string)ecElement.Element(ConfigurationXNames.VID);
                var vname = (string)ecElement.Element(ConfigurationXNames.VName);
                var units = (string)ecElement.Element(ConfigurationXNames.VUnits);
                var type = (ItemType)Enum.Parse(
                    typeof(ItemType),
                    (string)ecElement.Element(ConfigurationXNames.VType));
                var min = (string)ecElement.Element(ConfigurationXNames.ECMin);
                var max = (string)ecElement.Element(ConfigurationXNames.ECMax);
                var wellKnownName = (string)ecElement.Element(ConfigurationXNames.VWellKnowName);

                ecDictionary.Add(vid, new EC()
                {
                    Vid = vid,
                    Name = vname,
                    Units = units,
                    Type = type,
                    WellKnownName = wellKnownName,
                    Min = min,
                    Max = max
                });
            }

            return ecDictionary;
        }

        private Dictionary<string, EC> ReadHostConfigSettings(
            XContainer rootElement)
        {
            var hostConfigDictionary = new Dictionary<string, EC>();
            var hostConfigElement = rootElement?.Element(ConfigurationXNames.HostConfig);
            if (hostConfigElement is null)
            {
                return hostConfigDictionary;
            }

            foreach (var hcElement in hostConfigElement.Descendants(ConfigurationXNames.HC))
            {
                var vid = (string)hcElement.Element(ConfigurationXNames.VID);
                var name = (string)hcElement.Element(ConfigurationXNames.VName);
                var units = (string)hcElement.Element(ConfigurationXNames.VUnits);
                var type = (ItemType)Enum.Parse(
                    typeof(ItemType),
                    (string)hcElement.Element(ConfigurationXNames.VType));
                var min = (string)hcElement.Element(ConfigurationXNames.ECMin);
                var max = (string)hcElement.Element(ConfigurationXNames.ECMax);
                var wellKnownName = (string)hcElement.Element(ConfigurationXNames.VWellKnowName);
               

                hostConfigDictionary.Add(vid, new EC()
                {
                    Vid = vid,
                    Name = name,
                    Units = units,
                    Type = type,
                    Min = min,
                    Max = max,
                    WellKnownName = wellKnownName
                });
            }

            return hostConfigDictionary;
        }

        private Dictionary<string, Secs2DataItem> ReadSecs2DataItemsSettings(
            XContainer rootElement)
        {
            var secs2DataItemsSettings = new Dictionary<string, Secs2DataItem>();

            var secs2DataItemsElement = rootElement?.Element(ConfigurationXNames.Secs2DataItems);
            if (secs2DataItemsElement is null)
            {
                return secs2DataItemsSettings;
            }

            foreach (var secs2DataItemElement in secs2DataItemsElement.Elements(ConfigurationXNames.Secs2DataItem))
            {
                var dataItemName = (string)secs2DataItemElement.Element(ConfigurationXNames.Secs2DataItemName);
                var dataItemType = (ItemType)Enum.Parse(
                    typeof(ItemType),
                    (string)secs2DataItemElement.Element(ConfigurationXNames.Secs2DataItemType));

                secs2DataItemsSettings.Add(dataItemName, new Secs2DataItem()
                {
                    Name = dataItemName,
                    Type = dataItemType
                } );
            }

            return secs2DataItemsSettings;
        }


        private Dictionary<string, CE> ReadCeSettings(
            XContainer rootElement)
        {
            var ceDictionary = new Dictionary<string, CE>();

            var collectionEventsElement = rootElement?.Element(ConfigurationXNames.CollectionEvents);
            if (collectionEventsElement is null)
            {
                return ceDictionary;
            }

            foreach (var collectionEventElement in collectionEventsElement.Elements(
                         ConfigurationXNames.CollectionEvent))
            {
                var ceid = (string)collectionEventElement.Element(ConfigurationXNames.CEID);
                var ceidName = (string)collectionEventElement.Element(ConfigurationXNames.CEName);
                var wellKnownName =
                    (string)collectionEventElement.Element(ConfigurationXNames.VWellKnowName);


                ceDictionary.Add(ceid, new CE()
                {
                    Ceid = ceid,
                    Name = ceidName,
                    WellKnownName = wellKnownName
                });
            }

            return ceDictionary;
        }


        private Dictionary<string, Alarm> ReadAlarmsSettings(
            XContainer rootElement)
        {
            var alarmsSettings = new Dictionary<string, Alarm>();

            var alarmsElement = rootElement?.Element(ConfigurationXNames.Alarms);
            if (alarmsElement is null)
            {
                return alarmsSettings;
            }

            foreach (var alarmElement in alarmsElement.Elements(ConfigurationXNames.Alarm))
            {
                var alid = (string)alarmElement.Element(ConfigurationXNames.ALID);
                var altx = (string)alarmElement.Element(ConfigurationXNames.ALTX);
                var alname = (string)alarmElement.Element(ConfigurationXNames.ALName);
                var setCeid = (string)alarmElement.Element(ConfigurationXNames.ALSetCEID) ?? string.Empty;
                var clearCeid = (string)alarmElement.Element(ConfigurationXNames.ALClearCEID) ?? string.Empty;

               alarmsSettings.Add(alid,new Alarm()
               {
                   AlId = alid,
                   AlTxt = altx,
                   AlName = alname,
                   AlSetCeid = setCeid,
                   AlClearCeid = clearCeid
               });
            }

            return alarmsSettings;
        }

        private List<PipingMessage> ReadMessagesSettings(
            XContainer cpElement)
        {
            var messagesSettings = new List<PipingMessage>();

            var messagesElement = cpElement?.Element(ConfigurationXNames.CPMessages);
            if (messagesElement is null)
            {
                return messagesSettings;
            }

            foreach (var messageElement in messagesElement.Elements(ConfigurationXNames.CPMessage))
            {
                var stream = Byte.Parse(
                    (string)messageElement.Element(ConfigurationXNames.CPStream));
                var function = Byte.Parse(
                    (string)messageElement.Element(ConfigurationXNames.CPFunction));

                var controlStates = messageElement.Element(ConfigurationXNames.CPControlStates)
                                        ?.Elements(ConfigurationXNames.CPControlState)
                                        .Select(
                                            controlStateElement => (ControlSMStates)Enum.Parse(
                                                typeof(ControlSMStates),
                                                controlStateElement.Value))
                                        .Select(states => (ControlSMState)(int)states)
                                        .ToArray()
                                    ?? new ControlSMState[0];

                messagesSettings.Add( new PipingMessage()
                {
                    Stream = stream, Function = function, ControlState = controlStates.ToList()
                });
            }

            return messagesSettings;
        }

        #endregion

        public bool SaveSettings(string filePath)
        {
            try
            {
                using var writer = XmlWriter.Create(filePath, _writerSettings);
                lock (_configLock)
                {
                    _configurationDocument?.Save(writer);
                    
                }
                writer.Flush();
            }
            catch (XmlException)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            lock (_configLock)
            {
                return _configurationDocument == null ? string.Empty : _configurationDocument.ToString();
            }
        }

        public string CompareTo(MessagesConfigurationManager messagesConfigurationToCompare)
        {
            var sb = new StringBuilder();

            CompareHostConfig(sb, messagesConfigurationToCompare.HostConfigDictionary);
            CompareSecs2DataItems(sb,messagesConfigurationToCompare.Secs2DataItemDictionary);

            CompareSv(sb,messagesConfigurationToCompare.SvDictionary);
            CompareDv(sb,messagesConfigurationToCompare.DvDictionary);
            CompareEc(sb, messagesConfigurationToCompare.EcDictionary);
            CompareCe(sb, messagesConfigurationToCompare.CeDictionary);
            CompareAlarms(sb, messagesConfigurationToCompare.AlarmsDictionary);
            ComparePipingMessages(sb, AllowedOutMessages ,messagesConfigurationToCompare.AllowedOutMessages, true);
            ComparePipingMessages(sb, AllowedInMessages ,messagesConfigurationToCompare.AllowedInMessages, false);

            return sb.ToString();
        }

        private void CompareSv(StringBuilder sb, Dictionary<string, Variable> dictionaryToCompare)
        {
            CompareVariable(sb, SvDictionary, dictionaryToCompare);
        }

        private void CompareDv(StringBuilder sb, Dictionary<string, Variable> dictionaryToCompare)
        {
            CompareVariable(sb, DvDictionary, dictionaryToCompare);
        }

        private void CompareCe(StringBuilder sb, Dictionary<string, CE> dictionaryToCompare)
        {
            foreach (var variable in CeDictionary)
            {
                if (dictionaryToCompare.ContainsKey(variable.Key))
                {
                    dictionaryToCompare.TryGetValue(variable.Key, out CE valueToCompare);

                    if (valueToCompare == null)
                    {
                        continue;
                    }

                    if (variable.Value.Name != valueToCompare.Name)
                    {
                        sb.AppendLine($"CEID {variable.Key} => name issue: {variable.Value.Name} / {valueToCompare.Name}");
                    }

                    if (variable.Value.WellKnownName != valueToCompare.WellKnownName)
                    {
                        sb.AppendLine($"CEID {variable.Key},{variable.Value.Name} => WellKnownName issue: {variable.Value.WellKnownName} / {valueToCompare.WellKnownName}");
                    }

                }
                else
                {
                    sb.AppendLine($"CEID {variable.Key},{variable.Value.Name} is not present");
                }
            }
        }

        private void CompareHostConfig(StringBuilder sb, Dictionary<string, EC> dictionaryToCompare)
        {
            foreach (var variable in HostConfigDictionary)
            {
                if (dictionaryToCompare.ContainsKey(variable.Key))
                {
                    dictionaryToCompare.TryGetValue(variable.Key, out EC valueToCompare);

                    if (valueToCompare == null)
                    {
                        continue;
                    }

                    if (variable.Value.Name != valueToCompare.Name)
                    {
                        sb.AppendLine($"HC {variable.Key} => name issue: {variable.Value.Name} / {valueToCompare.Name}");
                    }

                    if (variable.Value.Units != valueToCompare.Units)
                    {
                        sb.AppendLine($"HC {variable.Key},{variable.Value.Name} => Units issue: {variable.Value.Units} / {valueToCompare.Units}");
                    }

                    if (variable.Value.WellKnownName != valueToCompare.WellKnownName)
                    {
                        sb.AppendLine($"HC {variable.Key},{variable.Value.Name} => WellKnownName issue: {variable.Value.WellKnownName} / {valueToCompare.WellKnownName}");
                    }

                    if (variable.Value.Type != valueToCompare.Type)
                    {
                        sb.AppendLine($"HC {variable.Key},{variable.Value.Name} => Type issue: {variable.Value.Type} / {valueToCompare.Type}");
                    }

                    if (variable.Value.Min != valueToCompare.Min)
                    {
                        sb.AppendLine($"HC {variable.Key},{variable.Value.Name} => Min issue: {variable.Value.Min} / {valueToCompare.Min}");
                    }

                    if (variable.Value.Max != valueToCompare.Max)
                    {
                        sb.AppendLine($"HC {variable.Key},{variable.Value.Name} => Max issue: {variable.Value.Max} / {valueToCompare.Max}");
                    }
                }
                else
                {
                    sb.AppendLine($"HC {variable.Key},{variable.Value.Name} is not present");
                }
            }
        }

        private void CompareEc(StringBuilder sb, Dictionary<string, EC> dictionaryToCompare)
        {
            foreach (var variable in EcDictionary)
            {
                if (dictionaryToCompare.ContainsKey(variable.Key))
                {
                    dictionaryToCompare.TryGetValue(variable.Key, out EC valueToCompare);

                    if (valueToCompare == null)
                    {
                        continue;
                    }

                    if (variable.Value.Name != valueToCompare.Name)
                    {
                        sb.AppendLine($"ECID {variable.Key} => name issue: {variable.Value.Name} / {valueToCompare.Name}");
                    }

                    if (variable.Value.Units != valueToCompare.Units)
                    {
                        sb.AppendLine($"ECID {variable.Key},{variable.Value.Name} => Units issue: {variable.Value.Units} / {valueToCompare.Units}");
                    }

                    if (variable.Value.WellKnownName != valueToCompare.WellKnownName)
                    {
                        sb.AppendLine($"ECID {variable.Key},{variable.Value.Name} => WellKnownName issue: {variable.Value.WellKnownName} / {valueToCompare.WellKnownName}");
                    }

                    if (variable.Value.Type != valueToCompare.Type)
                    {
                        sb.AppendLine($"ECID {variable.Key},{variable.Value.Name} => Type issue: {variable.Value.Type} / {valueToCompare.Type}");
                    }

                    if (variable.Value.Min != valueToCompare.Min)
                    {
                        sb.AppendLine($"ECID {variable.Key},{variable.Value.Name} => Min issue: {variable.Value.Min} / {valueToCompare.Min}");
                    }

                    if (variable.Value.Max != valueToCompare.Max)
                    {
                        sb.AppendLine($"ECID {variable.Key},{variable.Value.Name} => Max issue: {variable.Value.Max} / {valueToCompare.Max}");
                    }
                }
                else
                {
                    sb.AppendLine($"ECID {variable.Key},{variable.Value.Name} is not present");
                }
            }
        }

        private void CompareSecs2DataItems(StringBuilder sb, Dictionary<string, Secs2DataItem> dictionaryToCompare)
        {
            foreach (var variable in Secs2DataItemDictionary)
            {
                if (dictionaryToCompare.ContainsKey(variable.Key))
                {
                    dictionaryToCompare.TryGetValue(variable.Key, out Secs2DataItem valueToCompare);

                    if (valueToCompare == null)
                    {
                        continue;
                    }

                    if (variable.Value.Name != valueToCompare.Name)
                    {
                        sb.AppendLine($"Secs2DataItem {variable.Key} => name issue: {variable.Value.Name} / {valueToCompare.Name}");
                    }

                    if (variable.Value.Type != valueToCompare.Type)
                    {
                        sb.AppendLine($"Secs2DataItem {variable.Key} => type issue: {variable.Value.Type} / {valueToCompare.Type}");
                    }
                }
                else
                {
                    sb.AppendLine($"Secs2DataItem {variable.Key}, is not present");
                }
            }
        }

        private void CompareAlarms(StringBuilder sb, Dictionary<string, Alarm> dictionaryToCompare)
        {
            foreach (var variable in AlarmsDictionary)
            {
                if (dictionaryToCompare.ContainsKey(variable.Key))
                {
                    dictionaryToCompare.TryGetValue(variable.Key, out Alarm valueToCompare);

                    if (valueToCompare == null)
                    {
                        continue;
                    }

                    if (variable.Value.AlName != valueToCompare.AlName)
                    {
                        sb.AppendLine($"Alarm {variable.Key} => name issue: {variable.Value.AlName} / {valueToCompare.AlName}");
                    }

                    if (variable.Value.AlTxt != valueToCompare.AlTxt)
                    {
                        sb.AppendLine($"Alarm {variable.Key},{variable.Value.AlName} => text issue: {variable.Value.AlTxt} / {valueToCompare.AlTxt}");
                    }

                    if (variable.Value.AlSetCeid != valueToCompare.AlSetCeid)
                    {
                        sb.AppendLine($"Alarm {variable.Key},{variable.Value.AlName} => set ceid issue: {variable.Value.AlSetCeid} / {valueToCompare.AlSetCeid}");
                    }

                    if (variable.Value.AlClearCeid != valueToCompare.AlClearCeid)
                    {
                        sb.AppendLine($"Alarm {variable.Key},{variable.Value.AlName} => clear ceid issue: {variable.Value.AlClearCeid} / {valueToCompare.AlClearCeid}");
                    }
                }
                else
                {
                    sb.AppendLine($"Alarm {variable.Key},{variable.Value.AlName} is not present");
                }
            }
        }

        private void ComparePipingMessages(StringBuilder sb, List<PipingMessage> messagesSource, List<PipingMessage> messagesToCompare, bool isOut)
        {
            foreach (var variable in messagesSource)
            {
                if (messagesToCompare.FirstOrDefault(m=>m.Stream == variable.Stream && m.Function == variable.Function) is {} message)
                {
                    if (variable.ControlState.Count != message.ControlState.Count)
                    {
                        sb.AppendLine($"Message {variable.Stream},{variable.Function} => configuration issue: {variable.ControlState} / {message.ControlState}");
                    }
                }
                else
                {
                    var section = isOut
                        ? "IsAllowedOut"
                        : "IsAllowedIn";
                    sb.AppendLine($"Message {variable.Stream},{variable.Function} is not present in {section} section");
                }
            }
        }

        private void CompareVariable(StringBuilder sb, Dictionary<string, Variable> sourceDictionary, Dictionary<string, Variable> dictionaryToCompare)
        {
            foreach (var variable in sourceDictionary)
            {
                if (dictionaryToCompare.ContainsKey(variable.Key))
                {
                    dictionaryToCompare.TryGetValue(
                        variable.Key,
                        out Variable valueToCompare);

                    if (valueToCompare == null)
                    {
                        continue;
                    }

                    if (variable.Value.Name != valueToCompare.Name)
                    {
                        sb.AppendLine($"VID {variable.Key} => name issue: {variable.Value.Name} / {valueToCompare.Name}");
                    }

                    if (variable.Value.Units != valueToCompare.Units)
                    {
                        sb.AppendLine($"VID {variable.Key},{variable.Value.Name} => Units issue: {variable.Value.Units} / {valueToCompare.Units}");
                    }

                    if (variable.Value.WellKnownName != valueToCompare.WellKnownName)
                    {
                        sb.AppendLine($"VID {variable.Key},{variable.Value.Name} => WellKnownName issue: {variable.Value.WellKnownName} / {valueToCompare.WellKnownName}");
                    }

                    if (variable.Value.Type != valueToCompare.Type)
                    {
                        sb.AppendLine($"VID {variable.Key},{variable.Value.Name} => Type issue: {variable.Value.Type} / {valueToCompare.Type}");
                    }
                }
                else
                {
                    sb.AppendLine($"VID {variable.Key},{variable.Value.Name} is not present");
                }
            }
        }
    }

    internal class Variable
    {
        public string Vid { get; set; }
        public string Name { get; set; }
        public string Units { get; set; }
        public ItemType Type { get; set; }
        public string WellKnownName { get; set; }

    }

    internal class EC : Variable
    {
        public string Min { get; set; }
        public string Max { get; set; }
    }

    internal class Secs2DataItem
    {
        public string Name { get; set; }
        public ItemType Type { get; set; }
    }

    internal class CE
    {
        public string Ceid { get; set; }
        public string Name { get; set; }
        public string WellKnownName { get; set; }
    }

    internal class Alarm
    {
        public string AlId { get; set; }
        public string AlTxt { get; set; }
        public string AlName { get; set; }
        public string AlSetCeid { get; set; }
        public string AlClearCeid { get; set; }
    }

    internal class PipingMessage
    {
        public byte Stream { get; set; }
        public byte Function { get; set; }
        public List<ControlSMState> ControlState { get; set; }
    }
}
