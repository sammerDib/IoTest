using System;
using System.Collections.ObjectModel;

namespace UnitySC.Shared.UI.Controls.WizardNavigationControl
{
    public delegate void CurrentPageChangedHandler(INavigable newPage, INavigable oldPage);

    public interface INavigationManager
    {
        event CurrentPageChangedHandler CurrentPageChanged;

        ObservableCollection<INavigable> AllPages { get; }

        INavigable CurrentPage { get; set; }

        void NavigateToPage(INavigable destPage);

        void NavigateToNextPage();

        void NavigateToPrevPage();

        INavigable GetFirstPage();

        INavigable GetNextPage();

        INavigable GetPrevPage();

        void RemoveAllPages();

        INavigable GetPage(Type pageType);
    }
}
