using AdcBasicObjects;

using ADCEngine;

namespace PatternInspectionModule
{
    internal class PatternRoiCharacterizationFactory : IModuleFactory
    {
        //Specifique aux pattern inspection
        public static Characteristic RoiID = new Characteristic(typeof(CharacListID), "RoiID");

        public PatternRoiCharacterizationFactory()
        {
            ComparatorViewModelBase.editors.Add(typeof(CharacListID), ListIDComparatorViewModel.GetViewModel);
        }

        public override string ModuleName
        {
            get { return "PatternRoiCharac"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_PatternInspection; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new PatternRoiCharacterizationModule(this, id, recipe);
        }
    }
}
