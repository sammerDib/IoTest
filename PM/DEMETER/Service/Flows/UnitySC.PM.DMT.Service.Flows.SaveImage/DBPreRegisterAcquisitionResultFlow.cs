using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Proxy;

namespace UnitySC.PM.DMT.Service.Flows.SaveImage
{
    public class DBPreRegisterAcquisitionResultFlow : FlowComponent<DBPreRegisterAcquisitionResultInput, DBPreRegisterAcquisitionResultResult, DBPreRegisterAcquisitonResultConfiguration>
    {
        private readonly DbRegisterAcquisitionServiceProxy _registerAcquisitionService;

        public DBPreRegisterAcquisitionResultFlow(DBPreRegisterAcquisitionResultInput input, DbRegisterAcquisitionServiceProxy registerAcquisitionService) : base(input, "DBPreRegisterAcquisitionResult")
        {
            _registerAcquisitionService = registerAcquisitionService;
        }

        protected override void Process()
        {
            // No try-catch needed here as the check already happened in the Input's CheckValidity
            var resultType = Input.DmtResultType.ToResultType();
            SetProgressMessage($"Starting pre-registering {resultType} in the database");
            OutPreRegisterAcquisition outPreRegister;
            if (Input.PreviousInternalDbResId == -1)
            {
                Logger.Debug($"Pre-registering {resultType} as first result in the database");
                outPreRegister = _registerAcquisitionService.PreRegisterAcquisition(Input.ToolKey.Value,
                 Input.ChamberKey.Value, Input.Recipe, Input.AutomationInfo, Input.FileName, Input.AcquisitionPath,
                 resultType, Input.Idx, Input.AcquisitionLabel, Input.FilterTag);
            }
            else
            {
                Logger.Debug($"Pre-registering {resultType} as result with parent in the database");
                outPreRegister =
                    _registerAcquisitionService
                        .PreRegisterAcquisition_SameParent(Input.PreviousInternalDbResId,
                                                           Input.AutomationInfo.ModuleStartRecipeTime, Input.FileName,
                                                           resultType, Input.Idx,
                                                           Input.AcquisitionLabel, Input.FilterTag);
            }

            Result.OutPreRegister = outPreRegister;
            SetProgressMessage($"Successfully pre-registered {resultType} in the database");
        }
    }
}
