using System;
using System.Collections.Generic;
using System.IO;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.Shared.Hardware.Camera.DummyCamera
{
    public class DummyCamera : DummyIDSCamera
    {
        private DummyCameraConfig _config;
        private int _imageIndex = -1;
        private List<string> _images = new List<string>();

        public DummyCamera(DummyCameraConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
            _config = config;
            Width = config.Width;
            Height = config.Height;
            MinExposureTimeMs = 0.023;
            MaxExposureTimeMs = 7;
            MinGain = 0;
            MaxGain = 32;
            LoadImageList();
            State = new DeviceState(DeviceStatus.Ready);
        }

        public override void Init()
        {
            base.Init();
            Logger.Information("Init Camera as dummy");
        }

        protected override USPImage GetNextImage()
        {
            USPImage procimg = new USPImage();

            _imageIndex = (_imageIndex + 1) % _images.Count;
            procimg.Load(_images[_imageIndex]);

            return procimg;

            /*using (USPImageMil procimg = new USPImageMil())
            {
                _imageIndex = (_imageIndex + 1) % _images.Count;
                procimg.Load(_images[_imageIndex]);

                //procimg.Load(@"C:\Projects\UnitySC\UnityControl\PM\PSD\Service\UnitySC.PM.DMT.Service.Host\bin\Debug\DummyCameraImages\Fringes\32-ET250dyn020.tif");
                MilImage milImage = procimg.GetMilImage();
                if (milImage.SizeBand > 1)
                    milImage.Convert(MIL.M_RGB_TO_L);
                if (milImage.SizeX != _config.Width || milImage.SizeY != _config.Height)
                {
                    using (MilImage milResized = new MilImage())
                    {
                        milResized.Alloc2d(milImage.OwnerSystem, Width, Height, milImage.Type, milImage.Attribute);
                        MilImage.Resize(milImage, milResized, MIL.M_FILL_DESTINATION, MIL.M_FILL_DESTINATION, MIL.M_DEFAULT);
                        procimg.SetMilImage(milResized);
                    }
                }

                Messenger.Send<CameraMessage>(new CameraMessage() { Camera = this, Image = procimg });
                procimg.AddRef();
                return procimg;
            }*/
        }

        private void LoadImageList()
        {
            foreach (string path in _config.Images)
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    _images.Add(fullPath);
                }
                else
                {
                    if (Directory.Exists(fullPath))
                    {
                        List<string> list = new List<string>();
                        list.AddRange(Directory.EnumerateFiles(fullPath, "*.bmp", SearchOption.AllDirectories));
                        list.AddRange(Directory.EnumerateFiles(fullPath, "*.tif", SearchOption.AllDirectories));
                        list.AddRange(Directory.EnumerateFiles(fullPath, "*.tiff", SearchOption.AllDirectories));
                        list.AddRange(Directory.EnumerateFiles(fullPath, "*.png", SearchOption.AllDirectories));
                        if (list.Count == 0)
                            throw new ApplicationException("No image files in directory: " + path);
                        list.Sort(new NaturalStringComparer());
                        _images.AddRange(list);
                    }
                    else
                    {
                        throw new ApplicationException("can't find path: " + path);
                    }
                }
            }
        }
    }
}
