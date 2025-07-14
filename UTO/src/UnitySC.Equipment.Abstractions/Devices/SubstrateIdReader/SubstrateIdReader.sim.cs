using Agileo.EquipmentModeling;

using UnitsNet;

namespace UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader
{
    public partial class SubstrateIdReader
    {
        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            base.InternalInitialize(mustForceInit);
            Positioner?.Initialize(mustForceInit);
            InternalSimulateRequestRecipes(tempomat);
        }

        protected virtual void InternalSimulateRequestRecipes(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(0.2));
        }

        protected virtual void InternalSimulateRead(string recipeName, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(0.2));
        }

        protected virtual void InternalSimulateGetImage(string imagePath, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(0.2));
        }
    }
}
