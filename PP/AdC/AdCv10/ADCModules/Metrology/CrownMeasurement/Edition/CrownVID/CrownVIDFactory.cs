using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADCEngine;
using AdcTools;

namespace CrownMeasurementModule.CrownEditor
{
    class CrownVIDFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "CrownVID"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new CrownVIDModule(this, id, recipe);
        }
        ////------
        //public override DataProducerType DataProducer { get { return DataProducerType.OptionnalData; } }
        public override bool ModifiesData { get { return false; } }
    }
}
