using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("B3E6EFB4-51AC-40BD-B518-0EB9045008E1")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITableElementListList
    {
        #region Public Methods

        void Add(TableElementList item);

        void Clear();

        bool Contains(TableElementList item);

        int IndexOf(TableElementList item);

        void Insert(int index, TableElementList item);

        void Remove(TableElementList item);

        void RemoveAt(int index);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        ITableElementList this[int index] { get; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("90B925EE-4E92-4B06-ACC4-69552100411A")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ITableElementListList))]
    public class TableElementListList : ITableElementListList
    {
        #region Private Fields

        private readonly List<TableElementList> _list;

        #endregion Private Fields

        #region Public Constructors

        public TableElementListList(List<TableElementList> list)
        {
            _list = list;
        }

        public TableElementListList(TableElementList[] array)
        {
            _list = array.ToList();
        }

        public TableElementListList()
        {
            _list = new List<TableElementList>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(TableElementList item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(TableElementList item)
        {
            return _list.Contains(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(TableElementList item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TableElementList item)
        {
            _list.Insert(index, item);
        }

        public void Remove(TableElementList item)
        {
            _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public string ToIdentString(int identation)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(TableElementListList)}: Count = {Count}");
            foreach (var item in _list)
                sb.AppendLine(item.ToIdentString(identation + Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation)}/{nameof(TableElementListList)}");

            return sb.ToString();
        }

        public override string ToString() => ToIdentString(0);

        #endregion Public Methods

        #region Public Properties

        public int Count => _list.Count;

        #endregion Public Properties

        #region Public Indexers

        ITableElementList ITableElementListList.this[int index] => this[index];

        public TableElementList this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        #endregion Public Indexers
    }
}
