using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.DMT.Service.Test
{
    class ClassLogWithoutCaller
    {
        private ILogger _logger;
        public ClassLogWithoutCaller(ILogger logger)
        {
            _logger = logger;
        }

        public void Log()
        {
            _logger.Debug("Debug without caller");
            _logger.Information("Information without caller");
            _logger.Warning("Warning without caller");
            _logger.Error("Error without caller");
        }
    }
}
