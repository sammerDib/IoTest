using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ADC.ViewModel;

namespace ADC.View.RunTime
{
    public partial class LogControl : UserControl
    {
        private System.Windows.Threading.DispatcherTimer _timerLog;
        private ImageMemoryViewModel _imageMemoryViewModel = new ImageMemoryViewModel();
        private const int NbOfMaxLogLines = 5000;

        //=================================================================
        // 
        //=================================================================
        public LogControl()
        {
            //FrameworkCompatibilityPreferences.AreInactiveSelectionHighlightBrushKeysSupported = false;
            //---------------------------------------------------------
            // WPF
            //---------------------------------------------------------
            InitializeComponent();
            DataContext = _imageMemoryViewModel;

            //---------------------------------------------------------
            // Timer pour mettre à jour l'IHM et afficher le log
            //---------------------------------------------------------
            _timerLog = new System.Windows.Threading.DispatcherTimer();
            _timerLog.Interval = new TimeSpan(0, 0, 0, 0, 200/*ms*/);
            _timerLog.Tick += new EventHandler(timerLog_Elapsed);
            _timerLog.Start();
            CommandBinding cb = new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute);
            lbLog.CommandBindings.Add(cb);
            menuCopy.CommandBindings.Add(cb);
        }

        private void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            ListBox lb = e.OriginalSource as ListBox;
            string copyContent = String.Empty;
            foreach (string item in lb.SelectedItems)
            {
                copyContent += item;
                copyContent += Environment.NewLine;
            }
            Clipboard.SetText(copyContent);
        }

        private void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBox lb = e.OriginalSource as ListBox;
            // CanExecute only if there is one or more selected Item.   
            if (lb.SelectedItems.Count > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }


        //=================================================================
        // Timer event to refresh UI
        //=================================================================
        //private int count = 0;	//to debug the timer
        private void timerLog_Elapsed(object sender, EventArgs e)
        {
            List<string> logstr = AdcTools.Serilog.StringSink.EatString();
            if (logstr.Any())
            {
                logstr.ForEach(str => lbLog.Items.Add(str));
                int nbItems = lbLog.Items.Count;
                if (nbItems > NbOfMaxLogLines)
                {
                    for (int i = 0; i < nbItems - NbOfMaxLogLines; i++)
                        lbLog.Items.RemoveAt(0);
                    nbItems = lbLog.Items.Count;
                }
                // Make sure the last item is made visible               
                lbLog.ScrollIntoView(lbLog.Items[nbItems - 1]);
            }

            _imageMemoryViewModel.Value = ADCEngine.ADC.Instance.ImageMemory();
            _imageMemoryViewModel.UpdateCpuLoad();
        }
    }
}
