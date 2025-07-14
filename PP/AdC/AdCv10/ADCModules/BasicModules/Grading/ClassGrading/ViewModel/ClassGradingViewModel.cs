using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace BasicModules.Grading.ClassGrading.ViewModel
{
    internal class ClassGradingViewModel : ObservableRecipient
    {
        private ClassGradingParameter _parameter;
        public ObservableCollection<ClassGradingRule> Rules => _parameter.ClassGradingRules;
        public List<string> DefectClasses { get; private set; }

        public ClassGradingViewModel(ClassGradingParameter parameter)
        {
            _parameter = parameter;
        }

        private ClassGradingRule _selectedRule;
        public ClassGradingRule SelectedRule
        {
            get => _selectedRule;
            set
            {
                if (_selectedRule != value)
                {
                    _selectedRule = value;
                    OnPropertyChanged();
                    DeleteCommand.NotifyCanExecuteChanged();
                }
            }
        }

        #region

        private AutoRelayCommand _loadCommand;
        public AutoRelayCommand LoadCommand
        {
            get
            {
                return _loadCommand ?? (_loadCommand = new AutoRelayCommand(
              () =>
              {
                  _parameter.Synchronize();
                  DefectClasses = new List<string>(_parameter.DefectClassList);
                  OnPropertyChanged(nameof(DefectClasses));
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _addRuleCommand;
        public AutoRelayCommand AddRuleCommand
        {
            get
            {
                return _addRuleCommand ?? (_addRuleCommand = new AutoRelayCommand(
              () =>
              {
                  Rules.Add(new ClassGradingRule() { BiggerThan = 100, Criteria = GradingRule.GradingCriteria.Count, GradingMark = ADCEngine.Recipe.Grading.Reject, DefectClass = DefectClasses?.FirstOrDefault() });
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _deleteCommand;
        public AutoRelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new AutoRelayCommand(
              () =>
              {
                  Rules.Remove(SelectedRule);
              },
              () => { return SelectedRule != null; }));
            }
        }

        #endregion
    }
}
