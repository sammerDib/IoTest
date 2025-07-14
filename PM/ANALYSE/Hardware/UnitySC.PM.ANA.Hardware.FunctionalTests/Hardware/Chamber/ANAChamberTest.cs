using System;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware
{
    public class ANAChamberTest : FunctionalTest
    {
        private ACSController _controllerACS;
        private IANAChamber _anaChamber;

        public override void Run()
        {
            Logger.Information("\n******************\n[ANAChamberTest] START\n******************\n");
            Init();
            TurnOnAndOff_FFUTest();
            TestInputsANAChamber();
            Logger.Information("\n******************\n[ANAChamberTest] STOP\n******************\n");
        }

        public void Init()
        {
            HardwareManager.Controllers.TryGetValue("ACSMotion", out var controller);
            _controllerACS = (ACSController)controller;
            if (_controllerACS == null)
            {
                throw new Exception("[TestInputANAChamber]Controller ACS is null");
            }

            _anaChamber = (HardwareManager.Chamber as IANAChamber);
            if (_anaChamber == null)
            {
                throw new Exception("[TestInputANAChamber]ANAChamber is null");
            }
        }

        public void TurnOnAndOff_FFUTest()
        {
            Logger.Information("[Turn on FFU test] Start !");
            
            _anaChamber.TurnOffFFU();
            Logger.Information("FFU IS OFF");
            _anaChamber.TurnOnFFU();
            if (!_anaChamber.FFUState())
            {
                throw new Exception("Expected FFU is ON, actual FFU is OFF");
            }
            Logger.Information("FFU IS ON");
            _anaChamber.TurnOffFFU();
            if (_anaChamber.FFUState())
            {
                throw new Exception("Expected FFU is OFF, actual FFU is ON");
            }
            Logger.Information("FFU IS OFF");
            Logger.Information("[Turn on FFU test] Stop with succes !");            
        }

        public void TestInputsANAChamber()
        {
            Logger.Information("[TestInputsTMAPChamber] Start !");
            bool IsActive;
            IsActive = _anaChamber.EMOState();
            Logger.Information($"Resut of EMOState state is {IsActive} !");
            IsActive = _anaChamber.PrepareToTransferState();
            Logger.Information($"Resut of PrepareToTransfert state is {IsActive} !");
            Logger.Information("[TestInputsANAChamber] Stop !");
        }
    }
}
