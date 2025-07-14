using System;
using System.ServiceModel;

using CO.Psx.Sensor;

using Cyber.ImageUtils;

namespace UnitySC.PM.Shared.Hardware.Camera.CyberOptics
{
    /// <summary>
    /// Callback data containing acquired images.
    /// This data is part of the camera buffers, and can only be used in the Messenger callback: no reference should be kept.
    /// Use CyberImage.Copy() if you need to keep a reference to the acquired data out of the Messenger callback scope!
    /// </summary>
    public class PSF15CameraData
    {
        // Full camera data.
        public CO.Psx.Engine.SiteImageStack Data;

        /// <summary>
        /// Shortcut to the 16 bits height map.
        /// Dynamic: 0 to 2^16-1 corresponds to the wrapHeight size setting (each 16 bits value has to be multiplied by wrapHeight/2^16 to get height values).
        /// Origin: depends on the startHeight parameter. For example, with startHeight=-0.5 (default), 0 to 2^16-1 covers -wrapHeight/2 to +wrapHeight/2
        ///  -which is often optimal, as the accuracy is best in the center-.
        /// </summary>
        public CyberImage HeightMap => Data.GetHeightImage(CO.Psx.Sensor.HeightImageType.Height16);

        /// <summary>
        /// Shortcut to the 8 bits quality map.
        /// <=4 => no data => 0µm set in height map.
        /// 5-14 => low confidence.
        /// 15-200 => ok
        /// +200 => ko
        /// </summary>
        public CyberImage QualityMap => Data.GetHeightImage(CO.Psx.Sensor.HeightImageType.ModIndex);

        /// <summary>
        /// Shortcut to the first raw image of each channel -6 raw images are acquired for each of the 6 channels-.
        /// Returns 6 images, each one possibly null if raw images where not retrieved.
        /// </summary>
        public CyberImage[] Access1RawImagePerChannel()
        {
            CyberImage[] res = new CyberImage[6];
            Int32 idx = 0;

            SiteCapture siteCapture = Data.SiteCapture;
            foreach (var availableChannel in siteCapture.AvailableChannels)
            {
                if (!availableChannel.is2D)
                {
                    var imageCaptures = siteCapture.GetImageCapturesForChannel(availableChannel);
                    if (imageCaptures.Count < 1)
                    {
                        throw new CommunicationException("No 6 raw images per channel?");
                    }

                    res[idx++] = imageCaptures[0].CyberImage;
                }
            }

            return res;
        }
    }
}
