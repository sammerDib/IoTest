using ADCEngine;
using AdcTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haze
{
    public class HazeData : ObjectBase
    {
        //=================================================================
        // Dispose
        //=================================================================

        protected override void Dispose(bool disposing)
        {
            //if (_originalProcessingImage != null)
            //{
            //    _originalProcessingImage.Dispose();
            //    _originalProcessingImage = null;
            //}
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
