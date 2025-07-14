using System;

using Cyber.ImageUtils;

namespace UnitySC.PM.Shared.Hardware.Camera.CyberOptics
{
    /// <summary>
    /// Stores a copy of the most usefull of the camera datas.
    /// </summary>
    public class PSF15CameraImages
    {
        public UInt16[,] HeightMap;

        public byte[,] QualityMap;

        /// <summary>
        /// May be null.
        /// </summary>
        public byte[,] RawImage0;

        public byte[,] RawImage1;
        public byte[,] RawImage2;
        public byte[,] RawImage3;
        public byte[,] RawImage4;
        public byte[,] RawImage5;

        /// <summary>
        /// May be null.
        /// </summary>
        public byte[,] TwoDImage;

        /// <summary>
        /// Combines height and quality maps to a float array (size must be identical), suitable for 3DA files.
        /// Every value with quality <= nanThreshold will be outputted as nan.
        /// </summary>
        public static Single[,] Save3DMapsAsFloat(UInt16[,] heightMap, byte[,] qualityMap, byte nanThreshold = 4)
        {
            Single[,] target = new Single[qualityMap.GetLength(0), qualityMap.GetLength(1)];
            Save3DMapsAsFloat(heightMap, qualityMap, target, nanThreshold);

            return target;
        }

        public static void Save3DMapsAsFloat(UInt16[,] heightMap, byte[,] qualityMap, Single[,] target, byte nanThreshold = 4)
        {
            unsafe
            {
                fixed (UInt16* heightOrigin_ptr = heightMap)
                fixed (byte* qualityOrigin_ptr = qualityMap)
                fixed (Single* targetOrigin_ptr = target)
                {
                    byte* quality_ptr = qualityOrigin_ptr;
                    byte* qualityEnd_ptr = qualityOrigin_ptr + qualityMap.Length;

                    Single* target_ptr = targetOrigin_ptr;
                    UInt16* height_ptr = heightOrigin_ptr;
                    while (quality_ptr < qualityEnd_ptr)
                    {
                        // Write one pixel.
                        if ((*(quality_ptr++)) <= nanThreshold)
                        {
                            // Low quality.
                            *(target_ptr++) = Single.NaN;
                            height_ptr += 1;
                        }
                        else
                        {
                            // Valid pixel.
                            *(target_ptr++) = *(height_ptr++);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fast memory copy. Memory allocation if needed (resolution changes not supported).
        /// </summary>
        public void CopyFrom(PSF15CameraData data)
        {
            unsafe
            {
                // 3D maps.
                CyberImage heightMap = data.HeightMap;
                if (HeightMap == null)
                {
                    // Allocate memory.
                    HeightMap = new UInt16[heightMap.Columns, heightMap.Rows];
                    QualityMap = new byte[heightMap.Columns, heightMap.Rows];
                }

                fixed (UInt16* ptr = HeightMap)
                {
                    Int32 bufferSize = HeightMap.Length << 1;
                    new ReadOnlySpan<byte>(heightMap.Buffer.ToPointer(), bufferSize).CopyTo(new Span<byte>(ptr, bufferSize));
                }

                CopyRawImage(data.QualityMap, QualityMap);

                // Raw images.
                CyberImage[] rawImages = data.Access1RawImagePerChannel();
                CyberImage rawImage0 = rawImages[0];
                if (rawImage0 == null)
                {
                    // Free memory.
                    RawImage0 = null;
                    RawImage1 = null;
                    RawImage2 = null;
                    RawImage3 = null;
                    RawImage4 = null;
                    RawImage5 = null;
                }
                else
                {
                    if (RawImage0 == null)
                    {
                        // Allocate memory
                        RawImage0 = new byte[rawImage0.Columns, rawImage0.Rows];
                        RawImage1 = new byte[rawImage0.Columns, rawImage0.Rows];
                        RawImage2 = new byte[rawImage0.Columns, rawImage0.Rows];
                        RawImage3 = new byte[rawImage0.Columns, rawImage0.Rows];
                        RawImage4 = new byte[rawImage0.Columns, rawImage0.Rows];
                        RawImage5 = new byte[rawImage0.Columns, rawImage0.Rows];
                    }

                    CopyRawImage(rawImage0, RawImage0);
                    CopyRawImage(rawImages[1], RawImage1);
                    CopyRawImage(rawImages[2], RawImage2);
                    CopyRawImage(rawImages[3], RawImage3);
                    CopyRawImage(rawImages[4], RawImage4);
                    CopyRawImage(rawImages[5], RawImage5);
                }

                // 2D image.
                CyberImage twoDImage = data.Data.GetProcessed2DCoaxImage(0);
                if (twoDImage == null)
                {
                    // Free memory.
                    TwoDImage = null;
                }
                else
                {
                    if (TwoDImage == null)
                    {
                        // Allocate memory.
                        TwoDImage = new byte[twoDImage.Columns, twoDImage.Rows];
                    }

                    CopyRawImage(twoDImage, TwoDImage);
                }
            }
        }

        private void CopyRawImage(CyberImage source, byte[,] target)
        {
            unsafe
            {
                fixed (byte* ptr = target)
                {
                    new ReadOnlySpan<byte>(source.Buffer.ToPointer(), target.Length).CopyTo(new Span<byte>(ptr, target.Length));
                }
            }
        }
    }
}
