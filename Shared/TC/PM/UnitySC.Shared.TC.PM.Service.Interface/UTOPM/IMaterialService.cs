using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Data;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.PM.Service.Interface
{
    // UTO Client -> PM (UTO.API) Serveur
    [ServiceContract]
    public interface IMaterialService
    {
        [OperationContract]
        Response<bool> PrepareForTransfer(TransferType transferType, MaterialTypeInfo materialTypeInfo);              // Requete d'echange

        [OperationContract]
        Response<Material> UnloadMaterial();

        [OperationContract]
        Response<VoidResult> LoadMaterial(Material wafer);

        [OperationContract]
        Response<VoidResult> PostTransfer();                    // Fin echange => fermeture porte

        [OperationContract]
        Response<VoidResult> StartRecipe();                    // 

        [OperationContract]
        Response<VoidResult> AbortRecipe();                    // 

        [OperationContract]
        Response<bool> Initialization();                    // Faire init

        [OperationContract]
        Response<double> GetAlignmentAngle();

        [OperationContract]
        Response<List<Length>> GetSupportedWaferDimensions();
    }
}
