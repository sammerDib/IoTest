using System;
using System.Collections.Generic;
using System.Threading;

using AcquisitionAdcExchange;

using ADCEngine;

using AdcRobotExchange;

using AdvancedModules.Edition.VID.HazeVID;

using FormatHAZE;

using HazeLSModule;

namespace AdvancedModules.Edition.VID
{
    public class HazeVIDModule : ModuleBase
    {
        public readonly HazeParameter paramHazemax_ppm;
        public readonly HazeParameter paramHazemin_ppm;
        public readonly HazeParameter paramHazemean_ppm;
        public readonly HazeParameter paramHazemedian_ppm;
        public readonly HazeParameter paramHazestddev_ppm;
        private LSHazeData _dataHaze;

        //=================================================================
        // Constructeur
        //=================================================================
        public HazeVIDModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramHazemax_ppm = new HazeParameter(this, "HazeMaxPPM");
            paramHazemin_ppm = new HazeParameter(this, "HazeMinxPPM");
            paramHazemean_ppm = new HazeParameter(this, "HazeMeanPPM");
            paramHazemedian_ppm = new HazeParameter(this, "HazeMedianPPM");
            paramHazestddev_ppm = new HazeParameter(this, "HazeStdDevPPM");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("cluster " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            //TODO : recup des données
            HazeLSMeasure hazeData = (HazeLSMeasure)obj;
            _dataHaze = hazeData.Data;

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("HazeVID",
                () =>
                {
                    try
                    {
                        ProcessHazeVID();
                    }
                    catch (Exception ex)
                    {
                        string msg = "Haze VID failed: " + ex.Message;
                        HandleException(new ApplicationException(msg, ex));
                    }
                    finally
                    {
                        base.OnStopping(oldState);
                    }
                });
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessHazeVID()
        {
            VidHaze vidMaxPPM = new VidHaze();
            SetVIDVariable(ref vidMaxPPM, paramHazemax_ppm, _dataHaze.max_ppm);
            VidHaze vidMinPPM = new VidHaze();
            SetVIDVariable(ref vidMinPPM, paramHazemin_ppm, _dataHaze.min_ppm);
            VidHaze vidAvgPPM = new VidHaze();
            SetVIDVariable(ref vidAvgPPM, paramHazemedian_ppm, _dataHaze.median_ppm);
            VidHaze vidMeanPPM = new VidHaze();
            SetVIDVariable(ref vidMeanPPM, paramHazemean_ppm, _dataHaze.mean_ppm);
            VidHaze vidStdDevPPM = new VidHaze();
            SetVIDVariable(ref vidStdDevPPM, paramHazestddev_ppm, _dataHaze.stddev_ppm);

            List<VidBase> list = new List<VidBase>();
            list.Add(vidMaxPPM);
            list.Add(vidMinPPM);
            list.Add(vidAvgPPM);
            list.Add(vidMeanPPM);
            list.Add(vidStdDevPPM);
            ADC.Instance.TransferToRobotStub.TransferVids(Recipe.Toolname, $"{Recipe.Wafer.GetWaferInfo(eWaferInfo.JobID)}{Recipe.Wafer.Basename}", list);
        }

        private void SetVIDVariable(ref VidHaze Vid, HazeParameter param, double valueMeasure)
        {
            Vid.VidNumber = param.VidNumber;
            Vid.VidLabel = param.VidLabel;
            Vid.Measure = valueMeasure;
            Vid.Xcoordinate = (int)0;
            Vid.Ycoordinate = (int)0;
            Vid.UnitValue = "µm";
        }


    }
}
