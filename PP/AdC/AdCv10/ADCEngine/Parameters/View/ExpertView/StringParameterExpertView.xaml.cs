using System.Windows;
using System.Windows.Controls;

using AdcTools.Widgets;

namespace ADCEngine.View
{
    public partial class StringParameterExpertView : UserControl
    {
        public StringParameterExpertView()
        {
            InitializeComponent();
        }

        //=================================================================
        //
        //=================================================================
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PathStringParameter param = (PathStringParameter)DataContext;

            //-------------------------------------------------------------
            // Open Folder Dialog
            //-------------------------------------------------------------
            if (param.Filter == null)
            {
                param.String = SelectFolderDialog.ShowDialog(param.String);
            }
            //-------------------------------------------------------------
            // Open File Dialog
            //-------------------------------------------------------------
            else
            {
                System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();

                openFileDlg.Filter = param.Filter;
                openFileDlg.InitialDirectory = param.Value.Directory;
                if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    param.String = openFileDlg.FileName;
            }
        }

    }
}
