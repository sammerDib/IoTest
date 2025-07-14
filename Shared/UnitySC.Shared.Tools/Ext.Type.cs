using System;
using System.Reflection;

namespace UnitySC.Shared.Tools
{
    public static class TypeExt
    {
        /// <summary>
        /// Rend la taille (le nombre d'éléments de l'enum).
        /// Le type doit évidemment être enum.
        /// </summary>
        /// <param name="this_t"></param>
        /// <returns></returns>
        public static int enumTaille(this Type this_t)
        {
            return Enum.GetValues(this_t).Length;
        }

        /// <summary>
        /// Rend le texte associé à une valeur d'enum.
        /// </summary>
        /// <param name="this_t"></param>
        /// <param name="valeurEnum"></param>
        /// <returns></returns>
        public static string enumName(this Type this_t, object valeurEnum)
        {
            return Enum.GetName(this_t, valeurEnum);
        }

        /// <summary>
        /// Idem getField(string), mais en remontant les types parents si la réponse était nulle.
        /// </summary>
        public static FieldInfo getFieldRecursive(this Type this_t, string name, BindingFlags ùflags_bf = BindingFlags.Default)
        {
            var ret_fi = this_t.GetField(name, ùflags_bf);
            if (ret_fi != null)
            {
                return ret_fi;
            }

            // Base.
            this_t = this_t.BaseType;
            return this_t == null ? null : this_t.getFieldRecursive(name, ùflags_bf);
        }
    }
}