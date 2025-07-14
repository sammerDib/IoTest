using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.ResultUI.Common.Enums;

namespace UnitySC.Shared.ResultUI.Common.ViewModel
{
    public abstract class LotViewHeaderVM : ObservableRecipient, IDisposable
    {
        /// <summary>
        /// Complete Label information identifying the selected result. (displayed in header title)
        /// </summary>
        protected string _selectedResultFullName = string.Empty;

        public string SelectedResultFullName
        {
            get => _selectedResultFullName;
            set
            {
                if (_selectedResultFullName != value)
                {
                    _selectedResultFullName = value;
                    OnSelectedResultFullNameChanged(value);
                    OnPropertyChanged();
                }
            }
        }

        protected abstract void OnSelectedResultFullNameChanged(string name);

        private List<KeyValuePair<LotView, string>> _lotViews;

        public List<KeyValuePair<LotView, string>> LotViews
        {
            get => _lotViews;
            set
            {
                if (_lotViews != value)
                {
                    _lotViews = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Selected lot view
        /// </summary>
        protected KeyValuePair<LotView, string> _lotSelectedView;

        public KeyValuePair<LotView, string> LotSelectedView
        {
            get => _lotSelectedView;
            set
            {
                if (_lotSelectedView.Key != value.Key || _lotSelectedView.Value != value.Value)
                {
                    _lotSelectedView = value;
                    if (value.Value != null)
                    {
                        OnLotSelectedViewChanged(value);
                    }
                    OnPropertyChanged();
                }
            }
        }

        protected abstract void OnLotSelectedViewChanged(KeyValuePair<LotView, string> selectedView);

        protected abstract void Cleanup();

        public virtual void Dispose()
        {
            Cleanup();
        }
    }
}
