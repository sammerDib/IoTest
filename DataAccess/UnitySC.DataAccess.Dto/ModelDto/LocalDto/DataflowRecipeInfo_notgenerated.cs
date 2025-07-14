using System;
using System.Runtime.Serialization;

namespace UnitySC.DataAccess.Dto
{
    [DataContract]
    public class DataflowRecipeInfo
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Guid IdGuid { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public int StepId { get; set; }

        [DataMember]
        public string StepName { get; set; }

        [DataMember]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string GetRecipePath(bool needNameOnly = false)
        {
            return needNameOnly ? Name : $"{ProductName}/{StepName}/{Name}";
        }
    }
}
