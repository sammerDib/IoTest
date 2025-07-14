using System;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Proxy
{
    public class DbRegisterResultServiceProxy
    {
        // For inline/post-processing results
        private ServiceInvoker<IRegisterResultService> _dbRegisterResultService;

        public DbRegisterResultServiceProxy()
        {
            _dbRegisterResultService = new ServiceInvoker<IRegisterResultService>("RegisterResultService", ClassLocator.Default.GetInstance<SerilogLogger<IRegisterResultService>>(), null, ClassLocator.Default.GetInstance<ModuleConfiguration>().DataAccessAddress);
        }

        #region Result Registering methods
        public OutPreRegister PreRegisterResult(int sourceToolKey, int sourceChamberKey, RecipeInfo recipeinfo, RemoteProductionInfo autominfo, ResultType resultType, byte idx = 0, ResultFilterTag tag = ResultFilterTag.None, string resultLabelName = null)
        {
            return _dbRegisterResultService.TryInvokeAndGetMessages(s => s.PreRegisterResult(sourceToolKey, sourceChamberKey, recipeinfo.GetBaseRecipeInfo(), autominfo, resultType, idx, tag, resultLabelName))?.Result;
        }

        public OutPreRegister PreRegisterResult_SameParent(long parentResultId, DateTime moduleStartRecipeTime, ResultType resultType, byte idx = 0, ResultFilterTag tag = ResultFilterTag.None, string resultLabelName = null)
        {
            return _dbRegisterResultService.TryInvokeAndGetMessages(s => s.PreRegisterResult_SameParent(parentResultId, moduleStartRecipeTime, resultType, idx, tag, resultLabelName))?.Result;
        }

        public OutPreRegister PreRegisterResult(InPreRegister preRegister)
        {
            return _dbRegisterResultService.TryInvokeAndGetMessages(s => s.PreRegisterResultWithPreRegisterObject(preRegister))?.Result;
        }

        public bool? UpdateResultState(long resultItemId, ResultState resultState)
        {
            return _dbRegisterResultService.TryInvokeAndGetMessages(s => s.UpdateResultState(resultItemId, resultState))?.Result;
        }
        #endregion //Result Registering
  
    }
}
