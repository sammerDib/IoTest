using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class PSDAxes : MotionAxes
    {
        public PSDAxes(PSDAxesConfig config, Dictionary<string, MotionControllerBase> controllersDico,
            IGlobalStatusServer globalStatusServer, ILogger logger, IReferentialManager referentialManager)
            : base(config, controllersDico, globalStatusServer, logger, referentialManager)
        {
        }

        public override void Init(List<Message> initErrors)
        {
            base.Init(initErrors);
        }

        public PositionBase GetPos()
        {
            return GetPosition();
        }

        public void MoveToIndex(int indexPos)
        {
            var newPosition = new PMAxisMove("Linear", new Length(indexPos, LengthUnit.Millimeter));
            Move(newPosition);
            WaitMotionEnd(1500);
        }

        public override void TriggerUpdateEvent()
        {
            foreach (var controller in MotionControllers)
            {
                controller.TriggerUpdateEvent();
            }
        }
    }
}
