using System;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.GUI.Components;
using Agileo.Semi.Gem300.Abstractions.E87;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment
{
    public class SlotMapViewModel : Notifier
    {
        private static readonly Random Random = new();

        public SlotMapViewModel()
        {
            if (IsInDesignMode)
            {
                GenerateRandomSlots();
            }
        }

        public ObservableCollection<Slot> Slots { get; } = new();

        public void UpdateSlotMap(Carrier carrier, bool async = true)
        {
            void InternalUpdateSlotMap()
            {
                Slots.Clear();
                if (carrier == null)
                {
                    SubstrateCount = null;
                    Capacity = 0;
                    SlotMapStatus = SlotMapStatus.NotRead;
                    Reason = null;

                    for (int i = 0; i < 25; i++)
                    {
                        Slots.Insert(0, new Slot
                        {
                            Index = i + 1,
                            State = SlotState.Empty
                        });
                    }
                }
                else
                {
                    SubstrateCount = carrier.SubstrateCount;
                    Capacity = carrier.Capacity;
                    SlotMapStatus = carrier.SlotMapStatus;
                    Reason = carrier.Reason;

                    for (var index = 0; index < carrier.SlotMap.Count; index++)
                    {
                        var slotState = carrier.SlotMap[index];
                        var item = carrier.ContentMap?.ElementAtOrDefault(index);

                        Slots.Insert(0, new Slot
                        {
                            Index = index + 1,
                            SubstrateId = item?.SubstrateID,
                            LotId = item?.LotID,
                            State = slotState
                        });
                    }
                }
            }

            if (async)
            {
                DispatcherHelper.DoInUiThreadAsynchronously(InternalUpdateSlotMap);
            }
            else
            {
                DispatcherHelper.DoInUiThread(InternalUpdateSlotMap);
            }
        }

        private void GenerateRandomSlots()
        {
            for (int i = 0; i < 25; i++)
            {
                Slots.Insert(0, new Slot
                {
                    Index = i + 1,
                    State = (SlotState) Random.Next(0, 6),
                    SubstrateId = Guid.NewGuid().ToString().Substring(0, 12),
                    LotId = "TEST LOT"
                });
            }
        }

        private int? _substrateCount;

        public int? SubstrateCount
        {
            get => _substrateCount;
            set => SetAndRaiseIfChanged(ref _substrateCount, value);
        }

        private int _capacity;

        public int Capacity
        {
            get => _capacity;
            set => SetAndRaiseIfChanged(ref _capacity, value);
        }

        private SlotMapStatus _slotMapStatus;

        public SlotMapStatus SlotMapStatus
        {
            get => _slotMapStatus;
            set => SetAndRaiseIfChanged(ref _slotMapStatus, value);
        }

        private Reason? _reason;

        public Reason? Reason
        {
            get => _reason;
            set => SetAndRaiseIfChanged(ref _reason, value);
        }
    }
}
