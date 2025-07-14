using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UnitySC.Shared.Tools
{
    public static class EnumExt
    {
        public static T Next<T>(this T source) where T : IConvertible//enum
        {
            Type t = typeof(T);

            if (!t.IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            IConvertible cT = (IConvertible)source;
            IEnumerable<IConvertible> convertibles = Enum.GetValues(t).Cast<IConvertible>();

            int i = (int)(IConvertible)source;
            i++;
            if (i > (int)convertibles.Max())

                return (T)convertibles.Min();
            else
                return (T)(IConvertible)i;
        }

        public static T Previous<T>(this T source) where T : IConvertible//enum
        {
            Type t = typeof(T);

            if (!t.IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            IConvertible cT = (IConvertible)source;
            IEnumerable<IConvertible> convertibles = Enum.GetValues(t).Cast<IConvertible>();

            int i = (int)(IConvertible)source;
            i--;
            if (i < (int)convertibles.Min())

                return (T)convertibles.Max();
            else
                return (T)(IConvertible)i;
        }


        public static bool In<T>(this T source, params T[] enums) where T : IConvertible//enum
        {
            Type t = typeof(T);

            if (!t.IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            return enums.Contains(source);
        }

        public static string GetDescriptionFromEnumValue(this Enum value)
        {
            var attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }


        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }


    }
}
