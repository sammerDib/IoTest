using System.ComponentModel.Composition;

namespace UnitySC.PM.Shared.UC
{
    [InheritedExport(typeof(IStateUc))]
    public interface IStateUc : IPmUc
    {
    }
}
