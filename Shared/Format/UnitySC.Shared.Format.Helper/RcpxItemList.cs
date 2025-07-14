using System.Collections.Generic;
using System.Linq;

namespace UnitySC.Shared.Format.Helper
{
    public class RcpxItemList<T> where T : RectpxItem
    {
        private readonly Dictionary<string, List<T>> _map = new Dictionary<string, List<T>>();
        public Dictionary<string, List<T>> Map { get { return _map; } }

        private List<T> _sortedItems = null;
        public List<T> ItemList { get { return _sortedItems; } }

        public void AddItem(string sKey, T item)
        {
            if (!_map.ContainsKey(sKey))
            {
                _map.Add(sKey, new List<T>(1024));
            }
            _map[sKey].Add(item);
        }

        public void NotifyAddCompleted()
        {
            _sortedItems = SortByArea();
        }

        public bool FindItem(float fX_px, float fY_px, string[] keysArray, out T itFound)
        {
            bool bDefectFind = false;
            itFound = null;
            float fDistMinimum = float.MaxValue;
            if (keysArray == null)
            {
                keysArray = _map.Keys.ToArray();
            }
            foreach (string sKey in keysArray)
            {
                if (_map.ContainsKey(sKey))
                {
                    foreach (var item in _map[sKey])
                    {
                        if (item.Rectpx.Contains(fX_px, fY_px))
                        {
                            bDefectFind = true;
                            float dDist = item.DistancepxFrom(fX_px, fY_px);
                            if (dDist <= fDistMinimum)
                            {
                                if (dDist == fDistMinimum)
                                {
                                    // keep the smaller one
                                    if (item.Areapx > itFound.Areapx)
                                        continue;
                                }
                                fDistMinimum = dDist;
                                itFound = item;
                            }
                        }
                    }
                }
            }
            return bDefectFind;
        }

        public bool FindItemWithModifiers(float fX_px, float fY_px, string[] keysArray, out T itFound, float displayFactor, float displayMinSize_px)
        {
            bool bDefectFind = false;
            itFound = null;
            float fDistMinimum = float.MaxValue;
            if (keysArray == null)
            {
                keysArray = _map.Keys.ToArray();
            }
            foreach (string sKey in keysArray)
            {
                if (_map.ContainsKey(sKey))
                {
                    foreach (var item in _map[sKey])
                    {
                        if (item.ApplyModifiers(displayFactor,displayMinSize_px).Contains(fX_px, fY_px))
                        {
                            bDefectFind = true;
                            float dDist = item.DistancepxFrom(fX_px, fY_px);
                            if (dDist <= fDistMinimum)
                            {
                                if (dDist == fDistMinimum)
                                {
                                    // keep the smaller one
                                    if (item.Areapx > itFound.Areapx)
                                        continue;
                                }
                                fDistMinimum = dDist;
                                itFound = item;
                            }
                        }
                    }
                }
            }
            return bDefectFind;
        }

        private List<T> SortByArea()
        {
            var displayList = new List<T>(4096);
            foreach (string sKey in _map.Keys.ToArray())
            {
                displayList.AddRange(_map[sKey]);
            }
            displayList.Sort(); // re-arrange item so the bigger will draw in first place
            return displayList;
        }
    }
}
