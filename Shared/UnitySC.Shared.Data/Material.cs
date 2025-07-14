using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data
{
    [DataContract(Namespace = "")]
    public class MaterialTypeInfo
    {
        [DataMember]
        public int MaterialType { get; set; } // declared in TC: Frame, Silicon, Glass...
        [DataMember]
        public Length WaferDimension { get; set; }
    }

    [DataContract(Namespace = "")]
    public class Material : MaterialTypeInfo, IComparable<Material>
    {
        public const int LOADPORT_COUNT = 4;
        public const int CARRIER_CAPACITY_MAX = 25;

        [DataMember]
        public Guid GUIDWafer { get; set; }

        [DataMember]
        public int LoadportID { get; set; }

        [DataMember]
        public string CarrierID { get; set; }

        [DataMember]
        public int SlotID { get; set; }

        [DataMember]
        public string ProcessJobID { get; set; }

        [DataMember]
        public String ControlJobID { get; set; }

        [DataMember]
        public String LotID { get; set; }

        [DataMember]
        public double OrientationAngle { get; set; }

        [DataMember]
        public String SubstrateID { get; set; }

        [DataMember]
        public String AcquiredID { get; set; }

        [DataMember]
        public JobPosition JobPosition { get; set; }
        [DataMember]
        public DateTime JobStartTime{ get; set; }

        [DataMember]
        public String EquipmentID{ get; set; } // Tool name

        [DataMember]
        public String DeviceID { get; set; } // Info from Automation Host in EquipementConstant

        public String WaferBaseName => !string.IsNullOrEmpty(SubstrateID) ? SubstrateID : AcquiredID;

        public Material Clone()
        {
            return (Material)MemberwiseClone();
        }
        public int CompareTo(Material other)
        {
            if (other == null)
                return 1;

            int guidComparison = GUIDWafer.CompareTo(other.GUIDWafer);
            if (guidComparison != 0)
                return guidComparison;

            int loadportIdComparison = LoadportID.CompareTo(other.LoadportID);
            if (loadportIdComparison != 0)
                return loadportIdComparison;

            int carrierIdComparison = CarrierID.CompareTo(other.CarrierID);
            if (carrierIdComparison != 0)
                return carrierIdComparison;

            int slotIDComparison = SlotID.CompareTo(other.SlotID);
            if (slotIDComparison != 0)
                return slotIDComparison;

            int lotIDComparison = LotID.CompareTo(other.LotID);
            if (lotIDComparison != 0)
                return lotIDComparison;

            int diameterComparison = WaferDimension.CompareTo(other.WaferDimension);
            if (diameterComparison != 0)
                return diameterComparison;
            return 0;
        }

        public static bool operator ==(Material material1, Material material2)
        {
            if (ReferenceEquals(material1, null) && ReferenceEquals(material2, null))
                return true;

            if (ReferenceEquals(material1, null) || ReferenceEquals(material2, null))
                return false;

            return material1.GUIDWafer == material2.GUIDWafer &&
                   material1.LoadportID == material2.LoadportID &&
                   material1.CarrierID == material2.CarrierID &&
                   material1.SlotID == material2.SlotID &&
                   material1.ProcessJobID == material2.ProcessJobID &&
                   material1.ControlJobID == material2.ControlJobID &&
                   material1.LotID == material2.LotID &&
                   material1.OrientationAngle == material2.OrientationAngle &&
                   material1.SubstrateID == material2.SubstrateID &&
                   material1.AcquiredID == material2.AcquiredID &&
                   material1.MaterialType == material2.MaterialType &&
                   material1.JobPosition == material2.JobPosition &&
                   material1.WaferDimension == material2.WaferDimension;
        }

        public static bool operator !=(Material material1, Material material2)
        {
            return !(material1 == material2);
        }

        public bool IsValid
        {
            get
            {
                return (GUIDWafer != null) && (GUIDWafer != Guid.Empty) &&
                   (LoadportID >= 1) && (LoadportID <= LOADPORT_COUNT) &&
                   (!CarrierID.IsNullOrEmpty()) &&
                   (SlotID >= 1) && (SlotID <= CARRIER_CAPACITY_MAX) &&
                   (!ProcessJobID.IsNullOrEmpty()) &&
                   (!ControlJobID.IsNullOrEmpty()) &&
                   (!LotID.IsNullOrEmpty()) &&
                   ((WaferDimension != null) && (WaferDimension.Millimeters > 0)) &&
                   (!SubstrateID.IsNullOrEmpty());
            }
        }

        public override string ToString()
        {
            string carrierId = CarrierID.IsNullOrEmpty() ? "UNK" : CarrierID;
            string processJobID = ProcessJobID.IsNullOrEmpty() ? "UNK" : ProcessJobID;
            string substrateID = SubstrateID.IsNullOrEmpty() ? "UNK" : SubstrateID;
            string lotID = LotID.IsNullOrEmpty() ? "UNK" : LotID;

            return  $"CID={carrierId}.{SlotID} - PJID={processJobID} - SubID={substrateID} - Lot={lotID} - WPos={JobPosition}";
        }

        public override bool Equals(object obj)
        {
            return obj is Material material &&
                   GUIDWafer.Equals(material.GUIDWafer) &&
                   LoadportID == material.LoadportID &&
                   CarrierID == material.CarrierID &&
                   SlotID == material.SlotID &&
                   ProcessJobID == material.ProcessJobID &&
                   ControlJobID == material.ControlJobID &&
                   LotID == material.LotID &&
                   OrientationAngle == material.OrientationAngle &&
                   SubstrateID == material.SubstrateID &&
                   AcquiredID == material.AcquiredID &&
                   MaterialType == material.MaterialType &&
                   WaferBaseName == material.WaferBaseName &&
                   JobPosition == material.JobPosition &&
                   WaferDimension == material.WaferDimension;
        }

        public override int GetHashCode()
        {
            return (GUIDWafer, LoadportID, CarrierID, SlotID, ProcessJobID, ControlJobID, LotID, OrientationAngle, SubstrateID, AcquiredID, MaterialType, WaferBaseName, WaferDimension).GetHashCode();
        }
    }


    [DataContract(Namespace = "")]
    public enum JobPosition
    {
        [EnumMember(Value = "First")]
        First,
        [EnumMember(Value = "Between")]
        Between,
        [EnumMember(Value = "Last")]
        Last,
        [EnumMember(Value = "FirstAndLast")]
        FirstAndLast
    }
}
