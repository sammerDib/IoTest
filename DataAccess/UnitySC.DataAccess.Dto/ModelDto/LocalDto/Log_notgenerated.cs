using System.Runtime.Serialization;

namespace UnitySC.DataAccess.Dto
{
    public partial class Log
    {
        /// <summary>
        /// Type d'action faite
        /// </summary>
        [DataContract]
        public enum ActionTypeEnum
        {
            [EnumMember]
            Add = 0,

            [EnumMember]
            Edit = 1,

            [EnumMember]
            Remove = 2,

            [EnumMember]
            Connect = 3,

            [EnumMember]
            Disconnect = 4,

            [EnumMember]
            Restore = 5
        }

        /// <summary>
        /// Table concernée par l'action
        /// </summary>
        [DataContract]
        public enum TableTypeEnum
        {
            [EnumMember]
            User = 0,

            [EnumMember]
            Chamber = 1,

            [EnumMember]
            Tool = 2,

            [EnumMember]
            Recipe = 3,

            [EnumMember]
            Configuration = 4,

            [EnumMember]
            WaferType = 5,

            [EnumMember]
            Step = 6,

            [EnumMember]
            Dataflow = 7,

            [EnumMember]
            Product = 8,

            [EnumMember]
            WaferCategory = 9,

            [EnumMember]
            Material = 10,

            [EnumMember]
            RecipeFile = 11,

           [EnumMember]
            Vid = 12
        }

        [DataMember]
        public ActionTypeEnum? Action
        {
            get { return (ActionTypeEnum?)ActionType; }
            set { ActionType = (int)value; }
        }

        [DataMember]
        public TableTypeEnum? Table
        {
            get { return (TableTypeEnum?)TableType; }
            set { TableType = (int)value; }
        }
    }
}
