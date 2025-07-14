using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Data
{
    /// <summary>
    /// Unified user : TC + Database
    /// </summary>
    [DataContract]
    public class UnifiedUser
    {
        /// <summary>
        /// Database user Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// User name : Database and TC
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// User rights
        /// </summary>
        [DataMember]
        public List<UserRights> Rights { get; set; }

        [DataMember]
        public UserProfiles Profile { get; set; }
    }
}