using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

using Matrox.MatroxImagingLibrary;

namespace AcquisitionAdcExchange
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC: Id de recette ADC
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class RecipeId
    {
        [DataMember] public int Id;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC: Recette à exécuter et les infos
    /// décrivant les conditions d'acquisition.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class RecipeData
    {
        [DataMember] public string ADCRecipeFileName;
        [DataMember] public string ADCRecipeGuid;
        [DataMember] public string ADCRecipeVersion;
        [DataMember] public List<AcquisitionLayerInfoBase> Layers;
        [DataMember] public WaferInfo WaferInfo;
        [DataMember] public CorrectorInfo CorrectorInfo;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Échange Acquisition/ADC: Status de l'exécution d'une recette.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class RecipeStatus
    {
        [DataMember] public eRecipeStatus Status;
        [DataMember] public string Message;
    }

    public enum eRecipeStatus
    {
        Unknown,
        Processing,
        Error,
        Completed
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Interface entre l'Acquisition et l'ADC (en fait le "MergeContext").
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [ServiceContract]
    public interface IAdcAcquisition
    {
        // Init et Stop ne sont utiles que lorsqu'on ne passe pas par WCF
        void InitADC(MIL_ID applicationId, MIL_ID systemId);
        void StopADC();

        // Les autres fonctions peuvent être utilisées soit directement soit pas WCF
        [OperationContract] RecipeId StartRecipe(RecipeData recipeData);
        [OperationContract] void SetAcquitionImages(RecipeId id, List<AcquisitionData> AcquisitionImageList);
        [OperationContract] void AbortRecipe(RecipeId id);
        [OperationContract] void StopRecipe(RecipeId id);
        [OperationContract] bool FeedImage(RecipeId id, AcquisitionData acqData);
        [OperationContract] RecipeStatus GetRecipeStatus(RecipeId id);
    }
}
