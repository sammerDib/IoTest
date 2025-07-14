using System;
using System.Diagnostics;
using System.Windows;

using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Camera.IDS
{
    public class ImageMemoryTest : FunctionalTest
    {
        public override void Run()
        {
            var cameraTop = (UI324xCpNir)HardwareManager.Cameras["1"];

            long memoryBefore = Process.GetCurrentProcess().WorkingSet64;
            for (int i = 1; i <= 10; i++)
            {
                var imageResolution = new Size(500, 800);
                cameraTop.SetImageResolution(imageResolution);
            }
            long memorySetAfter = Process.GetCurrentProcess().WorkingSet64;

            long diff = Math.Abs(memorySetAfter - memoryBefore);
            if (diff > 2097152 /* 10 images = 4 Mo if memory leak */) throw new Exception("Allocating 10 images without releasing the memory induce memory leak");
        }
    }
}
