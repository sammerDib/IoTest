using System;
using System.Collections.Generic;
using System.Drawing;

namespace AdcTools
{
    public class Grid<T>
    {
        private List<T>[,] grid = new List<T>[0, 0];
        public int Step { get; private set; }
        public Size SizeInSteps { get; private set; }
        public Rectangle FullExtent { get; private set; }
        private Point _offset;

        private int _count = 0;
        public int MaxCount { get; private set; }
        public int Count
        {
            get { return _count; }
            private set
            {
                if (value > MaxCount)
                    MaxCount = value;
                _count = value;
            }
        }

        //=================================================================
        // Construction
        //=================================================================
        public Grid()
        {
        }

        public Grid(Size size, int step = 1)
        {
            Rectangle rect = new Rectangle(new Point(0, 0), size);
            Init(rect, step);
        }

        public Grid(Rectangle rect, int step = 1)
        {
            Init(rect, step);
        }

        public void Init(Rectangle rect, int step = 1)
        {
            Count = 0;
            Step = step;
            FullExtent = rect.SnapToGrid(Step);

            SizeInSteps = FullExtent.Size.DividedBy(Step);
            grid = new List<T>[SizeInSteps.Width, SizeInSteps.Height];

            _offset = FullExtent.TopLeft().DividedBy(step);
            _offset.X = -_offset.X;
            _offset.Y = -_offset.Y;
        }

        //=================================================================
        // Insertion
        //=================================================================
        private void _Insert(Point p, T t)
        {
            p = p.DividedBy(Step);
            p.Offset(_offset);

            if (grid[p.X, p.Y] == null)
                grid[p.X, p.Y] = new List<T>();

            grid[p.X, p.Y].Add(t);
        }

        public void Insert(Point p, T t)
        {
            Count++;
            _Insert(p, t);
        }

        public void Insert(Rectangle r, T t)
        {
            Count++;
            r = r.SnapToGrid(Step);
            if (r.Width <= 0 || r.Height <= 0)
                throw new ApplicationException("insertion of an empty rectangle in the grid");

            Point p = new Point();
            for (p.X = r.Left; p.X < r.Right; p.X += Step)
            {
                for (p.Y = r.Top; p.Y < r.Bottom; p.Y += Step)
                {
                    _Insert(p, t);
                }
            }
        }

        //=================================================================
        // Get
        //=================================================================
        public IEnumerable<T> GetAll()
        {
            return Get(FullExtent);
        }


        //-----------------------------------------------------------------
        // Get a partir d'un point
        //-----------------------------------------------------------------
        public IEnumerable<T> Get(Point p)
        {
            HashSet<T> hashset = new HashSet<T>();

            p = p.DividedBy(Step);
            p.Offset(_offset);

            if (grid[p.X, p.Y] != null)
                hashset.UnionWith(grid[p.X, p.Y]);

            return hashset;
        }

        //-----------------------------------------------------------------
        // Get à partir d'un rectangle
        //-----------------------------------------------------------------
        public IEnumerable<T> Get(Rectangle r)
        {
            HashSet<T> hashset = new HashSet<T>();

            r = r.SnapToGrid(Step);

            Point p = new Point();
            for (p.X = r.Left; p.X < r.Right; p.X += Step)
            {
                for (p.Y = r.Top; p.Y < r.Bottom; p.Y += Step)
                {
                    Point p2 = p.DividedBy(Step);
                    p2.Offset(_offset);
                    if (grid[p2.X, p2.Y] != null)
                        hashset.UnionWith(grid[p2.X, p2.Y]);
                }
            }

            return hashset;
        }

        //-----------------------------------------------------------------
        // Get à partir d'une region
        //-----------------------------------------------------------------
        public IEnumerable<T> Get(Region region)
        {
            IEnumerable<Rectangle> rects = region.ToRectangles();

            HashSet<T> hashset = new HashSet<T>();
            foreach (Rectangle rect in rects)
            {
                IEnumerable<T> l = Get(rect);
                hashset.UnionWith(l);
            }
            return hashset;
        }

        //=================================================================
        // Remove
        //=================================================================
        private bool _Remove(Point p, T t)
        {
            p = p.DividedBy(Step);
            p.Offset(_offset);

            if (grid[p.X, p.Y] == null)
                return false;

            return grid[p.X, p.Y].Remove(t);
        }

        //-----------------------------------------------------------------
        // Remove a partir d'un point
        //-----------------------------------------------------------------
        public bool TryRemove(Point p, T t)
        {
            Count--;

            return _Remove(p, t);
        }

        public void Remove(Point p, T t)
        {
            bool bRemoved = TryRemove(p, t);

            if (!bRemoved)
                throw new ApplicationException("can not remove from grid");
        }

        //-----------------------------------------------------------------
        // Remove a partir d'un rectangle
        //-----------------------------------------------------------------
        public bool TryRemove(Rectangle r, T t)
        {
            bool bRemoved = false;

            Count--;
            r = r.SnapToGrid(Step);

            Point p = new Point();
            for (p.X = r.Left; p.X < r.Right; p.X += Step)
            {
                for (p.Y = r.Top; p.Y < r.Bottom; p.Y += Step)
                {
                    bRemoved = bRemoved | _Remove(p, t);
                }
            }

            return bRemoved;
        }

        public void Remove(Rectangle r, T t)
        {
            bool bRemoved = TryRemove(r, t);

            if (!bRemoved)
                throw new ApplicationException("can not remove from grid");
        }


    }
}
