using System;
using System.Threading.Tasks;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class RecipeWizardStepBaseVM : VMSharedBase, IWizardNavigationItem, INavigable
    {
        #region IWizardNavigationItem Implementation

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isMeasure;

        public bool IsMeasure
        {
            get { return _isMeasure; }
            set
            {
                if (_isMeasure != value)
                {
                    _isMeasure = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isValidated;

        public bool IsValidated
        {
            get { return _isValidated; }
            set
            {
                if (_isValidated != value)
                {
                    _isValidated = value;
                    OnPropertyChanged();

                }
            }
        }

        #endregion IWizardNavigationItem Implementation

        #region INavigable Implementation

        public virtual Task PrepareToDisplay()
        {
            return Task.CompletedTask;
        }

        public virtual bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            return true;
        }

        #endregion INavigable Implementation

        public IWizardNavigationItem GetWizardPage(Type wizardPageType)
        {
            return ((IWizardNavigationItem)ServiceLocator.NavigationManager.GetPage(wizardPageType));
        }
    }
}
