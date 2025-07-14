using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public static class CameraTestUtils
    {
        public static DummyUSPImage CreateProcessingImageFromFile(string imgPath)
        {
            var img = new DummyUSPImage(imgPath);
            return img;
        }

        public static string GetDefaultDataPath(string imgName)
        {
            return DirTestUtils.GetWorkingDirectoryDataPath("\\data\\CameraSignal\\" + imgName);
        }
    }
}
