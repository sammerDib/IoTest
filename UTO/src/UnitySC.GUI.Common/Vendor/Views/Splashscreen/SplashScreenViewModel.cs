using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Splashscreen
{
    /// <summary>
    /// Splash screen : A window that usually appears while application is launching. 
    /// You can display loading progress, percentage of loading progress and/or the application name and version. 
    /// </summary>
    public class SplashScreenViewModel : Notifier
    {
        /// <summary>
        /// DesignTime constructor
        /// </summary>
        public SplashScreenViewModel() : this(1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenViewModel" /> class.
        /// </summary>
        public SplashScreenViewModel(uint stepMaximum)
        {
            StepMaximum = stepMaximum;
            if (stepMaximum == 1) IsProgressIndeterminate = true;
        }

        #region Properties

        private bool _isProgressIndeterminate;

        /// <summary>
        /// Gets or sets whether splash screen progress shows actual values or generic, continuous progress feedback.
        /// </summary>
        public bool IsProgressIndeterminate
        {
            get => _isProgressIndeterminate;
            private set => SetAndRaiseIfChanged(ref _isProgressIndeterminate, value);
        }

        /// <summary>
        /// Gets an opacity calculated based on <see cref="ProgressPercentage" />.
        /// </summary>
        public double ProgressOpacity => Math.Round(ProgressPercentage / 100, 1);

        /// <summary>
        /// Gets the progression (<see cref="Step" /> / <see cref="StepMaximum" />), in percent.
        /// </summary>
        public double ProgressPercentage => Math.Round((double) Step / StepMaximum * 100.0, 0);

        private string _softwareName;

        /// <summary>
        /// Gets or sets the name of software that show the splash screen.
        /// </summary>
        public string SoftwareName
        {
            get => _softwareName ?? "Equipment Controller";
            set => SetAndRaiseIfChanged(ref _softwareName, value);
        }

        private string _softwareVersion;

        /// <summary>
        /// Gets or sets the version of software that show the splash screen.
        /// </summary>
        public string SoftwareVersion
        {
            get => _softwareVersion ?? "1.0";
            set => SetAndRaiseIfChanged(ref _softwareVersion, value);
        }

        private int _step = 1;

        /// <summary>
        /// Gets or sets the current step of splash screen's progression.
        /// </summary>
        /// <remarks>
        /// If you set Value to a number that is less than <see cref="StepMinimum" />, Value is set to
        /// <see cref="StepMinimum" />.
        /// If you set Value to a number that is greater than <see cref="StepMaximum" />, Value is set to
        /// <see cref="StepMaximum" />.
        /// </remarks>
        public int Step
        {
            get => _step;
            private set
            {
                // Coerce the step in progress range
                if (value < StepMinimum) value = (int) StepMinimum;
                if (StepMaximum < value) value = (int) StepMaximum;

                SetAndRaiseIfChanged(ref _step, value);
                OnPropertyChanged(nameof(ProgressPercentage));
                OnPropertyChanged(nameof(ProgressOpacity));
            }
        }

        private string _stepDescription = string.Empty;

        /// <summary>
        /// Gets or sets the text describing the action on-going at the current step.
        /// </summary>
        public string StepDescription
        {
            get => _stepDescription;
            private set => SetAndRaiseIfChanged(ref _stepDescription, value);
        }

        /// <summary>
        /// Gets the highest possible value of splash screen's progression.
        /// </summary>
        public uint StepMaximum { get; }

        /// <summary>
        /// Gets the lowest possible value of splash screen's progression.
        /// </summary>
        public static uint StepMinimum => 1;

        private string _equipmentName = "Equipment";

        /// <summary>
        /// Gets or sets the name of the tool on which software is running.
        /// </summary>
        public string EquipmentName
        {
            get => _equipmentName;
            set => SetAndRaiseIfChanged(ref _equipmentName, value);
        }

        private string _mainCauseException;

        /// <summary>
        /// Display the main cause of the thrown exception
        /// </summary>
        public string MainCauseException
        {
            get => _mainCauseException;
            private set
            {
                SetAndRaiseIfChanged(ref _mainCauseException, value);
                OnPropertyChanged(nameof(ResizeMode));
            }
        }

        private string _exceptionDetails;

        /// <summary>
        /// Display Exception details when an exception was thrown
        /// </summary>
        public string ExceptionDetails
        {
            get => _exceptionDetails;
            private set => SetAndRaiseIfChanged(ref _exceptionDetails, value);
        }

        /// <summary>
        /// Define the resize mode as the Enum <see cref="ResizeMode"/>.
        /// </summary>
        public ResizeMode ResizeMode => string.IsNullOrEmpty(MainCauseException)
            ? ResizeMode.NoResize
            : ResizeMode.CanResizeWithGrip;

        private ICommand _shutDownCommand;

        /// <summary>
        /// Define the Shut Down Command
        /// </summary>
        public ICommand ShutDownCommand => _shutDownCommand ??= new DelegateCommand(ShutDownCommandExecute, ShutDownCommandCanExecute);

        private bool ShutDownCommandCanExecute() => !string.IsNullOrEmpty(MainCauseException);

        private void ShutDownCommandExecute() => App.Instance?.ShutdownActions();

        #endregion Properties

        #region Public API

        /// <summary>
        /// Displays the specified message as the message />.
        /// </summary>
        /// <param name="message">Message to set as message.</param>
        public void Display(string message)
        {
            UpdateDisplay(message);
        }

        /// <summary>
        /// Increments the splash screen progress by the specified amount and changes the stepDescription/>.
        /// </summary>
        /// <param name="stepDescription">Message that will be displayed in SplashScreen window</param>
        /// <param name="count">Increment step count</param>
        public void Increment(string stepDescription, int count = 1)
        {
            UpdateDisplay(stepDescription, count);
        }

        /// <summary>
        /// Displays the specified exception.
        /// </summary>
        /// <param name="mainCause"></param>
        /// <param name="exceptionDetails"></param>
        public void DisplayException(string mainCause, string exceptionDetails)
        {
            MainCauseException = mainCause;
            ExceptionDetails = exceptionDetails;
        }

        /// <summary>
        /// Hides the exception being viewed
        /// </summary>
        public void HideException()
        {
            MainCauseException = null;
            ExceptionDetails = null;
        }

        #endregion Public API

        #region Other Methods

        /// <summary>
        /// Should ensure that current view model state is rendered on screen.
        /// </summary>
        /// <remarks>
        /// From : http://geekswithblogs.net/NewThingsILearned/archive/2008/08/25/refresh--update-wpf-controls.aspx
        /// The trick is to call the Invoke method with DispatcherPriority of Render or lower.
        /// Since we don't want to do anything, I created an empty delegate.
        /// So how come this achieves refresh functionality?
        /// By calling Dispatcher.Invoke, the code essentially asks the system to execute all operations that are Render or
        /// higher priority,
        /// thus the control will then render itself (drawing the new content).
        /// Afterwards, it will then execute the provided delegate (which is our empty method).
        /// </remarks>
        private static void Refresh()
        {
            Application.Current?.Dispatcher?.BeginInvoke(DispatcherPriority.Background, (Action) delegate { });
        }

        /// <summary>
        /// Updates the current step's number and the text describing it.
        /// </summary>
        /// <param name="newMessage">New message to display. When <see langword="null" />, text is not updated.</param>
        /// <param name="delta">Value added to the current step number. Coerced in range [1, total number of steps].</param>
        /// <remarks>
        /// When the total number of steps is undetermined (less than or equal to zero), the current step number is not
        /// updated.
        /// </remarks>
        private void UpdateDisplay(string newMessage = null, int delta = 0)
        {
            if (Application.Current?.Dispatcher?.CheckAccess() ?? false)
            {
                // In UI thread, update the values
                if (newMessage != null) StepDescription = newMessage;
                Step += delta;

                // Request a refresh, but seems to be done asynchronously and not working every time...
                Refresh();
            }
            else
            {
                // Not in UI thread, invoke dispatcher to get back in UI thread.
                Application.Current?.Dispatcher?.BeginInvoke(new Action(() => UpdateDisplay(newMessage, delta)));
            }
        }

        #endregion Other Methods
    }
}
