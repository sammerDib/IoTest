using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Agileo.Common.Shared;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Enums;

using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.CarrierConfigurations.Models
{
    [Serializable]
    public sealed class CarrierConfiguration : IResourceCleaner, ICloneable, INotifyPropertyChanged
    {
        #region Variables

        private string OwnerEquipmentId;
        private SampleDimension _carrierType;
        private string _id;
        private Collection<SlotState> _mappingTable;
        private Collection<string> _mappingScribe;
        private Collection<int> _indexSlots;
        private DateTime _putTimeStamp;
        private byte _capacity;
        private LoadPortState _state = LoadPortState.Unclamped;
        private Collection<bool> _infoChecked = new Collection<bool>();
        private byte _infoSomme;

        #endregion Variables

        #region Constructors

        /// <summary>
        /// Constructor without parameters other than id
        /// </summary>
        /// <param name="ownerEquipmentId"></param>
        public CarrierConfiguration(string ownerEquipmentId) :
            this(ownerEquipmentId, 0, SampleDimension.NoDimension)
        {
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="ownerEquipmentId"></param>
        /// <param name="capacity"></param>
        /// <param name="carrierType"></param>
        public CarrierConfiguration(string ownerEquipmentId, byte capacity, SampleDimension carrierType)
        {
            OwnerEquipmentId = ownerEquipmentId;

            _id = ownerEquipmentId;
            PutTimeStamp = DateTime.Now;

            Capacity = capacity;
            MappingTable = new Collection<SlotState>();
            MappingScribe = new Collection<string>();
            IndexSlots = new Collection<int>();
            Type = carrierType;
            FullTab();
        }

        /// <summary>
        /// Constructor by recopy
        /// </summary>
        /// <param name="other"></param>
        public CarrierConfiguration(CarrierConfiguration other)
        {
            OwnerEquipmentId = other.OwnerEquipmentId;

            _id = other.Id;
            Capacity = other.Capacity;
            Type = other.Type;
            PutTimeStamp = other.PutTimeStamp;

            _mappingTable = new Collection<SlotState>(other.MappingTable.ToList());
            _mappingScribe = new Collection<string>(other.MappingScribe.ToList());
            _indexSlots = new Collection<int>(other.IndexSlots.ToList());
            _infoChecked = new Collection<bool>(other._infoChecked.ToList());

            InfoSomme = other.InfoSomme;
        }
        #endregion Constructors

        #region Properties

        /// <summary>
        /// Set or get the current carrier's ID
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id == value) return;

                _id = value;
                OnPropertyChanged(p => Id);
            }
        }

        /// <summary>
        /// Set or get the current carrier's LoadPortState
        /// </summary>
        public LoadPortState State
        {
            get { return _state; }

            set
            {
                if (_state == value) return;
                _state = value;
                OnPropertyChanged(p => State);
            }
        }

        /// <summary>
        /// SlotState collection contained in the carrier
        /// </summary>
        /// <remarks>
        /// MappingTable can only be changed using <see>
        ///     <cref>SetMappingTable</cref>
        /// </see>
        /// method.
        /// </remarks>
        public Collection<SlotState> MappingTable
        {
            get { return _mappingTable; }
            set { _mappingTable = value; OnPropertyChanged(p => MappingTable); }
        }

        /// <summary>
        /// Scribe collection contained in the carrier
        /// </summary>
        public Collection<string> MappingScribe
        {
            get { return _mappingScribe; }
            set { _mappingScribe = value; OnPropertyChanged(p => MappingScribe); }
        }

        /// <summary>
        /// Index collection contained in the carrier
        /// </summary>
        public Collection<int> IndexSlots
        {
            get { return _indexSlots; }
            set { _indexSlots = value; OnPropertyChanged(p => IndexSlots); }
        }

        /// <summary>
        /// Maximum number of wafers contained in the carrier.
        /// </summary>
        public byte Capacity
        {
            get { return _capacity; }
            set { if (_capacity == value) return; _capacity = value; OnPropertyChanged(p => Capacity); }
        }

        /// <summary>
        /// Date & Time when the Carrier was placed on the LoadPort.
        /// </summary>
        /// <remarks>
        /// PutTimeStamp can only be changed internally, using the constructor
        /// </remarks>
        public DateTime PutTimeStamp
        {
            get { return _putTimeStamp; }
            private set { if (_putTimeStamp == value) return; _putTimeStamp = value; OnPropertyChanged(p => PutTimeStamp); }
        }

        /// <summary>
        /// Indicates the size of wafers, <see cref="SampleDimension"/>, contained within the Carrier.
        /// </summary>
        public SampleDimension Type
        {
            get { return _carrierType; }
            set { if (_carrierType == value) return; _carrierType = value; OnPropertyChanged(p => Type); }
        }

        #endregion Properties

        #region InfoPad
        /// <summary>
        /// Return InfoPad's sum configured in the carrier
        /// </summary>
        public byte InfoSomme
        {
            get { return _infoSomme; }
            set { _infoSomme = value; OnPropertyChanged(p => InfoSomme); }
        }
        /// <summary>
        /// Collection of boolean: InfoPads checked
        /// </summary>
        public Collection<bool> InfoChecked
        {
            get { return _infoChecked; }
            set { _infoChecked = value; OnPropertyChanged(p => InfoChecked); }
        }
        #endregion InfoPad

        #region Overrides
        /// <summary>
        /// Overide ToString() to save carrier
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("[Carrier on {0}]", OwnerEquipmentId));

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (prop.PropertyType.IsGenericType || prop.PropertyType.IsArray)
                {
                    IEnumerable e = (IEnumerable)prop.GetValue(this, null);
                    if (e != null)
                    {
                        sb.Append(prop.Name + " =");
                        foreach (var item in e)
                            sb.AppendFormat(" {0} /", item);

                        if (sb[sb.Length - 1] == '=') sb.Append(" EMPTY");
                        else sb.Remove(sb.Length - 1, 1);

                        sb.AppendLine();
                    }
                    else
                        sb.AppendLine(string.Format("{0} = {1}", prop.Name, "NULL"));
                }
                else
                    sb.AppendLine(string.Format("{0} = {1}", prop.Name, prop.GetValue(this, null) ?? "NULL"));
            }

            return sb.ToString();
        }
        #endregion Overrides

        #region IResourceCleaner & ICloneable
        /// <summary>
        /// Clean the mapping table
        /// </summary>
        public void Clean()
        {
            MappingTable.Clear();
            MappingTable = null;
        }

        /// <summary>
        /// Clone current carrier
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new CarrierConfiguration(this);
        }
        #endregion IResourceCleaner & ICloneable

        #region INotifyPropertyChanged Members
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged<TProp>(Expression<Func<object, TProp>> propertySelector)
        {
            if (PropertyChanged == null)
                return;
            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null || memberExpression.Member.MemberType != MemberTypes.Property)
                Debug.Assert(false, string.Format("{0} is an invalid property selector.", propertySelector));
            PropertyChanged(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
        }
        #endregion INotifyPropertyChanged Members

        #region Internal
        internal void FullTab()
        {
            for (int i = 0; i < Capacity; i++)
            {
                IndexSlots.Add(i + 1);

                MappingTable.Add(SlotState.HasWafer);
                MappingScribe.Add("");
            }
            for (int i = 0; i < 9; ++i)
                _infoChecked.Add(false);
        }
        /// <summary>
        /// Update the <see cref="MappingTable"/> at <paramref name="slotId"/> position, according to <paramref name="statusOfDeposit"/>.
        /// </summary>
        /// <param name="slotId">Numero of the slot where the action is done. Should be in range [1..<see cref="Capacity"/>].</param>
        /// <param name="statusOfDeposit">deposit flag  <see cref="LocationSenseType"/>.</param>
        /// <remarks>
        /// If <paramref name="statusOfDeposit"/> is "Yes", the slot status is assumed "Correct".
        /// </remarks>
        internal void WaferDeposit(byte slotId, YesNo statusOfDeposit)
        {
            if (Capacity != MappingTable.Count)
            {
                return;
            }

            if (1 <= slotId && slotId <= Capacity)
            {
                MappingTable[slotId - 1] = statusOfDeposit == YesNo.Yes ? SlotState.HasWafer : SlotState.NoWafer;
            }
        }
        #endregion Internal
    }
}
