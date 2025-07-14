using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Format.Base;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Dummy
{
    public class DummyResultVM : ResultWaferVM
    {
        public override string FormatName => string.Empty;

        private string _label;

        public string DummyLabel
        {
            get => _label; 
            set => SetProperty(ref _label, value);
        }

        public string DummyResPath => ResultDataObj == null ? "<Null Obj>" : ResultDataObj.ResFilePath;

        private ImageSource _img;

        public ImageSource DummyImage
        {
            get => _img ?? (_img = Application.Current.TryFindResource("WorkInProgressImage") as ImageSource);
            set => SetProperty(ref _img, value);
        }

        public DummyResultVM() : base(null)
        {
            DummyLabel = "DUMMY RESULT WAFER VIEW";
        }
        
        public override void UpdateResData(IResultDataObject resdataObj)
        {
            base.UpdateResData(resdataObj);
            OnPropertyChanged(nameof(DummyResPath));
        }
    }
}
