using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel
{
    public abstract class AnaPageNavigationVM : PageNavigationVM
    {
        protected AnaNavigationVM _navigationVM;

        public abstract bool IsVisibleInHeader { get; }

        public AnaPageNavigationVM(AnaNavigationVM navigationVM)
        {
            _navigationVM = navigationVM;
        }

        /// <summary>
        /// Goto Application 1 when it is selected
        /// </summary>
        private AutoRelayCommand _gotoNextPage;
        public AutoRelayCommand GotoNextPage
        {
            get
            {
                return _gotoNextPage ?? (_gotoNextPage = new AutoRelayCommand(
              () =>
              {
                  _navigationVM.NavigateToNextPage();
              },
              () =>
              {
                  return _navigationVM.CanNavigateToNextPage();
              })
              );
            }
        }



    }
}
