using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.DataAccess.SQL
{
    [DbConfigurationType(typeof(DataContextConfiguration))]
    public partial class InspectionEntities : DbContext
    {
        public InspectionEntities(string ConnectionString)
            : base(ConnectionString)
        {
            // base : conflit entre la propriétée Configuration de DbContext et la table Configuration ...
            base.Configuration.ProxyCreationEnabled = false;
            base.Database.CommandTimeout = 400;
        }
    }
}
