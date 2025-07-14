using AcquisitionAdcExchange;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using UnitySC.Shared.Tools;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Object ADC qui contient une data de l'acquisition.
    /// 
    /// L'acqisition a son propre format de données (qui est basique) et
    /// l'ADC a aussi son propore format de données (ObjectBase, ImageBase
    /// et leurs dérivées)
    /// qui est indépendant de l'acquisition.
    ///
    /// Cette classe est utilisée juste pour faire le lien entre ces deux
    /// formats:
    /// - MergeContext traduit le format de l'acquisition (AquisitionData
    ///  ou autre) en AcquisitionDataObject,
    /// - les DataLoaders convertissent l'AcquisitionDataObject en format 
    /// ADC pur (ObjectBase ou ImageBase).
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public abstract class AcquisitionDataObject : ObjectBase
    {
        public AcquisitionData AcqData;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Object ADC qui contient une image de l'acquisition.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class AcquisitionImageObject : AcquisitionDataObject
    {
        public MilImage MilImage = new MilImage();

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (MilImage != null)
            {
                MilImage.Dispose();
                MilImage = null;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // Clonage
        //=================================================================
        protected override void CloneTo(DisposableObject obj)
        {
            AcquisitionImageObject clone = (AcquisitionImageObject)obj;
            clone.MilImage.AddRef();
        }


    }
}
