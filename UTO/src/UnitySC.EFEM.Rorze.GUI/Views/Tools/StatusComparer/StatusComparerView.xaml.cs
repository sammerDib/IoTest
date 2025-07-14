using System;
using System.Windows;
using System.Windows.Controls;

using Agileo.Common.Logging;

using Castle.Core.Internal;

using UnitySC.EFEM.Rorze.GUI.Views.Tools.StatusComparer.Enums;

namespace UnitySC.EFEM.Rorze.GUI.Views.Tools.StatusComparer
{
    /// <summary>
    /// Interaction logic for StatusComparerView.xaml
    /// </summary>
    public partial class StatusComparerView
    {
        public StatusComparerView()
        {
            InitializeComponent();
        }

        private void StatusSourceCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!sender.Equals(StatusSourceCb))
                return;

            var statusComparer = (StatusComparer)DataContext;

            statusComparer.StatusTypes.Clear();
            statusComparer.UnselectAllStatusTypes();

            if (e.AddedItems.IsNullOrEmpty())
            {
                statusComparer.SelectedStatusSource = null;
                return;
            }

            // We are in a ComboBox: the unique selected element is the new added item.
            var selectedValue = e.AddedItems[0].ToString();

            if (string.IsNullOrEmpty(selectedValue))
            {
                statusComparer.SelectedStatusSource = null;
                return;
            }

            statusComparer.SelectedStatusSource = (StatusSource)Enum.Parse(typeof(StatusSource), selectedValue);

            switch (statusComparer.SelectedStatusSource)
            {
                case StatusSource.RV201:
                    foreach (var value in Enum.GetValues(typeof(RV201Status)))
                    {
                        var statusType = (RV201Status)value;
                        statusComparer.StatusTypes.Add(statusType.ToString());
                    }

                    break;
                case StatusSource.RV101:
                    foreach (var value in Enum.GetValues(typeof(RV101Status)))
                    {
                        var statusType = (RV101Status)value;
                        statusComparer.StatusTypes.Add(statusType.ToString());
                    }

                    break;
                case StatusSource.RE201:
                    foreach (var value in Enum.GetValues(typeof(RE201Status)))
                    {
                        var statusType = (RE201Status)value;
                        statusComparer.StatusTypes.Add(statusType.ToString());
                    }

                    break;
                case StatusSource.RR754To757:
                    foreach (var value in Enum.GetValues(typeof(RR754To757Status)))
                    {
                        var statusType = (RR754To757Status)value;
                        statusComparer.StatusTypes.Add(statusType.ToString());
                    }

                    break;
                case StatusSource.RA420:
                    foreach (var value in Enum.GetValues(typeof(RA420Status)))
                    {
                        var statusType = (RA420Status)value;
                        statusComparer.StatusTypes.Add(statusType.ToString());
                    }

                    break;
                case StatusSource.Dio0:
                    foreach (var value in Enum.GetValues(typeof(Dio0Status)))
                    {
                        var statusType = (Dio0Status)value;
                        statusComparer.StatusTypes.Add(statusType.ToString());
                    }

                    break;
                case StatusSource.Dio1:
                    foreach (var value in Enum.GetValues(typeof(Dio1Status)))
                    {
                        var statusType = (Dio1Status)value;
                        statusComparer.StatusTypes.Add(statusType.ToString());
                    }

                    break;
                case StatusSource.Dio2:
                    foreach (var value in Enum.GetValues(typeof(Dio2Status)))
                    {
                        var statusType = (Dio2Status)value;
                        statusComparer.StatusTypes.Add(statusType.ToString());
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{statusComparer.SelectedStatusSource} is not interpreted.");
            }
        }

        private void StatusTypeCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!sender.Equals(StatusTypeCb))
                return;

            var statusComparer = (StatusComparer)DataContext;

            if (e.AddedItems.IsNullOrEmpty())
            {
                statusComparer.UnselectAllStatusTypes();
                return;
            }

            // We are in a ComboBox: the unique selected element is the new added item.
            var selectedValue = e.AddedItems[0].ToString();

            if (string.IsNullOrEmpty(selectedValue))
            {
                statusComparer.UnselectAllStatusTypes();
                return;
            }

            switch (statusComparer.SelectedStatusSource)
            {
                case StatusSource.RV201:
                    statusComparer.SelectedRV201Status = (RV201Status)Enum.Parse(typeof(RV201Status), selectedValue);
                    break;
                case StatusSource.RV101:
                    statusComparer.SelectedRV101Status = (RV101Status)Enum.Parse(typeof(RV101Status), selectedValue);
                    break;
                case StatusSource.RE201:
                    statusComparer.SelectedRE201Status = (RE201Status)Enum.Parse(typeof(RE201Status), selectedValue);
                    break;
                case StatusSource.RR754To757:
                    statusComparer.SelectedRR754To757Status = (RR754To757Status)Enum.Parse(typeof(RR754To757Status), selectedValue);
                    break;
                case StatusSource.RA420:
                    statusComparer.SelectedRA420Status = (RA420Status)Enum.Parse(typeof(RA420Status), selectedValue);
                    break;
                case StatusSource.Dio0:
                    statusComparer.SelectedDio0Status = (Dio0Status)Enum.Parse(typeof(Dio0Status), selectedValue);
                    break;
                case StatusSource.Dio1:
                    statusComparer.SelectedDio1Status = (Dio1Status)Enum.Parse(typeof(Dio1Status), selectedValue);
                    break;
                case StatusSource.Dio2:
                    statusComparer.SelectedDio2Status = (Dio2Status)Enum.Parse(typeof(Dio2Status), selectedValue);
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{statusComparer.SelectedStatusSource} is not interpreted.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!sender.Equals(CompareBtn))
                return;

            try
            {
                var statusComparer = (StatusComparer)DataContext;
                StatusComparerTb.Text = statusComparer.ParseAndCompare();
            }
            catch (Exception ex)
            {
                Logger.GetLogger(nameof(StatusComparer)).Warning(ex, "Error occurred while preparing statuses parsing.");
                StatusComparerTb.Text = "/!\\ Error occurred while preparing parsing";
            }
        }
    }
}
