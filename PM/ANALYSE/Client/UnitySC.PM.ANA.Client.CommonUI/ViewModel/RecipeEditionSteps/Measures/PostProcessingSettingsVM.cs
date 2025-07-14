using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures;
using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Server.Implementation;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures
{
    public class PostProcessingSettingsVM : ObservableObject, IDisposable
    {

        private ResultCorrectionType _correctionType;

        public PostProcessingSettingsVM(ResultCorrectionType correctionType=ResultCorrectionType.None)
        {
            _correctionType = correctionType;

            // For debug
            //Outputs.Add( new PostProcessingOutputVM("Test",15,"cm",ResultCorrectionType.Linear));
        }

        private void OutputsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.OldItems != null)
                foreach(var old in e.OldItems)
                    ((PostProcessingOutputVM)old).PropertyChanged -= PostProcessingSettingsVMChanged;

            if(e.NewItems != null)
             foreach (var newi in e.NewItems)
                ((PostProcessingOutputVM)newi).PropertyChanged += PostProcessingSettingsVMChanged;

        }

        private void PostProcessingSettingsVMChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsModified = true;
        }

        #region events

        public event EventHandler PostProcessingSettingsModified;

        #endregion events

        private MountainsConfiguration _mountainsConfig;
        public MountainsConfiguration MountainsConfig
        {
            get => _mountainsConfig; set { if (_mountainsConfig != value) { _mountainsConfig = value; OnPropertyChanged(); } }
        }

        private string _latest3DAPath;
        public string Latest3DAPath
        {
            get => _latest3DAPath; set { if (_latest3DAPath != value) { _latest3DAPath = value; OnPropertyChanged(); } }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set { if (_isEnabled != value) { _isEnabled = value; OnPropertyChanged(); AdvancedSettingsIsVisible = false; } }
        }

        private bool _advancedSettingsIsVisible;
        public bool AdvancedSettingsIsVisible
        {
            get => _advancedSettingsIsVisible; set { if (_advancedSettingsIsVisible != value) { _advancedSettingsIsVisible = value; OnPropertyChanged(); } }
        }

        private bool _pdfIsSaved;
        public bool PdfIsSaved
        {
            get => _pdfIsSaved; set { if (_pdfIsSaved != value) { _pdfIsSaved = value; OnPropertyChanged(); } }
        }

        private string _templateName = "New template";
        public string TemplateName
        {
            get => _templateName; set { if (_templateName != value) { _templateName = value; OnPropertyChanged(); } }
        }

        private ExternalMountainsTemplate _template;
        public ExternalMountainsTemplate Template
        {
            get => _template; set { if (_template != value) { _template = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<PostProcessingOutputVM> _outputs = new ObservableCollection<PostProcessingOutputVM>();
        public ObservableCollection<PostProcessingOutputVM> Outputs
        {
            get => _outputs; 
            set
            { 
                if(!(_outputs is null))
                {
                    _outputs.ToList().ForEach(x => x.PropertyChanged -= PostProcessingSettingsVMChanged);
                }                   
                    
                _outputs = value;

                if (!(_outputs is null))
                {
                    _outputs.ToList().ForEach(x => x.PropertyChanged += PostProcessingSettingsVMChanged);
                }

                OnPropertyChanged(); 
            }
        }

        private AutoRelayCommand _editCommand;
        public AutoRelayCommand EditCommand
        {
            get
            {
                return _editCommand ?? (_editCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      MountainsActiveXSupervisor mountainsActiveXSupervisor = ClassLocator.Default.GetInstance<MountainsActiveXSupervisor>();
                      mountainsActiveXSupervisor.CheckActiveXExternal();

                      var mountainsVM = new MountainsDisplayVM();
                      string fileName = Path.Combine(Directory.GetCurrentDirectory(), DateTime.Now.ToString("yyyyMMdd_HHmmss")) + ".mnt";
                      bool? isHostedByPM = MountainsConfig.IsHostedByPM;
                      if (isHostedByPM == true)
                      {
                          if (Template != null)
                          {
                              Template.SaveToFile(fileName);
                              mountainsVM.InitExistingDoc(fileName);
                              if (!Latest3DAPath.IsNullOrEmpty())
                              {
                                  mountainsVM.SubstituteStudiable(Latest3DAPath);
                              }
                          }
                          else
                          {
                              mountainsVM.InitNewDoc(fileName);
                              if (!Latest3DAPath.IsNullOrEmpty())
                              {
                                  mountainsVM.SubstituteStudiable(Latest3DAPath);
                              }
                          }
                          if (ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<MountainsDisplay>(mountainsVM) == true)
                          {
                              UpdateOutputs(mountainsVM.OutputsResult);
                              Template = new ExternalMountainsTemplate();
                              Template.LoadFromFile(fileName);
                          }
                          try
                          {
                              File.Delete(fileName);
                          }
                          catch
                          {
                              ClassLocator.Default.GetInstance<ILogger>().Information($"Error during delete mountains tempfile {fileName}");
                          }
                      }
                  }
                  catch(Exception ex)
                  {
                      string message = "Error with mountains : Check licence and server connexion / Mountains configuration";
                      ClassLocator.Default.GetInstance<ILogger>().Error(ex, message);
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, message);
                  }

                  
              },
              () => { return CanEdit(); }));
            }
        }

        private AutoRelayCommand _importCommand;
        public AutoRelayCommand ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      var mountainsVM = new MountainsImportDisplayVM(MountainsConfig.IsHostedByPM);
                      if (ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<MountainsImportDisplay>(mountainsVM) == true)
                      {
                          TemplateName = mountainsVM.SelectedTemplate.Name;
                          UpdateOutputs(mountainsVM.OutputsResult);
                          Template = mountainsVM.CurrentContent;
                      }
                  }

                  catch(Exception ex) 
                  {
                      string message = "Error with mountains : Check licence and server connexion / Mountains configuration";
                      ClassLocator.Default.GetInstance<ILogger<MountainsImportDisplayVM>>().Error(ex, message);
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, message);
                  }
              },
              () => { return true; }));
            }
        }   

        private bool CanEdit()
        {
            if (MountainsConfig.IsHostedByPM == false  || MountainsConfig == null)
            {
                return false;
            }
            return true;
        }
        private void UpdateOutputs(List<ExternalProcessingResultItem> externalProcessingResultItems)
        {

    


            _outputs.ToList().ForEach(x => x.PropertyChanged -= PostProcessingSettingsVMChanged);
            // Remove items.
            Outputs.RemoveAll(oldi => !externalProcessingResultItems.Select(newi => newi.Description).Contains(oldi.Key));

            // New items
            foreach (var newi in externalProcessingResultItems)
            {
                // new
                if (!Outputs.Select(oldi => oldi.Key).Contains(newi.Description))
                {
                    Outputs.Add(new PostProcessingOutputVM(newi.Description, newi.DoubleValue.Value, newi.Unit, _correctionType));
                }
            }
            _outputs.ToList().ForEach(x => x.PropertyChanged += PostProcessingSettingsVMChanged);
            IsModified = true;
        }

        public void Dispose()
        {
            _outputs.CollectionChanged -= OutputsChanged;
        }

        private bool _isModified = false;

        public bool IsModified
        {
            get => _isModified;
            set
            {
                _isModified = value;
                if (_isModified)
                {
                    PostProcessingSettingsModified?.Invoke(this, null);
                }
                OnPropertyChanged();
            }
        }
    }

}
