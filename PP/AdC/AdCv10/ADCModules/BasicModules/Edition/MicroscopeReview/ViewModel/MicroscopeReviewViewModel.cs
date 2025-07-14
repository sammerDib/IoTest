using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace BasicModules.Edition.MicroscopeReview.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class MicroscopeReviewViewModel : ObservableRecipient
    {
        private MicroscopeReviewParameter _parameter;
        public List<MicroscopeReviewClassViewModel> Classes { get; private set; }

        public MicroscopeReviewViewModel(MicroscopeReviewParameter parameter)
        {
            _parameter = parameter;
            Classes = new List<MicroscopeReviewClassViewModel>();
        }

        public bool SelectAll
        {
            get => Classes.All(x => x.UseReview);
            set
            {
                if (value != SelectAll)
                {
                    Classes.ForEach(x => x.UseReview = value);
                }
                OnPropertyChanged();
            }
        }

        public void ClassesChanged()
        {
            _parameter.ReportChange();
            OnPropertyChanged(nameof(SelectAll));
        }

        #region commands

        private AutoRelayCommand _loadCommand;
        public AutoRelayCommand LoadCommand
        {
            get
            {
                return _loadCommand ?? (_loadCommand = new AutoRelayCommand(
              () =>
              {
                  _parameter.Synchronize();
                  Classes = _parameter.MicroscopeReviewClassses.Select(x => new MicroscopeReviewClassViewModel(x.Value, () => { ClassesChanged(); })).ToList();
                  OnPropertyChanged(nameof(Classes));
                  OnPropertyChanged(nameof(SelectAll));
              },
              () => { return true; }));
            }
        }

        #endregion
    }
}
