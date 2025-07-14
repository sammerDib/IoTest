using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.EventsPackages
{    
    public class CWorkingSizeChangeEventPackage
    {
        public OnRequestToChangeWorkingSize EvtOnRequestToChangeWorkingSize;
        public event OnWorkingSizeChange EvtOnWorkingSizeChange;

        public void SetWorkingSize(int pSize_Inches)
        {
            if (EvtOnWorkingSizeChange != null)
                EvtOnWorkingSizeChange(pSize_Inches);
        }
    }


    public class PackEventClientApplication
    {
        public SetDataVariable EvtSetDataVariable;
        public TriggerEvent EvtTriggerEvent;
    }
}
