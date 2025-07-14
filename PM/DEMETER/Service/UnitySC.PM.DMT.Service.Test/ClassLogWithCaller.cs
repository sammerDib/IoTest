using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.DMT.Service.Test
{
    public class ClassLogWithCaller
    {
        private ILogger _logger;
        public ClassLogWithCaller(ILogger<ClassLogWithCaller> logger)
        {
            _logger = logger;
        }

        public void Log()
        {
            _logger.Debug("Debug with caller");
            _logger.Information("Information with caller");
            _logger.Warning("Warning with caller");
            _logger.Error("Error with caller");
        }
    }
}
