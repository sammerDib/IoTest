using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.Aligner
{
    public partial class Aligner
    {
        protected virtual void InternalSimulateAlign(
            Angle target,
            AlignType alignType,
            Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
        }

        protected virtual void InternalSimulateCentering(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
        }

        protected virtual void InternalSimulatePrepareTransfer(
            EffectorType effector,
            SampleDimension dimension,
            MaterialType materialType,
            Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
        }

        protected virtual void InternalSimulateSetDateAndTime(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(0.2));
        }

        protected virtual void InternalSimulateClamp(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(0.2));
        }

        protected virtual void InternalSimulateUnclamp(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(0.2));
        }

        protected virtual void InternalSimulateMoveZAxis(bool isBottom, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(0.2));
        }
    }
}
