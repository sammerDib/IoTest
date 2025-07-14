using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using UnitySC.PM.Shared.Hardware.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;

using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public interface IOpcMultiParams
    {
        Variant[] ToVariantArray();
    }

    public delegate void DeliverMessagesDelegate(string msgName, object value);

    public class OpcController
    {
        /**  SetMethodValueAsync :
         *   c# short == uint PLC
         *   Où regarder : sur le client Twincat OPC UA sample, par exemple pour SetPosition->InputArguments de FW212CZttenuationFilter, position = 'i=5'
         *   Dans Workstation.ServiceModel.Ua.DataTypeIds, 'i=5' => UInt16 -> ushort en c#
         **/

#if DEBUG_OPC
        public const bool DebugOpc = true;
#else
        public const bool DebugOpc = false;
#endif
        private const double NOTIFICATION_INTERVAL = 20;
        private ILogger _logger;

        private OpcControllerConfig _opcControllerConfig;
        private DeliverMessagesDelegate _deliverMessagesDelegate;

        private string _endPointUrl;
        private string _rootNodeId;
        private string _deviceId;
        private bool _ioEnable;
        private object _lock = new object();

        public Dictionary<uint, DataAttribute> HandleDataAttributes { get; set; } = new Dictionary<uint, DataAttribute>();

        public ClientSessionChannel Channel { get; set; }
        public List<MonitoredItemCreateRequest> MonitoredItems = new List<MonitoredItemCreateRequest>();

        private CreateSubscriptionResponse SubscriptionResponse { get; set; }
        private uint _subscriptionId;
        private IDisposable _token { get; set; }

        private System.Timers.Timer _controlSubscriptionStillAlive;
        private Stopwatch _isAlive_sw = new Stopwatch();
        private const int Timeout_ms = 30_000;

        private int _currentMeterSubscription;
        public int NewMeterSubscription { get; set; } = -1;

        private CreateSubscriptionRequest _subscriptionRequest = new CreateSubscriptionRequest
        {
#if DEBUG
            RequestedPublishingInterval = 100,
            RequestedMaxKeepAliveCount = 10000,
            RequestedLifetimeCount = 10000 * 3,
#else
            RequestedPublishingInterval = NOTIFICATION_INTERVAL,
            RequestedMaxKeepAliveCount = 30,
            RequestedLifetimeCount = 30 * 3,
#endif
            PublishingEnabled = true,
        };

        public OpcDevice OpcDevice { get; set; } = new OpcDevice();

        public OpcController(OpcControllerConfig opcControllerConfig, ILogger logger, DeliverMessagesDelegate deliverMessagesDelegate)
        {
            Init(opcControllerConfig, logger, deliverMessagesDelegate);
        }

        public OpcController(OpcControllerConfig opcControllerConfig, ILogger logger, DeliverMessagesDelegate deliverMessagesDelegate, bool ioEnable)
        {
            Init(opcControllerConfig, logger, deliverMessagesDelegate);
            _ioEnable = ioEnable;
        }

        private void Init(OpcControllerConfig opcControllerConfig, ILogger logger, DeliverMessagesDelegate deliverMessagesDelegate)
        {
            var useLocalAddresses = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().UseLocalAddresses;
            _logger = logger;
            _opcControllerConfig = opcControllerConfig;
            _deliverMessagesDelegate = deliverMessagesDelegate;

            string hostname;
            if (useLocalAddresses)
            {
                _logger.Warning("Localhost mode activated");
                hostname = "localhost";
            }
            else
            {
                hostname = opcControllerConfig.OpcCom.Hostname;
            }

            _endPointUrl = "opc.tcp://" + hostname + ":" + opcControllerConfig.OpcCom.Port;
            _rootNodeId = opcControllerConfig.OpcCom.RootNodeId;
            _deviceId = opcControllerConfig.OpcCom.DeviceNodeID;
        }

        public void Init(List<Message> errorList)
        {
            try
            {
                Connect();

                _controlSubscriptionStillAlive = new System.Timers.Timer(500);
                _controlSubscriptionStillAlive.Elapsed += new ElapsedEventHandler(RefreshSubscriptionTimer_Elapsed);
                _controlSubscriptionStillAlive.AutoReset = false;
                _controlSubscriptionStillAlive.Start();
            }
            catch (Exception Ex)
            {
                errorList.Add(new Message(MessageLevel.Error, "Connection failed : " + Ex.Message, _deviceId));
            }
        }

        public void Connect()
        {
            try
            {
                Task.Run(() => ConnnectToEndpointsWithNoSecurityAndWithNoCertificateAsync()).GetAwaiter().GetResult();
                Task.Run(() => BrowseDeviceNodeAsync()).GetAwaiter().GetResult();
                Task.Run(() => SubscriptionAsync()).GetAwaiter().GetResult();

                _logger.Information($"'{_opcControllerConfig.Name}' OPC communication with client is started");
            }
            catch (AggregateException aex)
            {
                _logger.Error($"OPC Open connection : {aex.Flatten().Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(FormatMessage($"The OPC connection opened by '{_opcControllerConfig.Name}' failed - Exception: " + ex.Message));
                throw;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_controlSubscriptionStillAlive != null)
                {
                    _controlSubscriptionStillAlive.Stop();
                    _controlSubscriptionStillAlive.Close();
                    _controlSubscriptionStillAlive = null;
                }
                Task.Run(() =>
                {
                    Channel.CloseAsync();
                    _logger.Information("Device PLC closed");
                }).GetAwaiter().GetResult();

                Channel = null;
            }
            catch (Exception ex)
            {
                _logger.Error(FormatMessage("The disconnection between OPC and PLC has failed + Exception: " + ex.Message));
            }
            finally
            {
                _controlSubscriptionStillAlive?.Start();
            }
        }

        public bool ResetController()
        {
            bool success = false;
            try
            {
                HandleDataAttributes.Clear();
                MonitoredItems.Clear();
                OpcDevice.Attributes.Clear();

                Disconnect();
                Connect();

                var stateIsOpend = Channel.State;
                if (stateIsOpend != CommunicationState.Opened)
                {
                    success = SpinWait.SpinUntil(() =>
                    {
                        stateIsOpend = Channel.State;
                        return stateIsOpend == CommunicationState.Opened;
                    }
                    , 60000);
                }

                if (stateIsOpend != CommunicationState.Opened)
                {
                    _logger?.Error(FormatMessage("Reset Failed : unable ."));
                    return false;
                }
            }
            catch (ServiceResultException sre)
            {
                _logger.Error($"OPC Open connection : {sre.Message} {sre.HResult}");
                throw;
            }
            catch (AggregateException aex)
            {
                _logger.Error($"OPC Open connection : {aex.Flatten().Message}");
                throw;
            }
            catch (Exception Ex)
            {
                _logger?.Error(FormatMessage("Reset OPC - Exception: " + Ex.Message));
                throw;
            }
            return true;
        }

        private async Task ConnnectToEndpointsWithNoSecurityAndWithNoCertificateAsync()
        {
            // describe this client application.
            var clientDescription = new ApplicationDescription
            {
                ApplicationName = "USP OPC Client",
                ApplicationType = ApplicationType.Client
            };

            ClientSessionChannelOptions clientSessionChannelOptions = new ClientSessionChannelOptions();
#if DEBUG
            clientSessionChannelOptions.SessionTimeout = 600000.0;
#else
            clientSessionChannelOptions.SessionTimeout = 240000.0;
#endif

            // create a 'ClientSessionChannel', a client-side channel that opens a 'session' with the server.
            Channel = new ClientSessionChannel(
                clientDescription,
                null, // no x509 certificates
                new AnonymousIdentity(), // no user identity
                _endPointUrl, // the public endpoint of a server
                SecurityPolicyUris.None,
                null,
                clientSessionChannelOptions);

            try
            {
                // try opening a session and reading a few nodes.
                await Channel.OpenAsync();
            }
            catch (AggregateException aex)
            {
                _logger.Error($"OPC Open connection : {aex.Flatten().Message}");
                throw;
            }
            catch (Exception ex)
            {
                await Channel.AbortAsync();
                Console.WriteLine(ex.Message);
            }
        }

        private async Task BrowseDeviceNodeAsync()
        {
            try
            {
                BrowseRequest browseRequest = BrowseRequestAsync(ExpandedNodeId.Parse(ObjectIds.RootFolder));
                BrowseResponse browseResponse = await Channel.BrowseAsync(browseRequest).ConfigureAwait(false);

                // Browse the root to the NodeId
                foreach (String node in _rootNodeId.Split('/'))
                {
                    browseResponse = await BrowseResultsAsync(node, browseResponse.Results[0].References).ConfigureAwait(false);
                }

                // Browse device
                uint handle = 0;
                foreach (var device in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                {
                    if (device.DisplayName == _deviceId)
                    {
                        OpcDevice.DeviceName = device.DisplayName;
                        OpcDevice.DeviceIdentifier = device.NodeId.ToString();
                        OpcDevice.Attributes = new Dictionary<string, List<OpcAttribute>>();

                        browseResponse = await BrowseResultsAsync(device).ConfigureAwait(false); ;

                        // Add attribute
                        foreach (var attribute in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                        {
                            browseResponse = await BrowseResultsAsync(attribute).ConfigureAwait(false);

                            List<OpcAttribute> attributeValues = new List<OpcAttribute>();
                            foreach (var item in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                            {
                                if (attribute.NodeClass.ToString() != NodeClass.Method.ToString())
                                {
                                    attributeValues.Add(new OpcAttribute
                                    {
                                        DisplayName = item.DisplayName,
                                        Identifier = item.NodeId.ToString()
                                    });
                                }

                                // Monitoring only variables
                                if (attribute.NodeClass.ToString() == NodeClass.Object.ToString() &
                                    item.NodeClass.ToString() == NodeClass.Variable.ToString())
                                {
                                    // Add items to the subscription
                                    MonitoredItems.Add(CreateMonitorItem(handle, item.NodeId.ToString()));

                                    if (_ioEnable)
                                    {
                                        ReadResponse readResponse;

                                        // Handle list to display items
                                        HandleDataAttributes.Add(handle, new DataAttribute(item.DisplayName, AttributeType.DigitalIO, item.NodeId.ToString(), _deviceId));

                                        // Update value
                                        readResponse = await ReadResultAsync(item.NodeId.ToString()).ConfigureAwait(false);
                                        HandleDataAttributes[handle].DigitalValue = (bool)readResponse.Results[0].Value;
#if DEBUG_OPC
                                        Console.WriteLine("{0}, {1}", item.DisplayName, HandleDataAttributes[handle].Value);
#endif
                                    }

                                    handle++;
                                }
                            }

                            if (attributeValues.IsEmpty())
                            {
                                attributeValues.Add(new OpcAttribute
                                {
                                    DisplayName = attribute.DisplayName,
                                    Identifier = attribute.NodeId.ToString()
                                });

                                // Monitoring only variables
                                if (attribute.NodeClass.ToString() == NodeClass.Variable.ToString())
                                {
                                    // Add items to the subscription
                                    MonitoredItems.Add(CreateMonitorItem(handle, attribute.NodeId.ToString()));

                                    if (device.DisplayName == _deviceId)
                                    {
                                        ReadResponse readResponse;

                                        // Handle list to display attributes
                                        HandleDataAttributes.Add(handle, new DataAttribute(attribute.DisplayName, AttributeType.Other, attribute.NodeId.ToString(), _deviceId));

                                        // Update value
                                        readResponse = await ReadResultAsync(attribute.NodeId.ToString()).ConfigureAwait(false);
                                        HandleDataAttributes[handle].Value = readResponse.Results[0].Value;
#if DEBUG_OPC
                                        Console.WriteLine("{0}, {1}", attribute.DisplayName, HandleDataAttributes[handle].Value);
#endif
                                    }
                                    handle++;
                                }
                            }

                            OpcDevice.Attributes.Add(attribute.DisplayName, attributeValues);
                        }
                    }
                }
            }
            catch (ServiceResultException ex)
            {
                _logger.Error(ex, "Browse PMs OPC Server failed");
            }
        }

        private async Task SubscriptionAsync()
        {
            _logger.Verbose("Create subscription");

            SubscriptionResponse = await Channel.CreateSubscriptionAsync(_subscriptionRequest).ConfigureAwait(false);
            _subscriptionId = SubscriptionResponse.SubscriptionId;
            var itemsToCreate = MonitoredItems.ToArray();

            var itemsRequest = new CreateMonitoredItemsRequest
            {
                SubscriptionId = _subscriptionId,
                ItemsToCreate = itemsToCreate,
            };

            await Channel.CreateMonitoredItemsAsync(itemsRequest).ConfigureAwait(false);

            // Subscribe to PublishResponse stream
            _token = Channel.Where(pr => pr.SubscriptionId == _subscriptionId).Subscribe(pr =>
            {
                // loop through all the data change notifications
                var dcns = pr.NotificationMessage.NotificationData.OfType<DataChangeNotification>();

                foreach (var dcn in dcns)
                {
                    foreach (var min in dcn.MonitoredItems)
                    {
                        if (min.Value.Value != null)
                        {
                            HandleDataAttributes.TryGetValue(min.ClientHandle, out var dataAttribute);
                            if (dataAttribute != null)
                            {
                                dataAttribute.Value = min.Value.Value;
                                _deliverMessagesDelegate(dataAttribute.Name, dataAttribute.Value);
#if DEBUG_OPC
                                Console.WriteLine("     {0}, {1}", dataAttribute.Identifier, dataAttribute.Value);
#endif
                            }
                        }
                    }
                }
            });

            _logger.Verbose(FormatMessage("The subscription has been registered"));
        }

        private async Task UnsubscribeAsync()
        {
            var request = new DeleteSubscriptionsRequest
            {
                SubscriptionIds = new uint[] { _subscriptionId }
            };
            await Channel.DeleteSubscriptionsAsync(request);
            _token.Dispose();
        }

        public async void ReconnectSubscription()
        {
            await UnsubscribeAsync();
            await SubscriptionAsync();
        }

        private async Task<BrowseResponse> BrowseResultsAsync(ReferenceDescription rd)
        {
            BrowseRequest browseRequest = BrowseRequestAsync(rd.NodeId);
            return await Channel.BrowseAsync(browseRequest).ConfigureAwait(false);
        }

        private async Task<BrowseResponse> BrowseResultsAsync(String node, ReferenceDescription[] rds)
        {
            BrowseResponse browseResponse = null;
            foreach (var rd in rds ?? new ReferenceDescription[0])
            {
                if (rd.DisplayName.Text == node ||
                    node == null)
                {
                    BrowseRequest browseRequest = BrowseRequestAsync(rd.NodeId);
                    browseResponse = await Channel.BrowseAsync(browseRequest).ConfigureAwait(false);
                }
            }

            return browseResponse;
        }

        private BrowseRequest BrowseRequestAsync(ExpandedNodeId expandedNodeId)
        {
            BrowseRequest browseRequest = new BrowseRequest
            {
                NodesToBrowse = new BrowseDescription[]
                {
                    new BrowseDescription
                    {
                        NodeId = ExpandedNodeId.ToNodeId(expandedNodeId, Channel.NamespaceUris),
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = NodeId.Parse(ReferenceTypeIds.HierarchicalReferences),
                        NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                        IncludeSubtypes = true,
                        ResultMask = (uint)BrowseResultMask.All
                    }
                },
            };

            return browseRequest;
        }

        private async Task<ReadResponse> ReadResultAsync(string nodeId)
        {
            ReadRequest readRequest = new ReadRequest
            {
                NodesToRead = new[]
                {
                    new ReadValueId
                    {
                        NodeId = NodeId.Parse(nodeId),
                        AttributeId = AttributeIds.Value
                    }
                }
            };

            return await Channel.ReadAsync(readRequest).ConfigureAwait(false);
        }

        private MonitoredItemCreateRequest CreateMonitorItem(uint handle, string identifier)
        {
            MonitoredItemCreateRequest mICR = new MonitoredItemCreateRequest
            {
                ItemToMonitor = new ReadValueId
                {
                    NodeId = NodeId.Parse(identifier),
                    AttributeId = AttributeIds.Value,
                },
                MonitoringMode = MonitoringMode.Reporting,
                RequestedParameters = new MonitoringParameters
                {
                    ClientHandle = handle,
                    SamplingInterval = -1,
                    QueueSize = 0,
                    DiscardOldest = true
                }
            };

            return mICR;
        }

        #region Commands

        public async Task<object> MessageReceivedAsync(string name)
        {
            if (Channel == null)
            {
                return false;
            }

            var response = await Channel.CallAsync(new CallRequest
            {
                MethodsToCall = new[]
                {
                    new CallMethodRequest
                    {
                        ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                        MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                        InputArguments = new Variant[] { new Variant(true)}
                    }
                }
            });

            await Task.Delay(500).ConfigureAwait(false);
            ReadResponse readResponse = await Channel.ReadAsync(new ReadRequest
            {
                NodesToRead = new[]
                {
                    new ReadValueId
                    {
                        NodeId = NodeId.Parse(OpcDevice.DeviceIdentifier + ".StreamingMessage"),
                        AttributeId = AttributeIds.Value
                    }
                }
            });

            return readResponse.Results[0].GetValue();
        }

        public async Task<object> StreamingMessageAsync(string name)
        {
            if (Channel == null)
            {
                return false;
            }

            var response = await Channel.CallAsync(new CallRequest
            {
                MethodsToCall = new[]
                 {
                     new CallMethodRequest
                     {
                         ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                         MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                         InputArguments = new Variant[] { new Variant(true)}
                     }
                 }
            });

            Thread.Sleep(500);
            ReadResponse readResponse = await Channel.ReadAsync(new ReadRequest
            {
                NodesToRead = new[]
                {
                    new ReadValueId
                    {
                        NodeId = NodeId.Parse(OpcDevice.DeviceIdentifier + ".MessageReceived"),
                        AttributeId = AttributeIds.Value
                    }
                }
            });

            return readResponse.Results[0].GetValue();
        }

        public async Task SetMethodValueAsync(string name)
        {
            try
            {
                if (Channel != null && OpcDevice.DeviceIdentifier != null)
                {
                    var response = await Channel.CallAsync(new CallRequest
                    {
                        MethodsToCall = new[]
                        {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true) }
                        }
                    }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async void SetMethodValueAsync(string name, ushort value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(value) }
                        }
                    }
                });
            }
        }

        public async Task SetMethodValueAsync(string name, int value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(value) }
                        }
                    }
                });
            }
        }

        public async void SetMethodValueAsync(string name, double value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(value) }
                        }
                    }
                });
            }
        }

        public async void SetMethodValueAsync(string name, WaveplateAnglesPolarisation arg, double value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(arg), new Variant(value) }
                        }
                    }
                });
            }
        }

        public async void SetMethodValueAsync(string name, string value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(value) }
                        }
                    }
                });
            }
        }

        public async Task SetMethodValueAsync(string name, bool value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(value) }
                        }
                    }
                });
            }
        }

        public async void SetMethodValueAsync(string name, string arg, bool value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(arg), new Variant(value) }
                        }
                    }
                });
            }
        }

        public async void SetMethodValueAsync(string name, string arg, byte value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(arg), new Variant(value) }
                        }
                    }
                });
            }
        }

        public async Task SetMethodValueAsync(string name, IOpcMultiParams paramsValue)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = paramsValue.ToVariantArray()
                        }
                    }
                });
            }
        }

        public async void SetMethodValueAsync(string name, string arg, float value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(arg), new Variant(value) }
                        }
                    }
                });
            }
        }

        public async void SetMethodValueAsync(string name, string arg, double value)
        {
            if (Channel != null && OpcDevice.DeviceIdentifier != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant(arg), new Variant(value) }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="identifier">Full path OPC</param>
        /// <param name="value"></param>
        public async void SetAttributeValue(string identifier, bool value)
        {
            if (Channel == null)
            {
                return;
            }

            WriteRequest writeRequest = new WriteRequest
            {
                NodesToWrite = new[]
                {
                     new WriteValue
                     {
                         NodeId = NodeId.Parse(identifier),
                         AttributeId = AttributeIds.Value,
                         IndexRange = null,
                         Value = new DataValue(value)
                     }
                 }
            };

            var writeResponse = await Channel.WriteAsync(writeRequest).ConfigureAwait(false);
        }

        public async Task<bool> GetAttributeIoValue(string identifier, string name)
        {
            if (Channel == null)
            {
                return false;
            }

            ReadResponse readResponse = await Channel.ReadAsync(new ReadRequest
            {
                NodesToRead = new[]
               {
                    new ReadValueId
                    {
                        NodeId = NodeId.Parse(identifier),
                        AttributeId = AttributeIds.Value
                    }
                }
            });

            return (bool)readResponse.Results[0].GetValue();
        }

        #endregion Commands

        private string FormatMessage(string message)
        {
            return ($"[{_deviceId}]{message}").Replace('\r', ' ').Replace('\n', ' ');
        }

        private void RefreshSubscriptionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (NewMeterSubscription > -1)
            {
                try
                {
                    if (_currentMeterSubscription != NewMeterSubscription)
                    {
                        _currentMeterSubscription = NewMeterSubscription;
                        _isAlive_sw.Restart();
                    }

                    if (_currentMeterSubscription == NewMeterSubscription && _isAlive_sw.ElapsedMilliseconds > Timeout_ms)
                    {
                        ReconnectSubscription();
                        _logger.Debug(FormatMessage($"Reconnect subscription to '{_opcControllerConfig.Name}'"));
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(FormatMessage($"RefreshSubscriptionTimer - '{_opcControllerConfig.Name}': {ex.Message}"));
                }
                finally
                {
                    _controlSubscriptionStillAlive.Start();
                }
            }
        }
    }
}
