using System;
using System.IO;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.PSI;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class PSIFlowDummy : PSIFlow
    {
        public PSIFlowDummy(PSIInput input) : base(input)
        {
        }

        protected override void Process()
        {
            CheckCancellation();
            Logger.Debug("[PSIFlowDummy] Start Dummy flow.");
            SetProgressMessage("[PSIFlowDummy] in progress ");

            Thread.Sleep(700);

            var rnd = new Random();
            Result.StepHeight = (20.0 * rnd.NextDouble() + 10.0).Nanometers();
            Result.Roughness = (5 * rnd.NextDouble() + 2.0).Nanometers();

            Result.QualityScore = 0.98;
            string filePath = Path.Combine(Path.GetDirectoryName(PathHelper.GetExecutingAssemblyPath()), @"DummyImages\VSITopo.3da");
            if (File.Exists(filePath))
            {
                Result.NanoTopographyImage = new ServiceImage();
              //  Result.Image3DA = new MatrixFloatFile(filePath);

                using (var mff = new MatrixFloatFile(filePath))
                {
                    var serviceImg = new ServiceImage();
                    serviceImg.Type = ServiceImage.ImageType._3DA;
                    serviceImg.DataHeight = mff.Header.Height;
                    serviceImg.DataWidth = mff.Header.Width;
                    serviceImg.Data = mff.WriteInMemory(false);
                    Result.NanoTopographyImage = serviceImg;
                }
            }
            Logger.Debug("[PSIFlowDummy] Success.");
        }
    }
}
