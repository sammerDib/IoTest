using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;
using Agileo.Semi.Gem300.Abstractions.E87;

using Castle.Core.Internal;

using UnitySC.GUI.Common.Vendor.Helpers;

using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.Carriers
{
    /// <summary>
    /// This class is used to encapsulate the LoadPorts to be able to have a null item so we can display on the chipsItems a Loadport object with the value null.
    /// This value will be taken in charge by the xaml to display NONE
    /// </summary>
    public class LoadPortChips
    {
        public LoadPortChips(LoadPort loadPort)
        {
            LoadPort = loadPort;
        }

        public LoadPort LoadPort { get; set; }
    }

    public class CarrierEditionPopup : Notifier
    {
        static CarrierEditionPopup()
        {
            DataTemplateGenerator.Create(typeof(CarrierEditionPopup), typeof(CarrierEditionPopupView));
        }

        public CarrierEditionPopup(Agileo.Semi.Gem300.Abstractions.E87.Carrier carrier = null)
        {
            LoadPorts.Add(_noneLoadPort);
            LoadPorts.AddRange(GUI.Common.App.Instance.EquipmentManager.Equipment.AllOfType<LoadPort>().Select(lp => new LoadPortChips(lp)));
            SelectedLoadPort = _noneLoadPort;
            InsertSlot(25);
            Capacities = new List<int> { 1, 13, 25, 26 };
            SelectedCapacity = Slots.Count;

            FillProperties(carrier);
        }

        #region Fields

        private readonly LoadPortChips _noneLoadPort = new LoadPortChips(null);

        private readonly List<Slot> _selectedSlots = new List<Slot>();

        private List<Slot> _slotsBuffer = new List<Slot>();

        #endregion

        #region Properties

        public List<LoadPortChips> LoadPorts { get; } = new List<LoadPortChips>();

        private LoadPortChips _selectedLoadPort;

        public LoadPortChips SelectedLoadPort
        {
            get => _selectedLoadPort;
            set => SetAndRaiseIfChanged(ref _selectedLoadPort, value);
        }

        public ObservableCollection<Slot> Slots { get; } = new ObservableCollection<Slot>();

        public List<int> Capacities { get;}

        private int _selectedCapacity;

        public int SelectedCapacity
        {
            get => _selectedCapacity;
            set
            {
                if (_selectedCapacity == value) return;
                if (value < 1 || value > 100)
                {
                    value = _selectedCapacity;

                }
                UpdateCapacity(value);
                SetAndRaiseIfChanged(ref _selectedCapacity,value);
            }
        }

        private string _usage;

        public string Usage
        {
            get => _usage;
            set => SetAndRaiseIfChanged(ref _usage, value);
        }

        private SlotState? _slotStateSelected;

        public SlotState? SlotStateSelected
        {
            get => _slotStateSelected;
            set
            {
                if (SetAndRaiseIfChanged(ref _slotStateSelected, value))
                {
                    foreach (var slot in _selectedSlots)
                    {
                        slot.State = value ?? SlotState.Empty;
                    }
                    OnPropertyChanged(nameof(SubstrateCount));
                }
            }
        }

        public int SubstrateCount => Slots.Count(s => s.State != SlotState.Empty);

        private string _substrateId;

        public string SubstrateId
        {
            get => _substrateId;
            set
            {
                if (SetAndRaiseIfChanged(ref _substrateId, value))
                {
                    foreach (var slot in _selectedSlots)
                    {
                        slot.SubstrateId = value;
                    }
                }
            }
        }

        private string _lotId;

        public string LotId
        {
            get => _lotId;
            set
            {
                if (SetAndRaiseIfChanged(ref _lotId, value))
                {
                    foreach (var slot in _selectedSlots)
                    {
                        slot.LotId = value;
                    }
                }
            }
        }

        private string _carrierId;

        public string CarrierId
        {
            get => _carrierId;
            set => SetAndRaiseIfChanged(ref _carrierId, value);
        }

        private bool _enableEdition = true;

        public bool EnableEdition
        {
            get => _enableEdition;
            set => SetAndRaiseIfChanged(ref _enableEdition, value);
        }

        private bool _enableEditionOfSlotMap;

        public bool EnableEditionOfSlotMap
        {
            get => _enableEditionOfSlotMap;
            set => SetAndRaiseIfChanged(ref _enableEditionOfSlotMap, value);
        }

        #endregion

        #region Methods

        private void InsertSlot(int value)
        {
            for (int i = Slots.Count; i < value; i++)
            {
                Slots.Insert(0,new Slot
                {
                    Index = i + 1,
                    LotId = string.Empty,
                    State = SlotState.Empty,
                    SubstrateId = string.Empty
                });
            }
        }

        private void UpdateCapacity(int capacity)
        {
            var currentCapacity = Slots.Count;
            if (currentCapacity > capacity)
            {
                if (_slotsBuffer.Count < Slots.Count)
                {
                    _slotsBuffer = Slots.ToList();
                }

                Slots.Clear();
                for (int i = _slotsBuffer.Count - 1; i >= _slotsBuffer.Count - capacity; i--)
                {
                    Slots.Insert(0, _slotsBuffer[i]);
                }
            }
            if (currentCapacity < capacity)
            {
                Slots.Clear();

                for (int i = _slotsBuffer.Count - 1; i >= 0; i--)
                {
                    if (Slots.Count == capacity) break;

                    Slots.Insert(0, _slotsBuffer[i]);
                }

                InsertSlot(capacity);
            }
        }

        private void FillProperties(Agileo.Semi.Gem300.Abstractions.E87.Carrier carrier)
        {
            if (carrier != null)
            {
                Slots.Clear();

                for (var index = 0; index < carrier.ContentMap.Count; index++)
                {
                    var mapItem = carrier.ContentMap[index];

                    var slot = new Slot
                    {
                        SubstrateId = mapItem.SubstrateID,
                        LotId = mapItem.LotID,
                        Index = Slots.Count + 1,
                        State = SlotState.Empty
                    };

                    if (carrier.SlotMap.Count >= index + 1)
                    {
                        slot.State = carrier.SlotMap[index];
                    }

                    Slots.Insert(0, slot);
                }

                SelectedCapacity = carrier.Capacity;
                Usage = carrier.Usage;
                CarrierId = carrier.ObjID;

                if (carrier.LocationId.IsNullOrEmpty()) return;
                
                var associatedLoadPort = App.ControllerInstance.GemController.E87Std.GetLoadPortByLocationID(carrier.LocationId);
                var loadPortsItem = GUI.Common.App.Instance.EquipmentManager.Equipment.AllOfType<LoadPort>()
                    .SingleOrDefault(lp => lp.InstanceId == associatedLoadPort.PortID);
                SelectedLoadPort = LoadPorts.FirstOrDefault(x => x.LoadPort == loadPortsItem);
            }
            else
            {
                SelectedLoadPort = _noneLoadPort;
                Usage = string.Empty;
                CarrierId = string.Empty;
                Slots.Clear();
                InsertSlot(25);
            }
        }

        #endregion

        public void UpdateSelectedSlot(IEnumerable<Slot> selectedSlot)
        {
            _selectedSlots.Clear();
            _selectedSlots.AddRange(selectedSlot);

            EnableEditionOfSlotMap = _selectedSlots.Count != 0;

            var distinctSubstrateSelected = _selectedSlots.Select(x => x.SubstrateId).Distinct().ToList();
            var distinctLotIdSelected = _selectedSlots.Select(x => x.LotId).Distinct().ToList();
            var distinctSlotStateSelected = _selectedSlots.Select(x => x.State).Distinct().ToList();

            string substrateIdDisplayed;
            if (distinctSubstrateSelected.Count == 1)
            {
                substrateIdDisplayed = distinctSubstrateSelected.First();
            }
            else
            {
                substrateIdDisplayed = _selectedSlots.Count != 0 ? "*" : "";
            }

            string lotIdDisplayed;
            if (distinctLotIdSelected.Count == 1)
            {
                lotIdDisplayed = distinctLotIdSelected.First();
            }
            else
            {
                lotIdDisplayed = _selectedSlots.Count != 0 ? "*" : "";
            }

            var slotStateDisplayed = distinctSlotStateSelected.Count == 1 ? (SlotState?)distinctSlotStateSelected.First() : null;

            SetAndRaiseIfChanged(ref _substrateId, substrateIdDisplayed, nameof(SubstrateId));
            SetAndRaiseIfChanged(ref _lotId, lotIdDisplayed, nameof(LotId));
            SetAndRaiseIfChanged(ref _slotStateSelected, slotStateDisplayed, nameof(SlotStateSelected));
        }

        public E87PropertiesList GetE87PropertiesList()
        {
            var contentMap = new List<ContentMapItem>();
            var slotMap = new List<SlotState>();

            foreach (var slot in Slots)
            {
                contentMap.Insert(0, new ContentMapItem(slot.LotId, slot.SubstrateId));
                slotMap.Insert(0, slot.State);
            }

            var propertiesList = new E87PropertiesList(slotMap, contentMap);
            propertiesList.Capacity = (byte)SelectedCapacity;
            propertiesList.Usage = Usage;

            return propertiesList;
        }
    }
}
