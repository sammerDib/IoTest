using UnitySC.PM.EME.Service.Interface.Context;

namespace UnitySC.PM.EME.Service.Core.Context
{
    public interface IContextManager
    {
        void Apply(EMEContextBase context);

        T GetCurrent<T>() where T : EMEContextBase, new();
    }
}
