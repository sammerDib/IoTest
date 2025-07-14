using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Aerotech.Ensemble;

using UnitySC.PM.Shared.Hardware.Service.Interface.Common;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class AerotechAxisIDLink
    {
        [DataMember]
        public string AerotechID { get; set; }

        [DataMember]
        public string AxisID { get; set; }

        [DataMember]
        public int AerotechIndex { get; set; }
    }

    [Serializable]
    [DataContract]
    public class AerotechControllerConfig : ControllerConfig
    {
        #region Fields

        [DataMember]
        /// <summary>
        /// Selects between primary encoder and MXH multiplier for the PSO setup
        /// </summary>
        public bool UseMXHChannel { get; set; }

        /// <summary>
        /// Defines the diagnostic (stage status) interval in milliseconds
        /// </summary>
        public int RefreshDiagRate;

        /// <summary>
        /// The ID of the aerotech controller on the network
        /// </summary>
        public string ControllerID;

        /// <summary>
        /// Define if slit door state (opened/closed) is monitored by controller 
        /// </summary>
        public bool MonitorProcessDoor { get; set; }

        /// <summary>
        /// Define Input of axis associated with slit door state (opened/closed) for monitoring by controller 
        /// </summary>
        public AxisMask ExternalFaultAxisMask;

        [Category("EthernetCom")]
        [DisplayName("EthernetCom")]
        [DataMember]
        public EthernetCom EthernetCom { get; set; }
        public List<AerotechAxisIDLink> AerotechAxisIDLinks { get; set; }


        #endregion

        #region Constructors
        public AerotechControllerConfig()
          : base()
        {
            EthernetCom = new EthernetCom();
        }
        #endregion

        #region Properties
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        #endregion

        #region Private methods
        #endregion
    }
}
