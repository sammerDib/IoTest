using System.Collections.Generic;
using System.Linq;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class NamingStrategy
    {
        public static string GetCloneName(string name, IEnumerable<string> usedNames)
        {
            var newName = string.Concat(name, "_Copy");
            return GetIncrementedName(newName, usedNames);
        }

        /// <summary>
        /// Get the specified name concatenated with '(x)', let x be the number of identical names already existing + 1.
        /// </summary>
        public static string GetIncrementedName(string name, IEnumerable<string> usedNames)
        {
            var enumerable = usedNames.ToList();
            if (!enumerable.Contains(name)) return name;

            var index = 2;
            while (true)
            {
                var indexedName = string.Concat(name, " (", index, ")");
                if (!enumerable.Contains(indexedName))
                {
                    return indexedName;
                }

                index++;
            }
        }
    }
}
