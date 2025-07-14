using System;
using System.Windows;

using AdcTools;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class RangeComparatorViewModel : ComparatorViewModelBase
    {
        public MinMaxViewModel Value { get; private set; }

        //=================================================================
        // From ComparatorViewModelBase
        //=================================================================
        public override bool IsNull
        {
            get { return Value.IsNull; }
            set
            {
                if (value)
                    Value.HasMin = Value.HasMax = false;
                else
                    throw new ApplicationException("IsNull cannot be set to false");
            }
        }

        public override ComparatorBase Comparator
        {
            set
            {
                _comparator = value;

                RangeComparator cmp = (RangeComparator)_comparator;
                if (cmp == null)
                    Value = new MinMaxViewModel(0, 0);
                else
                    Value = new MinMaxViewModel(cmp.Min, cmp.Max);
            }
        }

        //=================================================================
        // View et ViewModel
        //=================================================================
        public static ComparatorViewModelBase GetViewModel()
        {
            RangeComparatorViewModel vm = new RangeComparatorViewModel();

            return vm;
        }

        public override Window GetUI()
        {
            RangeComparatorDialog dlg = new RangeComparatorDialog();
            dlg.DataContext = this;
            return dlg;
        }

        //=================================================================
        // 
        //=================================================================
        public void Synchronize()
        {
            if (_comparator == null)
                _comparator = new RangeComparator();

            RangeComparator cmp = (RangeComparator)_comparator;
            cmp.Min = Value.Min;
            cmp.Max = Value.Max;
        }

    }
}
