using System.Collections.Generic;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.CommonUI.ViewModel
{
    public class MeasureSummaryVM
    {
        private List<string> _outputs;
        public string Name { get; set; }

        public Side Side { get; set; }

        public List<string> Outputs
        {
            get
            {
                if (_outputs == null)
                    _outputs = new List<string>();
                return _outputs;
            }
            set
            {
                _outputs = value;
            }
        }
    }
}
