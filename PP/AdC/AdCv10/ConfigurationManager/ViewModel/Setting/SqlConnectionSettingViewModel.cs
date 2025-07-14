using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

using ConfigurationManager.Configuration;

namespace ConfigurationManager.ViewModel.Setting
{
    public class SqlConnectionSettingViewModel : SettingBaseViewModel
    {
        public SqlConnectionSettingViewModel(Configuration.Setting setting) : base(setting)
        {

        }

        public string DataSource
        {
            get
            {
                try
                {
                    SqlConnection sqlConnection = new SqlConnection(GetSqlConnectionWithoutEF());
                    return sqlConnection.DataSource;
                }
                catch (Exception ex)
                {
                    State = SettingState.Error;
                    Error = ex.Message;
                    return null;
                }
            }
            set
            {
                try
                {
                    string dataSource = "data source=";
                    Value = Value.Replace(dataSource + DataSource, dataSource + value);
                }
                catch (Exception ex)
                {
                    State = SettingState.Error;
                    Error = ex.Message;
                }
                OnPropertyChanged();
            }
        }

        public override void Validate()
        {
            State = SettingState.InProgress;
            Task.Factory.StartNew(() =>
            {
                try
                {

                    using (SqlConnection connection = new SqlConnection(GetSqlConnectionWithoutEF()))
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

        /// <summary>
        /// Suppression de la partie entity framework de la chaine de connexion
        /// </summary>
        /// <returns></returns>
        private string GetSqlConnectionWithoutEF()
        {
            string sqlConnectionWithoutEF = Value.Substring(Value.IndexOf("data source="));
            sqlConnectionWithoutEF = sqlConnectionWithoutEF.Substring(0, sqlConnectionWithoutEF.IndexOf(";MultipleActiveResultSets") + 1);
            return sqlConnectionWithoutEF;
        }
    }
}
