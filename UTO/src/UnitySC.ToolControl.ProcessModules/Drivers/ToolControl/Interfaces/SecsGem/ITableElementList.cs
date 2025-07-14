using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("9E980F5D-0E6C-4174-8D63-5343B8530173")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITableElementList
    {
        #region Public Methods

        void Add(TableElement item);

        void Clear();

        bool Contains(TableElement item);

        int IndexOf(TableElement item);

        void Insert(int index, TableElement item);

        void Remove(TableElement item);

        void RemoveAt(int index);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        ITableElement this[int index] { get; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("7567D8FE-FBE4-4474-8026-7AB8B20216AD")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ITableElementList))]
    public class TableElementList : ITableElementList
    {
        #region Private Fields

        private readonly List<TableElement> _list;

        #endregion Private Fields

        #region Public Constructors

        public TableElementList(List<TableElement> list)
        {
            _list = list;
        }

        public TableElementList(TableElement[] array)
        {
            _list = array.ToList();
        }

        public TableElementList()
        {
            _list = new List<TableElement>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(TableElement item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(TableElement item)
        {
            return _list.Contains(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(TableElement item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TableElement item)
        {
            _list.Insert(index, item);
        }

        public void Remove(TableElement item)
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
            sb.AppendLine($"{new string(' ', identation)}{nameof(TableElementList)}: Count = {Count}");
            foreach (var item in _list)
                sb.AppendLine(item.ToIdentString(identation + Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation)}/{nameof(TableElementList)}");

            return sb.ToString();
        }

        public override string ToString() => ToIdentString(0);

        #endregion Public Methods

        #region Public Properties

        public int Count => _list.Count;

        #endregion Public Properties

        #region Public Indexers

        ITableElement ITableElementList.this[int index] => this[index];

        public TableElement this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        #endregion Public Indexers
    }
}
