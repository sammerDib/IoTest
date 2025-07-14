using System.Text;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Service.Core.Flows.Distortion
{
    public class DistortionFlowDummy : DistortionFlow
    {
        public DistortionFlowDummy(DistortionInput input, IEmeraCamera camera) : base(input, camera)
        {
        }

        protected override void Process()
        {
            Result.DistortionData = new DistortionData
            {
                CameraMat = new[] { 1.0, 5.0, 2.0, 1.0, 5.0, 1.0, 0.0, 0.0, 1.0 },
                NewOptimalCameraMat = new[] { 1.0, 5.0, 7.0, 1.0, 4.0, 1.0, 0.0, 0.0, 1.0 },
                DistortionMat = new[] { 1.0, 5.0, 2.0, 1.0, 1.0 },
                RotationVec = new[] { 1.0, 5.0, 2.0 },
                TranslationVec = new[] { 4.0, 5.0, 2.0 }
            };

            LogMatrix(Logger, "Camera matrix:", Result.DistortionData.CameraMat, 3, 3);
            LogMatrix(Logger, "New optimal camera correction matrix", Result.DistortionData.NewOptimalCameraMat, 3, 3);
            LogVector(Logger, "Distortion matrix", Result.DistortionData.DistortionMat);
            LogVector(Logger, "Rotation vector", Result.DistortionData.RotationVec);
            LogVector(Logger, "Translation vector", Result.DistortionData.TranslationVec);
        }
        
        //[17:53:40.279 INF]  New optimal camera correction matrix:
        //1 5 7
        //1 4 1
        //0 0 1
        private static void LogMatrix(ILogger logger, string label, double[] matrix, int rows, int cols)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{label}:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sb.Append(matrix[i * cols + j] + " ");
                }
                sb.AppendLine();
            }
            logger.Information(sb.ToString());
        }

        //[17:53:40.280 INF]  Rotation vector:
        //1 5 2
        private static void LogVector(ILogger logger, string label, double[] vector)
        {
            var sb = new StringBuilder();
            sb.Append($"{label}:\n");
            foreach (double t in vector)
                sb.Append(t + " ");
            
            sb.AppendLine();
            logger.Information(sb.ToString());
        }
    }
}
