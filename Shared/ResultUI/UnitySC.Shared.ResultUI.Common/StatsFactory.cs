using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.ResultUI.Common.Enums;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts.Histograms;

namespace UnitySC.Shared.ResultUI.Common
{
    public static class StatsFactory
    {
        public static KeyValuePair<HistogramType, string> LastSelectedHistogram;
        public static ResultValueType LastResultValueType;

        /// <summary>
        /// Return Enum description.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(string name)
        {
            var type = typeof(T);

            var field = type.GetField(name);
            object[] customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string description = customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
            return description;
        }

        /// <summary>
        /// Return enums with descriptions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<KeyValuePair<T, string>> GetEnumsWithDescriptions<T>()
        {
            var histoTypes = new List<KeyValuePair<T, string>>();
            var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            foreach (var value in values)
            {
                string name = Enum.GetName(typeof(T), value);
                histoTypes.Add(new KeyValuePair<T, string>(value, GetEnumDescription<T>(name)));
            }
            return histoTypes;
        }

        /// <summary>
        /// Return the view model of the current histogram to display.
        /// </summary>
        /// <param name="histoType"></param>
        /// <returns></returns>
        public static HistogramVMBase GetCurrentHistogram(HistogramType histoType)
        {
            HistogramVMBase histoVM = null;
            switch (histoType)
            {
                case HistogramType.Average:
                    histoVM = new AverageHistogramVM();
                    break;

                case HistogramType.Cumul:
                    histoVM = new TotalCumulationHistogramVM();
                    break;

                case HistogramType.MultibarsSlots:
                    histoVM = new CumulationBySlotsHistogramVM();
                    break;

                case HistogramType.MultibarsDefectClasses:
                    histoVM = new SlotDefectClassesHistogramVM();
                    break;

                default:
                    // Exception
                    break;
            }
            return histoVM;
        }
    }
}
