using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public class AdvancedSettingsStepVM : ObjectiveStepBaseVM
    {
        public AdvancedSettingsStepVM(ObjectiveToCalibrateVM objective) : base(objective)
        {
        }
    }
}
