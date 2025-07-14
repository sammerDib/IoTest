using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class LiseHfAxes : MotionAxes
    {
        public LiseHfAxes(LiseHfAxesConfig config, Dictionary<string, MotionControllerBase> controllersDico,
            IGlobalStatusServer globalStatusServer, ILogger logger, IReferentialManager referentialManager) :
            base(config, controllersDico, globalStatusServer, logger, referentialManager)
        {
        }

        public override void Init(List<Message> initErrors)
        {
            base.Init(initErrors);
        }

        public void MoveToIndex(int indexPos)
        {
            var newPosition = new PMAxisMove("Linear", new Length(indexPos, LengthUnit.Millimeter));
            Move(newPosition);
            WaitMotionEnd(1500);
        }

        public int GetCurrentIndex()
        {
            return (int)GetPosition().ToXTPosition().X;
        }

        public double GetCurrentT()
        {
            return GetPosition().ToXTPosition().T;
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
