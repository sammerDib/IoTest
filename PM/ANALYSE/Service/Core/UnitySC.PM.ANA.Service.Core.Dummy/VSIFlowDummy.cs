using System.IO;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.VSI;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class VSIFlowDummy : VSIFlow
    {
        public VSIFlowDummy(VSIInput input) : base(input)
        {
            Logger.Debug("[VSIFlowDummy] Start Dummy flow.");
        }

        protected override void Process()
        {
            CheckCancellation();
            Logger.Debug("[VSIFlowDummy] Start Dummy flow.");
            SetProgressMessage("[VSIFlowDummy] in progress ");
            Thread.Sleep(300);
            Result.QualityScore = 0.98;
            string filePath = Path.Combine(Path.GetDirectoryName(PathHelper.GetExecutingAssemblyPath()), @"DummyImages\VSITopo.3da");
            if (File.Exists(filePath))
            {
                using (var mff = new MatrixFloatFile(filePath))
                {
                    var serviceImg = new ServiceImage();
                    serviceImg.Type = ServiceImage.ImageType._3DA;
                    serviceImg.DataHeight = mff.Header.Height;
                    serviceImg.DataWidth = mff.Header.Width;
                    serviceImg.Data = mff.WriteInMemory(false);
                    Result.TopographyImage = serviceImg;
                }
            }
            Logger.Debug("[VSIFlowDummy] Success.");
        }
    }
}
