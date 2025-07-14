using System;
using System.Collections.Generic;
using System.Drawing;

namespace AdcTools
{
    public class DeepGrid<T>
    {
        private Grid<T>[] _grid;
        private long[] _gridAreaLimit;

        //=================================================================
        // Properties
        //=================================================================
        public Rectangle FullExtent { get { return _grid[0].FullExtent; } }
        public int Depth { get; private set; }

        public int MaxCount
        {
            get
            {
                int count = 0;
                foreach (Grid<T> g in _grid)
                    count += g.MaxCount;
                return count;
            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (Grid<T> g in _grid)
                    count += g.Count;
                return count;
            }
        }


        //=================================================================
        // Construction
        //=================================================================
        public DeepGrid()
        {
        }

        public DeepGrid(Size size, int step = 1, int depth = 2, int factor = 10)
        {
            Rectangle rect = new Rectangle(new Point(0, 0), size);
            Init(rect, step);
        }

        public DeepGrid(Rectangle rect, int step = 1, int depth = 2, int factor = 10)
        {
            Init(rect, step);
        }

        public void Init(Rectangle rect, int step = 1, int depth = 2, int factor = 10)
        {
            Depth = depth;
            _grid = new Grid<T>[depth];
            _gridAreaLimit = new long[depth];

            for (int i = 0; i < depth; i++)
            {
                _grid[i] = new Grid<T>(rect, step);
                step *= factor;
                _gridAreaLimit[i] = step * step;
            }

            _gridAreaLimit[depth - 1] = rect.Area();
        }

        //=================================================================
        // Insertion
        //=================================================================
        public void Insert(Point p, T t)
        {
            _grid[0].Insert(p, t);
        }

        public void Insert(Rectangle r, T t)
        {
            long area = r.Area();

            // Calcul du niveau de grille
            //...........................
            int level;
            for (level = 0; level < Depth; level++)
            {
                if (area <= _gridAreaLimit[level])
                {
                    break;
                }
            }

            if (level == Depth)
                throw new ApplicationException("Insertion out of DeepGrid");

            // Insertion dans une seule sous-grille
            //.....................................
            _grid[level].Insert(r, t);
        }

        //=================================================================
        // Get
        //=================================================================
        public List<T> GetAll()
        {
            return Get(FullExtent);
        }

        //-----------------------------------------------------------------
        // Get a partir d'un point
        //-----------------------------------------------------------------
        public List<T> Get(Point p)
        {
            List<T> list = new List<T>();
            foreach (Grid<T> g in _grid)
            {
                IEnumerable<T> l = g.Get(p);
                list.AddRange(l);
            }

            return list;
        }

        //-----------------------------------------------------------------
        // Get a partir d'un rectangle
        //-----------------------------------------------------------------
        public List<T> Get(Rectangle r)
        {
            List<T> list = new List<T>();
            foreach (Grid<T> g in _grid)
            {
                IEnumerable<T> l = g.Get(r);
                list.AddRange(l);
            }

            return list;
        }

        //-----------------------------------------------------------------
        // Get a partir d'une region
        //-----------------------------------------------------------------
        public IEnumerable<T> Get(Region region)
        {
            IEnumerable<Rectangle> rects = region.ToRectangles();

            HashSet<T> list = new HashSet<T>();
            foreach (Grid<T> g in _grid)
            {
                foreach (Rectangle rect in rects)
                {
                    IEnumerable<T> l = g.Get(rect);
                    list.UnionWith(l);
                }
            }

            return list;
        }


        //=================================================================
        // Remove
        //=================================================================

        //-----------------------------------------------------------------
        // TryRemove
        //-----------------------------------------------------------------
        public bool TryRemove(Point p, T t)
        {
            return _grid[0].TryRemove(p, t);
        }

        public bool TryRemove(Rectangle r, T t)
        {
            bool bRemoved = false;

            long area = r.Area();

            for (int i = 0; i < Depth; i++)
            {
                if (area <= _gridAreaLimit[i])
                {
                    bRemoved = _grid[i].TryRemove(r, t);
                    break;
                }
            }

            return bRemoved;
        }

        //-----------------------------------------------------------------
        // Remove
        //-----------------------------------------------------------------
        public void Remove(Point p, T t)
        {
            _grid[0].Remove(p, t);
        }

        public void Remove(Rectangle r, T t)
        {
            bool bRemoved = TryRemove(r, t);

            if (!bRemoved)
                throw new ApplicationException("can not remove from grid");
        }


    }
}
