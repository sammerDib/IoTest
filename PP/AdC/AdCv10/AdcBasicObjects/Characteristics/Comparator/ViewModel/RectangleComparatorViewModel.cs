using System;
using System.Windows;

using AdcTools;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class RectangleComparatorViewModel : ComparatorViewModelBase
    {
        public MinMaxViewModel X { get; private set; }
        public MinMaxViewModel Y { get; private set; }

        //=================================================================
        // From ComparatorViewModelBase
        //=================================================================
        public override bool IsNull
        {
            get { return X.IsNull && Y.IsNull; }
            set
            {
                if (value)
                {
                    X.HasMin = X.HasMax = false;
                    Y.HasMin = Y.HasMax = false;
                }
                else
                {
                    throw new ApplicationException("IsNull cannot be set to false");
                }
            }
        }

        public override ComparatorBase Comparator
        {
            set
            {
                _comparator = value;

                RectangleComparator cmp = (RectangleComparator)_comparator;
                if (cmp == null)
                {
                    X = new MinMaxViewModel(0, 0);
                    Y = new MinMaxViewModel(0, 0);
                }
                else
                {
                    X = new MinMaxViewModel(cmp.MinX, cmp.MaxX);
                    Y = new MinMaxViewModel(cmp.MinY, cmp.MaxY);
                }
            }
        }
        //=================================================================
        // View et ViewModel
        //=================================================================
        public static ComparatorViewModelBase GetViewModel()
        {
            RectangleComparatorViewModel vm = new RectangleComparatorViewModel();

            return vm;
        }

        public override Window GetUI()
        {
            RectangleComparatorDialog dlg = new RectangleComparatorDialog();
            dlg.DataContext = this;
            return dlg;
        }

        //=================================================================
        // 
        //=================================================================
        public void Synchronize()
        {
            if (_comparator == null)
                _comparator = new RectangleComparator();

            RectangleComparator cmp = (RectangleComparator)_comparator;
            cmp.MinX = (float)X.Min;
            cmp.MaxX = (float)X.Max;
            cmp.MinY = (float)Y.Min;
            cmp.MaxY = (float)Y.Max;
        }

    }
}
