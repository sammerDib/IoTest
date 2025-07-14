using System;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools.Collection;

using BasicModules;

using UnitySC.Shared.LibMIL;

using LibProcessing;

using Matrox.MatroxImagingLibrary;

using PatternInspectionTools;

namespace PatternInspectionModule
{
    public class PatternInspectionModule : ImageModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================

        public readonly FileParameter paramPatFile;
        public readonly IntParameter paramMultiPatExecutorNb;

        private ClonePool<PatternExecutor> PoolExec = null;

        private ProcessingImage _PatFileRoimask = null;
        public ProcessingImage PatFileRoimask
        {
            get { return _PatFileRoimask; }
        }
        public int NbRoimaskRegion { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public PatternInspectionModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramPatFile = new FileParameter(this, "File", "Pattern Inspect. Files (*.pat)|*.pat");

            paramMultiPatExecutorNb = new IntParameter(this, "MultiExecutorNb", 1, Scheduler.GetNbTasksPerPool());
            paramMultiPatExecutorNb.Value = 1;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            PatternExecutor PatExec = new PatternExecutor(Mil.Instance.HostSystem);
            if (PatExec == null)
            {
                throw new Exception("INIT : PatExec == null");
            }

            string sErr = "";
            if (!PatExec.LoadFromPatFile(paramPatFile.FullFilePath, out sErr))
            {
                throw new Exception(String.Format("INIT : Could not load Pat File <{0}> : {1}", paramPatFile.FullFilePath, sErr));
            }

            _PatFileRoimask = new ProcessingImage();
            MIL_ID roiid = PatExec.GetRoiMask();
            if (roiid != MIL.M_NULL)
            {
                // Transfert de propriété du buffer MIL
                using (MilImage milImage = new MilImage(roiid, transferOnwership: true))
                {
                    _PatFileRoimask.SetMilImage(milImage);
                    roiid = MIL.M_NULL;
                    PatExec.ForgetRoiMask();
                }
            }
            NbRoimaskRegion = PatExec.GetNbRoiRegion();

            //PoolExec = new ClonePool<PatternExecutor>(PatExec, paramMultiPatExecutorNb.Value);
            PoolExec = new CloneDynamicPool<PatternExecutor>(PatExec, paramMultiPatExecutorNb.Value);
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            ImageBase image = (ImageBase)obj;
            sw.Restart();

            PatternExecutor PatExec = PoolExec.GetFirstAvailable();
            if (State != eModuleState.Aborting && PatExec == null)
                throw new ApplicationException("PROCESS : PatExec == null");

            if (State == eModuleState.Aborting || PatExec == null || Recipe.PartialAnalysis)
            {
                if (PatExec != null)
                    PoolExec.Release(PatExec);
                logDebug("PatternInspection " + obj + " skipped");
                return;
            }

            MilImage DieSrcImg = image.CurrentProcessingImage.GetMilImage();
            bool bSuccess = PatExec.Inspect(DieSrcImg.MilId);
            PoolExec.Release(PatExec);

            sw.Stop();
            string status = bSuccess ? "done" : (State == eModuleState.Aborting) ? "aborted" : "failed";
            logDebug($"PatternInspection {obj} : {status} in {sw.ElapsedMilliseconds} ms");

            ProcessChildren(obj);
        }

        //=================================================================
        //
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            if (_PatFileRoimask != null)
            {
                _PatFileRoimask.Dispose();
                _PatFileRoimask = null;
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
            //logDebug("PatternInspection  ===> LAUNCH ABORTING !!");

            base.Abort();
            if (_PatFileRoimask != null)
            {
                _PatFileRoimask.Dispose();
                _PatFileRoimask = null;
            }
            if (PoolExec != null)
                PoolExec.Abort();
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string error = base.Validate();
            if (error != null)
                return error;

            if (paramMultiPatExecutorNb < 1 || paramMultiPatExecutorNb > Scheduler.GetNbTasksPerPool())
                return "Wrong Number of executors";

            return null;
        }
    }

}
