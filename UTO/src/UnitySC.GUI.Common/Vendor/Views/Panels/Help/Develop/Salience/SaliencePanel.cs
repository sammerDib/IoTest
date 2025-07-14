using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Saliences;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Salience
{
    public class SaliencePanel : BusinessPanel
    {
        #region Constructor & Setup & Cleanup

        public SaliencePanel() : this("Saliences", IconFactory.PathGeometryFromRessourceKey("SalienceIcon"))
        {
        }

        public SaliencePanel(string id, IIcon icon = null) : base(id, icon)
        {
        }

        #endregion Constructor & Setup & Cleanup

        #region Properties

        public int? TotalAlarmSalienceCount => SelectedBusinessPanel?.Saliences.Count(SalienceType.Alarm);

        public int? TotalCautionSalienceCount => SelectedBusinessPanel?.Saliences.Count(SalienceType.Caution);

        public int? TotalUnfinishedTaskSalienceCount => SelectedBusinessPanel?.Saliences.Count(SalienceType.UnfinishedTask);

        public int? TotalUserAttentionSalienceCount => SelectedBusinessPanel?.Saliences.Count(SalienceType.UserAttention);

        public int? TotalSalienceCount
        {
            get
            {
                return TotalAlarmSalienceCount
                       + TotalCautionSalienceCount
                       + TotalUnfinishedTaskSalienceCount
                       + TotalUserAttentionSalienceCount;
            }
        }

        #endregion Properties

        public IEnumerable<BusinessPanel> BusinessPanels => AgilControllerApplication.Current.UserInterface.BusinessPanels;


        private BusinessPanel _selectedBusinessPanel;

        public BusinessPanel SelectedBusinessPanel
        {
            get { return _selectedBusinessPanel; }
            set
            {
                if (_selectedBusinessPanel == value) return;

                if (_selectedBusinessPanel != null)
                {
                    _selectedBusinessPanel.Saliences.PropertyChanged -= SaliencesOnPropertyChanged;
                }

                _selectedBusinessPanel = value;

                if (_selectedBusinessPanel != null)
                {
                    _selectedBusinessPanel.Saliences.PropertyChanged += SaliencesOnPropertyChanged;
                }

                OnPropertyChanged(null);
            }
        }

        private void SaliencesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalAlarmSalienceCount));
            OnPropertyChanged(nameof(TotalCautionSalienceCount));
            OnPropertyChanged(nameof(TotalUnfinishedTaskSalienceCount));
            OnPropertyChanged(nameof(TotalUserAttentionSalienceCount));
            OnPropertyChanged(nameof(TotalSalienceCount));
        }

        #region AddCommand

        private ICommand _addCommand;

        public ICommand AddCommand => _addCommand ??= new DelegateCommand<SalienceType?>(AddCommandExecute, AddCommandCanExecute);

        private bool AddCommandCanExecute(SalienceType? salienceType)
        {
            return SelectedBusinessPanel != null;
        }

        private void AddCommandExecute(SalienceType? salienceType)
        {
            if (salienceType != null)
            {
                SelectedBusinessPanel.Saliences.Add(salienceType.Value, 1);
            }
        }

        #endregion

        #region RemoveCommand

        private ICommand _removeCommand;

        public ICommand RemoveCommand => _removeCommand ??= new DelegateCommand<SalienceType?>(RemoveCommandExecute, RemoveCommandCanExecute);

        private bool RemoveCommandCanExecute(SalienceType? salienceType)
        {
            return SelectedBusinessPanel != null
                   && salienceType.HasValue
                   && SelectedBusinessPanel.Saliences.Count(salienceType.Value) > 0;
        }

        private void RemoveCommandExecute(SalienceType? salienceType)
        {
            if (salienceType != null)
            {
                SelectedBusinessPanel.Saliences.Remove(salienceType.Value);
            }
        }

        #endregion

        #region ResetCommand

        private ICommand _resetCommand;

        public ICommand ResetCommand => _resetCommand ??= new DelegateCommand<SalienceType?>(ResetCommandExecute, ResetCommandCanExecute);

        private bool ResetCommandCanExecute(SalienceType? salienceType)
        {
            return SelectedBusinessPanel != null
                   && salienceType.HasValue
                   && SelectedBusinessPanel.Saliences.Count(salienceType.Value) > 0;
        }

        private void ResetCommandExecute(SalienceType? salienceType)
        {
            if (salienceType != null)
            {
                SelectedBusinessPanel.Saliences.Reset(salienceType.Value);
            }
        }

        #endregion

        #region ResetAllCommand

        private ICommand _resetAllCommand;

        public ICommand ResetAllCommand => _resetAllCommand ??= new DelegateCommand(ResetAllCommandExecute, ResetAllCommandCanExecute);

        private bool ResetAllCommandCanExecute()
        {
            return SelectedBusinessPanel != null
                   && (SelectedBusinessPanel.Saliences.AlarmCount > 0
                       || SelectedBusinessPanel.Saliences.CautionCount > 0
                       || SelectedBusinessPanel.Saliences.UnfinishedTaskCount > 0
                       || SelectedBusinessPanel.Saliences.UserAttentionCount > 0);
        }

        private void ResetAllCommandExecute()
        {
            SelectedBusinessPanel.Saliences.ResetAll();
        }

        #endregion
    }
}
