using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class ResourcesHelper
    {
        public static T Get<T>(string resourceKey)
        {
            var application = Application.Current;

            if (application == null)
            {
                return default;
            }

            if (application.Dispatcher != null && !application.Dispatcher.CheckAccess())
            {
                return application.Dispatcher.Invoke(() => Get<T>(resourceKey));
            }

            var resource = application.TryFindResource(resourceKey);

            return resource is T typedResource ? typedResource : default;
        }

        public static Dictionary<string, T> GetAll<T>()
        {
            var application = Application.Current;

            if (application == null)
            {
                return new Dictionary<string, T>();
            }

            if (application.Dispatcher != null && !application.Dispatcher.CheckAccess())
            {
                return application.Dispatcher.Invoke(GetAll<T>);
            }

            var dictionary = new Dictionary<string, T>();
            RecursivePopulate(application.Resources, dictionary);
            return dictionary;
        }

        private static void RecursivePopulate<T>(ResourceDictionary dictionary, IDictionary<string, T> collection)
        {
            if (dictionary == null) return;

            foreach (var entry in dictionary.MergedDictionaries)
            {
                RecursivePopulate(entry, collection);
            }

            foreach (DictionaryEntry entry in dictionary)
            {
                if (entry.Value is not T typedResource || entry.Key is not string key)
                {
                    continue;
                }

                if (collection.ContainsKey(key))
                {
                    collection[key] = typedResource;
                }
                else
                {
                    collection.Add(key, typedResource);
                }
            }
        }

        public static SolidColorBrush GetSolidColorBrushOrDefault(string resourceKey)
        {
            return DispatcherHelper.DoInUiThread(
                () =>
                {
                    var resource = Application.Current?.TryFindResource(resourceKey);
                    return resource switch
                    {
                        Color color => new SolidColorBrush(color),
                        SolidColorBrush brush => brush,
                        _ => Brushes.Magenta
                    };
                });
        }
    }
}
