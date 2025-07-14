using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

using UnitySC.DataAccess.Service.Implementation;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Host
{
    public static class HelperFdcDb
    {
        private const double DatabaseTotalSize = 10240; //MB

        public static double GetDatabaseSize()
        {
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var connection = unitOfWork.GetDatabaseConnection() as SqlConnection;
                var command = new SqlCommand("sp_spaceused", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string databaseSizeString = reader["database_size"].ToString();
                        string sizeValue = new string(databaseSizeString.TakeWhile(char.IsDigit).ToArray());
                        if (double.TryParse(sizeValue, out double databaseSize))
                        {
                            return databaseSize;
                        }
                    }
                }
                return 0;
            }
        }

        public static double GetDatabaseFreeSpacePercentage()
        {
            double occupiedSpace = GetDatabaseSize();

            double freeSpacePercentage = ((DatabaseTotalSize - occupiedSpace) / DatabaseTotalSize) * 100;

            return freeSpacePercentage;
        }

        public static int GetRecipesCount()
        {
            var service = ClassLocator.Default.GetInstance<RecipeService>();
            int count = service.GetRecipesCount().Result;
            return count;
        }

        public static int GetResultsCount()
        {
            var service = ClassLocator.Default.GetInstance<RecipeService>();
            int count = service.GetResultsCount().Result;
            return count;
        }

        public static int GetAcquisitionsCount()
        {
            var service = ClassLocator.Default.GetInstance<RecipeService>();
            int count = service.GetAcquisitionsCount().Result;
            return count;
        }
        public static int GetProductsCount()
        {
            var service = ClassLocator.Default.GetInstance<RecipeService>();
            int count = service.GetProductsCount().Result;
            return count;
        }
    }
}
