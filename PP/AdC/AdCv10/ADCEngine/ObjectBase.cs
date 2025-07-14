using UnitySC.Shared.Tools;

namespace ADCEngine
{
    public abstract class ObjectBase : DisposableObject
    {
        public string Name { get; protected set; }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return Name;
        }
    }
}
