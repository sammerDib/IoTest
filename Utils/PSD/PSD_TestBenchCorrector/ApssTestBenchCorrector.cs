using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AppsTestBenchCorrector
{
    public partial class ApssTestBenchCorrector : Form
    {
        private CDataPicture _picture = null;
        private string _path;

        public ApssTestBenchCorrector()
        {
            InitializeComponent();

            _picture = new CDataPicture();
        }

        private void BTSelectPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "images PSD (*.tiff)|*.tiff|All files (*.*)|*.*";
            if (!string.IsNullOrEmpty(_path))
                dlg.InitialDirectory = _path;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TBSelectedFile.Text = dlg.FileName;
                PBSummaryView.Image = Image.FromFile(TBSelectedFile.Text);
            }
            else
            {
                TBSelectedFile.Text = "";
            }

            _path = Directory.GetCurrentDirectory();
        }

        private void BTProcess_Click(object sender, EventArgs e)
        {
            if (TBSelectedFile.Text != "")
            {
                _picture.Input.FilePath = TBSelectedFile.Text;
                _picture.Input.WaferSize = Convert.ToInt32(TBWaferSize.Text);
                _picture.Input.AngleValueSearch = TBAngleValue.Text;
                _picture.Input.PixelSize = Convert.ToInt32(TBPixelSize.Text);

                PBSummaryView.Image = Image.FromFile(TBSelectedFile.Text);

                Cursor.Current = Cursors.WaitCursor;
                if (_picture.LoadImage())
                {
                    _picture.CorrectorXYTheta();
                    Update_Result();
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void BTDetail_Click(object sender, EventArgs e)
        {
            AppTestBenchCorrector_Detail dlg = new AppTestBenchCorrector_Detail(_picture);
            dlg.ShowDialog();
        }

        private void Update_Result()
        {
            TBThetaCorrection.Text = _picture.Result.ThetaCorrection.ToString();
            TBCorrectionX.Text = _picture.Result.XCorrection.ToString();
            TBCorrectionY.Text = _picture.Result.YCorrection.ToString();
        }
    }
}
