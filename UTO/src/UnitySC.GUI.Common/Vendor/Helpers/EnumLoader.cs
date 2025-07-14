using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Agileo.Common.Tracing;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    /// <summary>
    /// Provide services to get enum values by enum metadata.
    /// </summary>
    public static class EnumLoader
    {
        private static readonly Dictionary<string, Array> _loadedEnums = new Dictionary<string, Array>();
        private static Assembly[] _loadedAssemblies;

        private static Assembly[] GetLoadedAssemblies() =>
            _loadedAssemblies ?? (_loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies());

        /// <summary>
        /// Get an array with all values contained in the specified enum type.
        /// </summary>
        /// <param name="enumTypeName">The <see cref="Type.FullName"/> of the enum.</param>
        /// <param name="assemblyName">The name of the assembly which contains the enum.
        /// <remarks>Do not specify the Assembly.FullName. Only the dll name as follow : "Agileo.Gui" for example. </remarks>
        /// </param>
        /// <returns>An array that contains the values of the constants in specified enum type</returns>
        public static Array GetEnumValues(string enumTypeName, string assemblyName)
        {
            Assembly enumAssembly;
            Type enumType;
            var enumKey = $"{assemblyName}+{enumTypeName}";

            try
            {
                /* STEP 1: check if cache memory already contains desired enum values */
                if (_loadedEnums.ContainsKey(enumKey))
                {
                    return _loadedEnums[enumKey];
                }

                /* STEP 2: Check if assembly do not have already loaded */
                var loadedAssemblies = GetLoadedAssemblies();
                enumAssembly = loadedAssemblies.FirstOrDefault(assembly => assembly.GetName().Name.Equals(assemblyName));

                /* STEP 3: If desired assembly no not have found in loaded assemblies, try to load it by its name */
                if (enumAssembly == null)
                {
                    enumAssembly = AppDomain.CurrentDomain.Load(assemblyName);
                }
            }
            catch (AppDomainUnloadedException e)
            {
                App.Instance.Tracer.Trace(nameof(EnumLoader), TraceLevelType.Error,
                    $"Unable to load assembly '{assemblyName}' because AppDomain is not loaded.", e);
                return Array.Empty<object>();
            }
            catch (ArgumentNullException e)
            {
                App.Instance.Tracer.Trace(nameof(EnumLoader), TraceLevelType.Error,
                    "To be able to load enums values from assembly, the assembly name cannot be empty.",
                    e);
                return Array.Empty<object>();
            }
            catch (Exception e)
            {
                App.Instance.Tracer.Trace(nameof(EnumLoader), TraceLevelType.Error,
                    $"Unable to load the assembly '{assemblyName}'.", e);
                return Array.Empty<object>();
            }

            try
            {
                /* STEP 4 : Get the enum type */
                enumType = enumAssembly.GetType(enumTypeName);
            }
            catch (ArgumentNullException e)
            {
                App.Instance.Tracer.Trace(nameof(EnumLoader), TraceLevelType.Error,
                    "To be able to retrieve enum type, the enum type name cannot be empty.",
                    e);
                return Array.Empty<object>();
            }
            catch (Exception e)
            {
                App.Instance.Tracer.Trace(nameof(EnumLoader), TraceLevelType.Error,
                    $"Unable to retrieve the enum type from the type named '{enumTypeName}'.", e);
                return Array.Empty<object>();
            }

            if (enumType == null)
            {
                App.Instance.Tracer.Trace(nameof(EnumLoader), TraceLevelType.Error,
                    $"Unable to retrieve the enum type from the type named '{enumTypeName}' in the assembly {assemblyName}.");
                return Array.Empty<object>();
            }

            /* STEP 5 : get enum values, add these in cache memory and return it */
            var enumValues = Enum.GetValues(enumType);
            _loadedEnums.Add(enumKey, enumValues);
            return enumValues;
        }
    }
}
