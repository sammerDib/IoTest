using System;
using System.Collections.ObjectModel;
using Agileo.MessageDataBus;
using System.Timers;

using Agileo.Common.Communication;
using Agileo.Common.Communication.Modbus;
using Agileo.Common.Tracing;

using System.Collections.Generic;

using UnitySC.Beckhoff.Emulator.EventArgs;

namespace UnitySC.Beckhoff.Emulator
{
    public class Simulator
    {
        #region Variables

        private string traceManagerAlias = "UnitySC.Beckhoff.Emulator";

        private Timer timer; //this timer represent the cycle of the automate

        private Collection<GenericDriverParameters> drivers = new Collection<GenericDriverParameters>(); //drivers

        private States _currentState; //store the current state of the automate --> used by a property

        #endregion Variables

        #region Properties

        public MDBConfiguration MDBConfig; //the MDB configuration 


        public States CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    OnStateChanged(_currentState);
                }
            }
        }

        #endregion properties

        #region Events
        public event EventHandler<StateEventArgs> StateChanged; //raised when the state of the automate changed
        public void OnStateChanged(States state)
        {
            if (StateChanged != null)
            {
                StateChanged(this, new StateEventArgs(state));
            }
        }

        public event EventHandler<ConnexionEventArgs> ConnexionStateChanged; //raised when a communication event is raised
        public void OnConnexionStateChanged(bool connected)
        {
            if (ConnexionStateChanged != null)
                ConnexionStateChanged(this, new ConnexionEventArgs(connected));
        }
        #endregion Events

        #region Constructor

        public Simulator()
        {
            #region Drivers
            drivers.Add(new ModbusDriverConfig(
                "EK9000Driver",
                "Agileo.Common.Communication",
                "Agileo.Common.Communication.Modbus.ModbusServerDriver",
                "127.0.0.1", 502, 100
            ));
            #endregion Drivers

            #region Configuration
            
            List<Agileo.SemiDefinitions.ErrorDescription> errors = MessageDataBus.Instance.RegisterDrivers(drivers);

            if (errors.Count == 0)
            {
                errors = MessageDataBus.Instance.AppendFromFile(@"Resources/TagsConfiguration.xml");

                if (errors.Count == 0)
                {
                    foreach (var group in MessageDataBus.Instance.Groups.Values)
                    {
                        var eg = (ExternalGroup)group;
                        eg.CommunicationClosed += CommunicationClosed;
                        eg.CommunicationEstablished += CommunicationEstablished;
                        eg.OnTagValueChanged += OnTagValueChanged;
                    }
                }
                else
                {
                    TraceManager.Instance().Trace(traceManagerAlias, TraceLevelType.Error, "Message Data Bus Configuration Error");
                }
            }
            else
            {
                TraceManager.Instance().Trace(traceManagerAlias, TraceLevelType.Error, "Driver Registration Error");
            }

            #endregion Configuration

            timer = new Timer(100);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        #endregion Constructor

        #region eventRaised

        private void OnTagValueChanged(object sender, DrivenTagValueChangedEventArgs e)
        {
            TraceManager.Instance().Trace(traceManagerAlias, TraceLevelType.Info, e.TimeStamp + " => " + e.TagName + " changed to : " + e.Value);
        }

        private void CommunicationClosed(object sender, System.EventArgs e)
        {
            OnConnexionStateChanged(false);
        }

        private void CommunicationEstablished(object sender, System.EventArgs e)
        {
            OnConnexionStateChanged(true);
        }
        #endregion eventRaised

        #region simulator
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            
            //TODO Implement simulation here

            timer.Enabled = true;
        }
        #endregion simulator
    }
}
