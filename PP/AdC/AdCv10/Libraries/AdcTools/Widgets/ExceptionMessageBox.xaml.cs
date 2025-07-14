using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Serilog;


namespace AdcTools
{
    /// <summary>
    /// Interaction logic for ExceptionMessageBox.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class ExceptionMessageBox : Window
    {
        //=================================================================
        // Fonctions statiques
        //=================================================================
        public static void Show(string message, Exception ex = null, bool isFatal = false)
        {
            if (isFatal)
                Log.Fatal(message + "\n" + ex);
            else
                Log.Error(message + "\n" + ex);
            ExceptionMessageBox dlg = new ExceptionMessageBox(message, ex, isFatal);
            dlg.ShowDialog();
        }

        public static ImageSource ToImageSource(System.Drawing.Icon icon)
        {
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        //=================================================================
        // 
        //=================================================================
        public ExceptionMessageBox(string message, Exception ex, bool isFatal)
        {
            InitializeComponent();

            Window mainw = Application.Current.MainWindow;
            if (mainw != null & mainw != this && mainw.IsActive)
                Owner = mainw;
            Title = Application.ResourceAssembly.GetName().Name;
            System.Drawing.Icon systemIcon = isFatal ? System.Drawing.SystemIcons.Error : System.Drawing.SystemIcons.Warning;
            Icon = ToImageSource(systemIcon);

            Message = message;
            if (ex == null)
            {
                DebugInfo = message;
            }
            else
            {
                Details = ex.Message;
                DebugInfo = message + "\n" + ex.ToString();
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        //=================================================================
        // Properties
        //=================================================================
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ExceptionMessageBox), new PropertyMetadata(""));
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty DetailsProperty =
            DependencyProperty.Register("Details", typeof(string), typeof(ExceptionMessageBox), new PropertyMetadata(""));
        public string Details
        {
            get { return (string)GetValue(DetailsProperty); }
            set { SetValue(DetailsProperty, value); }
        }

        public static readonly DependencyProperty DebugInfoProperty =
            DependencyProperty.Register("DebugInfo", typeof(string), typeof(ExceptionMessageBox), new PropertyMetadata(""));
        public string DebugInfo
        {
            get { return (string)GetValue(DebugInfoProperty); }
            set { SetValue(DebugInfoProperty, value); }
        }

    }
}
