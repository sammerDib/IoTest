using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

using ConfigurationManager.Configuration;

namespace ConfigurationManager.ViewModel.Setting
{
    public class ResultDbSettingViewModel : SettingBaseViewModel
    {
        private const string _sqlConnection = "Server=tcp:{0}; Initial Catalog=DB_ADC_RESULTSv3; Integrated Security = False; User Id = PswdSQL; Password = test";
        public ResultDbSettingViewModel(Configuration.Setting setting) : base(setting)
        {
        }

        public override void Validate()
        {
            State = SettingState.InProgress;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(string.Format(_sqlConnection, Value)))
                    {
                        connection.Open();
                        System.Windows.Application.Current.Dispatcher.Invoke((() => { State = connection.State == System.Data.ConnectionState.Open ? SettingState.Valid : SettingState.Error; }));
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { State = SettingState.Error; Error = ex.Message; }));
                }
            });
        }
    }
}


