using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;
using AdcTools.Collection;

using BasicModules;

using UnitySC.Shared.LibMIL;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class HeightMeasurementDieModule : ModuleBase, ICharacterizationModule, IClusterizerModule
    {

        private List<Characteristic> supportedCharacteristics = new List<Characteristic>();
        public List<Characteristic> AvailableCharacteristics { get { return supportedCharacteristics; } }

        /// <summary>
        /// Index du module de clusteurisation pour générer des numéros de cluster uniques
        /// </summary>
        protected int ClusterizerIndex;
        /// <summary>
        /// Nombre de modules de clusteurisation, pour générer des numéros de cluster uniques
        /// </summary>
        protected int NbClusterizers;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly FileParameter paramRHMFile;
        public readonly BoolParameter paramUseHeightMap;
        public readonly BoolParameter paramUseCoplanarityMap;
        public readonly BoolParameter paramUseSubstrateCoplanarity;
        public readonly IntParameter paramMultiExecutorNumber;

        private ClonePool<HMDieExecutor> PoolExec = null;

        //=================================================================
        // Constructeur
        //=================================================================
        public HeightMeasurementDieModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ModuleProperty = eModuleProperty.Stage;

            // characteristic always compute
            supportedCharacteristics.TryAdd(ClusterCharacteristics.AbsolutePosition);

            paramRHMFile = new FileParameter(this, "RHMFile", "Height measure recipe. Files (*.rhm)|*.rhm");

            paramUseHeightMap = new BoolParameter(this, "UseHeightMap");
            paramUseHeightMap.ValueChanged +=
                (use) =>
                {
                    if (use)
                    {
                        supportedCharacteristics.TryAdd(Cluster3DCharacteristics.HeightAverage);
                        supportedCharacteristics.TryAdd(Cluster3DCharacteristics.HeightStdDev);
                        supportedCharacteristics.TryAdd(Cluster3DCharacteristics.HeightMin);
                        supportedCharacteristics.TryAdd(Cluster3DCharacteristics.HeightMax);
                    }
                    else
                    {
                        supportedCharacteristics.Remove(Cluster3DCharacteristics.HeightAverage);
                        supportedCharacteristics.Remove(Cluster3DCharacteristics.HeightStdDev);
                        supportedCharacteristics.Remove(Cluster3DCharacteristics.HeightMin);
                        supportedCharacteristics.Remove(Cluster3DCharacteristics.HeightMax);
                    }
                };
            paramUseHeightMap.Value = true;

            paramUseCoplanarityMap = new BoolParameter(this, "UseCoplaMap");
            paramUseCoplanarityMap.ValueChanged +=
                (use) =>
                {
                    if (use)
                    {
                        supportedCharacteristics.TryAdd(Cluster3DCharacteristics.Coplanarity);
                    }
                    else
                    {
                        supportedCharacteristics.Remove(Cluster3DCharacteristics.Coplanarity);
                    }
                };
            paramUseCoplanarityMap.Value = false;

            paramUseSubstrateCoplanarity = new BoolParameter(this, "UseSubstrateCoplaMap");
            paramUseSubstrateCoplanarity.ValueChanged +=
                (use) =>
                {
                    if (use)
                    {
                        supportedCharacteristics.TryAdd(Cluster3DCharacteristics.SubstrateCoplanarity);
                    }
                    else
                    {
                        supportedCharacteristics.Remove(Cluster3DCharacteristics.SubstrateCoplanarity);
                    }
                };
            paramUseSubstrateCoplanarity.Value = false;

            paramMultiExecutorNumber = new IntParameter(this, "MultiExecutorNumber", 1, Scheduler.GetNbTasksPerPool());
            paramMultiExecutorNumber.Value = 1;
        }


        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string error = base.Validate();
            if (error != null)
                return error;

            if (!paramUseHeightMap.Value && !paramUseCoplanarityMap.Value && !paramUseSubstrateCoplanarity)
                return "No measures computation has been set";

            if (paramMultiExecutorNumber < 1 || paramMultiExecutorNumber > Scheduler.GetNbTasksPerPool())
                return "Wrong Number of executors";

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            HMDieExecutor HMExec = new HMDieExecutor(Mil.Instance.HostSystem);
            if (HMExec == null)
            {
                throw new ApplicationException("INIT : HMExec == null");
            }

            if (!HMExec.LoadFromFile(paramRHMFile.FullFilePath))
            {
                throw new ApplicationException(String.Format("INIT : Could not load Height measurement recipe File <{0}>", paramRHMFile.FullFilePath));
            }

            //PoolExec = new ClonePool<HMDieExecutor>(HMExec, paramMultiExecutorNumber.Value);
            PoolExec = new CloneDynamicPool<HMDieExecutor>(HMExec, paramMultiExecutorNumber.Value);

            var list = Recipe.ModuleList.Select(kvp => kvp.Value).OfType<IClusterizerModule>().ToList();
            ClusterizerIndex = list.IndexOf(this);
            NbClusterizers = list.Count();
        }


        //=================================================================
        // 
        //================================================================= 
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            if (State == eModuleState.Aborting)
                return;

            logDebug("Die Height measure " + obj);
            ImageBase image = (ImageBase)obj;
            MatrixBase matrix = image.Layer.Matrix;
            Interlocked.Increment(ref nbObjectsIn);


            HMDieExecutor HMExec = PoolExec.GetFirstAvailable();
            if (State != eModuleState.Aborting)
            {
                if (HMExec == null)
                    throw new ApplicationException("PROCESS : Height measurement die Executor == null");
            }


            if (State == eModuleState.Aborting || HMExec == null)
            {
                if (HMExec != null)
                    PoolExec.Release(HMExec);
                logDebug("HMDie " + obj + " ABORT ENDED");
                return;
            }


            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();

            int num = CreateClusterNumber(image, 0);
            //logDebug(String.Format("###  clusterDieHM : ~~~~~~~~~~ {0}+{1}*({2}+{3}*0) = {4}", layer.Index, Recipe.NbLayers, image.ImageIndex, layer.MaxDataIndex, idx));


            Cluster3DDieHM cluster = new Cluster3DDieHM(num, image);
            bool bSuccess = HMExec.Measure(ref cluster, paramUseHeightMap.Value, paramUseCoplanarityMap.Value, paramUseSubstrateCoplanarity.Value);
            PoolExec.Release(HMExec);

            cluster.characteristics[ClusterCharacteristics.AbsolutePosition] = cluster.micronQuad.SurroundingRectangle;

            //sw.Stop();
            //logDebug("HMDie " + obj + " ~~~~~~~~~~ after wake up Done <" + cluster.Name  + "> in " + sw.ElapsedMilliseconds + " ms");

            if (bSuccess)
                ProcessChildren(cluster);
            else if (State != eModuleState.Aborting)
                logError("HMDie Measure fail for " + obj);

            cluster.DelRef();
        }

        //=================================================================
        //
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            if (PoolExec != null)
                PoolExec.Dispose();

            base.OnStopping(oldState);
        }

        //=================================================================
        //
        //=================================================================
        public override void Abort()
        {
            base.Abort();
            if (PoolExec != null)
                PoolExec.Abort();
        }

        /// <summary>
        /// Crée un numéro de cluster unique
        /// </summary>
        /// <param name="index">index du cluster dans l'image</param>
        protected int CreateClusterNumber(ImageBase image, int index)
        {
            int number = ClusterizerIndex + NbClusterizers * (image.ImageIndex + image.Layer.MaxDataIndex * index);
            return number;
        }


    }
}
