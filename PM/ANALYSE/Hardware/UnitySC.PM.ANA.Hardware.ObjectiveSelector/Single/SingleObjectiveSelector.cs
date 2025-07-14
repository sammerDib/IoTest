using System;
using System.Linq;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Hardware.ObjectiveSelector
{
    public class SingleObjectiveSelector : IObjectiveSelector
    {
        private readonly ILogger _logger;   
        public ObjectivesSelectorConfigBase Config { get; set; }
        public string DeviceID { get; set; }
        public ModulePositions Position { get; set; }


        public SingleObjectiveSelector(SingleObjectiveSelectorConfig singleObjectivesSelectorConfig,ILogger logger)
        {
            DeviceID = singleObjectivesSelectorConfig.DeviceID;
            Config = singleObjectivesSelectorConfig;
            _logger = logger;
        }

        public ObjectiveConfig GetObjectiveInUse()
        {
            return Config.Objectives?.FirstOrDefault();
        }

        public void Init()
        {
            if (Config.Objectives.Count > 1)
                throw new InvalidOperationException("Single objective selector can't contains more than one objective");
        }

        public void SetObjective(ObjectiveConfig newObjectiveToUse)
        {
            // Nothing
        }

        public void WaitMotionEnd()
        {
            // Nothing
        }

        public void Disconnect()
        {
            // Nothing
        }
    }
}
