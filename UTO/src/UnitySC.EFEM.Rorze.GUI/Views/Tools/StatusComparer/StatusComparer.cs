using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Tracing;
using Agileo.GUI.Components.Tools;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.GUI.Views.Tools.StatusComparer.Enums;
using UnitySC.GUI.Common.Vendor.Helpers;

using LoadPortGpioStatus =
    UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Status.LoadPortGpioStatus;

namespace UnitySC.EFEM.Rorze.GUI.Views.Tools.StatusComparer
{
    public class StatusComparer : Tool
    {
        private string _status1 = "";

        private string _status2 = "";

        #region Constructor

        static StatusComparer()
        {
            DataTemplateGenerator.Create(typeof(StatusComparer), typeof(StatusComparerView));
        }

        public StatusComparer(string id, IIcon icon = null)
            : base(id, icon)
        {
            StatusSources = Enum.GetValues(typeof(StatusSource)).Cast<StatusSource>().ToList();
        }

        #endregion



        #region Properties

        /// <summary>The first status to be parsed.</summary>
        public string Status1
        {
            get => _status1;
            set
            {
                if (_status1.Equals(value))
                {
                    return;
                }

                _status1 = value;
                OnPropertyChanged(nameof(Status1));
            }
        }

        /// <summary>The second status to be parsed.</summary>
        public string Status2
        {
            get => _status2;
            set
            {
                if (_status2.Equals(value))
                {
                    return;
                }

                _status2 = value;
                OnPropertyChanged(nameof(Status2));
            }
        }

        /// <summary>
        /// The selected <see cref="StatusSource" />. Value is null if nothing is selected.
        /// </summary>
        internal StatusSource? SelectedStatusSource { get; set; }

        /// <summary>
        /// The selected <see cref="RV201Status" /> Value is null if nothing is selected or if the
        /// <see cref="SelectedStatusSource" /> is not the RV201.
        /// </summary>
        internal RV201Status? SelectedRV201Status { get; set; }

        /// <summary>
        /// The selected <see cref="RE201Status" /> Value is null if nothing is selected or if the
        /// <see cref="SelectedStatusSource" /> is not the RE201.
        /// </summary>
        internal RE201Status? SelectedRE201Status { get; set; }

        /// <summary>
        /// The selected <see cref="RV101Status" /> Value is null if nothing is selected or if the
        /// <see cref="SelectedStatusSource" /> is not the RV101.
        /// </summary>
        internal RV101Status? SelectedRV101Status { get; set; }

        /// <summary>
        /// The selected <see cref="RR754To757Status" /> Value is null if nothing is selected or if the
        /// <see cref="SelectedStatusSource" /> is not the RR75[4-7].
        /// </summary>
        internal RR754To757Status? SelectedRR754To757Status { get; set; }

        /// <summary>
        /// The selected <see cref="RA420Status" /> Value is null if nothing is selected or if the
        /// <see cref="SelectedStatusSource" /> is not the RA420.
        /// </summary>
        internal RA420Status? SelectedRA420Status { get; set; }

        /// <summary>
        /// The selected <see cref="Dio0Status" /> Value is null if nothing is selected or if the
        /// <see cref="SelectedStatusSource" /> is not the DIO0.
        /// </summary>
        internal Dio0Status? SelectedDio0Status { get; set; }

        /// <summary>
        /// The selected <see cref="Dio1Status" /> Value is null if nothing is selected or if the
        /// <see cref="SelectedStatusSource" /> is not the DIO1.
        /// </summary>
        internal Dio1Status? SelectedDio1Status { get; set; }

        /// <summary>
        /// The selected <see cref="Dio2Status" /> Value is null if nothing is selected or if the
        /// <see cref="SelectedStatusSource" /> is not the DIO2.
        /// </summary>
        internal Dio2Status? SelectedDio2Status { get; set; }

        public List<StatusSource> StatusSources { get; }

        public ObservableCollection<string> StatusTypes { get; } = new();

        #endregion Properties

        #region Internal Methods

        internal void UnselectAllStatusTypes()
        {
            SelectedRV201Status = null;
            SelectedRV101Status = null;
            SelectedRE201Status = null;
            SelectedRA420Status = null;
            SelectedDio0Status = null;
            SelectedDio1Status = null;
            SelectedDio2Status = null;
        }

        internal string ParseAndCompare()
        {
            switch (SelectedStatusSource)
            {
                case StatusSource.RV201:
                    return ParseAndCompareRV201Statuses(Status1, Status2);
                case StatusSource.RE201:
                    return ParseAndCompareRE201Statuses(Status1, Status2);
                case StatusSource.RV101:
                    return ParseAndCompareRV101Statuses(Status1, Status2);
                case StatusSource.RR754To757:
                    return ParseAndCompareRR754To757Statuses(Status1, Status2);
                case StatusSource.RA420:
                    return ParseAndCompareRA420Statuses(Status1, Status2);
                case StatusSource.Dio0:
                    return ParseAndCompareDio0Statuses(Status1, Status2);
                case StatusSource.Dio1:
                    return ParseAndCompareDio1Statuses(Status1, Status2);
                case StatusSource.Dio2:
                    return ParseAndCompareDio2Statuses(Status1, Status2);
                case null:
                    throw new InvalidOperationException(
                        "Attempt to compare statuses without indicating their source.");
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{SelectedStatusSource} is not interpreted.");
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private string ParseAndCompareRV201Statuses(string status1, string status2)
        {
            switch (SelectedRV201Status)
            {
                case RV201Status.STAT:
                    return ParseAndCompareStatuses(typeof(RV201LoadPortStatus), status1, status2);
                case RV201Status.GPIO:
                    return ParseAndCompareStatuses(typeof(LoadPortGpioStatus), status1, status2);
                case null:
                    throw new InvalidOperationException(
                        "Attempt to compare statuses without indicating their type.");
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{SelectedRV201Status} is not interpreted.");
            }
        }

        private string ParseAndCompareRV101Statuses(string status1, string status2)
        {
            switch (SelectedRV101Status)
            {
                case RV101Status.STAT:
                    return ParseAndCompareStatuses(typeof(RV101LoadPortStatus), status1, status2);
                case RV101Status.GPIO:
                    return ParseAndCompareStatuses(typeof(LoadPortGpioStatus), status1, status2);
                case null:
                    throw new InvalidOperationException(
                        "Attempt to compare statuses without indicating their type.");
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{SelectedRV101Status} is not interpreted.");
            }
        }

        private string ParseAndCompareRE201Statuses(string status1, string status2)
        {
            switch (SelectedRE201Status)
            {
                case RE201Status.STAT:
                    return ParseAndCompareStatuses(typeof(RE201LoadPortStatus), status1, status2);
                case RE201Status.GPIO:
                    return ParseAndCompareStatuses(typeof(RE201GpioStatus), status1, status2);
                case null:
                    throw new InvalidOperationException(
                        "Attempt to compare statuses without indicating their type.");
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{SelectedRE201Status} is not interpreted.");
            }
        }

        private string ParseAndCompareRR754To757Statuses(string status1, string status2)
        {
            switch (SelectedRR754To757Status)
            {
                case RR754To757Status.STAT:
                    return ParseAndCompareStatuses(typeof(RobotStatus), status1, status2);
                case RR754To757Status.GPIO:
                    return ParseAndCompareStatuses(typeof(RobotGpioStatus), status1, status2);
                case null:
                    throw new InvalidOperationException(
                        "Attempt to compare statuses without indicating their type.");
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{SelectedRR754To757Status} is not interpreted.");
            }
        }

        private string ParseAndCompareRA420Statuses(string status1, string status2)
        {
            switch (SelectedRA420Status)
            {
                case RA420Status.STAT:
                    return ParseAndCompareStatuses(typeof(AlignerStatus), status1, status2);
                case RA420Status.GPIO:
                    return ParseAndCompareStatuses(typeof(AlignerGpioStatus), status1, status2);
                case null:
                    throw new InvalidOperationException(
                        "Attempt to compare statuses without indicating their type.");
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{SelectedRA420Status} is not interpreted.");
            }
        }

        private string ParseAndCompareDio0Statuses(string status1, string status2)
        {
            switch (SelectedDio0Status)
            {
                case Dio0Status.STAT:
                    return ParseAndCompareStatuses(typeof(IoStatus), status1, status2);
                case Dio0Status.GPIO:
                    return ParseAndCompareStatuses(typeof(RC550GeneralIoStatus), status1, status2);
                case Dio0Status.GDIO_FanDetection:
                    return ParseAndCompareStatuses(
                        typeof(FanDetectionSignalData),
                        status1,
                        status2);
                case Dio0Status.GDIO_E84:
                    return ParseAndCompareStatuses(typeof(E84SignalData), status1, status2);
                case null:
                    throw new InvalidOperationException(
                        "Attempt to compare statuses without indicating their type.");
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{SelectedDio0Status} is not interpreted.");
            }
        }

        private string ParseAndCompareDio1Statuses(string status1, string status2)
        {
            switch (SelectedDio1Status)
            {
                case Dio1Status.STAT:
                    return ParseAndCompareStatuses(typeof(IoStatus), status1, status2);
                case Dio1Status.GDIO:
                    return ParseAndCompareStatuses(typeof(Dio1SignalData), status1, status2);
                case null:
                    throw new InvalidOperationException(
                        "Attempt to compare statuses without indicating their type.");
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{SelectedDio1Status} is not interpreted.");
            }
        }

        private string ParseAndCompareDio2Statuses(string status1, string status2)
        {
            switch (SelectedDio2Status)
            {
                case Dio2Status.STAT:
                    return ParseAndCompareStatuses(typeof(IoStatus), status1, status2);
                case Dio2Status.GDIO:
                    return ParseAndCompareStatuses(typeof(Dio2SignalData), status1, status2);
                case null:
                    throw new InvalidOperationException(
                        "Attempt to compare statuses without indicating their type.");
                default:
                    throw new ArgumentOutOfRangeException(
                        $"{SelectedDio2Status} is not interpreted.");
            }
        }

        private static string ParseAndCompareStatuses(
            Type statusType,
            string status1,
            string status2)
        {
            var cmp = string.Empty;

            try
            {
                var statusProperties = statusType.GetProperties();

                // Use reflection to create an interpreted status from each given string.
                var interpretedStatusConstructor =
                    statusType.GetConstructor(new[] { typeof(string) });

                if (interpretedStatusConstructor == null)
                {
                    throw new InvalidOperationException(
                        $"{statusType.Name} does not contain a constructor with expected signature.");
                }

                var interpretedStatus1 =
                    interpretedStatusConstructor.Invoke(new object[] { status1 });
                var interpretedStatus2 =
                    interpretedStatusConstructor.Invoke(new object[] { status2 });

                // Compare each property of the interpreted statuses to see where are the differences.
                foreach (var propertyInfo in statusProperties)
                {
                    var status1PropertyValue = propertyInfo.GetValue(interpretedStatus1);
                    var status2PropertyValue = propertyInfo.GetValue(interpretedStatus2);

                    if (!Equals(status1PropertyValue, status2PropertyValue))
                    {
                        cmp +=
                            $"{propertyInfo.Name}: {status1PropertyValue} -> {status2PropertyValue}\n";
                    }
                }
            }
            catch (Exception e)
            {
                TraceManager.Instance()
                    .Trace(TraceLevelType.Warning, e, "Error occurred while comparing statuses.");
                cmp = "/!\\ Error occurred";
            }

            return cmp;
        }

        #endregion Private Methods
    }
}
