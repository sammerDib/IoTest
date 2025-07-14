using System;

using UnitySC.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.TC.Shared.Operations.Implementation
{
    public class UTOBaseOperations : IUTOBaseOperations
    {
        private ILogger _logger;
        private IAlarmOperations _alarmOperations;
        private IEquipmentConstantOperations _ecOperations;
        private ICommonEventOperations _ceOperations;
        private ICommunicationOperations _communicationOperations;

        public IAlarmOperations AlarmOperations { get => _alarmOperations; }
        public IEquipmentConstantOperations ECOperations { get => _ecOperations; }
        public ICommonEventOperations CEOperations { get => _ceOperations; }
        public ICommunicationOperations CommunicationOperations { get => _communicationOperations; set => _communicationOperations = value; }
        protected ILogger Logger { get => _logger; set => _logger = value; }

        public UTOBaseOperations()
        {
            Logger = ClassLocator.Default.GetInstance<ILogger>();
            _alarmOperations = ClassLocator.Default.GetInstance<IAlarmOperations>();
            _ecOperations = ClassLocator.Default.GetInstance<IEquipmentConstantOperations>();
            _ecOperations.Init(ClassLocator.Default.GetInstance<IAutomationConfiguration>().ECConfigurationFilePath);
            _ceOperations = ClassLocator.Default.GetInstance<ICommonEventOperations>();
            _ceOperations.Init(ClassLocator.Default.GetInstance<IAutomationConfiguration>().CEConfigurationFilePath);
            _communicationOperations = ClassLocator.Default.GetInstance<ICommunicationOperations>();
        }

        #region private methods

        protected int GetAlarmId(String alarmName)
        {
            //TODO
            return 1;
        }

        #endregion private methods
    }
}
