using System;
using System.Collections.Generic;
using System.Threading;

using AcquisitionAdcExchange;

using ADCEngine;

using AdcRobotExchange;

using CrownMeasurementModule.Objects;

using static CrownMeasurementModule.Objects.ProfileMeasure;

namespace AdvancedModules.Edition.VID
{
    public class CrownVIDModule : ModuleBase
    {
        public readonly CrownParameter paramEBR;
        public readonly CrownParameter paramTKR;
        public readonly CrownParameter paramTKS;
        public readonly CrownParameter paramGD;
        public readonly CrownParameter paramSCR;
        private double _AverageEBR = 0;
        private double _AverageTKR = 0;
        private double _AverageTKS = 0;
        private double _AverageGD = 0;
        private double _AverageSCR = 0;

        //=================================================================
        // Constructeur
        //=================================================================
        public CrownVIDModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramEBR = new CrownParameter(this, "CrownVIDEBR");
            paramTKR = new CrownParameter(this, "CrownVIDTKR");
            paramTKS = new CrownParameter(this, "CrownVIDTKS");
            paramGD = new CrownParameter(this, "CrownVIDGD");
            paramSCR = new CrownParameter(this, "CrownVIDSCR");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("cluster " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            // TODO : Recup des données
            ProfileMeasure profile = (ProfileMeasure)obj;


            int icount = 0;
            foreach (CrownProfileModule_ProfileStat profileStat in profile._profile)
            {
                _AverageEBR += profileStat.valueCrownSizeAverageEBR;
                _AverageGD += profileStat.valueCrownSizeAverageGD;
                _AverageSCR += profileStat.valueCrownSizeAverageEdg;
                _AverageTKR += profileStat.valueRingSizeAverageTaiko;
                _AverageTKS += profileStat.valueShoulderSizeAverageTaiko;
                icount++;
            }
            if (icount != 0)
            {
                _AverageEBR /= icount;
                _AverageGD /= icount;
                _AverageSCR /= icount;
                _AverageTKR /= icount;
                _AverageTKS /= icount;
            }

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("CrownVID",
                () =>
                {
                    try
                    {
                        ProcessCrownVID();
                    }
                    catch (Exception ex)
                    {
                        string msg = "CrownVID failed: " + ex.Message;
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
        private void ProcessCrownVID()
        {
            List<VidBase> list = new List<VidBase>();

            VidEdge EBR = new VidEdge();
            EBR.VidNumber = paramEBR.VidNumber;
            EBR.VidLabel = paramEBR.VidLabel;
            EBR.Name = "VIDEBR";
            EBR.Measure = _AverageEBR;
            EBR.UnitValue = "µm";

            VidEdge TKR = new VidEdge();
            TKR.VidNumber = paramTKR.VidNumber;
            TKR.VidLabel = paramTKR.VidLabel;
            TKR.Name = "VIDTKR";
            TKR.Measure = _AverageTKR;
            TKR.UnitValue = "µm";

            VidEdge TKS = new VidEdge();
            TKS.VidNumber = paramTKS.VidNumber;
            TKS.VidLabel = paramTKS.VidLabel;
            TKS.Name = "VIDTKS";
            TKS.Measure = _AverageTKS;
            TKS.UnitValue = "µm";

            VidEdge GD = new VidEdge();
            GD.VidNumber = paramGD.VidNumber;
            GD.VidLabel = paramGD.VidLabel;
            GD.Name = "VIDGD";
            GD.Measure = _AverageGD;
            GD.UnitValue = "µm";

            VidEdge SCR = new VidEdge();
            SCR.VidNumber = paramSCR.VidNumber;
            SCR.VidLabel = paramSCR.VidLabel;
            SCR.Name = "VIDSCR";
            SCR.Measure = _AverageSCR;
            SCR.UnitValue = "µm";

            list.Add(EBR);
            list.Add(SCR);
            list.Add(TKR);
            list.Add(TKS);
            list.Add(GD);

            ADC.Instance.TransferToRobotStub.TransferVids(Recipe.Toolname, $"{Recipe.Wafer.GetWaferInfo(eWaferInfo.JobID)}{Recipe.Wafer.Basename}", list);
        }
    }
}
