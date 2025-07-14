using System.Collections.Generic;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

using MergeContext.Context;

namespace Threshold3DModule
{
    public class Characterisation3DModule : ModuleBase, ICharacterizationModule
    {

        private List<Characteristic> supportedCharacteristics = new List<Characteristic>();
        public List<Characteristic> AvailableCharacteristics { get { return supportedCharacteristics; } }
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly BoolParameter paramUseAbsoluteHeight;
        public readonly BoolParameter paramUseHeightStdDev;
        public readonly BoolParameter paramUseNaNsCount;


        //=================================================================
        // Constructeur
        //=================================================================
        public Characterisation3DModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ModuleProperty = eModuleProperty.Stage;


            paramUseAbsoluteHeight = new BoolParameter(this, "UseAbsoluteHeight");

            // characteristic always compute
            supportedCharacteristics.TryAdd(Cluster3DCharacteristics.BareHeightAverage);
            supportedCharacteristics.TryAdd(Cluster3DCharacteristics.BareHeightMax);
            supportedCharacteristics.TryAdd(Cluster3DCharacteristics.BareHeightMin);

            paramUseHeightStdDev = new BoolParameter(this, "UseHeightStdDev");
            paramUseHeightStdDev.ValueChanged +=
                (use) =>
                {
                    if (use) supportedCharacteristics.TryAdd(Cluster3DCharacteristics.BareHeightStdDev);
                    else supportedCharacteristics.Remove(Cluster3DCharacteristics.BareHeightStdDev);
                };

            paramUseNaNsCount = new BoolParameter(this, "UseNaNsCount");
            paramUseNaNsCount.ValueChanged +=
                (use) =>
                {
                    if (use) supportedCharacteristics.TryAdd(Cluster3DCharacteristics.NaNsCount);
                    else supportedCharacteristics.Remove(Cluster3DCharacteristics.NaNsCount);
                };
        }

        //=================================================================
        // 
        //=================================================================

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            Cluster cluster = (Cluster)obj;
            logDebug("Caracterization 3D " + cluster);
            Interlocked.Increment(ref nbObjectsIn);

            ComputeClusterCharacteristics(cluster);

            ProcessChildren(obj);
        }
        //=================================================================
        // Calcul des caracteristiques du cluster en "sommant" les 
        // caracteristiques des blobs.
        //=================================================================
        protected void ComputeClusterCharacteristics(Cluster cluster)
        {
            float fBackgroundHeight3D = 0.0f;
            if (!paramUseAbsoluteHeight.Value)
            {
                WaferPosition waferPosition = cluster.Layer.GetContextMachine<WaferPosition>(ConfigurationType.WaferPosition.ToString());
                if (waferPosition != null)
                {
                    fBackgroundHeight3D = waferPosition.Background3DValue;
                }
                else
                {
                    logDebug("Background3DValue is not defined in the current context machine");
                }
            }

            //-------------------------------------------------------------
            // Calcul des caractérisques
            //-------------------------------------------------------------
            var ClusterStatTypesList = MilTo.StatList(MIL.M_STAT_MEAN, MIL.M_STAT_MAX, MIL.M_STAT_MIN);

            if (paramUseHeightStdDev.Value == true)
                ClusterStatTypesList.Add(MIL.M_STAT_STANDARD_DEVIATION);

            ComputeClusterStat(cluster, ClusterStatTypesList, paramUseNaNsCount.Value, (double)fBackgroundHeight3D);
        }

        private void ComputeClusterStat(Cluster cluster, List<MIL_INT> StatTypes, bool bCountNans, double backgroundHeight3D)
        {
            using (var stat = new MilImageResult())
            {
                using (MilImage NanMask = new MilImage())
                {
          
                    MilImage milImage = cluster.OriginalProcessingImage.GetMilImage(); // Buffer 3D
                    int depthIN = milImage.SizeBit; // Should be == 32

                    stat.AllocResult(milImage.OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);
                    // pour eviter de comptabiliser les NaNS
                    //stat.Calculate(milImage, StatTypes, MIL.M_IN_RANGE, float.MinValue, float.MaxValue);

                    NanMask.Alloc2dCompatibleWith(milImage);
                    MilImage.Binarize(milImage, NanMask, MIL.M_FIXED + MIL.M_IN_RANGE, float.MinValue, float.MaxValue);
                                        
                    stat.Stat(milImage, StatTypes, MIL.M_IN_RANGE, float.MinValue, float.MaxValue);
                    
                    NanMask.Arith(1.0, MIL.M_SUB_CONST);
                    NanMask.Arith(MIL.M_NULL, MIL.M_ABS);

                    if (StatTypes.Contains(MIL.M_STAT_MIN))
                        cluster.characteristics[Cluster3DCharacteristics.BareHeightMin] = stat.GetResult(MIL.M_STAT_MIN) - backgroundHeight3D;
                    if (StatTypes.Contains(MIL.M_STAT_MAX))
                        cluster.characteristics[Cluster3DCharacteristics.BareHeightMax] = stat.GetResult(MIL.M_STAT_MAX) - backgroundHeight3D;
                    if (StatTypes.Contains(MIL.M_STAT_MEAN))
                        cluster.characteristics[Cluster3DCharacteristics.BareHeightAverage] = stat.GetResult(MIL.M_STAT_MEAN) - backgroundHeight3D;
                    if (StatTypes.Contains(MIL.M_STAT_STANDARD_DEVIATION))
                        cluster.characteristics[Cluster3DCharacteristics.BareHeightStdDev] = stat.GetResult(MIL.M_STAT_STANDARD_DEVIATION);

                    if (bCountNans == true)
                    {
                        //NanMask.Alloc2dCompatibleWith(milImage);
                        // NaNs Values will be set to 1.0 and 0.0 to pixels with reliable height measures
                        //MilImage.Binarize(milImage, NanMask, MIL.M_FIXED + MIL.M_OUT_RANGE, float.MinValue, float.MaxValue);

                        stat.Stat(NanMask, MIL.M_STAT_SUM);
                        cluster.characteristics[Cluster3DCharacteristics.NaNsCount] = stat.GetResult(MIL.M_STAT_SUM);
                    }
                }
            }
        }
    }
}
