using System;
using System.Collections.Generic;

using UnitySC.Shared.Data;

namespace UnitySC.Shared.Dataflow.Shared
{
    public class DataflowID
    {
        public int Id { get; set; }
        public Guid IdGuid { get; set; }
        public List<Material> Wafers { get; set; }
        public string WafferId { get; set; }
        public string Lot { get; set; }
        public string Slot { get; set; }
    }
}
