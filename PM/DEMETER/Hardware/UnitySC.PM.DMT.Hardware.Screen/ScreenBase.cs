using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Hardware.Screen
{
    public abstract class ScreenBase : IDevice
    {
        public string Name { get; set; }
        public DeviceState State { get; set; } = new DeviceState(DeviceStatus.Unknown);
        public DeviceFamily Family => DeviceFamily.Screen;
        public string DeviceID { get; set; }

        protected IGlobalStatusServer GlobalStatusServer;

        public string Model { get; protected set; }

        public string SerialNumber { get; protected set; }
        public string Version { get; protected set; }
        public virtual int Width { get; protected set; }
        public virtual int Height { get; protected set; }
        public double PixelPitchHorizontal { get; protected set; }
        public double PixelPitchVertical { get; protected set; }
        public Polarisation Polarisation { get; protected set; }

        public List<Length> WavelengthPeaks { get; protected set; }

        /// <summary>
        /// ScreenwhiteWaitTime this property used to mark the peiod of displaying the white color on the screen
        /// to avoid the heating problem .
        /// </summary>
        public double ScreenWhiteDisplayTimeSec { get; protected set; } // Unit "Second"

        public List<ScreenBase> OtherScreens;

        public int DisplayID;

        /// <summary>
        /// The time required for the image to be actually displayed on the screen, in seconds.
        /// </summary>
        public double ScreenStabilizationTimeSec { get; set; }

        public DisplayPosition DisplayPosition { get; set; }

        public abstract void DisplayImage(USPImageMil procimg);

        public abstract Task DisplayImageAsync(USPImageMil procimg);

        public abstract void Clear();

        public abstract Task ClearAsync(Color color);

        protected ILogger Logger;
        protected DMTScreenConfig Config;

#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event StateChangedEventHandler OnStatusChanged;

        public abstract void Shutdown();

        public virtual void Init(DMTScreenConfig config, IGlobalStatusServer globalStatusServer,
            ScreenDensitronDM430GNControllerConfig screenDensitronConfig, ScreenController controller)
        {
            Config = config;
            Name = config.Name;
            DeviceID = config.DeviceID;
            Logger = new HardwareLogger(config.LogLevel.ToString(), Family.ToString(), Name);
            GlobalStatusServer = globalStatusServer;
        }
        protected virtual void ParseConfig(DMTScreenConfig config)
        {
            ScreenStabilizationTimeSec = config.ScreenStabilizationTimeSec;
            DisplayPosition = config.DisplayPosition;
            ScreenWhiteDisplayTimeSec = config.ScreenWhiteDisplayTimeSec;
            PixelPitchHorizontal = config.PixelPitchHorizontal;
            PixelPitchVertical = config.PixelPitchVertical;
            Polarisation = config.Polarisation;
            WavelengthPeaks = config.WavelengthPeaks;

            try
            {
                string[] strs = config.Resolution.Split(new char[] { 'x', '@' });
                Width = int.Parse(strs[0]);
                Height = int.Parse(strs[1]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Invalid screen resolution config", ex);
            }
        }



        public void ClearOtherScreens()
        {
            if (OtherScreens == null)
                return;
            foreach (var screen in OtherScreens)
            {
                screen.Clear();
            }
        }
    }
}
