using System;
using System.Windows;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class BooleanComparatorViewModel : ComparatorViewModelBase
    {
        public bool? Value { get; set; }

        //=================================================================
        // From ComparatorViewModelBase
        //=================================================================
        public override bool IsNull
        {
            get { return Value == null; }
            set
            {
                if (value)
                    Value = null;
                else
                    throw new ApplicationException("IsNull cannot be set to false");
            }
        }

        public override ComparatorBase Comparator
        {
            set
            {
                _comparator = value;

                BooleanComparator cmp = (BooleanComparator)_comparator;
                if (cmp == null)
                    Value = true;
                else
                    Value = cmp.value;
            }
        }

        //=================================================================
        // View et ViewModel
        //=================================================================
        public static ComparatorViewModelBase GetViewModel()
        {
            BooleanComparatorViewModel vm = new BooleanComparatorViewModel();

            return vm;
        }

        public override Window GetUI()
        {
            BooleanComparatorDialog dlg = new BooleanComparatorDialog();
            dlg.DataContext = this;
            return dlg;
        }

        //=================================================================
        // 
        //=================================================================
        public void Synchronize()
        {
            if (_comparator == null)
                _comparator = new BooleanComparator();

            BooleanComparator cmp = (BooleanComparator)_comparator;
            cmp.value = (Value == true);
        }

    }
}
