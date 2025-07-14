using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true)]
    [Guid("E3A8D1CC-5F8C-4841-984C-3466A4D26B80")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMaterialCarrierCollection : IEnumerable<IMaterialCarrier>
    {
        #region Public Methods

        void Add(IMaterialCarrier value);

        void Clear();

        bool Contains(IMaterialCarrier value);

        void Remove(IMaterialCarrier value);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        IMaterialCarrier this[int index] { get; set; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("6D109F71-2DAE-4627-8075-0374C59F84D1")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IMaterialCarrierCollection))]
    public class MaterialCarrierCollection : IMaterialCarrierCollection
    {
        #region Private Fields

        private readonly Collection<IMaterialCarrier> _collection;

        #endregion Private Fields

        #region Public Constructors

        public MaterialCarrierCollection(Collection<IMaterialCarrier> collection)
        {
            _collection = collection;
        }

        public MaterialCarrierCollection(IMaterialCarrier[] array) : this()
        {
            foreach (var item in array)
            {
                _collection.Add(item);
            }
        }

        public MaterialCarrierCollection()
        {
            _collection = new Collection<IMaterialCarrier>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(IMaterialCarrier value) => _collection.Add(value);

        public void Clear() => _collection.Clear();

        public bool Contains(IMaterialCarrier value) => _collection.Contains(value);

        public void Remove(IMaterialCarrier value) => _collection.Remove(value);

        [ComVisible(false)]
        public IEnumerator<IMaterialCarrier> GetEnumerator() => _collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        #endregion Public Methods

        #region Public Properties

        [ComVisible(false)]
        public Collection<IMaterialCarrier> Collection => _collection;

        public int Count => _collection.Count;

        #endregion Public Properties

        #region Public Indexers

        public IMaterialCarrier this[int index]
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
