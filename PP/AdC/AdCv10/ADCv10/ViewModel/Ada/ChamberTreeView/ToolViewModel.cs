using Utils.ViewModel;

using Dto = UnitySC.DataAccess.Dto;

namespace ADC.ViewModel.Ada.ChamberTreeView
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ToolViewModel : TreeViewItemViewModel
    {
        private Dto.Tool _tool;

        public ToolViewModel(Dto.Tool tool) : base(null, true)
        {
            _tool = tool;
            IsExpanded = true;
        }

        protected override void LoadChildren()
        {
            foreach (Dto.Chamber chamber in _tool.Chambers)
                base.Children.Add(new ChamberViewModel(chamber, this));
        }

        public override string ToString()
        {
            return _tool.Name;
        }

        public string ToolCategory => _tool.ToolCategory;
        public string Name => _tool.Name;
    }
}
