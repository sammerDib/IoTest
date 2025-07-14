using ADCEngine;

using FormatHAZE;

using UnitySC.Shared.Tools;

namespace HazeLSModule
{
    public class HazeLSMeasure : ObjectBase
    {

        //=================================================================
        // Property
        //=================================================================

        #region WaferInfo
        public int EdgeExlusion_um;     // Form LS Measurement parameter
        public double HazeMin_ppmCal;   // From Ada / Dataloader 
        public double HazeMax_ppmCal;   // From Ada / Dataloader
        #endregion

        public LSHazeData Data = null;


        public HazeLSMeasure()
        {

        }

        //public void Init(InputHazeConfiguration p_inputPrm)
        //{

        //}

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
