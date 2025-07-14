using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Calibration
{
    public class CalibrationManagerFDCProvider : IFDCProvider
    {
        private readonly FDCManager _fdcManager;

        private bool _probeLiseHFEnabled = false;

        private DateTime _fdcXYCalibrationCreationDate;
        private DateTime _fdcObjectivesCalibrationCreationDate;
        private DateTime _fdcLiseHFCalibrationCreationDate;

        private double _fdcPixelSize_x5NIR;
        private double _fdcPixelSize_x10NIR;
        private double _fdcPixelSize_x20NIR;
        private double _fdcPixelSize_x50NIR;
        private double _fdcPixelSize_x50VIS;

        private const string ANA_UptimeSinceLastXYCalibrationName = "ANA_UpTimeSinceLastXYCalibration";
        private const string ANA_UptimeSinceLastObjectivesCalibrationName = "ANA_UpTimeSinceLastObjectivesCalibration";
        private const string ANA_UptimeSinceLastLiseHFCalibrationName = "ANA_UpTimeSinceLastLiseHFCalibration";

        private const string ANA_PixelSize_x5NIR = "ANA_PixelSize_x5NIR";
        private const string ANA_PixelSize_x10NIR = "ANA_PixelSize_x10NIR";
        private const string ANA_PixelSize_x20NIR = "ANA_PixelSize_x20NIR";
        private const string ANA_PixelSize_x50NIR = "ANA_PixelSize_x50NIR";
        private const string ANA_PixelSize_x50VIS = "ANA_PixelSize_x50VIS";

        private readonly List<string> _providerFDCNames = new List<string>();

        public CalibrationManagerFDCProvider()
        {
            _fdcManager = ClassLocator.Default.GetInstance<FDCManager>();
        }

        private bool ToolHasLiseHFProbeEnabled()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            return hardwareManager.Probes.ContainsKey("ProbeLiseHF");
        }

        public virtual FDCData GetFDC(string fdcName)
        {
            FDCData fdcdata = null;
            switch (fdcName)
            {
                case ANA_UptimeSinceLastXYCalibrationName:
                    if (_fdcXYCalibrationCreationDate == default)
                    {
                        fdcdata = FDCData.MakeNew(fdcName, new TimeSpan(999_999, 0, 0, 0, 0));
                    }
                    else
                    {
                        fdcdata = FDCData.MakeNew(fdcName, DateTime.Now.Subtract(_fdcXYCalibrationCreationDate));
                    }
                    break;

                case ANA_UptimeSinceLastObjectivesCalibrationName:
                    if (_fdcObjectivesCalibrationCreationDate == default)
                    {
                        fdcdata = FDCData.MakeNew(fdcName, new TimeSpan(999_999, 0, 0, 0, 0));
                    }
                    else
                    {
                        fdcdata = FDCData.MakeNew(fdcName, DateTime.Now.Subtract(_fdcObjectivesCalibrationCreationDate));
                    }
                    break;

                case ANA_UptimeSinceLastLiseHFCalibrationName:
                    if (_probeLiseHFEnabled)
                    {
                        if (_fdcLiseHFCalibrationCreationDate == default)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, new TimeSpan(999_999, 0, 0, 0, 0));
                        }
                        else
                        {
                            fdcdata = FDCData.MakeNew(fdcName, DateTime.Now.Subtract(_fdcLiseHFCalibrationCreationDate));
                        }
                    }
                    break;

                case ANA_PixelSize_x5NIR:
                    fdcdata = FDCData.MakeNew(fdcName, _fdcPixelSize_x5NIR, "µm");
                    break;
                case ANA_PixelSize_x10NIR:
                    fdcdata = FDCData.MakeNew(fdcName, _fdcPixelSize_x10NIR, "µm");
                    break;
                case ANA_PixelSize_x20NIR:
                    fdcdata = FDCData.MakeNew(fdcName, _fdcPixelSize_x20NIR, "µm");
                    break;
                case ANA_PixelSize_x50NIR:
                    fdcdata = FDCData.MakeNew(fdcName, _fdcPixelSize_x50NIR, "µm");
                    break;
                case ANA_PixelSize_x50VIS:
                    fdcdata = FDCData.MakeNew(fdcName, _fdcPixelSize_x50VIS, "µm");
                    break;

                default: break;
            }
            return fdcdata;
        }

        public void ResetFDC(string fdcName)
        {
            // Nothing to do
        }

        public void SetInitialCountdownValue(string fdcName, double initvalue)
        {
            // Nothing to do
        }

        public void SetPersistentData(string fdcName, IPersistentFDCData persistentFDCData)
        {
            // Nothing to do
        }

        public void StartFDCMonitor()
        {
            // Nothing to do
        }

        public void Register()
        {
            // do not call in constructor since hardware will not be initialized yet there
            _probeLiseHFEnabled = ToolHasLiseHFProbeEnabled();

            InitProviderFDCNames();
            _fdcManager.RegisterFDCProvider(this, _providerFDCNames);
        }


        public void CreateFDC(List<ICalibrationData> calibrationData)
        {
            foreach (var calibration in calibrationData)
            {
                if (calibration is XYCalibrationData xyCalibration)
                {
                    _fdcXYCalibrationCreationDate = xyCalibration.CreationDate;
                }
                else if (calibration is ObjectivesCalibrationData objectivesCalibration)
                {
                    _fdcObjectivesCalibrationCreationDate = objectivesCalibration.CreationDate;

                    _fdcPixelSize_x5NIR = GetPixelSizeFromObjectiveCalibration(objectivesCalibration, "5X NIR");
                    _fdcPixelSize_x10NIR = GetPixelSizeFromObjectiveCalibration(objectivesCalibration, "10X NIR");
                    _fdcPixelSize_x20NIR = GetPixelSizeFromObjectiveCalibration(objectivesCalibration, "20X NIR");
                    _fdcPixelSize_x50NIR = GetPixelSizeFromObjectiveCalibration(objectivesCalibration, "50X NIR");
                    _fdcPixelSize_x50VIS = GetPixelSizeFromObjectiveCalibration(objectivesCalibration, "50X VIS");
                }
                else if (calibration is LiseHFCalibrationData liseHFCalibration)
                {
                    if (liseHFCalibration != null)
                    {
                        _fdcLiseHFCalibrationCreationDate = liseHFCalibration.CreationDate;
                    }
                }
            }

            var fdcList = new List<FDCData>()
            {
                FDCData.MakeNew(ANA_UptimeSinceLastXYCalibrationName, DateTime.Now.Subtract(_fdcXYCalibrationCreationDate)),
                FDCData.MakeNew(ANA_UptimeSinceLastObjectivesCalibrationName, DateTime.Now.Subtract(_fdcXYCalibrationCreationDate)),
                FDCData.MakeNew(ANA_PixelSize_x5NIR, _fdcPixelSize_x5NIR, "µm"),
                FDCData.MakeNew(ANA_PixelSize_x10NIR, _fdcPixelSize_x10NIR, "µm"),
                FDCData.MakeNew(ANA_PixelSize_x20NIR, _fdcPixelSize_x20NIR, "µm"),
                FDCData.MakeNew(ANA_PixelSize_x50NIR, _fdcPixelSize_x50NIR, "µm"),
                FDCData.MakeNew(ANA_PixelSize_x50VIS, _fdcPixelSize_x50VIS, "µm"),
            };
            if (_probeLiseHFEnabled)
            {
                fdcList.Add(FDCData.MakeNew(ANA_UptimeSinceLastLiseHFCalibrationName, DateTime.Now.Subtract(_fdcLiseHFCalibrationCreationDate)));
            }
            _fdcManager.SendFDCs(fdcList);
        }

        private void InitProviderFDCNames()
        {
            _providerFDCNames.Add(ANA_UptimeSinceLastXYCalibrationName);
            _providerFDCNames.Add(ANA_UptimeSinceLastObjectivesCalibrationName);

            _providerFDCNames.Add(ANA_PixelSize_x5NIR);
            _providerFDCNames.Add(ANA_PixelSize_x10NIR);
            _providerFDCNames.Add(ANA_PixelSize_x20NIR);
            _providerFDCNames.Add(ANA_PixelSize_x50NIR);
            _providerFDCNames.Add(ANA_PixelSize_x50VIS);

            if (_probeLiseHFEnabled)
            {
                _providerFDCNames.Add(ANA_UptimeSinceLastLiseHFCalibrationName);
            }
        }

        private double GetPixelSizeFromObjectiveCalibration(ObjectivesCalibrationData objCalibrations, string objectiveName)
        {
            var foundObj = objCalibrations.Calibrations.Find(x => x.ObjectiveName.ToLower() == objectiveName.ToLower());
            if (foundObj == null)
              return 0.0;

            return (foundObj.Image.PixelSizeX.Micrometers + foundObj.Image.PixelSizeY.Micrometers) * 0.5; // Note (04/02/2025) : so far  PixelSizeX == PixelSizeY, we assume we have square pixel
        }
    }
}
