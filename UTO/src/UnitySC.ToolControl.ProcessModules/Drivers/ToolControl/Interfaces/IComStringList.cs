using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true)]
    [Guid("6EF5D20E-0FC1-478E-A185-E426344BA210")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComStringList : IEnumerable<string>
    {
        #region Public Methods

        void Add(string item);

        void Clear();

        bool Contains(string item);

        int IndexOf(string item);

        void Insert(int index, string item);

        void Remove(string item);

        void RemoveAt(int index);

        string ToIdentString(int identation);

        #endregion Public Methods

        #region Public Properties

        int Count { get; }

        #endregion Public Properties

        #region Public Indexers

        string this[int index] { get; set; }

        #endregion Public Indexers
    }

    [ComVisible(true)]
    [Guid("AB718E40-77C7-4CAC-917F-238086A653FD")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IComStringList))]
    public class ComStringList : IComStringList
    {
        #region Private Fields

        private readonly List<string> _list;

        #endregion Private Fields

        #region Public Constructors

        public ComStringList(List<string> list)
        {
            _list = list;
        }

        public ComStringList(string[] array)
        {
            _list = array.ToList();
        }

        public ComStringList()
        {
            _list = new List<string>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(string item) => _list.Add(item);

        public void Clear() => _list.Clear();

        public bool Contains(string item) => _list.Contains(item);

        public IEnumerator GetEnumerator() => _list.GetEnumerator();

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(string item) => _list.IndexOf(item);

        public void Insert(int index, string item) => _list.Insert(index, item);

        public void Remove(string item) => _list.Remove(item);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        public string ToIdentString(int identation)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(ComStringList)}: Count = {Count}");
            foreach (var item in _list)
                sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}\"{item}\"");
            sb.AppendLine($"{new string(' ', identation)}/{nameof(ComStringList)}");

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

        public string this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        #endregion Public Indexers
    }
}
