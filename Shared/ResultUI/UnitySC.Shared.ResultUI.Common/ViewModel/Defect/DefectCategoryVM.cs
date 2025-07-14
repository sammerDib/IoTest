using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Defect
{
    public class DefectCategoryVM : ObservableObject
    {
        private readonly IMessenger _messenger;

        private ResultFormat _resFormat;
        public ResultFormat ResFormat
        {
            get => _resFormat;
            set => SetProperty(ref _resFormat, value);
        }

        private int? _idCode;
        public int? IDcode
        {
            get => _idCode;
            set => SetProperty(ref _idCode, value);
        }

        private string _labelCategory;
        public string LabelCategory
        {
            get => _labelCategory;
            set => SetProperty(ref _labelCategory, value);
        }

        private Color _colorCategory;
        public Color ColorCategory
        {
            get => _colorCategory;
            set => SetProperty(ref _colorCategory, value);
        }

        private Color _ellipsecolor;
        public Color EllipseColor
        {
            get => _ellipsecolor;
            set => SetProperty(ref _ellipsecolor, value);
        }

        private int _nbDefects;
        public int NbDefects
        {
            get => _nbDefects;
            set => SetProperty(ref _nbDefects, value);
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (SetProperty(ref _enabled, value))
                {
                    EllipseColor = _enabled ? Color.Orange : Color.Bisque;
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { SetProperty(ref _isSelected, value); }
        }

        public DefectCategoryVM()
        {
            _resFormat = ResultFormat.Unknow;

            _labelCategory = string.Empty;
            _colorCategory = Color.Transparent;
            _nbDefects = 0;
            _isSelected = false;
            _enabled = true;
            _idCode = 0;
            //_ellipsecolor = Color.Orange;

            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
        }

        public DefectCategoryVM(ResultFormat fmt, string label, Color color, int nbDefects, int idcode = 0)
        {
            _resFormat = fmt;

            _labelCategory = label;
            _colorCategory = color;
            _isSelected = true;
            _enabled = true;
            _ellipsecolor = Color.Orange;
            _nbDefects = nbDefects;
            _idCode = idcode;

            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
        }
    }
}
