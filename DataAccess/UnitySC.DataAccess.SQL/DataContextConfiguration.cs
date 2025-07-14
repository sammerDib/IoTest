using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.DataAccess.SQL
{
    public class DataContextConfiguration : System.Data.Entity.DbConfiguration
    {
        public DataContextConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new System.Data.Entity.SqlServer.SqlAzureExecutionStrategy(2, new TimeSpan(0, 0, 2)));
        }
    }
}
