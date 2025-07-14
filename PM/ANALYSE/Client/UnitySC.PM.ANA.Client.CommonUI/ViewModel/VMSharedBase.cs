using System;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel
{
    public class VMSharedBase : ViewModelBaseExt
    {
        private ILogger _logger;

        public ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    Type currentType = this.GetType();
                    var logger = typeof(ILogger<>);
                    var typedlogger = logger.MakeGenericType(currentType);
                    _logger = (ILogger)ClassLocator.Default.GetInstance(typedlogger);
                }
                return _logger;
            }
        }
    }
}
