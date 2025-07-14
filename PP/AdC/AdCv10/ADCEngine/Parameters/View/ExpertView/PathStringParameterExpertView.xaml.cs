using System;
using System.Windows;
using System.Windows.Controls;

using AdcTools.Widgets;

namespace ADCEngine.View
{
    public partial class PathStringParameterExpertView : UserControl
    {
        private PathStringParameter param { get { return (PathStringParameter)DataContext; } }

        public PathStringParameterExpertView()
        {
            InitializeComponent();
        }

        //=================================================================
        //
        //=================================================================
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (param.operation)
            {
                case PathStringParameter.Operation.Folder:
                    OpenFolderDialog();
                    break;
                case PathStringParameter.Operation.Open:
                    OpenFileDialog();
                    break;
                case PathStringParameter.Operation.Save:
                    SaveFileDialog();
                    break;
                default:
                    throw new ApplicationException("unknown PathStringParameter operation: " + param.operation);
            }
        }

        //=================================================================
        //
        //=================================================================
        private void OpenFolderDialog()
        {
            param.String = SelectFolderDialog.ShowDialog(param.String);
        }

        //=================================================================
        //
        //=================================================================
        private void OpenFileDialog()
        {
            System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();

            openFileDlg.Filter = param.Filter;
            openFileDlg.InitialDirectory = param.Value.Directory;
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                param.String = openFileDlg.FileName;
        }

        //=================================================================
        //
        //=================================================================
        private void SaveFileDialog()
        {
            System.Windows.Forms.SaveFileDialog saveFileDlg = new System.Windows.Forms.SaveFileDialog();

            saveFileDlg.Filter = param.Filter;
            saveFileDlg.InitialDirectory = param.Value.Directory;
            if (saveFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                param.String = saveFileDlg.FileName;
        }


    }
}
