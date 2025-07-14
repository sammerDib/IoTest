using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestExternalProcessing.ViewModel
{
    internal class MountainsVM : EPBaseVM
    {
        public MountainsVM() : base("Mountains")
        {
            _mountainsSupervisor = ClassLocator.Default.GetInstance<MountainsSupervisor>();
            ParametersVM = new MountainsExecutionParametersVM();
        }

        private List<string> _templates;
        public List<string> Templates
        {
            get => _templates; set { if (_templates != value) { _templates = value; OnPropertyChanged(); } }
        }

        private string _selectedTemplate;
        public string SelectedTemplate
        {
            get => _selectedTemplate; 
            set 
            { 
                if (_selectedTemplate != value) 
                { 
                    _selectedTemplate = value;
                    ParametersVM.TemplateFile = _selectedTemplate;
                    OnPropertyChanged(); 
                }
            }
        }

        private MountainsSupervisor _mountainsSupervisor;

        private List<ExternalProcessingResultItem> _resultItems;
        public List<ExternalProcessingResultItem> ResultItems
        {
            get => _resultItems; set { if (_resultItems != value) { _resultItems = value; OnPropertyChanged(); } }
        }

        private MountainsExecutionParametersVM _parameters;
        public MountainsExecutionParametersVM ParametersVM
        {
            get => _parameters; set { if (_parameters != value) { _parameters = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _startCommand;
        public AutoRelayCommand StartCommand
        {
            get
            {
                return _startCommand ?? (_startCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      ResultItems = null;

                      var res = _mountainsSupervisor.Execute(ParametersVM.Data);
                      ResultItems = res.Result;
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("External processing sucess", "Success");
                  }
                  catch (Exception ex)
                  {
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex,$"Execute external processing error {ex.Message}");
                  }
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _getResultsDefinedInTemplateCommand;
        public AutoRelayCommand GetResultsDefinedInTemplateCommand
        {
            get
            {
                return _getResultsDefinedInTemplateCommand ?? (_getResultsDefinedInTemplateCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      ResultItems = null;

                      var res = _mountainsSupervisor.GetResultsDefinedInTemplate(ParametersVM.Data.TemplateFile);
                      ResultItems = res.Result;
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Get results defined in template success", "Success");
                  }
                  catch (Exception ex)
                  {
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, $"Get results defined in template success error {ex.Message}");
                  }
              },
              () => { return true; }));
            }
        }

        public override void Init()
        {
            Templates = _mountainsSupervisor.GetTemplateFilePaths()?.Result;
        }
    }
}
