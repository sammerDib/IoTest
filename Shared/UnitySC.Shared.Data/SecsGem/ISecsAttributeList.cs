using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.Shared.Data.SecsGem
{
    [ComVisible(true)]
    [Guid("345F10A1-478E-4E46-90A3-D4306BA822D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISecsAttributeList
    {
        #region Public Methods

        void Add(ISecsAttribute item);

        void Clear();

        bool Contains(ISecsAttribute item);

        int IndexOf(ISecsAttribute item);

        void Insert(int index, ISecsAttribute item);

        void Remove(ISecsAttribute item);

        void RemoveAt(int index);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        ISecsAttribute this[int index] { get; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("2EAF892F-4C5E-462D-B29D-2100C236C0E3")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISecsAttributeList))]
    public class SecsAttributeList : ISecsAttributeList
    {
        #region Private Fields

        private readonly List<SecsAttribute> _list;

        #endregion Private Fields

        #region Public Constructors

        public SecsAttributeList(List<SecsAttribute> list)
        {
            _list = list;
        }

        public SecsAttributeList(SecsAttribute[] array)
        {
            _list = array.ToList();
        }

        public SecsAttributeList()
        {
            _list = new List<SecsAttribute>();
        }

        #endregion Public Constructors

        #region Public Methods
        public void Add(SecsAttribute item) => _list.Add(item);

        void ISecsAttributeList.Add(ISecsAttribute item) => Add(new SecsAttribute(item));


        public void Clear() => _list.Clear();

        public bool Contains(SecsAttribute item) => _list.Contains(item);

        public IEnumerator GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(SecsAttribute item) => _list.IndexOf(item);

        public void Insert(int index, SecsAttribute item) => _list.Insert(index, item);

        public void Remove(SecsAttribute item) => _list.Remove(item);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        public string ToIdentString(int identation)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(SecsAttributeList)}: Count = {Count}");
            foreach (var item in _list)
                sb.AppendLine($"{item.ToIdentString(identation + Constants.StringIdentation)}");
            sb.AppendLine($"{new string(' ', identation)}/{nameof(SecsAttributeList)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        bool ISecsAttributeList.Contains(ISecsAttribute item) => Contains(new SecsAttribute(item));

        int ISecsAttributeList.IndexOf(ISecsAttribute item) => IndexOf(new SecsAttribute(item));

        void ISecsAttributeList.Insert(int index, ISecsAttribute item) => Insert(index, new SecsAttribute(item));

        void ISecsAttributeList.Remove(ISecsAttribute item) => Remove(new SecsAttribute(item));

        #endregion Public Methods

        #region Public Properties

        public int Count => _list.Count;

        #endregion Public Properties

        #region Public Indexers

        ISecsAttribute ISecsAttributeList.this[int index] => this[index];

        public SecsAttribute this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        #endregion Public Indexers
    }
}
