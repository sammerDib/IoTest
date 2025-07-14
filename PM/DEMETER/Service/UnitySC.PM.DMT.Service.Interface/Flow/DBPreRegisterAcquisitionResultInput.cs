using System;
using System.Security.AccessControl;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class DBPreRegisterAcquisitionResultInput : IFlowInput
    {
        public int? ToolKey { get; set; }

        public int? ChamberKey { get; set; }

        public DMTRecipe Recipe { get; set; }

        public string AcquisitionPath { get; set; }

        public string FileName { get; set; }

        public DMTResult DmtResultType { get; set; }

        public RemoteProductionInfo AutomationInfo { get; set; }

        public byte Idx { get; set; }

        public string AcquisitionLabel { get; set; }

        public ResultFilterTag FilterTag { get; set; }

        public long PreviousInternalDbResId { get; set; } = -1;

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (AcquisitionPath.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.Message.Add("Cannot pre-register result an empty acquisition path");
            }

            if (FileName.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.Message.Add("Cannot pre-register result witha a, empty file name");
            }

            if (AutomationInfo is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot pre-register result without automation information");
            }

            if (PreviousInternalDbResId == -1)
            {
                if (!ToolKey.HasValue)
                {
                    result.IsValid = false;
                    result.Message.Add("Cannot pre-register result without a tool key if there is no parent result");
                }

                if (!ChamberKey.HasValue)
                {
                    result.IsValid = false;
                    result.Message.Add("Cannot pre-register result without a chamber key if there is no parent result");
                }

                if (Recipe is null)
                {
                    result.IsValid = false;
                    result.Message.Add("Cannot pre-register result without a recipe if there is no parent result");
                }
            }

            try
            {
                DmtResultType.ToResultType();
            }
            catch (Exception e)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot pre-register {DmtResultType} in the database. Those results are not meant to be saved in the database");
            }

            return result;
        }
    }
}
