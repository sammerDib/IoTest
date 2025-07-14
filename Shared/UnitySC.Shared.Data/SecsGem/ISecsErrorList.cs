using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.Shared.Data.SecsGem
{
    [ComVisible(true)]
    [Guid("2D13FC26-F1B5-49A1-999E-6083EAE34765")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISecsErrorList
    {
        #region Public Methods

        void Add(SecsError item);

        void Clear();

        bool Contains(SecsError item);

        int IndexOf(SecsError item);

        void Insert(int index, SecsError item);

        void Remove(SecsError item);

        void RemoveAt(int index);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        ISecsError this[int index] { get; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("E4026235-B6B4-4207-B01F-5BB4058037EA")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISecsErrorList))]
    public class SecsErrorList : ISecsErrorList
    {
        #region Private Fields

        private readonly List<SecsError> _list;

        #endregion Private Fields

        #region Public Constructors

        public SecsErrorList(List<SecsError> list)
        {
            _list = list;
        }

        public SecsErrorList(SecsError[] array)
        {
            _list = array.ToList();
        }

        public SecsErrorList()
        {
            _list = new List<SecsError>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(SecsError item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(SecsError item)
        {
            return _list.Contains(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(SecsError item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, SecsError item)
        {
            _list.Insert(index, item);
        }

        public void Remove(SecsError item)
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
            sb.AppendLine($"{new string(' ', identation)}{nameof(SecsErrorList)}: Count = {Count}");
            foreach (var item in _list)
                sb.AppendLine(item.ToIdentString(identation + Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation)}/{nameof(SecsErrorList)}");

            return sb.ToString();
        }

        public override string ToString() => ToIdentString(0);

        #endregion Public Methods

        #region Public Properties

        public int Count => _list.Count;

        #endregion Public Properties

        #region Public Indexers

        ISecsError ISecsErrorList.this[int index] => this[index];

        public SecsError this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        #endregion Public Indexers
    }
}
