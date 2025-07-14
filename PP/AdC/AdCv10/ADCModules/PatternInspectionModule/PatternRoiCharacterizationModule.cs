using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;
using AdcTools.Collection;

using BasicModules;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

using PatternInspectionTools;

namespace PatternInspectionModule
{
    public class PatternRoiCharacterizationModule : ModuleBase, ICharacterizationModule
    {
        private List<Characteristic> _PatternRoiIdsCharaceristicList = new List<Characteristic>();
        public List<Characteristic> AvailableCharacteristics { get { return _PatternRoiIdsCharaceristicList; } }

        private MilImage _PatternRoiMask = null;
        private ClonePool<MaskHistoExecutor> PoolExec = null;
        private int _NbRoiMaskRegion = 0;

        //=================================================================
        // Constructeur
        //=================================================================
        public PatternRoiCharacterizationModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            _PatternRoiIdsCharaceristicList.Add(PatternRoiCharacterizationFactory.RoiID);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            List<ModuleBase> AncestorPatinspectmodule = FindAncestors(mod => mod is PatternInspectionModule);
            if (AncestorPatinspectmodule.Count == 0)
            {
                throw new ApplicationException("No Pattern inspection module has been set prior to this module");
            }

            PatternInspectionModule DirectAncestor = AncestorPatinspectmodule[0] as PatternInspectionModule;
            _NbRoiMaskRegion = DirectAncestor.NbRoimaskRegion;
            if (DirectAncestor.PatFileRoimask != null)
                _PatternRoiMask = DirectAncestor.PatFileRoimask.GetMilImage();
            else
                throw new ApplicationException("No Pattern Roi ID mask is set !");


            MaskHistoExecutor HistoExec = new MaskHistoExecutor(Mil.Instance.HostSystem, _NbRoiMaskRegion);
            if (HistoExec == null)
            {
                throw new ApplicationException("INIT : HistoExec == null");
            }

            PoolExec = new CloneDynamicPool<MaskHistoExecutor>(HistoExec, Scheduler.GetNbTasksPerPool());
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            Cluster cluster = (Cluster)obj;
            logDebug("Pattern ROI IDs Caracterization " + cluster);
            Interlocked.Increment(ref nbObjectsIn);
            sw.Restart();
            MaskHistoExecutor HistoExec = PoolExec.GetFirstAvailable();
            if (State != eModuleState.Aborting)
            {
                if (HistoExec == null)
                {
                    throw new Exception("PROCESS : HistoExec == null");
                }
            }

            if (State == eModuleState.Aborting || HistoExec == null)
            {
                if (HistoExec != null)
                    PoolExec.Release(HistoExec);
                logDebug("Pattern ROI IDs Caracterization " + obj + " ABORT ENDED");
                return;
            }

            MilImage milImage = cluster.ResultProcessingImage.GetMilImage(); // vignette die binary 

            Rectangle vignetteRect = cluster.imageRect.NegativeOffset(cluster.DieOffsetImage);
            MIL_ID MaskRoiChild = MIL.M_NULL;
            MIL.MbufChild2d(_PatternRoiMask.MilId, vignetteRect.Left, vignetteRect.Top, vignetteRect.Width, vignetteRect.Height, ref MaskRoiChild);
            Characteristic carac = _PatternRoiIdsCharaceristicList[0];

            CharacListID RoiIds = new CharacListID(HistoExec.FindROI_Ids(milImage.MilId, MaskRoiChild));

            MIL.MbufFree(MaskRoiChild);
            MaskRoiChild = MIL.M_NULL;

            cluster.characteristics[carac] = RoiIds;

            PoolExec.Release(HistoExec);

            sw.Stop();

            logDebug("Pattern ROI IDs Caracterization " + obj + " ENDED (done in " + sw.ElapsedMilliseconds + " ms)");
            ProcessChildren(obj);
        }

        //=================================================================
        //
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            if (_PatternRoiMask != null)
            {
                _PatternRoiMask.Dispose();
                _PatternRoiMask = null;
            }
            if (PoolExec != null)
                PoolExec.Dispose();

            base.OnStopping(oldState);
        }

        //=================================================================
        //
        //=================================================================
        public override void Abort()
        {
            //Log("Pattern ROI IDs Caracterization  ===> LAUNCH ABORTING !!");

            base.Abort();
            if (_PatternRoiMask != null)
            {
                _PatternRoiMask.Dispose();
                _PatternRoiMask = null;
            }
            if (PoolExec != null)
                PoolExec.Abort();
        }
    }

}
