using UnitySC.Shared.Image;
using UnitySC.Shared.Data;

namespace UnitySC.PM.EME.Service.Interface.Recipe.Execution
{
    public class RecipeExecutionMessageFactory
    {
        public static RecipeExecutionMessage CreateStarted(RemoteProductionInfo remoteInfo)
        {
            return new RecipeExecutionMessage
            {
                Status = ExecutionStatus.Started,
                CurrentRemoteProductionInfo = remoteInfo
            };
        }

        public static RecipeExecutionMessage CreateCanceled(int totalImages, int imageIndex, RemoteProductionInfo remoteInfo)
        {
            return new RecipeExecutionMessage
            {
                Status = ExecutionStatus.Canceled,
                TotalImages = totalImages,
                ImageIndex = imageIndex,
                CurrentRemoteProductionInfo = remoteInfo
            };
        }

        public static RecipeExecutionMessage CreateFailed(string errorMessage, RemoteProductionInfo remoteInfo)
        {
            return new RecipeExecutionMessage
            {
                Status = ExecutionStatus.Failed,
                ErrorMessage = errorMessage,
                CurrentRemoteProductionInfo = remoteInfo
            };
        }
        
        public static RecipeExecutionMessage CreateFinished(RemoteProductionInfo remoteInfo)
        {
            return new RecipeExecutionMessage
            {
                Status = ExecutionStatus.Finished,
                CurrentRemoteProductionInfo = remoteInfo
            };
        }

        public static RecipeExecutionMessage CreateExecuting(int totalImages, int imageIndex, RemoteProductionInfo remoteInfo)
        {
            return new RecipeExecutionMessage
            {
                Status = ExecutionStatus.Executing,
                TotalImages = totalImages,
                ImageIndex = imageIndex,
                CurrentRemoteProductionInfo = remoteInfo
            };
        }

        public static RecipeExecutionMessage CreateExecuting(int totalImages, int imageIndex, ServiceImage thumbnail, string imageFilePath, RemoteProductionInfo remoteInfo)
        {
            return new RecipeExecutionMessage
            {
                Status = ExecutionStatus.Executing,
                TotalImages = totalImages,
                ImageIndex = imageIndex,
                Thumbnail = thumbnail,
                ImageFilePath = imageFilePath,
                CurrentRemoteProductionInfo = remoteInfo
            };
        }
    }
}
