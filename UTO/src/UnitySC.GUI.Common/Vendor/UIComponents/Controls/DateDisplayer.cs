using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class DateDisplayer : Control
    {
        static DateDisplayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateDisplayer),
                new FrameworkPropertyMetadata(typeof(DateDisplayer)));
        }

        public DateDisplayer()
        {
            SetCurrentValue(DisplayDateProperty, DateTime.Now.Date);
        }

        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(
            nameof(DisplayDate), typeof(DateTime), typeof(DateDisplayer),
            new PropertyMetadata(default(DateTime), DisplayDatePropertyChangedCallback));

        private static void DisplayDatePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((DateDisplayer)dependencyObject).UpdateComponents();
        }

        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        private static readonly DependencyPropertyKey ComponentOneContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "ComponentOneContent", typeof(string), typeof(DateDisplayer),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ComponentOneContentProperty =
            ComponentOneContentPropertyKey.DependencyProperty;

        public string ComponentOneContent
        {
            get { return (string)GetValue(ComponentOneContentProperty); }
            private set { SetValue(ComponentOneContentPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ComponentTwoContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "ComponentTwoContent", typeof(string), typeof(DateDisplayer),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ComponentTwoContentProperty =
            ComponentTwoContentPropertyKey.DependencyProperty;

        public string ComponentTwoContent
        {
            get { return (string)GetValue(ComponentTwoContentProperty); }
            private set { SetValue(ComponentTwoContentPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ComponentThreeContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "ComponentThreeContent", typeof(string), typeof(DateDisplayer),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ComponentThreeContentProperty =
            ComponentThreeContentPropertyKey.DependencyProperty;

        public string ComponentThreeContent
        {
            get { return (string)GetValue(ComponentThreeContentProperty); }
            private set { SetValue(ComponentThreeContentPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsDayInFirstComponentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IsDayInFirstComponent", typeof(bool), typeof(DateDisplayer),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsDayInFirstComponentProperty =
            IsDayInFirstComponentPropertyKey.DependencyProperty;

        public bool IsDayInFirstComponent
        {
            get { return (bool)GetValue(IsDayInFirstComponentProperty); }
            private set { SetValue(IsDayInFirstComponentPropertyKey, value); }
        }

        private void UpdateComponents()
        {
            var culture = Language.GetSpecificCulture();
            var dateTimeFormatInfo = GetDateFormat(culture);

            var month = DisplayDate.ToString(dateTimeFormatInfo.MonthDayPattern.Replace("MMMM", "MMM"), culture);
            ComponentOneContent = ToTitleCase(month, culture); //Day Month following culture order. We don't want the month to take too much space

            var day = DisplayDate.ToString("ddd,", culture);
            ComponentTwoContent = ToTitleCase(day, culture); // Day of week first

            var year = DisplayDate.ToString("yyyy", culture);
            ComponentThreeContent = ToTitleCase(year, culture); // Year always top
        }

        private static string ToTitleCase(string text, CultureInfo culture, string separator = " ")
        {
            var textInfo = culture.TextInfo;
            var lowerText = textInfo.ToLower(text);
            var words = lowerText.Split(new[] { separator }, StringSplitOptions.None);
            return string.Join(separator, words.Select(v => textInfo.ToTitleCase(v)));
        }

        private static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
        {
            if (culture.Calendar is GregorianCalendar)
            {
                return culture.DateTimeFormat;
            }
            GregorianCalendar foundCal = null;
            DateTimeFormatInfo dtfi;
            foreach (var cal in culture.OptionalCalendars)
            {
                var calendar = cal as GregorianCalendar;
                if (calendar == null) continue;
                // Return the first Gregorian calendar with CalendarType == Localized
                // Otherwise return the first Gregorian calendar
                if (foundCal == null)
                {
                    foundCal = calendar;
                }
                if (calendar.CalendarType != GregorianCalendarTypes.Localized) continue;
                foundCal = calendar;
                break;
            }
            if (foundCal == null)
            {
                // if there are no GregorianCalendars in the OptionalCalendars list, use the invariant dtfi
                dtfi = ((CultureInfo)CultureInfo.InvariantCulture.Clone()).DateTimeFormat;
                dtfi.Calendar = new GregorianCalendar();
            }
            else
            {
                dtfi = ((CultureInfo)culture.Clone()).DateTimeFormat;
                dtfi.Calendar = foundCal;
            }
            return dtfi;
        }

    }

}
