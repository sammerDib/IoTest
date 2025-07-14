using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Interface.Context;

namespace UnitySC.PM.EME.Service.Core.Context
{
    public class ContextManager : IContextManager
    {
        private readonly ContextApplier _contextApplier;
        private readonly ContextGetter _contextGetter;

        public ContextManager(EmeHardwareManager hardwareManager)
        {
            _contextApplier = new ContextApplier(hardwareManager);
            _contextGetter = new ContextGetter(hardwareManager);
        }

        public void Apply(EMEContextBase context)
        {
            _contextApplier.Apply(context);
        }

        public T GetCurrent<T>() where T : EMEContextBase, new()
        {
            return _contextGetter.GetCurrent<T>();
        }
    }
}
