using System.Drawing;
using System.Windows.Forms;

namespace AppsTestBenchCorrector
{
    public partial class AppTestBenchCorrector_Detail : Form
    {
        private CDataPicture _detailPicture;

        public AppTestBenchCorrector_Detail(CDataPicture picture)
        {
            InitializeComponent(picture);

            _detailPicture = picture;

            _detailPicture.CreateGraphics();

            PBPictureDetail.Image = _detailPicture.DisplayImage;

            _detailPicture.DrawColorCross(picture.NotchCenter, Color.Red);
            _detailPicture.DrawColorEncoumpassingCircle(picture.WaferCircle, Color.Red);

            PBPictureDetail.Refresh();
        }
    }
}
