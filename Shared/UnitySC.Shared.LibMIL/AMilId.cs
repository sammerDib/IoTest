using System;
using System.Text;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Tools;

namespace UnitySC.Shared.LibMIL
{
    public abstract class AMilId : DisposableObject
    {
        protected MIL_ID _milId = MIL.M_NULL;
        //protected abstract override void Dispose(bool disposing);

        //=================================================================
        // Properties
        //=================================================================
        public virtual MIL_ID MilId
        {
            get { return this._milId; }
        }

        //=================================================================
        // Cast
        //=================================================================
        public static implicit operator MIL_ID(AMilId milId)
        {
            return milId._milId;
        }

        //=================================================================
        // Check MIL errors
        //=================================================================
        public static void checkMilError(string message)
        {
            StringBuilder sb = new StringBuilder();
            if (MIL.MappGetError(Mil.Instance.Application, MIL.M_CURRENT + MIL.M_THREAD_CURRENT + MIL.M_MESSAGE, sb) != 0)
                throw new ApplicationException(message + ": " + sb.ToString());
        }

        public static void checkPropertyMilError(string PropertyName, string Category)
        {
            StringBuilder sb = new StringBuilder();
            if (MIL.MappGetError(Mil.Instance.Application, MIL.M_CURRENT + MIL.M_THREAD_CURRENT + MIL.M_MESSAGE, sb) != 0)
            {
                string msg = "Failed to set the <" + PropertyName + "> property in <" + Category + "> tab on the model\n\nMIL Message:" + sb.ToString();
                throw new ApplicationException(msg);
            }
        }

        //=================================================================
        // ToString
        //=================================================================
        public override string ToString()
        {
            return GetType().ToString() + "-" + _milId;
        }
    }
}
