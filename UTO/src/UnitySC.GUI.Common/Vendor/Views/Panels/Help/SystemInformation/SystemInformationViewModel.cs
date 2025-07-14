using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using Microsoft.Win32;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.SystemInformation
{
    public class SystemInformationViewModel : BusinessPanel
    {
        #region Fields
        private readonly SystemInformation _systemInformation = new SystemInformation();
        #endregion Fields

        #region Properties
        public AssemblyInformation SelectedAssembly
        {
            get { return _selectedAssembly; }
            set { _selectedAssembly = value; OnPropertyChanged(nameof(SelectedAssembly)); }
        }
        private AssemblyInformation _selectedAssembly;

        public string NetVersion => _systemInformation.NetVersion;

        public OperatingSystem OperatingSystem => _systemInformation.OperatingSystem;

        public string Process => _systemInformation.Process;

        public string OperatingSystemName => _systemInformation.OperatingSystemName;

        public ObservableCollection<AssemblyInformation> AssemblyInformations => _systemInformation.AssemblyInformations;

        #endregion Properties

        static SystemInformationViewModel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SystemInformationViewResources)));
        }

        public SystemInformationViewModel(string id, IIcon icon = null) : base(id, icon)
        {
            Commands.Add(new BusinessPanelCommand(nameof(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_SAVETXT), SaveAsText, PathIcon.ExportFile));
            Commands.Add(new BusinessPanelCommand(nameof(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_SAVEXML), SaveAsXml, PathIcon.ExportFile));
        }

        public SystemInformationViewModel() : this(nameof(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO), PathIcon.SystemInformation)
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException($" {nameof(SystemInformationViewModel)} default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        #region Commands

        #region SaveAsText
        public ICommand SaveAsText => _saveAsTextCommand ??= new DelegateCommand(SaveAsTextCommandExecute);
        private ICommand _saveAsTextCommand;
        
        protected void SaveAsTextCommandExecute()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = SystemInformationViewResources.SAVE_POPUP_TITLE,
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = "SystemInformation.txt"
            };
            if (saveFileDialog.ShowDialog() != true) return;

            string fileName = saveFileDialog.FileName;

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Messages.Show(new UserMessage(MessageLevel.Error, new LocalizableText(nameof(SystemInformationViewResources.CANNOT_SAVE_INFO), fileName)));
                return;
            }

            try
            {
                _systemInformation.SaveAsText(fileName);
                Messages.Show(new UserMessage(MessageLevel.Success, new LocalizableText(nameof(SystemInformationViewResources.INFORMATION_ARE_SAVED), fileName))
                {
                    Commands =
                    {
                        new UserMessageCommand(nameof(SystemInformationViewResources.OPEN_FOLDER), new DelegateCommand(() =>
                        {
                            if (!File.Exists(fileName)) return;
                            var argument = "/select, \"" + fileName +"\"";
                            System.Diagnostics.Process.Start("explorer.exe", argument);
                        }))
                    },
                    CanUserCloseMessage = true
                });
            }
            catch (Exception e)
            {
                Messages.Show(new UserMessage(MessageLevel.Error,
                    new LocalizableText(nameof(SystemInformationViewResources.CANNOT_SAVE_INFO_EXCEPTION), e.Message)));
            }
        }

        #endregion SaveAsText

        #region SaveAsXml
        public ICommand SaveAsXml => _saveAsXmlCommand ?? (_saveAsXmlCommand = new DelegateCommand(SaveAsXmlCommandExecute));
        private ICommand _saveAsXmlCommand;
        
        protected void SaveAsXmlCommandExecute()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = SystemInformationViewResources.SAVE_POPUP_TITLE,
                Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*",
                FileName = "SystemInformation.xml"
            };
            if (saveFileDialog.ShowDialog() != true) return;

            string fileName = saveFileDialog.FileName;

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Messages.Show(new UserMessage(MessageLevel.Error,
                    new LocalizableText(nameof(SystemInformationViewResources.CANNOT_SAVE_INFO), fileName)));

                return;
            }
            try
            {
                _systemInformation.SaveAsXml(fileName);
                Messages.Show(
                    new UserMessage(MessageLevel.Success,
                        new LocalizableText(nameof(SystemInformationViewResources.INFORMATION_ARE_SAVED), fileName))
                    {
                        Commands =
                        {
                            new UserMessageCommand(nameof(SystemInformationViewResources.OPEN_FOLDER),
                                new DelegateCommand(() =>
                                {
                                    if (!File.Exists(fileName))
                                    {
                                        return;
                                    }

                                    var argument = "/select, \"" + fileName + "\"";
                                    System.Diagnostics.Process.Start("explorer.exe", argument);
                                }))
                        },
                        CanUserCloseMessage = true
                    });
            }
            catch (Exception e)
            {
                Messages.Show(new UserMessage(MessageLevel.Error,
                    new LocalizableText(nameof(SystemInformationViewResources.CANNOT_SAVE_INFO_EXCEPTION), e.Message)));
            }
        }

        #endregion SaveAsXml

        #endregion Commands
    }
}
