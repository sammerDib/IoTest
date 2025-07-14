using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Core.Context
{
    public interface IContextManager
    {
        void Apply(ANAContextBase context);

        T GetCurrent<T>() where T : ANAContextBase, new();
    }
}
