using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmValidation;

namespace UnitySC.Shared.UI.ViewModel
{
    public class ValidationViewModelBase : ObservableObject, INotifyDataErrorInfo, IDataErrorInfo
    {
        #region Gestion des modifications et  Surcharge de OnPropertyChanged()

        private bool _hasChanged = false;

        public bool HasChanged
        {
            get => _hasChanged;
            set
            {
                _hasChanged = value;
                base.OnPropertyChanged();
            }
        }

        private bool _isNew = false;

        public bool IsNew
        {
            get => _isNew;
            set
            {
                _isNew = value;
                base.OnPropertyChanged();
            }
        }

        // This function are needed for ADC project, uncomment and fix it in order to build ADC
        /*public override void OnPropertyChanged()<T>([CallerMemberName] string propertyName = null, T oldValue = default(T), T newValue = default(T), bool broadcast = false)
        {
            base.OnPropertyChanged()<T>(propertyName, oldValue, newValue, broadcast);
            HasChanged = true;
        }

        public override void OnPropertyChanged()<T>(Expression<Func<T>> propertyExpression, T oldValue, T newValue, bool broadcast)
        {
            base.OnPropertyChanged()<T>(propertyExpression, oldValue, newValue, broadcast);
            HasChanged = true;
        }

        public override void OnPropertyChanged()([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(nameof(propertyName));
            HasChanged = true;
        }

        public override void OnPropertyChanged()<T>(Expression<Func<T>> propertyExpression)
        {
            base.OnPropertyChanged(nameof(propertyExpression));
            HasChanged = true;
        }*/

        #endregion Gestion des modifications et  Surcharge de OnPropertyChanged()

        protected ValidationHelper Validator { get; private set; }
        private NotifyDataErrorInfoAdapter NotifyDataErrorInfoAdapter { get; set; }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        // This function are needed for ADC project, uncomment and fix it in order to build ADC
        /*
        public ValidationObservableRecipient(IMessenger messenger) : base(messenger)
        {
            Init();
        }
        */

        public ValidationViewModelBase() : base()
        {
            Init();
        }

        private void Init()
        {
            Validator = new ValidationHelper();

            NotifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(Validator);

            Validator.ResultChanged += (sender, ValidationResultChangedEventArgs) =>
            {
                OnPropertyChanged(nameof(HasErrors));
                OnPropertyChanged(nameof(Error));
            };

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(HasChanged))
            {
                HasChanged = true;
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return NotifyDataErrorInfoAdapter.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get { return NotifyDataErrorInfoAdapter.HasErrors; }
        }

        public string Error
        {
            get
            {
                var vr = Validator.ValidateAll();
                return string.Join("\n", vr.ErrorList.Select(e => e.ErrorText));
            }
        }

        public string this[string columnName]
        {
            get
            {
                var vr = Validator.Validate(columnName);
                return string.Join("\n", vr.ErrorList.Select(e => e.ErrorText));
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { NotifyDataErrorInfoAdapter.ErrorsChanged += value; }
            remove { NotifyDataErrorInfoAdapter.ErrorsChanged -= value; }
        }
    }

    public static class ValidationObservableRecipientTools
    {
        public static void InitEnded(this IEnumerable<ValidationViewModelBase> vms)
        {
            foreach (var vm in vms)
            {
                vm.HasChanged = false;
            }
        }

        public static void ChangeProcessed(this IEnumerable<ValidationViewModelBase> vms)
        {
            foreach (var vm in vms)
            {
                vm.HasChanged = false;
            }
        }

        public static void InitEnded(this ValidationViewModelBase vm)
        {
            vm.HasChanged = false;
        }

        public static void IsNew(this ValidationViewModelBase vm)
        {
            vm.IsNew = true;
        }

        public static void ChangeProcessed(this ValidationViewModelBase vm)
        {
            if (vm != null)
                vm.HasChanged = false;
        }
    }
}
