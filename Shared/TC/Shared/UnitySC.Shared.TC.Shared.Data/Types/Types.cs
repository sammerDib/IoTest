using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Data
{
    [DataContract(Namespace = "")]
    public enum AlarmCriticality
    {
        [EnumMember(Value = "Information")]
        Information,

        [EnumMember(Value = "Warning")]
        Warning,

        [EnumMember(Value = "Minor")]
        Minor,

        [EnumMember(Value = "Major")]
        Major,

        [EnumMember(Value = "Critical")]
        Critical
    }

    [DataContract(Namespace = "")]
    public class Alarm : IComparable<Alarm>
    {

        [DataMember]
        public int ID;

        [DataMember]
        public String Name;

        [DataMember]
        public String Description;

        [DataMember]
        public AlarmCriticality Level; // ( Warning=0, Error=1)

        [DataMember]
        public bool Active;

        [DataMember]
        public bool Acknowledged;

        [DataMember]
        public List<ErrorID> FromErrors;

        [DataMember]
        public ActorType ActorType;

        [IgnoreDataMember, XmlIgnore]
        public ErrorID LastTriggeringError = ErrorID.Undefined;
        [IgnoreDataMember, XmlIgnore]
        public bool DoReactivation = false;
        [IgnoreDataMember, XmlIgnore]
        public Identity ErrorSource;

        public int CompareTo(Alarm other)
        {
            if (other == null)
                return 1;

            int comparison = ID.CompareTo(other.ID);
            if (comparison != 0)
                return comparison;

            comparison = Name.CompareTo(other.Name);
            if (comparison != 0)
                return comparison;

            comparison = Description.CompareTo(other.Description);
            if (comparison != 0)
                return comparison;

            comparison = Level.CompareTo(other.Level);
            if (comparison != 0)
                return comparison;

            comparison = Active.CompareTo(other.Active);
            if (comparison != 0)
                return comparison;

            comparison = Acknowledged.CompareTo(other.Acknowledged);
            if (comparison != 0)

                return comparison;
            comparison = LastTriggeringError.CompareTo(other.LastTriggeringError);
            if (comparison != 0)
                return comparison;

            comparison = DoReactivation.CompareTo(other.DoReactivation);
            if (comparison != 0)
                return comparison;

            comparison = FromErrors.Count.CompareTo(other.FromErrors.Count);
            if (comparison != 0)
                return comparison;

            return 0;
        }
        public static bool operator ==(Alarm alarm1, Alarm alarm2)
        {
            if (ReferenceEquals(alarm1, null) && ReferenceEquals(alarm2, null))
                return true;

            if (ReferenceEquals(alarm1, null) || ReferenceEquals(alarm2, null))
                return false;

            return alarm1.ID == alarm2.ID &&
                   alarm1.Name == alarm2.Name &&
                   alarm1.Description == alarm2.Description &&
                   alarm1.Level == alarm2.Level &&
                   alarm1.Active == alarm2.Active &&
                   alarm1.Acknowledged == alarm2.Acknowledged &&
                   alarm1.LastTriggeringError == alarm2.LastTriggeringError &&
                   alarm1.DoReactivation == alarm2.DoReactivation &&
                   alarm1.FromErrors.Count == alarm2.FromErrors.Count;
        }

        public static bool operator !=(Alarm alarm1, Alarm alarm2)
        {
            return !(alarm1 == alarm2);
        }

        public override string ToString()
        {
            return Name + "[" + ID + "]";
        }

        public override bool Equals(object obj)
        {
            return obj is Alarm alarm &&
                   ID.Equals(alarm.ID) &&
                   Name == alarm.Name &&
                   Description == alarm.Description &&
                   Level.Equals(alarm.Level) &&
                   Active == alarm.Active &&
                   Acknowledged == alarm.Acknowledged &&
                   LastTriggeringError.Equals(alarm.LastTriggeringError) &&
                   DoReactivation == alarm.DoReactivation &&
                   FromErrors.Count.Equals(alarm.FromErrors.Count);
        }

        public override int GetHashCode()
        {
            return (ID, Name, Description, Level, Active, Acknowledged, LastTriggeringError, DoReactivation).GetHashCode();
        }
    }


    public class DeviceErrorMessage
    {
        public ErrorID ErrorID;
        public string Message;
        public bool Active;
    }

    [DataContract(Namespace = "")]
    public enum TransferType
    {
        [EnumMember(Value = "Pick")]
        Pick,

        [EnumMember(Value = "Place")]
        Place
    }

    [DataContract(Namespace = "")]
    public class TransferMaterial : Material
    {
        public TransferMaterial(TransferType transferType)
        {
            TransferType = transferType;
            LoadportID = 0;
            CarrierID = String.Empty;
            SlotID = 0;
            ProcessJobID = String.Empty;
            ControlJobID = String.Empty;
            LotID = String.Empty;
            OrientationAngle = 0;
            SubstrateID = String.Empty;
            AcquiredID = String.Empty;
            MaterialType = 0;
        }

        [DataMember]
        public string Recipe
        {
            get;
            set;
        }

        [DataMember]
        public TransferType TransferType
        {
            get;
            set;
        }

        public void SetMaterial(Material material)
        {
            // TODO DATAFLOW : On peut aussi écrire LoadportID=material?.LoadportID ?? 0;
            // Ca évite le if (material == null)
            if (material == null)
            {
                LoadportID = 0;
                CarrierID = String.Empty;
                SlotID = 0;
                ProcessJobID = String.Empty;
                ControlJobID = String.Empty;
                LotID = String.Empty;
                OrientationAngle = 0;
                SubstrateID = String.Empty;
                AcquiredID = String.Empty;
                MaterialType = 0;
            }
            else
            {
                LoadportID = material.LoadportID;
                CarrierID = material.CarrierID;
                SlotID = material.SlotID;
                ProcessJobID = material.ProcessJobID;
                ControlJobID = material.ControlJobID;
                LotID = material.LotID;
                OrientationAngle = material.OrientationAngle;
                SubstrateID = material.SubstrateID;
                AcquiredID = material.AcquiredID;
                MaterialType = material.MaterialType;
            }
        }
    }

    public enum VidDataType
    {
        [XmlEnum("A")]
        String,

        F4,
        F8,
        I1,
        I2,
        I4,
        I8,
        U1,
        U2,
        U4,
        U8,

        [Description("BOOLEAN")]
        [XmlEnum("BOOLEAN")]
        Boolean,

        [Description("LIST")]
        [XmlEnum("L")]
        LIST
    }

    public static class DataTypeConverter
    {
        public static object ConvertTo(VidDataType targetDataType, object value)
        {
            if (value == null)
            {
                if (targetDataType == VidDataType.String)
                    return String.Empty;
                else
                    return null;
            }

            switch (targetDataType)
            {
                case VidDataType.String:
                    return Convert.ToString(value);

                case VidDataType.F4:
                case VidDataType.F8:
                    return Convert.ToDouble(value);

                case VidDataType.I1:
                    return Convert.ToSByte(value);

                case VidDataType.I2:
                    return Convert.ToInt16(value);

                case VidDataType.I4:
                    return Convert.ToInt32(value);

                case VidDataType.I8:
                    return Convert.ToInt64(value);

                case VidDataType.U1:
                    return Convert.ToByte(value);

                case VidDataType.U2:
                    return Convert.ToUInt16(value);

                case VidDataType.U4:
                    return Convert.ToUInt32(value);

                case VidDataType.U8:
                    return Convert.ToUInt64(value);

                case VidDataType.Boolean:
                    return Convert.ToBoolean(value);

                default:
                    throw new ArgumentOutOfRangeException("targetDataType", targetDataType, "Is not supported");
            }
        }

        internal static bool IsConvertible(VidDataType targetDataType, object value)
        {
            try
            {
                ConvertTo(targetDataType, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
