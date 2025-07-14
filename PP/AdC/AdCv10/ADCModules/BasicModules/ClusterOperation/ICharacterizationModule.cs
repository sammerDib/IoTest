using System.Collections.Generic;

using AdcBasicObjects;

namespace BasicModules
{
    public interface ICharacterizationModule
    {
        List<Characteristic> AvailableCharacteristics { get; }
    }
}
