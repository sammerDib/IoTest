using Agileo.MessageDataBus;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Agileo.Common.Tracing;
using Agileo.Common.Tracing.Listeners;
using Agileo.MessageDataBus.UI;

using UnitySC.Beckhoff.Emulator.EventArgs;
using UnitySC.Beckhoff.Emulator.Palette;

namespace UnitySC.Beckhoff.Emulator
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Variables
        
        private Simulator _simulator;

        #endregion Variables

        #region SimulatorProperties

        public TagListView TagList { get; set; }

        private string _clientConnected;
        public string ClientConnected
        {
            get
            {
                return _clientConnected;
            }
            set
            {
                if (_clientConnected != value)
                {
                    _clientConnected = value;
                    OnPropertyChanged(nameof(ClientConnected));
                }
            }
        }

        private string _currentState;
        public string CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    OnPropertyChanged(nameof(CurrentState));
                }
            }
        }

        public TraceManager Tracer = TraceManager.Instance();

        #endregion SimulatorProperties

        #region Properties

        public FrameworkElement InnerContent
        {
            get { return (FrameworkElement)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }

        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Events

        #region Main

        [Obsolete("Obsolete")]
        public MainWindow()
        {
            #region Simulator
            _simulator = new Simulator();
            _simulator.StateChanged += StateChanged;
            _simulator.ConnexionStateChanged += ConnectChanged;
            #endregion Simulator

            #region Init_IHM

            InitializeComponent();

            TagList = new TagListView(MessageDataBus.Instance);

            InnerContent = TagList;

            Palette.DataContext = new PaletteSelectorViewModel();

            TraceScreen.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, (_, _) =>
           {
               if (TraceScreen.SelectionMode == SelectionMode.Single)
               {
                   TraceScreen.SelectionMode = SelectionMode.Multiple;
                   TraceScreen.SelectAll();
               }
               else
               {
                   TraceScreen.SelectionMode = SelectionMode.Single;
                   TraceScreen.ScrollIntoView(TraceScreen.Items[TraceScreen.Items.Count - 1]);
               }
           }));

            CurrentState = "Init";

            if (MessageDataBus.Instance.IsDriverConnected("EK9000Driver"))
                ClientConnected = "Server is connected";
            else
                ClientConnected = "Server is not connected";
            this.DataContext = this;
            #endregion Init_IHM

            #region Tracer
            Tracer.Setup(new TracingConfig(), new DataLogFiltersConfig());
            ((BufferListener)Tracer.Listeners[1]).LineAdding += Tracer_LineAdding;
            #endregion Tracer

            CurrentState = "Running";
        }

        #endregion Main

        #region EventRaised

        [Obsolete("Obsolete")]
        private void Tracer_LineAdding(TraceLine traceLine)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                if (TraceScreen.Items.Count < 1000)
                {
                    TraceScreen.Items.Add(traceLine.Date + " => " + traceLine.Source + " " + traceLine.Text);
                    if (!TraceScreen.IsMouseCaptured)
                        TraceScreen.ScrollIntoView(TraceScreen.Items.GetItemAt(TraceScreen.Items.Count - 1));
                }
                else
                    TraceScreen.Items.Clear();
            });
        }

        private void ConnectChanged(object sender, ConnexionEventArgs e)
        {
            if (MessageDataBus.Instance.IsDriverConnected("EK9000Driver"))
                ClientConnected = "Server is connected";
            else
                ClientConnected = "Server is not connected";
        }

        private new void StateChanged(object sender, System.EventArgs e)
        {
            CurrentState = "State : " + _simulator.CurrentState.ToString();
        }

        #endregion EventRaised

        #region FrameWorkElement
        public static readonly DependencyProperty InnerContentProperty = DependencyProperty.Register("InnerContent", typeof(FrameworkElement), typeof(TagListView), new UIPropertyMetadata(null));
        #endregion FrameWorkElement

        #region custom

        #endregion custom

        #region Traces

        private void lbOutputs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string clip = "";
            try
            {
                foreach (string str in ((ListBox)sender).SelectedItems)
                {
                    clip = clip + str + "\n";
                }
            }
            catch (Exception)
            {
                // ignored
            }

            Clipboard.SetText(clip);
        }

        #endregion Traces
    }
}
