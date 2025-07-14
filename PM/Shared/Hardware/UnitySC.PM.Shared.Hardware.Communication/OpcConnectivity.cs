using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Common;

//using TcEventLoggerAdsProxyLib;
//using Serilog.Events;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;

namespace UnitySC.PM.Shared.Hardware.Communication
{
    public class OpcConnectivity
    {
        /**  SetMethodValueAsync :
         *   c# short == uint PLC
         *   Où regarder : sur le client Twincat OPC UA sample, par exemple pour SetPosition->InputArguments de FW212CZttenuationFilter, position = 'i=5'
         *   Dans Workstation.ServiceModel.Ua.DataTypeIds, 'i=5' => UInt16 -> ushort en c#
         **/        

        private ILogger _logger;

        private string _endPointUrl;
        private string _rootNodeId;
        private string _deviceId;
        private bool _ioEnable;

        public Dictionary<uint, DataAttribute> HandleDataAttributes { get; set; } = new Dictionary<uint, DataAttribute>();

        //private TcEventLogger _tcEventlogger;
        private const int LangId = 1031;

        public UaTcpSessionChannel Channel { get; set; }
        public List<MonitoredItemCreateRequest> MonitoredItems = new List<MonitoredItemCreateRequest>();
        public OpcDevice OpcDevice { get; set; } = new OpcDevice();

        public string State { get; set; }

        public OpcConnectivity(ILogger logger, OpcCom opcCom, OpcGatewayControllerConfig gatewayControllerConfig)
        {
            _logger = logger;
            _endPointUrl = "opc.tcp://" + opcCom.Hostname + ":" + opcCom.Port;
            _rootNodeId = opcCom.RootNodeId;
            _deviceId = gatewayControllerConfig.OpcSettings.DeviceNodeID;

            _logger.Information($"Init '{gatewayControllerConfig.Name}' OPC communication client");
            Connect();
        }

        public OpcConnectivity(ILogger logger, OpcCom opcCom, OpcGatewayControllerConfig gatewayControllerConfig, bool ioEnable)
        {
            _logger = logger;
            _endPointUrl = "opc.tcp://" + opcCom.Hostname + ":" + opcCom.Port;
            _rootNodeId = opcCom.RootNodeId;
            _deviceId = gatewayControllerConfig.OpcSettings.DeviceNodeID;
            _ioEnable = ioEnable;

            _logger.Information($"Init '{gatewayControllerConfig.Name}' OPC communication client");
            Connect();
        }

        public void Connect()
        {
            try
            {
                Task.Run(() => ConnnectToEndpointsWithNoSecurityAndWithNoCertificate()).GetAwaiter().GetResult();
                Task.Run(() => BrowseDeviceNode()).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "The OPC connection with PLC failed");
            }
        }

        public void Disconnect()
        {
            try
            {
                Task.Run(() =>
                {
                    Channel.CloseAsync();
                    _logger.Information("Device PLC closed");
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "The disconnection between OPC and PLC has failed");
            }
        }

        private async Task ConnnectToEndpointsWithNoSecurityAndWithNoCertificate()
        {
            // Discover available endpoints of server.
            var getEndpointsRequest = new GetEndpointsRequest
            {
                EndpointUrl = _endPointUrl,
                ProfileUris = new[] { TransportProfileUris.UaTcpTransport }
            };
            _logger.Information("Discover available endpoints of server");

            var getEndpointsResponse = await UaTcpDiscoveryService.GetEndpointsAsync(getEndpointsRequest).ConfigureAwait(false); ;
            _logger.Information("The service returned the Endpoints");

            // For each endpoint and user identity type, try creating a session and reading a few nodes.
            foreach (var selectedEndpoint in getEndpointsResponse.Endpoints.Where(e => e.SecurityPolicyUri == SecurityPolicyUris.None))
            {
                Channel = new UaTcpSessionChannel(new ApplicationDescription(),
                                                   null,
                                                   new AnonymousIdentity(),
                                                   selectedEndpoint);

                await Channel.OpenAsync();
                _logger.Information("The OPC connection with PLC has been established");
            }
        }

        private async Task BrowseDeviceNode()
        {
            try
            {
                BrowseRequest browseRequest = BrowseRequest(ExpandedNodeId.Parse(ObjectIds.RootFolder));
                BrowseResponse browseResponse = await Channel.BrowseAsync(browseRequest).ConfigureAwait(false); ;

                // Browse the root to the NodeId
                foreach (String node in _rootNodeId.Split('/'))
                {
                    browseResponse = await BrowseResults(node, browseResponse.Results[0].References).ConfigureAwait(false); ;
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

                        browseResponse = await BrowseResults(device).ConfigureAwait(false); ;

                        // Add attribute
                        foreach (var attribute in browseResponse.Results[0].References ?? new ReferenceDescription[0])
                        {
                            browseResponse = await BrowseResults(attribute).ConfigureAwait(false);

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
                                        readResponse = await ReadResult(item.NodeId.ToString()).ConfigureAwait(false);
                                        HandleDataAttributes[handle].DigitalValue = (bool)readResponse.Results[0].Value;
                                        //Console.WriteLine("{0}, {1}", item.DisplayName, HandleDataAttributes[handle].Value);
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
                                        readResponse = await ReadResult(attribute.NodeId.ToString()).ConfigureAwait(false);
                                        HandleDataAttributes[handle].Value = readResponse.Results[0].Value;
                                        //Console.WriteLine("{0}, {1}", attribute.DisplayName, HandleDataAttributes[handle].Value);
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

        private async Task<BrowseResponse> BrowseResults(ReferenceDescription rd)
        {
            BrowseRequest browseRequest = BrowseRequest(rd.NodeId);
            return await Channel.BrowseAsync(browseRequest).ConfigureAwait(false); ;
        }

        private async Task<BrowseResponse> BrowseResults(String node, ReferenceDescription[] rds)
        {
            BrowseResponse browseResponse = null;
            foreach (var rd in rds ?? new ReferenceDescription[0])
            {
                if (rd.DisplayName.Text == node ||
                    node == null)
                {
                    BrowseRequest browseRequest = BrowseRequest(rd.NodeId);
                    browseResponse = await Channel.BrowseAsync(browseRequest).ConfigureAwait(false); ;
                }
            }

            return browseResponse;
        }

        private BrowseRequest BrowseRequest(ExpandedNodeId expandedNodeId)
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

        private async Task<ReadResponse> ReadResult(string nodeId)
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

            return await Channel.ReadAsync(readRequest).ConfigureAwait(false); ;
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

        public async void SetMethodValueAsync(string name)
        {
            if (Channel != null)
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

        public async void SetMethodValueAsync(string name, ushort value)
        {
            if (Channel != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(value) }
                        }
                    }
                });
            }
        }

        public async void SetMethodValueAsync(string name, int value)
        {
            if (Channel != null)
            {
                var response = await Channel.CallAsync(new CallRequest
                {
                    MethodsToCall = new[]
                    {
                        new CallMethodRequest
                        {
                            ObjectId = NodeId.Parse(OpcDevice.DeviceIdentifier),
                            MethodId = NodeId.Parse(OpcDevice.DeviceIdentifier + "#" + name),
                            InputArguments = new Variant[] { new Variant(true), new Variant((ushort)value) }
                        }
                    }
                });
            }
        }

        public async void SetMethodValueAsync(string name, double value)
        {
            if (Channel != null)
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
            if (Channel != null)
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
            if (Channel != null)
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

        public async void SetMethodValueAsync(string name, bool value)
        {
            if (Channel != null)
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
    }
}
