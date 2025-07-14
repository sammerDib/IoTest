using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;
using AdcTools.Collection;

using BasicModules;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.LibMIL;

namespace DiameterMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class DiameterMeasurementDieModule : ModuleBase, ICharacterizationModule, IClusterizerModule
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum ePolarity
        {
            [Description("Any")]
            Any = MIL.M_ANY,
            [Description("Black Bump")]
            Positive = MIL.M_POSITIVE,
            [Description("White Bump")]
            Negative = MIL.M_NEGATIVE
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public enum eStrengthScore
        {
            [Description("None")]
            None = 0,
            [Description("Flat")]
            Flat,
            [Description("Custom (Expert)")]
            Custom
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public enum eRadiusScore
        {
            [Description("None")]
            None = 0,
            [Description("Smallest circle")]
            Smallest,
            [Description("Biggest circle")]
            Biggest,
            [Description("Custom (Expert)")]
            Custom
        }

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
        public readonly FileParameter paramRDMFile;
        public readonly IntParameter paramMultiExecutorNumber;
        public readonly DoubleParameter paramSearchRadiusMin;
        public readonly DoubleParameter paramSearchRadiusMax;
        public readonly EnumParameter<ePolarity> paramSearchPolarity;
        public readonly ConditionalDoubleParameter paramSearchMaxAssociatDist;
        public readonly ConditionalDoubleParameter paramSearchEdgeThreshold; // min 0.0 - 100.0
        public readonly ConditionalIntParameter paramSearchNbSubRegion; // min 8 ou 4  
        public readonly EnumParameter<eRadiusScore> paramSearchRadiusScore;
        public readonly StringParameter ParamCustomRadiusScore;
        public readonly EnumParameter<eStrengthScore> paramSearchStrengthScore;
        public readonly StringParameter ParamCustomStrengthScore;

        private ClonePool<DMDieExecutor> PoolExec = null;

        //=================================================================
        // Constructeur
        //=================================================================
        public DiameterMeasurementDieModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ModuleProperty = eModuleProperty.Stage;

            // characteristic always compute
            supportedCharacteristics.TryAdd(ClusterCharacteristics.AbsolutePosition);

            paramRDMFile = new FileParameter(this, "RDMFile", "Diameter 2D measure recipe. Files (*.rdm)|*.rdm");

            paramMultiExecutorNumber = new IntParameter(this, "MultiExecutorNumber", 1, Scheduler.GetNbTasksPerPool());
            paramMultiExecutorNumber.Value = 1;

            paramSearchRadiusMin = new DoubleParameter(this, "SearchRadiusMin", 0.0);
            paramSearchRadiusMin.Value = 0.0;
            paramSearchRadiusMax = new DoubleParameter(this, "SearchRadiusMax", 0.0);
            paramSearchRadiusMax.Value = 5.0;

            paramSearchPolarity = new EnumParameter<ePolarity>(this, "SearchPolarity");
            paramSearchPolarity.Value = ePolarity.Any;

            paramSearchNbSubRegion = new ConditionalIntParameter(this, "SearchNbSubRegion", 8, 360 * 2);
            paramSearchNbSubRegion.IsUsed = false;
            paramSearchNbSubRegion.Value = 8;

            paramSearchMaxAssociatDist = new ConditionalDoubleParameter(this, "SearchMaxAssociateDist");
            paramSearchMaxAssociatDist.IsUsed = false;
            paramSearchMaxAssociatDist.Value = 2.0;

            paramSearchEdgeThreshold = new ConditionalDoubleParameter(this, "SearchEdgeThreshold", 0.0, 100.0);
            paramSearchEdgeThreshold.IsUsed = false;
            paramSearchEdgeThreshold.Value = 2.0;

            paramSearchRadiusScore = new EnumParameter<eRadiusScore>(this, "SearchRadiusScore");
            paramSearchRadiusScore.ValueChanged +=
             (score) =>
             {
                 ParamCustomRadiusScore.IsEnabled = (score == eRadiusScore.Custom);
             };
            ParamCustomRadiusScore = new StringParameter(this, "CustomRadiusScoreParams");
            ParamCustomRadiusScore.IsEnabled = false;

            paramSearchStrengthScore = new EnumParameter<eStrengthScore>(this, "SearchStrengthScore");
            paramSearchStrengthScore.ValueChanged +=
             (score) =>
             {
                 ParamCustomStrengthScore.IsEnabled = (score == eStrengthScore.Custom);
             };
            ParamCustomStrengthScore = new StringParameter(this, "CustomStrengthScoreParams");
            ParamCustomStrengthScore.IsEnabled = false;

            supportedCharacteristics.TryAdd(Cluster2DCharacteristics.DiameterAverage);
            supportedCharacteristics.TryAdd(Cluster2DCharacteristics.OffsetAverage);
        }


        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string error = base.Validate();
            if (error != null)
                return error;

            if (paramSearchRadiusMin.Value >= paramSearchRadiusMax.Value)
                return "Search Min radius should be greater tha Max radius";

            if (paramMultiExecutorNumber < 1 || paramMultiExecutorNumber > Scheduler.GetNbTasksPerPool())
                return "Wrong Number of executors";

            if (paramSearchRadiusScore.Value == eRadiusScore.Custom)
            {
                String sMessage;
                ParamCustomRadiusScore.Value = ParamCustomRadiusScore.Value.Trim();
                if (!ValidCustomScore(ParamCustomRadiusScore.Value, "Radius", out sMessage))
                    return sMessage;
            }

            if (paramSearchStrengthScore.Value == eStrengthScore.Custom)
            {
                String sMessage;
                ParamCustomStrengthScore.Value = ParamCustomStrengthScore.Value.Trim();
                if (!ValidCustomScore(ParamCustomStrengthScore.Value, "Strength", out sMessage))
                    return sMessage;
            }

            return null;
        }

        static public bool ValidCustomScore(String sCustom, String slabel, out String sMessage)
        {
            if (String.IsNullOrEmpty(sCustom))
            {
                sMessage = String.Format("Empty Custom Radius Score", slabel);
                return false;
            }

            String[] sArr = sCustom.Split(new char[] { ';' });
            if (sArr.Length < 4 || sArr.Length > 5)
            {
                sMessage = String.Format("Custom {0} Score - Bad format - expected : min(0.00);low(0.00);hi(0.00);Max(0.00);[ScoreOffset(0.00)] MPV", slabel);
                return false;
            }

            DMDieExecutor.ScoreParams prm = ParseCustomScore(sCustom, out sMessage);
            return (prm != null);
        }

        static public DMDieExecutor.ScoreParams ParseCustomScore(String sCustom, out string serror)
        {
            String[] sArr = sCustom.Split(new char[] { ';' });
            if (sArr.Length < 4 || sArr.Length > 5)
            {
                serror = "Bad Number of separator \';\'";
                return null;
            }
            serror = "";
            double dmin, dlow, dhi, dmax;
            if (!Double.TryParse(sArr[0], out dmin))
            {
                if (sArr[0].ToLower().Trim() == "mpv")
                    dmin = MIL.M_MAX_POSSIBLE_VALUE;
                else
                    serror += "min ";
            }
            if (!Double.TryParse(sArr[1], out dlow))
            {
                if (sArr[1].ToLower().Trim() == "mpv")
                    dlow = MIL.M_MAX_POSSIBLE_VALUE;
                else
                    serror += "low ";
            }
            if (!Double.TryParse(sArr[2], out dhi))
            {
                if (sArr[2].ToLower().Trim() == "mpv")
                    dhi = MIL.M_MAX_POSSIBLE_VALUE;
                else
                    serror += "hi ";
            }
            if (!Double.TryParse(sArr[3], out dmax))
            {
                if (sArr[3].ToLower().Trim() == "mpv")
                    dhi = MIL.M_MAX_POSSIBLE_VALUE;
                else
                    serror += "max ";
            }

            if (serror.Length > 0)
            {
                serror = "Cannot parse : <" + serror + ">";
                return null;
            }

            DMDieExecutor.ScoreParams prm = new DMDieExecutor.ScoreParams(dmin, dlow, dhi, dmax);
            if (sArr.Length == 5)
            {
                double dscoroff = 0.0;
                if (!Double.TryParse(sArr[4], out dscoroff))
                {
                    serror = "Cannot parse score offset";
                }
                else
                    prm.dScoreOffset = dscoroff;
            }
            return prm;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            DMDieExecutor DMExec = new DMDieExecutor(Mil.Instance.HostSystem);
            if (DMExec == null)
            {
                throw new ApplicationException("INIT : DMExec == null");
            }

            DMDieExecutor.DMExecParams ExcPrm = new DMDieExecutor.DMExecParams(paramSearchRadiusMin.Value, paramSearchRadiusMax.Value);
            ExcPrm.dpolarity = (double)paramSearchPolarity.Value;
            if (paramSearchNbSubRegion.IsUsed)
                ExcPrm.dNbSubRegion = (double)paramSearchNbSubRegion.Value;
            if (paramSearchMaxAssociatDist.IsUsed)
                ExcPrm.dMaxAssocDist_px = (double)paramSearchMaxAssociatDist.Value;
            if (paramSearchEdgeThreshold.IsUsed)
                ExcPrm.dEdgeThreshold = (double)paramSearchEdgeThreshold.Value;

            if (paramSearchRadiusScore.Value != eRadiusScore.None)
            {
                string serr;
                DMDieExecutor.ScoreParams scoreprm = null;
                switch (paramSearchRadiusScore.Value)
                {
                    case eRadiusScore.Smallest:
                        scoreprm = new DMDieExecutor.ScoreParams(0.0, 0.0, 0.0, MIL.M_MAX_POSSIBLE_VALUE);
                        break;
                    case eRadiusScore.Biggest:
                        scoreprm = new DMDieExecutor.ScoreParams(0.0, MIL.M_MAX_POSSIBLE_VALUE, MIL.M_MAX_POSSIBLE_VALUE, MIL.M_MAX_POSSIBLE_VALUE);
                        break;
                    case eRadiusScore.Custom:
                        scoreprm = ParseCustomScore(ParamCustomRadiusScore.Value, out serr);
                        break;
                    default:
                        break;
                }
                ExcPrm.RadiusScore = scoreprm;

            }
            if (paramSearchStrengthScore.Value != eStrengthScore.None)
            {
                string serr;
                DMDieExecutor.ScoreParams scoreprm = null;
                switch (paramSearchStrengthScore.Value)
                {
                    case eStrengthScore.Flat:
                        scoreprm = new DMDieExecutor.ScoreParams(0.0, 0.0, MIL.M_MAX_POSSIBLE_VALUE, MIL.M_MAX_POSSIBLE_VALUE);
                        break;
                    case eStrengthScore.Custom:
                        scoreprm = ParseCustomScore(ParamCustomStrengthScore.Value, out serr);
                        break;
                    default:
                        break;
                }
                ExcPrm.StrengthScore = scoreprm;
            }

            if (!DMExec.LoadFromFile(paramRDMFile.FullFilePath, ExcPrm))
            {
                throw new ApplicationException(String.Format("INIT : Could not load 2D measurement recipe File <{0}>", paramRDMFile.FullFilePath));
            }

            //PoolExec = new ClonePool<HMDieExecutor>(HMExec, paramMultiExecutorNumber.Value);
            PoolExec = new CloneDynamicPool<DMDieExecutor>(DMExec, paramMultiExecutorNumber.Value);

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
            logDebug("Die 2D Diameter measure " + obj);
            ImageBase image = (ImageBase)obj;
            MatrixBase matrix = image.Layer.Matrix;
            Interlocked.Increment(ref nbObjectsIn);


            DMDieExecutor DMExec = PoolExec.GetFirstAvailable();
            if (State != eModuleState.Aborting)
            {
                if (DMExec == null)
                {
                    throw new ApplicationException("PROCESS : 2d diameter measurement die Executor == null");
                }
            }


            if (State == eModuleState.Aborting || DMExec == null)
            {
                if (DMExec != null)
                    PoolExec.Release(DMExec);
                logDebug("DMDie " + obj + " ABORT ENDED");
                return;
            }


            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Restart();

            int num = CreateClusterNumber(image, 0);
            //logDebug(String.Format("###  clusterDieHM : ~~~~~~~~~~ {0}+{1}*({2}+{3}*0) = {4}", layer.Index, Recipe.NbLayers, image.ImageIndex, layer.MaxDataIndex, idx));

            Cluster2DDieDM cluster = new Cluster2DDieDM(num, image);
            bool bSuccess = DMExec.Measure(ref cluster);
            PoolExec.Release(DMExec);

            cluster.characteristics[ClusterCharacteristics.AbsolutePosition] = cluster.micronQuad.SurroundingRectangle;

            //sw.Stop();
            //logDebug("HMDie " + obj + " ~~~~~~~~~~ after wake up Done <" + cluster.Name  + "> in " + sw.ElapsedMilliseconds + " ms");

            if (bSuccess)
                ProcessChildren(cluster);
            else if (State != eModuleState.Aborting)
                logError("DMDie Measure fail for " + obj);

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
