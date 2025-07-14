using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware
{
    public class ACSTest : FunctionalTest
    {
        public override void Run()
        {
            Logger.Information("\n******************\n[ACSTest] START\n******************\n");

            new List<Action>
            {
                TestSetFFUValue,
                TestGetPrepareToTransfertValue,
                TestGetEMOValue,
            }.ForEach(test =>
                {
                    test.Invoke();
                    Console.WriteLine($"{test.Method.Name} is successful");
                }
            );

            Logger.Information("[ACSTest] finished successfully !");

            Logger.Information("[ACSTest] END !");
        }

        public void TestSetFFUValue()
        {
            /*
            To do this test, you have to be next to the machine and check if the FFU starts (or if not the click)
             */
            Logger.Information("Start TestSetFFUValue");

            var aCSController = GetAcsController();

            aCSController.TurnOnFFU();
            
            aCSController.TurnOffFFU();

            Logger.Information("Stop TestSetFFUValue");
        }

        public void TestGetEMOValue()
        {
            // To run this test stand beside the machine and push the emergency button when indicated
            Logger.Information("Start TestGetEMOValue");

            var aCSController = GetAcsController();

#pragma warning disable IDE0059 //  Unnecessary assignment of a value, necessary for debugging
            bool EMOIsNotPushed = false;
#pragma warning restore IDE0059 //  Unnecessary assignment of a value, necessary for debugging
            EMOIsNotPushed = aCSController.GetEMOPushValue();
            if (EMOIsNotPushed)
            {
                throw new Exception("EMO Pushed value must not be false");
            }

            //ACTION ==> Push the EMO, check that the value to be changed

            bool NowEMOIsPushed = aCSController.GetEMOPushValue();
            if (!NowEMOIsPushed)
            {
                throw new Exception("EMO Pushed value must not be true");
            }

            Logger.Information("Stop TestGetEMOValue");
        }

        public void TestGetPrepareToTransfertValue()
        {
            Logger.Information("Start TestGetPrepareToTransfertValue");

            var aCSController = GetAcsController();

#pragma warning disable IDE0059 //  Unnecessary assignment of a value, necessary for debugging
            bool prepareToTransfertValue = true;
#pragma warning restore IDE0059 //  Unnecessary assignment of a value, necessary for debugging
            prepareToTransfertValue = aCSController.GetPrepareToTransfertValue();
            if (prepareToTransfertValue)
            {
                throw new Exception("Prepare to transfert value must be false");
            }
            Logger.Information("Stop TestGetPrepareToTransfertValue");
        }

        private ACSController GetAcsController()
        {
            HardwareManager.Controllers.TryGetValue("ACSMotion", out var controller);
            if (controller is ACSController)
            {
                var aCSController = controller as ACSController;
                return aCSController;
            }
            else
            {
                throw new Exception("Controller not found");
            }
        }
    }
}
