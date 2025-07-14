using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Devices.Controller.JobDefinition;
using UnitySC.GUI.Common.Configuration;

namespace UnitySC.UTO.Controller.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class ControllerConfiguration : UnityScConfiguration
    {
        /// <summary>
        /// Gets or sets the carrier pick order
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public CarrierPickOrder CarrierPickOrder { get; set; }

        /// <summary>
        /// Gets or sets the DataFlow folder name
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public string DataFlowFolderName { get; set; }

        /// <summary>
        /// Gets or sets the ToolKey
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public int ToolKey { get; set; }

        /// <summary>
        /// Gets or sets the OcrProfiles list
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public List<OcrProfile> OcrProfiles { get; set; }

        /// <summary>
        /// Gets or sets the UnloadCarrierAfterAbort
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool UnloadCarrierAfterAbort { get; set; }

        /// <summary>
        /// Gets or sets the UnloadCarrierBetweenJobs
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool UnloadCarrierBetweenJobs { get; set; }

        /// <summary>
        /// Gets or sets the DisableParallelControlJob
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool DisableParallelControlJob { get; set; }
        
        /// <summary>
        /// Gets or sets the inactivity duration before auto log off
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int InactivityTimeoutDuration { get; set; }


        /// <summary>
        /// Gets or sets the JobRecreateAfterInit
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool JobRecreateAfterInit { get; set; }

        /// <summary>
        /// Gets or sets the StartHotLot
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public bool StartHotLot { get; set; }

        /// <summary>
        /// Gets or sets the wafer result reception timeout duration before abort job
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public int ResultReceptionTimeoutDuration { get; set; }

        #region Overrides of UnityScConfiguration

        protected override void SetDefaults()
        {
            base.SetDefaults();
            
            CarrierPickOrder = CarrierPickOrder.BottomToTop;
            DataFlowFolderName = "4MET2229";
            ToolKey = 4;
            OcrProfiles = new List<OcrProfile>();
            UnloadCarrierAfterAbort = false;
            UnloadCarrierBetweenJobs = false;
            DisableParallelControlJob = false;
            InactivityTimeoutDuration = 15;
            JobRecreateAfterInit = false;
            StartHotLot = false;
            ResultReceptionTimeoutDuration = 30;
        }

        #endregion
    }
}
