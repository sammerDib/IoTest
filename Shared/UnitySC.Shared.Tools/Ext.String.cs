using System;
using System.Linq;

namespace UnitySC.Shared.Tools
{
    public static class StringExt
    {

        public static bool In(this string value, params string[] valeurs)
        {
            return valeurs.Contains(value);
        }

    }
}
