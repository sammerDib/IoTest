using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using UnitySC.DataAccess.Dto.ModelDto;

//using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.Result.CommonUI.ViewModel.LotWafer;
using UnitySC.Result.CommonUI.ViewModel.Wafers;

namespace UnitySC.Result.CommonUI.View.Converter
{
    /// <summary>
    /// Convert wafer state to a color
    /// </summary>
    public class ConvertWaferStateToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = new SolidColorBrush();
            if (value == null)
                return brush;

            var waferState = (ResultState)value;
            if (waferState > ResultState.Ok)
            {
                switch (waferState)
                {
                    case ResultState.Partial:
                        brush = (SolidColorBrush)Application.Current.FindResource("PartialResultBorderColor");
                        break;

                    case ResultState.Rework:
                        brush = (SolidColorBrush)Application.Current.FindResource("ReworkResultBorderColor");
                        break;

                    case ResultState.Reject:
                        brush = (SolidColorBrush)Application.Current.FindResource("RejectResultBorderColor");
                        break;

                    default:
                        break;
                }
            }
            // No border in case of error or NotProcess
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvertLotToWaferSlot : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                var nullslot = new WaferSlotVM[25];
                for (int i = 0; i < 25; i++)
                    nullslot[i] = new WaferSlotVM();
                return nullslot;
            }

            var LotSlotvm = (LotWaferSlotVM[])value;
            var waferslotvm = new WaferSlotVM[LotSlotvm.Length];
            for (int i = 0; i < LotSlotvm.Length; i++)
            {
                var slot = LotSlotvm[i];
                if (slot == null)
                    waferslotvm[i] = new WaferSlotVM();
                else if (slot.Item == null)
                    waferslotvm[i] = new WaferSlotVM(slot.SlotID, false);
                else
                    waferslotvm[i] = new WaferSlotVM(slot.SlotID, slot.State, slot.Item.Id);
            }
            return waferslotvm;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Add a border color to the wafer is it not process
    /// </summary>
    public class WaferBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var waferState = (ResultState)value;
            var brush = new SolidColorBrush();
            if (waferState == ResultState.NotProcess)
            {
                brush = (SolidColorBrush)Application.Current.FindResource("NullResultBorderColor");
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ComboboxSelectedValueConverter : IValueConverter
    {
        private readonly ResultQuery _defaultResult = new ResultQuery() { Id = -1, Name = "All" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                if (value == null)
                    return _defaultResult.Name;
                else
                    return (string)value;
            }
            else
            {
                if (value == null)
                    return _defaultResult;
                else
                    return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
            //if (parameter == null)
            //{
            //    if (value == null)
            //        return _defaultResult.Name;
            //    else
            //        return (string)value;
            //}
            //else
            //{
            //    if (value == null)
            //        return _defaultResult;
            //    else
            //        return value;
            //}
        }
    }

    public class ReferenceEqualsMultiConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Length == 2 && ReferenceEquals(values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
