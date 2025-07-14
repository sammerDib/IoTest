using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true)]
    [Guid("7A286816-82F5-4920-BB0E-48823C6F2CB8")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IProcessModuleCollection : IEnumerable<IProcessModule>
    {
        #region Public Methods

        void Add(IProcessModule value);

        void Add(string id, string name, ModuleState state, bool isSubstratePresent);

        void Clear();

        bool Contains(IProcessModule value);

        void Remove(IProcessModule value);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        IProcessModule this[int index] { get; set; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("D681D1C1-B228-40D7-A54A-81D8F414D005")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IProcessModuleCollection))]
    public class ProcessModuleCollection : IProcessModuleCollection
    {
        #region Private Fields

        private readonly Collection<IProcessModule> _collection;

        #endregion Private Fields

        #region Public Constructors

        public ProcessModuleCollection(Collection<IProcessModule> collection)
        {
            _collection = collection;
        }

        public ProcessModuleCollection(IProcessModule[] array) : this()
        {
            foreach (var item in array)
            {
                _collection.Add(item);
            }
        }

        public ProcessModuleCollection()
        {
            _collection = new Collection<IProcessModule>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(IProcessModule value) => _collection.Add(value);

        public void Add(string id, string name, ModuleState state, bool isSubstratePresent) => _collection.Add(new ProcessModule(id, name, state, isSubstratePresent));

        public void Clear() => _collection.Clear();

        public bool Contains(IProcessModule value) => _collection.Contains(value);

        [ComVisible(false)]
        public IEnumerator<IProcessModule> GetEnumerator() => _collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        public void Remove(IProcessModule value) => _collection.Remove(value);

        #endregion Public Methods

        #region Public Properties

        [ComVisible(false)]
        public Collection<IProcessModule> Collection => _collection;

        public int Count => _collection.Count;

        #endregion Public Properties

        #region Public Indexers

        public IProcessModule this[int index]
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
