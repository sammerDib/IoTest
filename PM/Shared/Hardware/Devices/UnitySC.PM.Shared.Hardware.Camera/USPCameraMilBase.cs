using System;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera
{
    public abstract class USPCameraMilBase : CameraBase, IDisposable
    {
        protected USPCameraMilBase(CameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
        }

        public abstract void Dispose();

        #region IMAGE PROCESSING

        public abstract USPImageMil SingleGrab();

        /// <summary>
        /// Copie une image grabbée vers une nouvelle USPImageMil
        /// </summary>
        public static USPImageMil CopyGrabbedImageToNewProcessingImage(USPImageMil grabImage)
        {
            var procimg = new USPImageMil();
            var milImage = new MilImage();

            var milGrabImage = grabImage.GetMilImage();
            milImage.Alloc2d(milGrabImage.SizeX, milGrabImage.SizeY, milGrabImage.Type, MIL.M_IMAGE + MIL.M_PROC);
            procimg.SetMilImage(milImage);

            MilImage.Copy(milGrabImage, milImage);
            return procimg;
        }

        #endregion IMAGE PROCESSING
    }
}
