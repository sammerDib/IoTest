
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using MvvmValidation;

namespace Utils.ViewModel
{
    public class ValidationViewModelBase : ObservableRecipient, INotifyDataErrorInfo, IDataErrorInfo
    {
        #region constructors

        public ValidationViewModelBase(IMessenger messenger) : base(messenger)
        {
            Init();
        }

        public ValidationViewModelBase() : base()
        {
            Init();
        }


        private void Init()
        {
            PropertyChanged += OnPropertyChanged_VM;

            Validator = new ValidationHelper();

            NotifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(Validator);


            Validator.ResultChanged += (sender, ValidationResultChangedEventArgs) =>
            {

                OnPropertyChanged(nameof(HasErrors));
                OnPropertyChanged(nameof(Error));

            };
        }

        #endregion

        #region Gestion des modifications et  Surcharge de OnPropertyChanged

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


        private void OnPropertyChanged_VM(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(HasChanged))
                HasChanged = true;
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
        }


        /*    public override void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            {
                base.OnPropertyChanged(propertyName);
                HasChanged = true;
            }


            public override void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
            {
             //   base.OnPropertyChanged(propertyExpression);
                HasChanged = true;
            }
        */
        #endregion

        protected ValidationHelper Validator { get; private set; }
        private NotifyDataErrorInfoAdapter NotifyDataErrorInfoAdapter { get; set; }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
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

                MvvmValidation.ValidationResult vr = Validator.ValidateAll();
                return string.Join("\n", vr.ErrorList.Select(e => e.ErrorText));
            }

        }


        public string this[string columnName]
        {
            get
            {
                MvvmValidation.ValidationResult vr = Validator.Validate(columnName);
                return string.Join("\n", vr.ErrorList.Select(e => e.ErrorText));
            }

        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { NotifyDataErrorInfoAdapter.ErrorsChanged += value; }
            remove { NotifyDataErrorInfoAdapter.ErrorsChanged -= value; }
        }
    }



    public static class ValidationViewModelBaseTools
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
