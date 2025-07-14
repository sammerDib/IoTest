using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Camera.DummyCamera
{
    [Serializable]
    public class DummyCameraConfig : CameraConfigBase
    {
        public int Width;
        public int Height;

        [XmlArrayItem("Path")]
        public List<string> Images;

        public DummyCameraConfig()
        {
            IsSimulated = true;
        }
    }
}