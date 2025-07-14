using System;
using System.Collections.Generic;
using System.Diagnostics;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.PersistentData;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Recipe
{
    public class ANARecipeExecutionManagerFDCProvider : IFDCProvider
    {
        #region PersitentData

        private PersistentSumData<double> _fdcRecipeExecutionTimeSumInSeconds;
        
        #endregion PersitentData
        
        #region NonPersitentData
        
        private int _fdcCameraMeanpixel = 100;

        #endregion NonPersitentData

        private readonly FDCManager _fdcManager;
        private readonly IGlobalStatusServer _globalStatusServer;
        private readonly Stopwatch _recipeExecutionTime;
        private readonly ImageOperators _imageOperatorsLib;

        private const string ANA_AverageRecipeExecutionTime = "ANA_AverageRecipeExecutionTime";
        private const string ANA_CameraMeanPixel = "ANA_CameraMeanPixel";
        private readonly List<string> _providerFDCNames = new List<string>();
     

        public ANARecipeExecutionManagerFDCProvider()
        {
            _fdcManager = ClassLocator.Default.GetInstance<FDCManager>();
            _globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            _recipeExecutionTime = new Stopwatch();
            _imageOperatorsLib = new ImageOperators();
        }

        private void InitProviderFDCNames()
        {
            _providerFDCNames.Add(ANA_AverageRecipeExecutionTime);
            _providerFDCNames.Add(ANA_CameraMeanPixel);
        }

        public FDCData GetFDC(string fdcName)
        {
            FDCData fdcdata = null;
            switch (fdcName)
            {
                case ANA_AverageRecipeExecutionTime:
                    var recipeExecTimeAverage = new TimeSpan(0, 0, (int)_fdcRecipeExecutionTimeSumInSeconds.Average());
                    fdcdata = FDCData.MakeNew(fdcName, recipeExecTimeAverage);
                    break;
                case ANA_CameraMeanPixel:
                    fdcdata = FDCData.MakeNew(fdcName, _fdcCameraMeanpixel);
                    break;

                default: break;
            }
            return fdcdata;
        }

        public void ResetFDC(string fdcName)
        {
            switch (fdcName)
            {
                case ANA_AverageRecipeExecutionTime:
                    _fdcRecipeExecutionTimeSumInSeconds.Clear();
                    _fdcManager.SetPersistentFDCData(_fdcRecipeExecutionTimeSumInSeconds);
                    break;

                default: break;
            }
        }

        public void SetInitialCountdownValue(string fdcName, double initvalue)
        {
            // Nothing to do
        }

        public void SetPersistentData(string fdcName, IPersistentFDCData persistentFDCData)
        {
            switch (fdcName)
            {
                case ANA_AverageRecipeExecutionTime:
                    if (persistentFDCData is PersistentSumData<double> pfdcWindow)
                    {
                        _fdcRecipeExecutionTimeSumInSeconds = pfdcWindow;
                    }
                    break;

                default: break;
            }
        }

        public void StartFDCMonitor()
        {
            // Nothing to do
        }

        public void Register()
        {
            _fdcRecipeExecutionTimeSumInSeconds = new PersistentSumData<double>(ANA_AverageRecipeExecutionTime);

            InitProviderFDCNames();
            _fdcManager.RegisterFDCProvider(this, _providerFDCNames);
        }

        public void StartRecipeTimer()
        {
            _recipeExecutionTime.Restart();
        }

        public void StopRecipeTimer()
        {
            _recipeExecutionTime.Stop();
        }

        public void CreateFDCAvgExecutionTime()
        {
            if (_globalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                _fdcRecipeExecutionTimeSumInSeconds.Add(_recipeExecutionTime.Elapsed.TotalSeconds);
                _fdcManager.SetPersistentFDCData(_fdcRecipeExecutionTimeSumInSeconds);
            }
        }

        public void CreateFDCCameraMeanPixel()
        {
            if (_globalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                try
                {
                    var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
                    var cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
                    var mainCameraId = hardwareManager.GetMainCamera().DeviceID;
                    var img = HardwareUtils.AcquireCameraImage(hardwareManager, cameraManager, mainCameraId);
                    _fdcCameraMeanpixel = (int) Math.Round(_imageOperatorsLib.ComputeMeanPixel(img));
                }
                catch (Exception)
                { }                
            }
        }
    }
}
