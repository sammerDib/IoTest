using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using AdcTools;
using AdcTools.Widgets;

using UnitySC.Shared.LibMIL;

using UnitySC.Shared.Tools;

using Xceed.Wpf.Toolkit.PropertyGrid;


namespace BasicModules.Trace
{
    /// <summary>
    /// Logique d'interaction pour TraceImageUserControl.xaml
    /// </summary>
    public partial class TraceImageUserControl : UserControl
    {
        private string _folder;
        private System.Windows.Threading.DispatcherTimer _timerImage;

        private TraceImageViewModel ViewModel { get { return (TraceImageViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public TraceImageUserControl()
        {
            InitializeComponent();
            if (!PropertyGridLayout.IsAutoHidden)
                PropertyGridLayout.ToggleAutoHide();
            if (!ImageListLayout.IsAutoHidden)
                ImageListLayout.ToggleAutoHide();

            //---------------------------------------------------------
            // Timer pour afficher l'image de trace
            //---------------------------------------------------------
            _timerImage = new System.Windows.Threading.DispatcherTimer();
            _timerImage.Interval = new TimeSpan(0, 0, 0, 0, 100/*ms*/);
            _timerImage.Tick += new EventHandler(timerLog_Elapsed);
            _timerImage.Start();

            ImageListBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            ImageListBox.Items.SortDescriptions.Add(new SortDescription("SourceModuleId", ListSortDirection.Ascending));
        }

        //=================================================================
        // Boutons Multimedia
        //=================================================================
        private void Button_Click_Eject(object sender, RoutedEventArgs e)
        {
            ViewModel.Ejected = true;
        }

        private void Button_Click_First(object sender, RoutedEventArgs e)
        {
            ImageListBox.SelectedIndex = 0;
        }

        private void Button_Click_Last(object sender, RoutedEventArgs e)
        {
            ImageListBox.SelectedIndex = ImageListBox.Items.Count - 1;
        }

        private void Button_Click_Prev(object sender, RoutedEventArgs e)
        {
            if (ImageListBox.SelectedIndex > 0)
                ImageListBox.SelectedIndex--;
        }

        private void Button_Click_Next(object sender, RoutedEventArgs e)
        {
            if (ImageListBox.SelectedIndex < ImageListBox.Items.Count - 1)
                ImageListBox.SelectedIndex++;
        }

        private void Button_Click_Play_Pause(object sender, RoutedEventArgs e)
        {
            ViewModel.Paused = !ViewModel.Paused;
            if (!ViewModel.Paused)
                ImageListBox.ScrollIntoView(ImageListBox.SelectedItem);
        }

        //=================================================================
        // Menu contextuel
        //=================================================================
        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string viewer = ConfigurationManager.AppSettings.Get("Debug.ImageViewer");
                if (viewer == null)
                    viewer = @"C:\Program Files\ImageJ\ImageJ.exe";

                TraceImage trace = ViewModel.SelectedTrace;

                PathString filename = trace.ToString() + ".tif";
                filename = filename.RemoveInvalidFilePathCharacters();
                filename = PathString.GetTempPath() / filename;

                bool b = ViewModel.SelectedTrace.Save(filename);
                if (b)
                    System.Diagnostics.Process.Start(viewer, filename);
            }
            catch (Exception ex)
            {
                ExceptionMessageBox.Show("Failed to export image to external viewer", ex);
            }
        }

        private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            TraceImage trace = ViewModel.SelectedTrace;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = trace.ToString();
            dlg.DefaultExt = ".tif";
            dlg.Filter = "Tiff|*.tif|All files|*.*";
            bool? b = dlg.ShowDialog();
            if (b == true)
                trace.Save(dlg.FileName);
        }

        private void MenuItemSaveList_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<TraceImage> list = ImageListBox.Items.OfType<TraceImage>();
            SaveImageList(list);
        }

        private void MenuItemSaveAll_Click(object sender, RoutedEventArgs e)
        {
            SaveImageList(ViewModel.TraceList);
        }

        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            TraceImage trace = ViewModel.SelectedTrace;
            Clipboard.SetImage(trace.WpfBitmap);
        }

        private void SaveImageList(IEnumerable<TraceImage> traceList)
        {
            if (!SelectFolderDialog.ShowDialog(ref _folder))
                return;

            PathString folder = _folder;

            ProgressDialog dialog = new ProgressDialog();
            dialog.Text = "Saving images to \"" + folder + "\"";
            dialog.Maximum = traceList.Count();
            dialog.action = new Action<object, DoWorkEventArgs>((sender, e) =>
                {
                    var worker = (BackgroundWorker)sender;
                    int count = 0;
                    foreach (TraceImage trace in traceList)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }

                        PathString filename = trace.ToString() + ".tif";
                        filename = filename.RemoveInvalidFilePathCharacters();
                        filename = folder / filename;
                        trace.Save(filename);
                        worker.ReportProgress(++count);
                    }
                });

            dialog.ShowDialog();
        }

        //=================================================================
        // MouseMove
        //=================================================================
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel.Paused)
            {
                // Get Image
                //..........
                TraceImage trace = ViewModel.SelectedTrace;
                if (trace == null || trace.image == null)
                    return;
                MilImage milImage = trace.image.CurrentProcessingImage.GetMilImage();
                if (milImage == null)
                    return;

                // Get Grey level
                //...............
                Point pti = e.GetPosition(Image);

                double grey;
                try
                {
                    grey = milImage.GetPixel((int)pti.X, (int)pti.Y);
                }
                catch (Exception)
                {
                    // hors de l'image
                    grey = double.NaN;
                }

                // Display Grey Level
                //...................
                statusTextbox.Text = String.Format("X: {0}    Y: {1}    Grey: {2}", (int)pti.X, (int)pti.Y, grey);
            }
        }

        //=================================================================
        // MouseLeave
        //=================================================================
        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ViewModel.Paused && ViewModel.SelectedTrace != null)
                statusTextbox.Text = ViewModel.SelectedTrace.ToString();
        }

        //=================================================================
        // 
        //=================================================================
        private void ImageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.Paused)
                ImageListBox.ScrollIntoView(ImageListBox.SelectedItem);
        }

        //=================================================================
        // 
        //=================================================================
        private void PropertyGrid_SelectedObjectChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Expand the items of the Proerty Grid
            //.....................................
            if (ViewModel.Paused)
            {
                System.Collections.IList p = PropertyGrid.Properties;
                foreach (var i in p)
                {
                    Xceed.Wpf.Toolkit.PropertyGrid.CustomPropertyItem pi = (Xceed.Wpf.Toolkit.PropertyGrid.CustomPropertyItem)i;
                    pi.IsExpanded = true;
                }
            }
        }

        //=================================================================
        // Change Zoombox default ViewFinder icon
        //=================================================================
        private void ZoomBox_Loaded(object sender, RoutedEventArgs e)
        {
            Image showViewFinderGlyphImage = (Image)ZoomBox.FindVisualChildByName("ShowViewFinderGlyphImage");
            if (showViewFinderGlyphImage != null)
            {
                System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("BasicModules.Trace.TraceImage.ViewFinder.png");
                BitmapSource bmp = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                showViewFinderGlyphImage.Source = bmp;
                ZoomBox.FitToBounds();
            }
        }

        //=================================================================
        // Timer event pour afficher l'image en cours
        //=================================================================
        private void timerLog_Elapsed(object sender, EventArgs e)
        {
            ViewModel.UpdateSelectedImage();
        }

    }
}
