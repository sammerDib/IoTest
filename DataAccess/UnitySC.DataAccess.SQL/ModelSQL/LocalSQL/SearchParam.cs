using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.DataAccess.SQL.ModelSQL.LocalSQL
{
    public class SearchParam
    {
        public int? ToolId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ProductId { get; set; }
        public string LotName { get; set; }
        public string RecipeName { get; set; }
        public int? ActorType { get; set; }
        public int? ResultState { get; set; }
        public string WaferName { get; set; }
        public int? ResultFilterTag { get; set; }
    }
}
