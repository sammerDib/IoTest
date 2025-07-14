using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using UnitySC.Shared.Tools;

namespace UnitySC.Shared.UI.Controls
{
    /// <summary>
    /// Interaction logic for HelpDisplay.xaml
    /// </summary>
    public partial class HelpDisplay : UserControl
    {
        private const string ContextualHelpFolder = @"EXTRACTS\ADCV9";
        private const string MainHelpFolder = @"HTML5\ADCV9\Responsive HTML5";
        private const string DefaultPage = "index.html";
        private const string HelpFolder = @"C:\UnitySC\ExternalADC\Help";
        private readonly string _helpFolder;

        public HelpDisplay()
        {
            InitializeComponent();
            _helpFolder = HelpFolder;
            if (!Path.IsPathRooted(_helpFolder))
            {
                _helpFolder = Path.Combine(Path.GetDirectoryName(PathHelper.GetExecutingAssemblyPath()), _helpFolder);
            }

            if (AutoResize)
                webHelpBrowser.Height = 0;
            webHelpBrowser.NavigateToString("<html></html>");
        }

        /// <summary>
        /// Ouverture du navigateur web
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            OpenMainHelp(NavigateToPageInMainHelp ? HelpName : null);
        }

        /// <summary>
        /// Nom de l'aide à afficher
        /// </summary>
        public string HelpName
        {
            get { return (string)GetValue(HelpNameProperty); }
            set { SetValue(HelpNameProperty, value); }
        }

        public static readonly DependencyProperty HelpNameProperty =
            DependencyProperty.Register("HelpName", typeof(string), typeof(HelpDisplay), new FrameworkPropertyMetadata(null, OnHelpChanged));

        /// <summary>
        /// Active la navigation vers une page spécicifique de l'aide au clique sur (More.. )
        /// </summary>
        public bool NavigateToPageInMainHelp
        {
            get { return (bool)GetValue(NavigateToPageInMainHelpProperty); }
            set { SetValue(NavigateToPageInMainHelpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigateToPageInMainHelp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigateToPageInMainHelpProperty =
            DependencyProperty.Register("NavigateToPageInMainHelp", typeof(bool), typeof(HelpDisplay), new PropertyMetadata(false));

        public static void OnHelpChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            string helpName = e.NewValue as string;
            var helpDisplay = GetHelpDisplay(obj);
            string helpFilePath = Path.Combine(helpDisplay._helpFolder, ContextualHelpFolder, string.Concat(helpName, ".html"));
            if (string.IsNullOrEmpty(helpName) || !File.Exists(helpFilePath))
            {
                helpDisplay.mainBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                helpDisplay.webHelpBrowser.Navigate(helpFilePath);
                helpDisplay.mainBorder.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// True => Reszie automatique en fonctionnement du contenu html de l'aide
        /// </summary>
        public bool AutoResize
        {
            get { return (bool)GetValue(AutoResizeProperty); }
            set { SetValue(AutoResizeProperty, value); }
        }

        public static readonly DependencyProperty AutoResizeProperty =
            DependencyProperty.Register("AutoResize", typeof(bool), typeof(HelpDisplay), new PropertyMetadata(false));

        /// <summary>
        /// True => Affiche le bouton (mMre..)
        /// </summary>
        public bool ShowMore
        {
            get { return (bool)GetValue(ShowMoreProperty); }
            set { SetValue(ShowMoreProperty, value); }
        }

        public static readonly DependencyProperty ShowMoreProperty =
            DependencyProperty.Register("ShowMore", typeof(bool), typeof(HelpDisplay), new PropertyMetadata(true));

        /// <summary>
        /// Récupére le control courant pour les dependancy property
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private static HelpDisplay GetHelpDisplay(DependencyObject sender)
        {
            HelpDisplay helpDisplay = null;
            if (sender is HelpDisplay)
                helpDisplay = (HelpDisplay)sender;
            if (helpDisplay == null)
                helpDisplay = GetHelpDisplay(sender);
            return helpDisplay;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            webHelpBrowser.Source = null;
        }

        private void webHelpBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Resize();
        }

        /// <summary>
        /// Redimensionnement du control en fonctionnemt du contenu html
        /// </summary>
        private void Resize()
        {
            if (AutoResize)
            {
                var browser = webHelpBrowser;
                var doc = browser.Document as mshtml.HTMLDocument;

                // Javascript pour cacher le scroll et obtenir la taille de la page html
                var script = (mshtml.IHTMLScriptElement)doc.createElement("SCRIPT");
                script.type = "text/javascript";
                script.text = "function HideScroll(){document.documentElement.style.overflow = 'hidden';} function GetHeight(){return this.document.body.clientHeight; }";

                //Ajout du script dans le head html
                var head = doc.getElementsByTagName("head").OfType<mshtml.HTMLHeadElement>().First();
                head.appendChild((mshtml.IHTMLDOMNode)script);

                // récupération de la taille
                int htmlHeight = Convert.ToInt32(browser.InvokeScript("GetHeight"));

                // On masque le scroll si la fenetre dépasse pas la taille max.
                if (htmlHeight < MaxHeight)
                {
                    browser.InvokeScript("HideScroll");
                    if (htmlHeight > 0)
                    {
                        browser.Height = htmlHeight + 15;
                    }
                }
                else
                {
                    browser.Height = MaxHeight;
                }
            }
        }

        public static void OpenMainHelp(string helpName = null)
        {
            var startInfo = new ProcessStartInfo("IExplore.exe");
            string mainHelpPath = Path.Combine(HelpFolder, MainHelpFolder);
            string urlPath = Path.Combine(mainHelpPath, DefaultPage);
            if (!File.Exists(urlPath))
                return;

            // Navigation vers une page spécifique
            if (!string.IsNullOrEmpty(helpName) && Directory.Exists(mainHelpPath))
            {
                string pageName;
                if (helpName.ToLower().StartsWith("func_", StringComparison.Ordinal)) // Page de catégorie
                    pageName = helpName + ".htm";
                else // page de module
                    pageName = "Md_" + helpName.Remove(helpName.LastIndexOf('_')) + ".htm";
                string filePath = Directory.EnumerateFiles(mainHelpPath, pageName, SearchOption.AllDirectories).FirstOrDefault();
                if (!string.IsNullOrEmpty(filePath))
                {
                    urlPath = string.Format("{0}#t={1}", urlPath, GetSubPath(filePath, mainHelpPath));
                }
            }

            startInfo.Arguments = urlPath;
            Process.Start(startInfo);
        }

        private static string GetSubPath(string path, string basedir)
        {
            if (!path.StartsWith(basedir, StringComparison.Ordinal))
                throw new ApplicationException("invalid directory: " + path + " expected: " + basedir);

            int start = basedir.Length + 1; // +1 pour le séparateur '\'
            int length = path.Length - start;
            return path.Substring(start, length);
        }
    }
}
