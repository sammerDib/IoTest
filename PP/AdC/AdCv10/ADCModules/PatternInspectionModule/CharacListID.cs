using System;
using System.Collections.Generic;

namespace PatternInspectionModule
{
    public class CharacListID
    {
        private List<int> _ll = null;
        public List<int> ListId { get { return _ll; } private set { _ll = value; } }

        public CharacListID()
        {
            ListId = new List<int>();
        }

        public CharacListID(IList<int> list)
        {
            ListId = new List<int>(list);
        }

        public CharacListID(String sFormatedListId)
        {
            ListId = new List<int>();
        }

        public void UpdateListId(string sFormatedListId)
        {
            // Format expected : "0+ 1 + 6+ 58+255"
            _ll.Clear();
            try
            {
                sFormatedListId = sFormatedListId.Replace(" ", String.Empty);
                String[] sTerm = sFormatedListId.Split('+');
                foreach (string sId in sTerm)
                {
                    int nId;
                    if (Int32.TryParse(sId, out nId))
                        _ll.Add(nId);
                }
            }
            catch (Exception e)
            {
                String sErrMsg = e.Message;
                _ll.Clear();
            }
        }

        public bool Contains(int nId)
        {
            return _ll.Contains(nId);
        }

        public override string ToString()
        {
            string sRes = String.Empty;
            if (_ll.Count > 0)
            {
                sRes = "";
                for (int nIdx = 0; nIdx < (_ll.Count - 1); nIdx++)
                {
                    sRes += _ll[nIdx].ToString();
                    sRes += "+ ";
                }
                sRes += _ll[_ll.Count - 1].ToString();
            }
            return sRes;
        }
    }
}
