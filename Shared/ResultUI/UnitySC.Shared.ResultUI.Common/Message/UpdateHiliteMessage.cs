
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.ResultUI.Common.Message
{
    public class UpdateRectHiliteMessage
    {
        private RectpxItem _defectRect;
        public RectpxItem DefectRect
        {
            get { return _defectRect; }
            set { _defectRect = value; }
        }
        
        private bool _show;
        public bool Show
        {
            get { return _show; }
            set { _show = value; }
        }

        public UpdateRectHiliteMessage(bool bShow, RectpxItem defectRect = null)
        {
            _defectRect = defectRect;
            _show = bShow;
        }
    }
}
