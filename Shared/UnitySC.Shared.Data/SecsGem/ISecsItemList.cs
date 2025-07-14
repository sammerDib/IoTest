using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace UnitySC.Shared.Data.SecsGem
{
    public interface ISecsItemList : IEnumerable<ISecsItem>
    {
        #region Public Methods

        void Add(ISecsItem item);

        void Clear();

        bool Contains(ISecsItem item);

        int IndexOf(ISecsItem item);

        void Insert(int index, ISecsItem item);

        void Remove(ISecsItem item);

        void RemoveAt(int index);

        int Count();

        #endregion Public Methods

        #region Public Indexers

        ISecsItem this[int index] { get; }

        #endregion Public Indexers
    }

    [DataContract]
    public class SecsItemList : ISecsItemList
    {
        #region Private Fields

        private List<SecsItem> _list;

        #endregion Private Fields

        #region Public Constructors

        public SecsItemList(List<SecsItem> list)
        {
            _list = list;
        }

        public SecsItemList(SecsItem[] array)
        {
            _list = array.ToList();
        }

        public SecsItemList()
        {
            _list = new List<SecsItem>();
        }

        #endregion Public Constructors

        #region Properties

        [DataMember]
        List<SecsItem> List
        {
            get { return _list; }
            set
            {
                _list = value;
            }
        }

        #endregion

        #region Public Methods

        public void Add(SecsItem item)
        {
            _list.Add(item);
        }

        public void Add(object item)
        {
            if(item is SecsItem)
               _list.Add((SecsItem)item);
        }

        void ISecsItemList.Add(ISecsItem item) => Add(new SecsItem(item));

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(SecsItem item)
        {
            return _list.Contains(item);
        }

        bool ISecsItemList.Contains(ISecsItem item) => _list.Contains(item);

        public IEnumerator GetEnumerator() => _list.GetEnumerator();

        IEnumerator<ISecsItem> IEnumerable<ISecsItem>.GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(SecsItem item)
        {
            return _list.IndexOf(item);
        }

        int ISecsItemList.IndexOf(ISecsItem item) => IndexOf(new SecsItem(item));

        public void Insert(int index, SecsItem item)
        {
            _list.Insert(index, item);
        }

        void ISecsItemList.Insert(int index, ISecsItem item) => Insert(index, new SecsItem(item));

        public void Remove(SecsItem item)
        {
            _list.Remove(item);
        }

        void ISecsItemList.Remove(ISecsItem item) => Remove(new SecsItem(item));

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public string ToIdentString(int identation)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(SecsItemList)}: Count = {Count()}");
            foreach (var item in _list)
                sb.AppendLine(item.ToIdentString(identation + 2));
            sb.AppendLine($"{new string(' ', identation)}/{nameof(SecsItemList)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        public int Count()
        {
            return _list?.Count ?? 0;
        }

        #endregion Public Methods

        #region Public Indexers

        ISecsItem ISecsItemList.this[int index] { get => this[index]; }

        public SecsItem this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        #endregion Public Indexers
    }
}
