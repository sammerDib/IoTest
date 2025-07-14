using System;

using ADCCommon;

namespace UnitySC.ADCAS300Like.Common
{
    public static class UpdateVIDMethods
	{		
		public static void ErrorTextIDVID(ICxClientApplication pClientApplication, String pErrorID, String pMsgError)
		{
			VariableResults lvResults = VariableResults.vrFAILURE;
			CxValueObject lCxValueError = new CxValueObject();

			String lVarName = String.Empty;
			if (pClientApplication != null)
			{
				// update error variables
				lVarName = "ErrorText";
				lCxValueError.SetValueAscii(0, 0, pMsgError);
				pClientApplication.SetWellKnownValue(0, lVarName, lCxValueError, out lvResults);
				if ((lvResults != VariableResults.vrSUCCESS) && (lvResults != VariableResults.vrLIMITCROSSED) && (lvResults != VariableResults.vrNOCHANGE))
				{
					throw new Exception("Error in UpdateErrorTextIDVID() - Set Error Text VID failed, VarName = " + lVarName);
				}
				lCxValueError = new CxValueObject();
				lVarName = "ErrorID";
				lCxValueError.SetValueAscii(0, 0, pErrorID.ToString());
				pClientApplication.SetWellKnownValue(0, lVarName, lCxValueError, out lvResults);
				if ((lvResults != VariableResults.vrSUCCESS) && (lvResults != VariableResults.vrLIMITCROSSED) && (lvResults != VariableResults.vrNOCHANGE))
				{

					throw new Exception("Error in UpdateErrorTextIDVID() - Set Error ID VID failed, VarName = " + lVarName);
				}
			}
			else
				throw new Exception("Error in UpdateErrorTextIDVID() - m_pClientApplication == NULL");
		}
	}
}
