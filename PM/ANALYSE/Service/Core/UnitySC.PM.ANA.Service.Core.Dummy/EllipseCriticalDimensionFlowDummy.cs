using System;
using System.IO;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.CD;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class EllipseCriticalDimensionFlowDummy : EllipseCriticalDimensionFlow
    {
        public EllipseCriticalDimensionFlowDummy(EllipseCriticalDimensionInput input) : base(input)
        {

        }

        protected override void Process()
        {
            var rnd = new Random();
          
            Thread.Sleep(150);

            // workaround pour trouver le shape (circle ou ellispe ou rectangular)
            bool UseCircleShape = (Input.ApproximateLength == Input.ApproximateWidth) && (Input.LengthTolerance == Input.WidthTolerance);

            Result.Length = (Input.ApproximateLength.Micrometers + 1.2 * (rnd.NextDouble() * (2.0 * Input.LengthTolerance.Micrometers) - Input.LengthTolerance.Micrometers)).Micrometers();
            if (UseCircleShape)
                Result.Width = Result.Length;
            else
                Result.Width = (Input.ApproximateWidth.Micrometers + 1.2 * (rnd.NextDouble() * (2.0 * Input.WidthTolerance.Micrometers) - Input.WidthTolerance.Micrometers)).Micrometers();
            string filePath = Path.Combine(Path.GetDirectoryName(PathHelper.GetExecutingAssemblyPath()), @"DummyImages\CDEllipse.png");
            if (File.Exists(filePath))
            {
                Result.ResultImage = new ServiceImage();
                Result.ResultImage.LoadFromFile(filePath);
                Result.ResultImage.Type = ServiceImage.ImageType.RGB;
            }

            if(UseCircleShape)
                Logger.Information($"{LogHeader} Circle Diameter: {Result.Width}", Result);
            else
                Logger.Information($"{LogHeader} Ellipse Width: {Result.Width} - Ellipse Length: {Result.Length}", Result);
        }
    }
}
