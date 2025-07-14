using System;
using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Proxy
{
    public class DbToolServiceProxy
    {
        private ServiceInvoker<IToolService> _dbToolService;

        public DbToolServiceProxy()
        {
            _dbToolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), null, ClassLocator.Default.GetInstance<ModuleConfiguration>().DataAccessAddress);
        }
        public bool CheckDatabaseVersion()
        {
            return _dbToolService.Invoke(s => s.CheckDatabaseVersion());
        }

        #region Tool
        public Tool GetTool(int toolKey, bool includeChambers = false)
        {
            return _dbToolService.Invoke(s => s.GetTool(toolKey, includeChambers));
        }
        public List<Tool> GetAllTools(bool includeChambers = false)
        {
            return _dbToolService.Invoke(s => s.GetAllTools(includeChambers));
        }

        public int SetTool(Tool tool, int? userId)
        {
            return _dbToolService.Invoke(s => s.SetTool(tool, userId));
        }
        #endregion Tool

        #region Chamber
        [Obsolete]
        public Chamber GetChamber(int chamberId)
        {
            return _dbToolService.Invoke(s => s.GetChamber(chamberId));
        }

        public Chamber GetChamber(int toolKey, int chamberKey)
        {
            return _dbToolService.Invoke(s => s.GetChamberFromKeys(toolKey, chamberKey));
        }
        public List<Chamber> GetAllChambers()
        {
            return _dbToolService.Invoke(s => s.GetAllChambers());
        }

        public int SetChamber(Chamber chamber, int? userId)
        {
            return _dbToolService.Invoke(s => s.SetChamber(chamber, userId));
        }
        #endregion Chamber

        #region Product and Step

        public List<Product> GetProductAndSteps(bool takeArchived = false)
        {
            return _dbToolService.Invoke(s => s.GetProductAndSteps(takeArchived));
        }

        public List<WaferCategory> GetWaferCategories()
        {
            return _dbToolService.Invoke(s => s.GetWaferCategories());
        }

        public int SetWaferCategory(WaferCategory waferCategory, int userId)
        {
            return _dbToolService.Invoke(s => s.SetWaferCategory(waferCategory, userId));
        }

        #endregion .Product and Step

        #region Vid

        public int FirstLowerFreeVid()
        {
            return _dbToolService.Invoke(s => s.FirstLowerFreeVid());
        }
        public int FirstUpperFreeVid()
        {
            return _dbToolService.Invoke(s => s.FirstUpperFreeVid());
        }

        public int SetVid(Vid vid, int userId)
        {
            return _dbToolService.Invoke(s => s.SetVid(vid, userId));
        }
        public List<Vid> GetAllVid()
        {
            return _dbToolService.Invoke(s => s.GetAllVid());
        }
        public void SetAllVid(List<Vid> vids, int userId)
        {
            _dbToolService.Invoke(s => s.SetAllVid(vids, userId));
        }

        public void RemoveVid(Vid vid, int userId)
        {
            _dbToolService.Invoke(s => s.RemoveVid(vid, userId));
        }

        #endregion
    }
}
