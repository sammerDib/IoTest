using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    [Serializable]
    [DataContract]
    public class ANAChuckConfig : USPChuckConfig<SubstrateSlotConfig>
    {
        #region Public Fields

        ///<summary>List of References</summary>
        [DataMember(Order = 100)]
        [Browsable(true), Category("Common")]
        public List<OpticalReferenceDefinition> ReferencesList
        {
            get;
            set;
        }

        #endregion

        #region Constructors
        ///<summary>Constructor</summary>
        public ANAChuckConfig()
        {
            SubstrateSlotConfigs = new List<SubstrateSlotConfig>();            
            Name = "";
            IsVacuumChuck = true;
            IsOpenChuck = false;
            ReferencesList = new List<OpticalReferenceDefinition>();
        }
        #endregion

        #region Methods

        #endregion        
    }
}

