using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Core.Context
{
    public class ContextManager : IContextManager
    {
        private readonly ContextApplier _contextApplier;
        private readonly ContextGetter _contextGetter;

        public ContextManager(AnaHardwareManager hardwareManager)
        {
            _contextApplier = new ContextApplier(hardwareManager);
            _contextGetter = new ContextGetter(hardwareManager);
        }

        public void Apply(ANAContextBase context)
        {
            _contextApplier.Apply(context);
        }

        public T GetCurrent<T>() where T : ANAContextBase, new()
        {
            return _contextGetter.GetCurrent<T>();
        }
    }
}
