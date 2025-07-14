using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    /// <summary>
    /// Allowing the manipulation of the application theme.
    /// </summary>
    public static class ThemeHelper
    {
        #region Privates

        private static ResourceDictionary ThemeDictionary { get; set; }

        /// <summary>
        /// Uses the application's current theme as the resource file to modify.
        /// </summary>
        private static void Initialize()
        {
            ThemeDictionary ??= Application.Current.Resources.MergedDictionaries[0];
        }


        /// <summary>
        /// Invokes in the UI thread the modification of the theme application.
        /// </summary>
        private static void ApplyResourceDictionary()
        {
            DispatcherHelper.DoInUiThread(InternalApplyResourceDictionary);
        }

        /// <summary>
        /// Apply the current modified ResourceDictionary and force all MergedDictionaries to refresh
        /// </summary>
        private static void InternalApplyResourceDictionary()
        {
            if (ThemeDictionary == null)
            {
                return;
            }

            var collection = new List<ResourceDictionary>();

            foreach (var resourcesMergedDictionary in Application.Current.Resources.MergedDictionaries)
            {
                if (resourcesMergedDictionary.Source == null)
                {
                    collection.Add(resourcesMergedDictionary);
                }
                else
                {
                    collection.Add(
                        new ResourceDictionary
                        {
                            Source = resourcesMergedDictionary.Source
                        });
                }
            }

            collection.RemoveAt(0);
            collection.Insert(0, ThemeDictionary);

            for (var index = 0; index < collection.Count; index++)
            {
                Application.Current.Resources.MergedDictionaries[index] = collection[index];
            }
        }

        #endregion

        /// <summary>
        /// Event raised when the application theme has been changed.
        /// </summary>
        public static event EventHandler ThemeApplied;
        
        /// <summary>
        /// Applies the resource dictionary as the current application theme.
        /// </summary>
        /// <param name="theme"></param>
        public static void Apply(ResourceDictionary theme)
        {
            ThemeDictionary = theme;
            ApplyResourceDictionary();
            ThemeApplied?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Applies the resource dictionary as theme in the targeted resource dictionary.
        /// </summary>
        public static void ApplyTo(ResourceDictionary source, ResourceDictionary targetDictionary)
        {
            var collection = new List<ResourceDictionary>();
            foreach (var resourcesMergedDictionary in Application.Current.Resources.MergedDictionaries)
            {
                if (resourcesMergedDictionary.Source == null)
                {
                    collection.Add(resourcesMergedDictionary);
                }
                else
                {
                    collection.Add(
                        new ResourceDictionary
                        {
                            Source = resourcesMergedDictionary.Source
                        });
                }
            }
            collection.RemoveAt(0);
            collection.Insert(0, source);

            targetDictionary.MergedDictionaries.Clear();

            foreach (var resourceDictionary in collection)
            {
                targetDictionary.MergedDictionaries.Add(resourceDictionary);
            }
        }

        /// <summary>
        /// Get the resource dictionary from the current application theme.
        /// </summary>
        public static ResourceDictionary GetCurrent()
        {
            Initialize();
            return ThemeDictionary;
        }

        /// <summary>
        /// Get a key/value dictionary containing the colors of the current application theme.
        /// </summary>
        public static Dictionary<string, Color> GetColors()
        {
            Initialize();

            var colors = new Dictionary<string, Color>();

            if (ThemeDictionary == null)
            {
                return colors;
            }

            foreach (DictionaryEntry entry in ThemeDictionary)
            {
                if (entry.Value is Color color)
                {
                    colors.Add(entry.Key.ToString(), color);
                }
            }

            return colors;
        }

        /// <summary>
        /// Applies a color scheme to the current application theme.
        /// </summary>
        public static void ApplyColors(Dictionary<string, Color> colors, bool raiseThemeChanged)
        {
            Initialize();

            if (ThemeDictionary == null)
            {
                return;
            }

            foreach (var color in colors)
            {
                ThemeDictionary[color.Key] = new Color
                {
                    A = color.Value.A,
                    R = color.Value.R,
                    G = color.Value.G,
                    B = color.Value.B
                };
            }

            ApplyResourceDictionary();

            if (raiseThemeChanged)
            {
                ThemeApplied?.Invoke(null, EventArgs.Empty);
            }
        }
    }
}
