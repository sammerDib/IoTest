using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto.ModelDto
{
    /// <summary>
    /// Classe represnetant les données de retour de quelques méthodeds LINQ
    /// </summary>
    [DataContract]
    public class ResultQuery
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsArchived { get; set; }

        /// <summary>
        /// Affiche la propriété Name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }

    [DataContract]
    public class ResultChamberQuery : ResultQuery
    {
        [DataMember]
        public ActorType ActorType { get; set; }

        [DataMember]
        public int ToolIdOwner { get; set; }

        /// <summary>
        /// Affiche la propriété Name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (ActorType == ActorType.Unknown)
                return $"{Name}";
            return $"{ActorType.GetLabelName()}";
        }
    }
}
