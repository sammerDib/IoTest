using System;
using System.IO;
using System.Threading.Tasks;

using ConfigurationManager.Configuration;

using UnitySC.Shared.UI.AutoRelayCommandExt;


namespace ConfigurationManager.ViewModel.Setting
{
    public class FileSettingViewModel : SettingBaseViewModel
    {
        public FileSettingViewModel(Configuration.Setting setting) : base(setting)
        {
        }

        public override void Validate()
        {
            State = SettingState.InProgress;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    bool exist = File.Exists(Value);
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        State = exist ? SettingState.Valid : SettingState.Error;
                        Error = "File not exist";
                    }));
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { State = SettingState.Error; Error = ex.Message; }));
                }
            });
        }


        private void OpenFile()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Value = dlg.FileName;
        }

        #region Command

        private AutoRelayCommand _openFile;
        public AutoRelayCommand OpenFileCommand
        {
            get
            {
                return _openFile ?? (_openFile = new AutoRelayCommand(
              () =>
              {
                  OpenFile();
              },
              () => { return true; }));
            }
        }

        #endregion
    }
}
