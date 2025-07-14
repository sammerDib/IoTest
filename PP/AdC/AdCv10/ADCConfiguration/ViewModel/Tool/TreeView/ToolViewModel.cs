using System;

using UnitySC.Shared.Tools;
using Dto = UnitySC.DataAccess.Dto;

using Utils.ViewModel;

namespace ADCConfiguration.ViewModel.Tool.TreeView
{
    public class ToolViewModel : TreeViewItemViewModel
    {
        private Dto.Tool _tool;
        internal bool _isHistoryMode;

        public ToolViewModel(Dto.Tool tool, bool isHistoryMode) : base(null, true)
        {
            _tool = tool;
            _isHistoryMode = isHistoryMode;
            IsExpanded = true;
            HasChanged = false;
        }

        protected override void LoadChildren()
        {
            //foreach (var chamber in _tool.Chambers)
            //    base.Children.Add(new ChamberViewModel(chamber, this));
        }

        public override string ToString()
        {
            return _tool.Name;
        }

        public int Id
        {
            get { return _tool.Id; }
        }

        public string Title
        {
            get { return _tool.Name; }
        }

        public string ToolCategory
        {
            get { return _tool.ToolCategory; }
        }

        public string Name
        {
            get { return _tool.Name; }
            set
            {
                _tool.Name = value;
                OnPropertyChanged();
            }
        }

        public int ToolKey
        {
            get { return _tool.ToolKey; }
            set
            {
                _tool.ToolKey = value;
                OnPropertyChanged();
            }
        }

        //public int PortNumber
        //{
        //    get { return _tool.PortNumber; }
        //    set
        //    {
        //        _tool.PortNumber = value;
        //        OnPropertyChanged();
        //    }
        //}

        public override void Save()
        {
            throw new NotImplementedException("Save need to implement interaction with USP database");
            //  ClassLocator.Default.GetInstance<IToolService>().SetTool(_tool, Services.Services.Instance.AuthentificationService.CurrentUser.Id);
            //  Services.Services.Instance.LogService.LogDebug(string.Format("Set tool Tool.Id {0} ", _tool.Id));
            //  HasChanged = false;
        }
    }
}
