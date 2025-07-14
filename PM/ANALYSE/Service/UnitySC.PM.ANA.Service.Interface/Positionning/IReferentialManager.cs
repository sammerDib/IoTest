using System.Collections.Generic;

namespace UnitySC.PM.ANA.Service.Interface.Positionning
{
    public interface IReferentialManager
    {

        /// <summary>
        /// Prepare the given CompositePosition to be converted to another Referential using a CompositePositionTransformer
        /// </summary>
        /// <param name="from"></param>
        Positionning.CompositePositionTransformer Convert(Positionning.CompositePosition from);

        /// <summary>
        /// Adds a BaseTransformation for a given Referential in this IReferentialManager
        /// It will be executed later when calling Convert().
        /// </summary>
        /// <param name="transformation"></param>
        Positionning.TransformationAdder Add(Positionning.BaseTransformation transformation);

        List<List<Positionning.BaseTransformation>> Transformations { get; set; }
    }
}
