using System;
using System.Collections.Generic;
using System.Threading;

using AcquisitionAdcExchange;

using ADCEngine;

using AdcRobotExchange;

using AdvancedModules.Edition.VID.GTVID;

using FormatGTR;

using GlobaltopoModule;

namespace AdvancedModules.Edition.VID
{
    public class GTVIDModule : ModuleBase
    {
        public readonly GTParameter paramGTWarp;
        public readonly GTParameter paramGTMaxPosWarp;
        public readonly GTParameter paramGTMaxNegWarp;
        public readonly GTParameter paramGTBowBF;
        public readonly GTParameter paramGTBowX;
        public readonly GTParameter paramGTBowY;
        public readonly GTParameter paramGTBowXY;
        public readonly GTParameter paramGTBowCenter;
        private DataGTR _dataGT;

        //=================================================================
        // Constructeur
        //=================================================================
        public GTVIDModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramGTWarp = new GTParameter(this, "WarpVID");
            paramGTMaxPosWarp = new GTParameter(this, "PositiveWarpVID");
            paramGTMaxNegWarp = new GTParameter(this, "NegativeWarpVID");
            paramGTBowBF = new GTParameter(this, "BowBFVID");
            paramGTBowX = new GTParameter(this, "BowXVID");
            paramGTBowY = new GTParameter(this, "BowYVID");
            paramGTBowXY = new GTParameter(this, "BowXYVID");
            paramGTBowCenter = new GTParameter(this, "BowCenterVID");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("cluster " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            //TODO : recup des données
            GTMeasure GTData = (GTMeasure)obj;
            _dataGT = GTData.Data;

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("GTVID",
                () =>
                {
                    try
                    {
                        ProcessGTVID();
                    }
                    catch (Exception ex)
                    {
                        string msg = "GT VID failed: " + ex.Message;
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
        private void ProcessGTVID()
        {
            List<VidBase> list = new List<VidBase>();

            VidBowWarpMeasure vidWarp = new VidBowWarpMeasure();
            SetVIDVariable(ref vidWarp, paramGTWarp, _dataGT.valueTWARPVID, _dataGT.m_fRadiusCenterBow_um);

            VidBowWarpMeasure vidBowBF = new VidBowWarpMeasure();
            SetVIDVariable(ref vidBowBF, paramGTBowBF, _dataGT.valueBOWBFVID, _dataGT.m_fRadiusCenterBow_um);

            VidBowWarpMeasure vidBowX = new VidBowWarpMeasure();
            SetVIDVariable(ref vidBowX, paramGTBowX, _dataGT.valueBOXVID, _dataGT.m_fRadiusCenterBow_um);

            VidBowWarpMeasure vidBowY = new VidBowWarpMeasure();
            SetVIDVariable(ref vidBowY, paramGTBowXY, _dataGT.valueBOYVID, _dataGT.m_fRadiusCenterBow_um);

            VidBowWarpMeasure vidBowXY = new VidBowWarpMeasure();
            SetVIDVariable(ref vidBowXY, paramGTBowXY, _dataGT.valueMAXBOWXYVID, _dataGT.m_fRadiusCenterBow_um);

            VidBowWarpMeasure vidBowCenter = new VidBowWarpMeasure();
            SetVIDVariable(ref vidBowY, paramGTBowCenter, _dataGT.valueBOCVID, _dataGT.m_fRadiusCenterBow_um);

            VidBowWarpMeasure vidWarpPos = new VidBowWarpMeasure();
            SetVIDVariable(ref vidWarpPos, paramGTMaxPosWarp, _dataGT.valueMAXPOSWARPVID, _dataGT.m_fRadiusCenterBow_um);

            VidBowWarpMeasure vidWarpNeg = new VidBowWarpMeasure();
            SetVIDVariable(ref vidWarpNeg, paramGTMaxNegWarp, _dataGT.valueMAXNEGWARPVID, _dataGT.m_fRadiusCenterBow_um);

            list.Add(vidWarp);
            list.Add(vidBowBF);
            list.Add(vidBowX);
            list.Add(vidBowY);
            list.Add(vidBowXY);
            list.Add(vidBowCenter);
            list.Add(vidWarpPos);
            list.Add(vidWarpNeg);

            ADC.Instance.TransferToRobotStub.TransferVids(Recipe.Toolname, $"{Recipe.Wafer.GetWaferInfo(eWaferInfo.JobID)}{Recipe.Wafer.Basename}", list);
        }

        private void SetVIDVariable(ref VidBowWarpMeasure Vid, GTParameter param, double valueMeasure, double CenterCoordinate)
        {
            Vid.VidNumber = param.VidNumber;
            Vid.VidLabel = param.VidLabel;
            Vid.Measure = valueMeasure;
            Vid.Xcoordinate = (int)CenterCoordinate;
            Vid.Ycoordinate = (int)CenterCoordinate;
            Vid.UnitValue = "µm";
        }
    }
}
