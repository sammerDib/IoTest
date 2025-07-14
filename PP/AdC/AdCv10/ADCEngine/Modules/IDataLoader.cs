using System.Collections.Generic;

using AcquisitionAdcExchange;

using UnitySC.Shared.Data.Enum;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interface des Modules DataLoader.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public interface IDataLoader
    {
        /// <summary> Couche de Données </summary>
        LayerBase Layer { get; }

        /// <summary> Nom de la couche de Données </summary>
        string LayerName { get; }

        /// <summary> Démarre le mode Reprocess pour rejouer une recette mergée</summary>
        void StartReprocess();

        /// <summary>
        /// Type de DataLoader
        /// </summary>
        ActorType DataLoaderActorType { get; }

        /// <summary>
        /// ResultType compatibles avec le dataloader
        /// </summary>
        IEnumerable<ResultType> CompatibleResultTypes { get; }
    }
}
