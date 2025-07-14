using System;
using System.Windows;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class StringComparatorViewModel : ComparatorViewModelBase
    {
        public string Value { get; set; }

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

                StringComparator cmp = (StringComparator)_comparator;
                if (cmp == null)
                    Value = "";
                else
                    Value = cmp.Value;
            }
        }

        //=================================================================
        // View et ViewModel
        //=================================================================
        public static ComparatorViewModelBase GetViewModel()
        {
            StringComparatorViewModel vm = new StringComparatorViewModel();

            return vm;
        }

        public override Window GetUI()
        {
            StringComparatorDialog dlg = new StringComparatorDialog();
            dlg.DataContext = this;
            return dlg;
        }

        //=================================================================
        // 
        //=================================================================
        public void Synchronize()
        {
            if (_comparator == null)
                _comparator = new StringComparator();

            StringComparator cmp = (StringComparator)_comparator;
            cmp.Value = Value;
        }

    }
}
