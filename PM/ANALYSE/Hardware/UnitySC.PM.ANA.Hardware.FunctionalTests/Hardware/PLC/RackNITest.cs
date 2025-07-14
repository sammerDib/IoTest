using System;
using System.Collections.Generic;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.Shared.Status.Service.Interface;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware
{
    public class RackNITest : FunctionalTest
    {
        public override void Run()
        {
            Logger.Information("\n******************\n[RackNITest] START\n******************\n");

            new List<Action>
            {
                TestReadDoorOpenInputChannelValue,
                TestWriteOutputOpticalActuatorChannelValue,
            }.ForEach(test =>
                {
                    test.Invoke();
                    Console.WriteLine($"{test.Method.Name} is successful");
                }
            );

            Logger.Information("[RackNITest] finished successfully !");

            /*
             * The tests below are to be run individually as they voluntarily raise an exception.
             */
            //GivenBadDestinationNumber_ReturnException();
            //GivenBadDeviceNumber_ReturnException();
            //GivenBadOutputName_ReturnException();
            //GivenBadInputName_ReturnException();

            Logger.Information("[RackNITest] END !");
        }

        public void TestReadDoorOpenInputChannelValue()
        {
            /*
             * This functional test is performed with the NIMAX tool in parallel.  Open NIMAX, click on device/Interface>NI USB-6501 "Dev2" then test panels.
             * Select the desired port (0, 1 or 2). Select input or output on the desired line.
             * - If it is an input, press start and see the status of the line (green if active)
             * - If it is an output, turn the selector to on or off. Press start.
             * Set a breakpoint on the feedback value and read the value.
             */

            Logger.Information("Start TestReadDoorOpenInputChannelValue");

            HardwareManager.Controllers.TryGetValue("RackNI", out var controller);
            var controllerRackNI = (NICouplerController)controller;

            var DoorOpen = (DigitalInput)controllerRackNI.GetInput("DoorOpen");

            bool readValue = controllerRackNI.DigitalRead(DoorOpen);
            Logger.Information($"{DoorOpen.Name} on port number {DoorOpen.Address.Module} " +
                $"and line {DoorOpen.Address.Channel} is {readValue}");

            Logger.Information("Stop TestReadDoorOpenInputChannelValue");
        }

        public void TestWriteOutputOpticalActuatorChannelValue()
        {
            Logger.Information("Start TestWriteOutputOpticalActuatorChannelValue");

            // WARNING To run this functional test, turn on the camera of the TMAP and watch the optical rail, we must observe the optical actuator move.
            HardwareManager.Controllers.TryGetValue("RackNI", out var controller);
            var controllerRackNI = (NICouplerController)controller;

            var OpticalActuator1 = (DigitalOutput)controllerRackNI.GetOutput("OpticalActuator1");
            var OpticalActuator2 = (DigitalOutput)controllerRackNI.GetOutput("OpticalActuator2");

            Logger.Information("INITIALISATION");

            //Allows the optical actuators to return to their initialized position
            //The sleeps are used to watch the movement with the camera
            Thread.Sleep(10000);

            // True or false according to the value indicated in NIMAX, see the procedure in the previous test
            // For the Optical Actuator, you can see live or on camera the movement of the camera rail
            controllerRackNI.DigitalWrite(OpticalActuator1, true);
            controllerRackNI.DigitalWrite(OpticalActuator2, true);

            //Watching the movement on camera
            Thread.Sleep(10000);

            Logger.Information("SECOND MOVE");

            controllerRackNI.DigitalWrite(OpticalActuator1, false);
            controllerRackNI.DigitalWrite(OpticalActuator2, false);
            Thread.Sleep(5000);

            Logger.Information("Stop TestWriteOutputOpticalActuatorChannelValue");
        }

        public void GivenBadDeviceNumber_ReturnException()
        {
            // This test throw an exception
            Logger.Information("Start GivenBadDeviceNumber_ReturnException");

            var config = new NICouplerControllerConfig()
            {
            };

            config.NIDeviceType = "USB-6501";
            config.Port = "Dev3";
            config.DeviceID = "USB-6501";
            config.Name = "DoorOpen";

            DigitalInput DoorOpen = new DigitalInput();
            DoorOpen.Name = "DoopOpen";
            DoorOpen.Address.Module = 1;
            DoorOpen.Address.Channel = 2;

            config.IOList.Add(DoorOpen);

            var globalStatusServer = UnitySC.Shared.Tools.ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var controllerRackNI = new NICouplerController(config, globalStatusServer, Logger);
        }

        public void GivenBadDestinationNumber_ReturnException()
        {
            Logger.Information("Start GivenBadDestinationNumber_ReturnException");

            // This test throw an exception
            var config = new NICouplerControllerConfig()
            {
            };

            config.NIDeviceType = "USB-6501";
            config.Port = "Dev5";
            config.DeviceID = "USB-6501";
            config.Name = "DoorOpen";

            DigitalInput DoorOpen = new DigitalInput();
            DoorOpen.Name = "DoopOpen";
            DoorOpen.Address.Module = 1;
            // Given bad destination. Line number 10 does not exist.
            DoorOpen.Address.Channel = 10;

            config.IOList.Add(DoorOpen);

            var globalStatusServer = UnitySC.Shared.Tools.ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var controllerRackNI = new NICouplerController(config, globalStatusServer, Logger);
        }

        public void GivenBadOutputName_ReturnException()
        {
            // This test throw an exception
            Logger.Information("Start GivenBadOutputName_ReturnException");

            HardwareManager.Controllers.TryGetValue("RackNI", out var controller);
            var NICouplerController = (NICouplerController)controller;

            NICouplerController.Connect();

            var BadName = (DigitalOutput)NICouplerController.GetOutput("BadName");

            Logger.Information("Open channels");
        }

        public void GivenBadInputName_ReturnException()
        {
            Logger.Information("Start GivenBadInputName_ReturnException");
            // This test throw an exception

            HardwareManager.Controllers.TryGetValue("RackNI", out var controller);
            var NICouplerController = (NICouplerController)controller;

            NICouplerController.Connect();

            var BadName = (DigitalInput)NICouplerController.GetInput("BadName");

            Logger.Information("Open channels");
        }

        public void TestFFUChannel()
        {
            Logger.Information("Start TestFFUChannel");
            /*Run this test next to the TMAP to ensure that the FFU is activated.
             * Also make sure to run the code until the end so that the FFU does not remain activated
             * (the parameter is set to false)
             */
            HardwareManager.Controllers.TryGetValue("RackNI", out var controller);
            var controllerRackNI = (NICouplerController)controller;

            var FFUFilter = (DigitalOutput)controllerRackNI.GetOutput("FFUFilter");

            controllerRackNI.DigitalWrite(FFUFilter, true);

            // Wait for the FFU to start up
            Thread.Sleep(20000);

            // Turn off the FFU
            controllerRackNI.DigitalWrite(FFUFilter, false);
            Logger.Information("Stop TestFFUChannel");
        }
    }
}
