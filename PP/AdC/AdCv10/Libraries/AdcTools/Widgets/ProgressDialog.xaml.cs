using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;


namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    // Un dialogue avec une progress bar qui affiche l'avancement d'une 
    // tache.
    ///////////////////////////////////////////////////////////////////////
    public partial class ProgressDialog : Window
    {
        //=================================================================
        // Data et Proprietees
        //=================================================================
        public BackgroundWorker worker = new BackgroundWorker();
        public Action<object, DoWorkEventArgs> action;
        public double Maximum
        {
            get { return progressBar.Maximum; }
            set { progressBar.Maximum = value; }
        }

        public double Minimum
        {
            get { return progressBar.Minimum; }
            set { progressBar.Minimum = value; }
        }

        public string Text
        {
            get { return label.Content.ToString(); }
            set { label.Content = value; }
        }


        //=================================================================
        // Constructeur
        //=================================================================
        public ProgressDialog()
        {
            InitializeComponent();

            Title = Assembly.GetExecutingAssembly().GetName().Name;
            Owner = Application.Current.MainWindow;

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.DoWork += DoWork;
        }

        //=================================================================
        // Surcharge de ShowDialog
        //=================================================================
        public bool? ShowDialog(object argument = null)
        {
            worker.RunWorkerAsync(argument);
            bool? b = base.ShowDialog();
            return b;
        }

        //=================================================================
        // Gestion du Worker
        //=================================================================
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (action != null)
                action.Invoke(sender, e);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                AttentionMessageBox.Show(e.Error.Message);
            }
            Close();
        }


        //=================================================================
        // Cancel
        //=================================================================
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            worker.CancelAsync();
            base.OnClosing(e);
        }

    }
}
