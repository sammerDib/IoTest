using ADCEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace BasicModules.VidReport
{
    /// <summary>
    /// Logique d'interaction pour VidReportControl.xaml
    /// </summary>
    public partial class VidReportControl : UserControl
    {
        private VidReportViewModel ViewModel { get { return (VidReportViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        internal VidReportControl(VidReportViewModel viewmodel)
		{
			DataContext = viewmodel;
			InitializeComponent();
		}

        //=================================================================
        //
        //=================================================================
        private void UserControl_Loaded(object sender, EventArgs e)
        {
            //-------------------------------------------------------------
            // Refresh the view Model
            //-------------------------------------------------------------
            ViewModel.Init();
        }

        //=================================================================
        // 
        //=================================================================
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
                ViewModel.ReportChange();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dropDownButton != null)
                dropDownButton.IsOpen = false;
        }

        private DropDownButton dropDownButton = null;
        private void Popup_Opened(object sender, RoutedEventArgs e)
        {
            dropDownButton = sender as DropDownButton;
        }


    }
}
