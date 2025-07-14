using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("31F9E408-8CE7-4A67-A263-A63793BA87F8")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISecsVariableList
    {
        #region Public Methods

        void Add(SecsVariable item);

        void Clear();

        bool Contains(SecsVariable item);

        int IndexOf(SecsVariable item);

        void Insert(int index, SecsVariable item);

        void Remove(SecsVariable item);

        void RemoveAt(int index);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        ISecsVariable this[int index] { get; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("4994D209-560B-4CA8-8CDA-1BD77DF9FF01")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISecsVariableList))]
    public class SecsVariableList : ISecsVariableList
    {
        #region Private Fields

        private readonly List<SecsVariable> _list;

        #endregion Private Fields

        #region Public Constructors

        public SecsVariableList(List<SecsVariable> list)
        {
            _list = list;
        }

        public SecsVariableList(SecsVariable[] array)
        {
            _list = array.ToList();
        }

        public SecsVariableList()
        {
            _list = new List<SecsVariable>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(SecsVariable item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(SecsVariable item)
        {
            return _list.Contains(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(SecsVariable item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, SecsVariable item)
        {
            _list.Insert(index, item);
        }

        public void Remove(SecsVariable item)
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
            sb.AppendLine($"{new string(' ', identation)}{nameof(SecsVariableList)}: Count = {Count}");
            foreach (var item in _list)
                sb.AppendLine(item.ToIdentString(identation + 2));
            sb.AppendLine($"{new string(' ', identation)}/{nameof(SecsVariableList)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        public int Count => _list.Count;

        #endregion Public Properties

        #region Public Indexers

        ISecsVariable ISecsVariableList.this[int index] => this[index];

        public SecsVariable this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        #endregion Public Indexers
    }
}
