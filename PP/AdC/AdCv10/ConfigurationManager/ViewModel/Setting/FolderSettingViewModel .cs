using System;
using System.IO;
using System.Threading.Tasks;

using ConfigurationManager.Configuration;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ConfigurationManager.ViewModel.Setting
{
    public class FolderSettingViewModel : SettingBaseViewModel
    {
        public FolderSettingViewModel(Configuration.Setting setting) : base(setting)
        {
        }

        public override void Validate()
        {
            State = SettingState.InProgress;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    bool exist = Directory.Exists(Value);
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        State = exist ? SettingState.Valid : SettingState.Error;
                        Error = "Directory not exist";
                    }
                    ));

                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { State = SettingState.Error; Error = ex.Message; }));
                }
            });
        }


        private void OpenFolder()
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Value = dlg.SelectedPath;
        }

        #region Command

        private AutoRelayCommand _openFolder;
        public AutoRelayCommand OpenFolderCommand
        {
            get
            {
                return _openFolder ?? (_openFolder = new AutoRelayCommand(
              () =>
              {
                  OpenFolder();
              },
              () => { return true; }));
            }
        }

        #endregion
    }
}
