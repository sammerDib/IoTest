using System.Collections.Generic;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IProbeSample
    {
        string Name { get; set; }
        string Info { get; set; }
        List<ProbeSampleLayer> Layers { get; set; }
    }
}
