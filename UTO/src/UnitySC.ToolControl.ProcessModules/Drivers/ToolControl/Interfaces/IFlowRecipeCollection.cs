using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true)]
    [Guid("D8C929A5-7044-44BE-9CBB-0F4CC95EE6D3")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IFlowRecipeCollection : IEnumerable<IFlowRecipeItem>
    {
        #region Public Methods

        void Add(IFlowRecipeItem value);

        void Add(string processModuleId, string moduleRecipeId, string moduleRecipe, double angleInDegree);

        void Clear();

        bool Contains(IFlowRecipeItem value);

        void Remove(IFlowRecipeItem value);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        IFlowRecipeItem this[int index] { get; set; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("2855C21E-6E6B-43D3-9068-42711091618C")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IFlowRecipeCollection))]
    public class FlowRecipeCollection : IFlowRecipeCollection
    {
        #region Private Fields

        private readonly Collection<IFlowRecipeItem> _collection;

        #endregion Private Fields

        #region Public Constructors

        public FlowRecipeCollection(Collection<IFlowRecipeItem> collection)
        {
            _collection = collection;
        }

        public FlowRecipeCollection(IFlowRecipeItem[] array)
        {
            _collection = new Collection<IFlowRecipeItem>();
            foreach (var item in array)
            {
                _collection.Add(item);
            }
        }

        public FlowRecipeCollection()
        {
            _collection = new Collection<IFlowRecipeItem>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(IFlowRecipeItem value) => _collection.Add(value);

        public void Add(string processModuleId, string moduleRecipeId, string moduleRecipeName, double angleInDegree) => _collection.Add(new FlowRecipeItem(processModuleId, moduleRecipeId, moduleRecipeName, angleInDegree));

        public void Clear() => _collection.Clear();

        public bool Contains(IFlowRecipeItem value) => _collection.Contains(value);

        public void Remove(IFlowRecipeItem value) => _collection.Remove(value);

        [ComVisible(false)]
        public IEnumerator<IFlowRecipeItem> GetEnumerator() => _collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        #endregion Public Methods

        #region Public Properties

        [ComVisible(false)]
        public Collection<IFlowRecipeItem> Collection => _collection;

        public int Count => _collection.Count;

        #endregion Public Properties

        #region Public Indexers

        public IFlowRecipeItem this[int index]
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
