using System;
using System.Collections.Generic;
using System.Text;
using UnitySC.ADCAS300Like.Common;

namespace UnitySC.ADCAS300Like.Common
{
    public class CProcessTypeUsed
    {
        enumProcessType m_ProcessTypes = enumProcessType.MODE_NONE;
        public bool bOCR = false;
        public bool bEdgeWithAligner = false;
        public bool bPSDFrontside = false;
        public bool bPSDBackside = false;
        public bool bTopographyFrontside = false;
        public bool bTopographyBackside = false;
        public bool bBrightField1 = false;
        public bool bBrightField2 = false;
        public bool bBrightField3 = false;
        public bool bBrightField4 = false;
        public bool bDarkview = false;
        public bool bPMEdge = false;
        public bool bPMReview = false;
        public bool bPMLS = false;
        public bool bPMCentering = false;
        public bool bFlipWafer = false;
        public enumFaceUsed FaceUsed = enumFaceUsed.fuDisabled;
        public bool bAllProcessesDisabled = false;

        public CProcessTypeUsed(enumProcessType pProcessTypes)
        {
            ProcessTypes = pProcessTypes;
        }

        public enumProcessType ProcessTypes
        {
            get { return m_ProcessTypes; }
            set
            {
                m_ProcessTypes = value;
                bOCR = (m_ProcessTypes & enumProcessType.OCR) == enumProcessType.OCR;
                bEdgeWithAligner = (m_ProcessTypes & enumProcessType.ALIGNER_EDGE) == enumProcessType.ALIGNER_EDGE;
                bDarkview = (m_ProcessTypes & enumProcessType.DARKVIEW) == enumProcessType.DARKVIEW;
                bBrightField1 = (m_ProcessTypes & enumProcessType.BRIGHTFIELD1) == enumProcessType.BRIGHTFIELD1;
                bBrightField2 = (m_ProcessTypes & enumProcessType.BRIGHTFIELD2) == enumProcessType.BRIGHTFIELD2;
                bBrightField3 = (m_ProcessTypes & enumProcessType.BRIGHTFIELD3) == enumProcessType.BRIGHTFIELD3;
                bBrightField4 = (m_ProcessTypes & enumProcessType.BRIGHTFIELD4) == enumProcessType.BRIGHTFIELD4;
                bTopographyFrontside = ((m_ProcessTypes & enumProcessType.FRONTSIDE) == enumProcessType.FRONTSIDE) && ((m_ProcessTypes & enumProcessType.TOPOGRAPHY) == enumProcessType.TOPOGRAPHY);
                bTopographyBackside = ((m_ProcessTypes & enumProcessType.BACKSIDE) == enumProcessType.BACKSIDE) && ((m_ProcessTypes & enumProcessType.TOPOGRAPHY) == enumProcessType.TOPOGRAPHY); 
                bPSDFrontside = ((m_ProcessTypes & enumProcessType.FRONTSIDE) == enumProcessType.FRONTSIDE) && ((m_ProcessTypes & enumProcessType.PSD) == enumProcessType.PSD);
                bPSDBackside = ((m_ProcessTypes & enumProcessType.BACKSIDE) == enumProcessType.BACKSIDE) && ((m_ProcessTypes & enumProcessType.PSD) == enumProcessType.PSD);
                bPMEdge = (m_ProcessTypes & enumProcessType.PM_EDGE) == enumProcessType.PM_EDGE;
                bPMReview = (m_ProcessTypes & enumProcessType.PM_REVIEW) == enumProcessType.PM_REVIEW;
                bFlipWafer = (m_ProcessTypes & enumProcessType.FLIP_WAFER) == enumProcessType.FLIP_WAFER;
                bPMLS = (m_ProcessTypes & enumProcessType.PM_LIGHTSPEED) == enumProcessType.PM_LIGHTSPEED;
                bPMCentering = (m_ProcessTypes & enumProcessType.PM_CENTERING) == enumProcessType.PM_CENTERING;
                bAllProcessesDisabled = (m_ProcessTypes & enumProcessType.ALL_PROCESSES_DISABLED) == enumProcessType.ALL_PROCESSES_DISABLED;
                FaceUsed = enumFaceUsed.fuDisabled;
                if (!bAllProcessesDisabled)
                {
                    if (bTopographyBackside)
                        FaceUsed = enumFaceUsed.fuBacksideOnly;
                    if (bDarkview || bBrightField1 || bBrightField2 || bTopographyFrontside || bPSDFrontside || bPMCentering)
                    {
                        if (bTopographyBackside || bPSDBackside)
                            FaceUsed = enumFaceUsed.fuBothFace;
                        else
                            FaceUsed = enumFaceUsed.fuFrontsideOnly;
                    }
                    if (bFlipWafer)
                        FaceUsed = enumFaceUsed.fuFlipWafer;
                }
            }
        }

    }    
}
