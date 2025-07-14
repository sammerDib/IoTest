using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

namespace UnitySC.Equipment.Abstractions.Devices.ReaderPositioner
{
    public partial class ReaderPositioner
    {
        protected virtual void InternalSimulateSetPosition(SampleDimension dimension, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(1));
            CurrentPosition = dimension;
        }
    }
}
