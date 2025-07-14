using System.Collections.ObjectModel;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Defect
{
    public abstract class DefectCategoriesVM : ResultWaferVM
    {
        protected DefectCategoriesVM(IResultDisplay resDisplay) : base(resDisplay)
        {
        }

        public abstract int TotalCountSelected { get; set; }
        public abstract int ActiveDefects { get; set; }

        public abstract ObservableCollection<DefectCategoryVM> DefectCategories { get; set; }
        public abstract AutoRelayCommand<DefectCategoryVM> OnUpdateCategoriesCommand { get; }
    }
}
