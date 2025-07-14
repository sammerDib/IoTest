using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Interface.Algo.PSIInput;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class MeasureNanoTopoConfiguration : MeasureConfigurationBase
    {
        [DataMember]
        public List<AcquisitionConfiguration> Acquisitions { get; set; }
        
        [DataMember]
        public List<AlgoConfiguration> Algos { get; set; }
        
        [DataMember]
        public Length MaxCompatibleLightWavelength { get; set; }
        
        [DataMember]
        public Length MinCompatibleLightWavelength { get; set; }
    }

    [DataContract]
    public class AlgoConfiguration
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public PhaseCalculationAlgo PhaseCalculation { get; set; }

        [DataMember]
        public PhaseUnwrappingAlgo PhaseUnwrapping { get; set; }

        /// <summary>
        /// StepSize = Wavelength * factor 
        /// Exemple :
        /// Un cycle entier de 2π correspond à un déplacement de 1/2 longueur d'onde, pi correspond à un déplacement de 1/2 longueur d'onde, 
        /// car la lumière fait un aller-retour en se réfléchissant sur l'objectif, 
        /// donc il faut compter 2 fois la distance. 
        /// Du coup, π/2 correspond à un déplacement de 1/8 longueur d'onde.
        /// </summary>
        [DataMember]
        public double FactorBetweenWavelengthAndStepSize { get; set; }

        [DataMember]
        public int StepCount { get; set; }
    }

    [DataContract]
    public class AcquisitionConfiguration
    {
        [DataMember]
        public NanoTopoAcquisitionResolution Resolution { get; set; }

        [DataMember]
        public int ImagesPerStep { get; set; }
    }
}
