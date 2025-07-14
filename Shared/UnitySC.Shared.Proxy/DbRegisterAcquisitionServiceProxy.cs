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
    public class DbRegisterAcquisitionServiceProxy
    {
        // For Process module acquisition results
        private ServiceInvoker<IRegisterResultService> _dbRegisterAcqService;

        public DbRegisterAcquisitionServiceProxy()
        {
            _dbRegisterAcqService = new ServiceInvoker<IRegisterResultService>("RegisterResultService", ClassLocator.Default.GetInstance<SerilogLogger<IRegisterResultService>>(), null, ClassLocator.Default.GetInstance<ModuleConfiguration>().DataAccessAddress);
        }

        #region Acquisition Result/Image Registering methods

        public OutPreRegisterAcquisition PreRegisterAcquisition(int sourceToolKey, int sourceChamberKey, RecipeInfo recipeinfo, RemoteProductionInfo autominfo, string filename, string pathname, ResultType resultType, byte idx = 0, string acqLabelName = null, ResultFilterTag tag = ResultFilterTag.None)
        {
            return _dbRegisterAcqService.TryInvokeAndGetMessages(s => s.PreRegisterAcquisition(sourceToolKey, sourceChamberKey, recipeinfo.GetBaseRecipeInfo(), autominfo, filename, pathname, resultType, idx, acqLabelName, tag))?.Result;
        }

        public OutPreRegisterAcquisition PreRegisterAcquisition_SameParent(long parentResultId, DateTime moduleStartRecipeTime, string filename, ResultType resultType, byte idx = 0, string acqLabelName = null, ResultFilterTag tag = ResultFilterTag.None)
        {
            return _dbRegisterAcqService.TryInvokeAndGetMessages(s => s.PreRegisterAcquisition_SameParent(parentResultId, moduleStartRecipeTime, filename, resultType, idx, acqLabelName, tag))?.Result;
        }

        public OutPreRegisterAcquisition PreRegisterAcquisition(InPreRegisterAcquisition preRegisterAcq)
        {
            return _dbRegisterAcqService.TryInvokeAndGetMessages(s => s.PreRegisterAcquisitionWithPreRegisterObject(preRegisterAcq))?.Result;
        }

        public bool? UpdateResultAcquisitionState(long resulAcqItemAcqId, ResultState resultacqState, string thumbnailExtension = null)
        {
            return _dbRegisterAcqService.TryInvokeAndGetMessages(s => s.UpdateResultAcquisitionState(resulAcqItemAcqId, resultacqState, thumbnailExtension))?.Result;
        }
        #endregion //Acquisition Result/Image Registering

    }
}
