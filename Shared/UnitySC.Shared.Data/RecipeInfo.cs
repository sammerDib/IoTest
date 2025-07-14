using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Data
{
    [DataContract]
    [Serializable]
    public class RecipeInfo : IRecipeInfo
    {
        [DataMember]
        [XmlIgnore]
        public string Name { get; set; }

        [DataMember]
        [XmlIgnore]
        public int Version { get; set; }

        [DataMember]
        [XmlIgnore]
        public DateTime Created { get; set; }

        [DataMember]
        [XmlIgnore]
        public int? StepId { get; set; }

        [DataMember]
        [XmlIgnore]
        public int? CreatorChamberId { get; set; }

        [DataMember]
        [XmlIgnore]
        public int? UserId { get; set; }

        [DataMember]
        [XmlIgnore]
        [IgnoreDataMember]
        public Guid Key { get; set; }

        [DataMember]
        [XmlIgnore]
        public ActorType ActorType { get; set; }

        [DataMember]
        [XmlIgnore]
        public string Comment { get; set; }

        [DataMember]
        [XmlIgnore]
        public bool IsTemplate { get; set; }

        [DataMember]
        [XmlIgnore]
        public bool IsShared { get; set; }


        // Needed for WCF contract 
        public RecipeInfo GetBaseRecipeInfo()
        {
            return new RecipeInfo()
            {
                Name = Name,
                Version = Version,
                Created = Created,
                StepId = StepId,
                CreatorChamberId = CreatorChamberId,
                UserId = UserId,
                Key = Key,
                ActorType = ActorType,
                Comment = Comment,
                IsTemplate = IsTemplate,
                IsShared = IsShared
            };
        }
    }
}
