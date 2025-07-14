using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.EFEM.Controller.HostInterface.Configuration;
using UnitySC.GUI.Common.Configuration;

namespace UnitySC.EFEM.Controller.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class EfemControllerConfiguration : UnityScConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets host communication configuration.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember]
        public HostConfiguration HostConfiguration { get; set; }

        #endregion Properties

        #region Override methods

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(base.ToString());
            builder.AppendLine($"<{nameof(EfemControllerConfiguration)}>");
            builder.AppendLine(HostConfiguration.ToString());
            return builder.ToString();
        }

        protected override void SetDefaults()
        {
            base.SetDefaults();

            HostConfiguration = new HostConfiguration();
        }

        #endregion Override methods
    }
}
