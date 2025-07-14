using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace UnitySC.Shared.Data.SecsGem
{
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

        int Count();

        #endregion Public Methods

        #region Public Indexers

        ISecsVariable this[int index] { get; }

        #endregion Public Indexers
    }

    [DataContract]
    public class SecsVariableList : ISecsVariableList
    {
        #region Private Fields

        private List<SecsVariable> _list;

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
            sb.AppendLine($"{new string(' ', identation)}{nameof(SecsVariableList)}: Count = {Count()}");
            foreach (var item in _list)
                sb.AppendLine(item.ToIdentString(identation + 2));
            sb.AppendLine($"{new string(' ', identation)}/{nameof(SecsVariableList)}");

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

        #region Public Properties

        [DataMember]
        public List<SecsVariable> List
        {
            get => _list;
            set
            {
                _list = value;
            }
        }
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
