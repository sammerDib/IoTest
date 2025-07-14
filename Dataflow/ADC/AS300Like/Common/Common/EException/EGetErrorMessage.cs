using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.EException
{
    public static class EGetErrorMessage
    {
        public static String GetErrorMessage(int pErrorCode)
        {
            if ((pErrorCode > EADCException.NO_ERRBASE_ADC - 1) && (pErrorCode < ETCPException.NO_ERRBASE_TCP))
                return EADCException.GetMessage(pErrorCode);
            if ((pErrorCode > ETCPException.NO_ERRBASE_TCP - 1) && (pErrorCode < 300))
                return ETCPException.GetMessage(pErrorCode);
            if ((pErrorCode > EEdgeException.NO_ERRBASE_EDGE - 1) && (pErrorCode < 500))
                return EEdgeException.GetMessage(pErrorCode);
            if ((pErrorCode > EDarkfieldException.NO_ERRBASE_DARKFIELD - 1) && (pErrorCode < EBrightFieldException.NO_ERRBASE_BRIGHTFIELD))
                return EDarkfieldException.GetMessage(pErrorCode);
            if ((pErrorCode > EBrightFieldException.NO_ERRBASE_BRIGHTFIELD - 1) && (pErrorCode < ENanotopoException.NO_ERRBASE_NANOTOPO))
                return EBrightFieldException.GetMessage(pErrorCode);
            if ((pErrorCode > ENanotopoException.NO_ERRBASE_NANOTOPO - 1) && (pErrorCode < ESoftwareException.NO_ERRBASE_COMMON))
                return ENanotopoException.GetMessage(pErrorCode);
            if ((pErrorCode > ESoftwareException.NO_ERRBASE_COMMON - 1) && (pErrorCode < EPMException.NO_ERRBASE_PM))
                return ESoftwareException.GetMessage(pErrorCode);
            if ((pErrorCode > EPMException.NO_ERRBASE_PM - 1) && (pErrorCode < EEFEMEngineException.NO_ERRBASE_EFEM))
                return ESoftwareException.GetMessage(pErrorCode);
            if ((pErrorCode > EEFEMEngineException.NO_ERRBASE_EFEM - 1) && (pErrorCode < 1199))
                return EEFEMEngineException.GetMessage(pErrorCode);
            return ESoftwareException.GetMessage(ESoftwareException.NO_ERROR_UNKNOWN);

        }
    }
}
