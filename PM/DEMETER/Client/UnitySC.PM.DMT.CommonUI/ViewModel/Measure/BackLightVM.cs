using UnitySC.Shared.UI.Enum;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Measure
{
    public class BackLightVM : MeasureVM
    {
        public BackLightVM()
        {
            Title = "Back Light";
        }

        public override HelpTag HelpTag => HelpTag.DMT_BackLight;
    }
}
