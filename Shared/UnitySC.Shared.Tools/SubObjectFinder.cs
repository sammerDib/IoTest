using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Tools
{
    public class SubObjectFinder
    {
        private const string PathSplitter = ".";

        /// <summary>
        /// Get all sub object of type T
        /// Methode used to recusively find all sub object of type "T" in parameter "serachIn"
        /// <typeparam name="T"> object to find </typeparam>
        /// <param name="serachIn"> object to browse </param>
        /// <param name="maxDepth"> Max depth of research of "T"</param>
        /// <returns></returns>
        public static Dictionary<string, T> GetAllSubObjectOfTypeT<T>(object searchIn, int maxDepth = 5)
        {
            var subObjects = new Dictionary<string, T>();
            if (IsBasicType(typeof(T)))
                throw new Exception("Basic type can't be used in T");

            return GetAllSubObjectByRecursivity(searchIn, searchIn.GetType().Name, maxDepth, 0, "", subObjects);
        }

        private static Dictionary<string, T> GetAllSubObjectByRecursivity<T>(object searchIn, string searchInName, int maxDepth, int currentDepth, string currentPath, Dictionary<string, T> resSubObjects)
        {
            currentDepth++;
            if (string.IsNullOrEmpty(currentPath))
                currentPath = searchInName;
            else
                currentPath += PathSplitter + searchInName;

            // Get properties of the current object
            foreach (var property in searchIn.GetType().GetProperties())
            {
                if (!IsBasicType(property.PropertyType))
                {
                    object propertyValue = property.GetValue(searchIn, null);
                    Debug.WriteLine($"Name {property.Name}");
                    if (propertyValue != null)
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                        {
                            int subItemIndex = 0;
                            foreach (object subItem in (IEnumerable)propertyValue)
                            {
                                if (subItem is null)
                                    continue;

                                string subItemName = property.Name + PathSplitter + subItemIndex + subItem.GetType().Name;

                                CheckAndBrowseSubItem<T>(subItem, subItemName, maxDepth, currentDepth, currentPath, resSubObjects);
                                subItemIndex++;
                            }
                        }
                        else
                        {
                            CheckAndBrowseSubItem<T>(propertyValue, property.Name, maxDepth, currentDepth, currentPath, resSubObjects);
                        }
                    }
                }
            }

            return resSubObjects;
        }

        private static void CheckAndBrowseSubItem<T>(object subItem, string subItemName, int maxDepth, int currentDepth, string currentPath, Dictionary<string, T> subObjects)
        {
            if (subItem is T)
            {
                subObjects.Add(currentPath + PathSplitter + subItemName, (T)subItem);
            }
            else if (currentDepth < maxDepth)
            {
                GetAllSubObjectByRecursivity<T>(subItem, subItemName, maxDepth, currentDepth, currentPath, subObjects);
            }
        }

        private static bool IsBasicType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(DateTime)
                || type == typeof(Length)
                || type == typeof(Angle);
        }
    }
}
