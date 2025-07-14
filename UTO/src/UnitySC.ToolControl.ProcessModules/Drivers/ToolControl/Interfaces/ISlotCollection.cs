using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true)]
    [Guid("7C0B2ACA-061B-496C-9293-2719FF893FB6")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISlotCollection : IEnumerable<ISlot>
    {
        #region Public Methods

        void Add(ISlot value);

        void Clear();

        bool Contains(ISlot value);

        void Remove(ISlot value);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        ISlot this[int index] { get; set; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("8F132A94-4EAB-4794-9BDB-765617096E46")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISlotCollection))]
    public class SlotCollection : ISlotCollection
    {
        #region Private Fields

        private readonly Collection<ISlot> _collection;

        #endregion Private Fields

        #region Public Constructors

        public SlotCollection(Collection<ISlot> collection)
        {
            _collection = collection;
        }

        public SlotCollection(ISlot[] array) : this()
        {
            foreach (var item in array)
            {
                _collection.Add(item);
            }
        }

        public SlotCollection()
        {
            _collection = new Collection<ISlot>();
        }


        #endregion Public Constructors

        #region Public Methods

        public void Add(ISlot value) => _collection.Add(value);

        public void Clear() => _collection.Clear();

        public bool Contains(ISlot value) => _collection.Contains(value);

        public void Remove(ISlot value) => _collection.Remove(value);

        [ComVisible(false)]
        public IEnumerator<ISlot> GetEnumerator() => _collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        #endregion Public Methods

        #region Public Properties

        [ComVisible(false)]
        public Collection<ISlot> Collection => _collection;

        public int Count => _collection.Count;

        #endregion Public Properties

        #region Public Indexers

        public ISlot this[int index]
        {
            get
            {
                return _collection[index];
            }
            set
            {
                _collection[index] = value;
            }
        }

        #endregion Public Indexers
    }
}
