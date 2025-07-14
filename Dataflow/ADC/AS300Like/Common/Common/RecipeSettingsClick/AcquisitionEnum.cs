using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Acquisition
{
    /// <summary>
    /// Possible statuses after an acquisition in remote mode
    /// </summary>
    public enum enumAcquisitionResult { OK, Abort};
    /// <summary>
    /// Defines the scan type possible with the system
    /// </summary>
    public enum enumLineScanType { ForwardOnly, BackAndForth, Circular };
    /// <summary>
    /// Identifies the possible scan modes for 2D analysis
    /// </summary>
    public enum enumScanModeType { CustomArea, AreaTrigger, LineScanTrigger,ReviewModule };
    /// <summary>
    /// Identifies the possible scan modes for 3D analysis
    /// </summary>
    public enum enum3DScanModeType { smCHrocodile, smMPLS180 };
    [Flags]
    public enum enumSaverType { esRawImages = 1, esCxAx = 2, esReflectivity = 4, esCalibration = 8, esCutUp = 16, esFileExport =32};
    [Flags]
    public enum enumPreProcessModuleID { ppmUnDeadPixel=1, ppmPSDCal=2,ppmCalibration=4 };


}
