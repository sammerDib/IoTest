using System.Windows;

using AdcBasicObjects;

namespace PatternInspectionModule
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ListIDComparatorViewModel : ComparatorViewModelBase
    {
        private bool _isNull = false;

        public string ListStringIds
        {
            get { return _model.ToString(); }
            set { _model.UpdateListId(value); }

        }
        private CharacListID _model = null;

        //=================================================================
        // From ComparatorViewModelBase
        //=================================================================
        public override bool IsNull
        {
            get { return _isNull; }
            set { _isNull = value; }
        }

        public override ComparatorBase Comparator
        {
            set
            {
                _comparator = value;

                ListIDComparator cmp = (ListIDComparator)_comparator;
                if (cmp == null)
                {
                    _model = new CharacListID();
                }
                else
                {
                    _model = new CharacListID(cmp.ListId);
                }
            }
        }

        //=================================================================
        // View et ViewModel
        //=================================================================
        public static ComparatorViewModelBase GetViewModel()
        {
            ListIDComparatorViewModel vm = new ListIDComparatorViewModel();

            return vm;
        }

        public override Window GetUI()
        {
            ListIDComparatorDialog dlg = new ListIDComparatorDialog();
            dlg.DataContext = this;
            return dlg;
        }

        //=================================================================
        // 
        //=================================================================
        public void Synchronize()
        {
            if (_comparator == null)
                _comparator = new ListIDComparator();

            ListIDComparator cmp = (ListIDComparator)_comparator;
            cmp.ListId = _model.ListId;
        }
    }
}

