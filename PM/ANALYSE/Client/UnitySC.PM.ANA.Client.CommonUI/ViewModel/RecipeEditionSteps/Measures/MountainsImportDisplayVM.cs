using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures;
using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Proxy;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures
{
    public class MountainsImportDisplayVM : ObservableObject, MvvmDialogs.IModalDialogViewModel
    {
        private const string TempFileName = "ImportTemp.mnt";

        public MountainsWPFControl MountainsWPFControl => ClassLocator.Default.GetInstance<MountainsWPFControl>();
        private MountainsSupervisor _mountainsSupervisor;
        private bool? _isHostedByPM;
        public List<Template> Templates { get; set; }
        public List<ExternalProcessingResultItem> OutputsResult { get; set; } = new List<ExternalProcessingResultItem>();       

        private Template _selectedTemplate;
        public Template SelectedTemplate
        {
            get => _selectedTemplate;
            set 
            { 
                if (_selectedTemplate != value) 
                { 
                    _selectedTemplate = value; 
                    OnPropertyChanged();
                    if (_isHostedByPM == true) MountainsWPFControl.ClearContent();
                    if (_selectedTemplate != null)
                    {
                        CurrentContent = _mountainsSupervisor.GetTemplateContent(_selectedTemplate.Path)?.Result;
                        var currentFilePath = Path.Combine(Directory.GetCurrentDirectory(), TempFileName);
                        if (_isHostedByPM == true && CurrentContent != null)
                        {
                            CurrentContent.SaveToFile(currentFilePath);
                            MountainsWPFControl.InitDoc(currentFilePath);
                        }
                    }
                }
            }
        }

        public ExternalMountainsTemplate CurrentContent { get; private set; }

        private AutoRelayCommand _applyCommand;
        public AutoRelayCommand ApplyCommand
        {
            get
            {
                return _applyCommand ?? (_applyCommand = new AutoRelayCommand(
              () =>
              {
                  if (_isHostedByPM == true)
                  {
                      OutputsResult = MountainsWPFControl.GetResultsDefinedInCurrentTemplate();
                      MountainsWPFControl.ClearContent();
                      var currentFilePath = Path.Combine(Directory.GetCurrentDirectory(), TempFileName);
                      try
                      {
                          File.Delete(currentFilePath);
                      }
                      catch
                      {
                          ClassLocator.Default.GetInstance<ILogger>().Information($"Error during mountains tempFile deletion {currentFilePath}");
                      }
                  }
                  else
                  {
                      var res = _mountainsSupervisor.GetResultsDefinedInTemplate(_selectedTemplate.Path);
                      OutputsResult = res.Result;
                  }

                  DialogResult = true;

              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _cancelCommand;
        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  if (_isHostedByPM == true)
                  {
                      MountainsWPFControl.ClearContent();
                      var currentFilePath = Path.Combine(Directory.GetCurrentDirectory(), TempFileName);
                      try
                      {
                          File.Delete(currentFilePath);
                      }
                      catch
                      {
                          ClassLocator.Default.GetInstance<ILogger>().Information($"Error during mountains tempFile deletion {currentFilePath}");
                      }
                  }
                  
                  DialogResult = false;
              },
              () => { return true; }));
            }
        }
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        public MountainsImportDisplayVM(bool? isHostedByPM)
        {
            _mountainsSupervisor = ClassLocator.Default.GetInstance<MountainsSupervisor>();
            _isHostedByPM = isHostedByPM;
            var paths = _mountainsSupervisor.GetTemplateFilePaths()?.Result;
            Templates = paths?.Select(x => new Template() { Path = x, Name = Path.GetFileNameWithoutExtension(x) }).ToList();
            SelectedTemplate = Templates?.FirstOrDefault();
        }
        public void Save(string filePath)
        {
            if (_isHostedByPM == true) MountainsWPFControl.Save(filePath);
        }


    }

    public class Template
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
