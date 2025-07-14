using Utils.ViewModel;

using Dto = UnitySC.DataAccess.Dto;

namespace ADC.ViewModel.Ada.ChamberTreeView
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ChamberViewModel : TreeViewItemViewModel
    {
        private Dto.Chamber _chamber;
        public ChamberViewModel(Dto.Chamber chamber, ToolViewModel parentState) : base(parentState, false)
        {
            _chamber = chamber;
        }

        public int ChamberId => _chamber.Id;

        public override string ToString()
        {
            return string.Format("{0}-{1}", _chamber.Name, _chamber.Id);
        }
    }
}
