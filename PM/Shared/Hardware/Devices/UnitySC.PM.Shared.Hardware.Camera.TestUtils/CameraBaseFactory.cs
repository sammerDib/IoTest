using System;

using Moq;

using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.TestUtils
{
    public static class CameraBaseFactory
    {
        public static CameraBase Build(Action<CameraBase> action = null)
        {
            var camera = new DummyIDSCamera(new DummyCameraConfig(),
                Mock.Of<IGlobalStatusServer>(),
                Mock.Of<ILogger>()
            );
            action?.Invoke(camera);
            return camera;
        }
    }
}
