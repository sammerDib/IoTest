using System;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    /// <summary>
    /// Mark that a context must be applied after another type of context has been applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class |
                       AttributeTargets.Struct,
                       Inherited = true,
                       AllowMultiple = true)]
    public class ApplyAfterAttribute : Attribute
    {
        public Type ContextType;

        public ApplyAfterAttribute(Type contextType)
        {
            ContextType = contextType;
        }
    }
}
