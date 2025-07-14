using ADCEngine;

using FormatGTR;

using UnitySC.Shared.Tools;

namespace GlobaltopoModule
{
    public class GTMeasure : ObjectBase
    {

        //=================================================================
        // Property
        //=================================================================

        public DataGTR Data = null;


        public GTMeasure()
        {
            Data = new DataGTR();
        }

        //=================================================================
        // Dispose
        //=================================================================

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        //=================================================================
        // Clonage
        //=================================================================
        protected override void CloneTo(DisposableObject obj)
        {
            //AcquisitionImageObject clone = (AcquisitionImageObject)obj;
            //clone.MilImage.AddRef();
        }
    }



}
