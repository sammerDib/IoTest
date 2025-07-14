using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using ADCEngine;

using BasicModules.Edition.KlarfEditor;
using BasicModules.KlarfEditor;

using UnitySC.Shared.Tools;

namespace BasicModules.Edition.MicroscopeReview
{
    public class MicroscopeReviewModule : ModuleBase
    {
        private const string _reviewExtension = ".rvw";
        private PathString _reviewFilePath;
        public Dictionary<string, List<KlarfDefect>> _reviewKlarfDefects;  // <className, KlarfDefect>

        //private List<string> _classesToReview;
        public MicroscopeReviewParameter Parameter { get; set; }


        public MicroscopeReviewModule(IModuleFactory factory, int id, Recipe recipe) : base(factory, id, recipe)
        {
            Parameter = new MicroscopeReviewParameter(this, "Review");
        }

        protected override void OnInit()
        {
            base.OnInit();

            KlarfEditorModule klarfEditorModuleParentModule = FindAncestors(mod => mod is KlarfEditorModule).OfType<KlarfEditorModule>().FirstOrDefault();
            if (klarfEditorModuleParentModule == null)
                throw new InvalidOperationException("Parent klarf module is missing");

            _reviewFilePath = Path.ChangeExtension(klarfEditorModuleParentModule.KlarfFilename, _reviewExtension);
            _reviewKlarfDefects = new Dictionary<string, List<KlarfDefect>>();

            foreach (string className in Parameter.MicroscopeReviewClassses.Keys)
            {
                _reviewKlarfDefects.Add(className, new List<KlarfDefect>());
            }
        }

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            Interlocked.Increment(ref nbObjectsIn);
            KlarfCluster klarfCluster = obj as KlarfCluster;
            if (Parameter.MicroscopeReviewClassses[klarfCluster.ClassName].UseReview)
            {
                _reviewKlarfDefects[klarfCluster.ClassName].AddRange(klarfCluster.KlarfDefects);
            }
        }

        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("starting processing task");

            Scheduler.StartSingleTask("ProcessReview", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                        ProcessReview();
                    else if (oldState == eModuleState.Aborting)
                        PurgeReview();
                    else
                        throw new ApplicationException("invalid state" + oldState);
                }
                catch (Exception ex)
                {
                    string msg = "Review generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    base.OnStopping(oldState);
                }
            });
        }

        /// <summary>
        /// Creating review file .rvw
        /// </summary>
        private void ProcessReview()
        {
            log("Creating review: " + _reviewFilePath);
            ApplyFilter();
            using (StreamWriter outputRvw = new StreamWriter(_reviewFilePath, false))
            {
                outputRvw.WriteLine("DefectNumber\tClusterNumber\tArea\tPosX\tPosY");
                foreach (string className in _reviewKlarfDefects.Keys)
                {
                    foreach (KlarfDefect klarfDefect in _reviewKlarfDefects[className])
                    {
                        outputRvw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t",
                            klarfDefect.DefectNumber,
                            klarfDefect.ClusterNumber,
                            klarfDefect.Area,
                            klarfDefect.PosX,
                            klarfDefect.PosY));
                        Interlocked.Increment(ref nbObjectsOut);
                    }
                }
            }
        }

        private void PurgeReview()
        {
            logDebug("review generation aborted");
            _reviewKlarfDefects.Clear();
        }


        private void ApplyFilter()
        {
            foreach (string className in _reviewKlarfDefects.Keys.ToList())
            {
                StrategyType strategy = Parameter.MicroscopeReviewClassses[className].Strategy;
                int nbSamples = Parameter.MicroscopeReviewClassses[className].NbSamples;
                List<KlarfDefect> defects = _reviewKlarfDefects[className];
                List<KlarfDefect> sortedDefect;

                // Trie
                switch (strategy)
                {
                    case StrategyType.All:
                    case StrategyType.First:
                    case StrategyType.Random:
                        sortedDefect = defects.OrderBy(x => x.DefectNumber).ToList();
                        break;
                    case StrategyType.Biggest:
                        sortedDefect = defects.OrderByDescending(x => x.Area).ToList();
                        break;
                    case StrategyType.Last:
                        sortedDefect = defects.OrderBy(x => x.DefectNumber).ToList();
                        sortedDefect.Reverse();
                        break;
                    case StrategyType.Smallest:
                    case StrategyType.SizeSampling:
                        sortedDefect = defects.OrderBy(x => x.Area).ToList();
                        break;
                    default:
                        throw new InvalidOperationException("Unknow StrategyType");
                }


                // On prend le nombre d'echantilon souhaité 
                if (nbSamples < sortedDefect.Count())
                {
                    switch (strategy)
                    {
                        case StrategyType.All:
                            _reviewKlarfDefects[className] = sortedDefect;
                            break;
                        case StrategyType.First:
                        case StrategyType.Last:
                        case StrategyType.Biggest:
                        case StrategyType.Smallest:
                            _reviewKlarfDefects[className] = sortedDefect.Take(nbSamples).ToList();
                            break;
                        case StrategyType.Random:
                            Random random = new Random();
                            List<KlarfDefect> randoms = new List<KlarfDefect>();
                            List<int> ValueRoll_List = new List<int>();
                            for (int i = 0; i < Math.Min(nbSamples, sortedDefect.Count); i++)
                            {
                                int ValueRoll = random.Next(0, sortedDefect.Count);
                                if (!ValueRoll_List.Contains(ValueRoll))
                                {
                                    ValueRoll_List.Add(ValueRoll);
                                    randoms.Add(sortedDefect[ValueRoll]);
                                }
                            }

                            _reviewKlarfDefects[className] = randoms;
                            break;
                        case StrategyType.SizeSampling:

                            int counter = (sortedDefect.Count / nbSamples) - 1;
                            List<KlarfDefect> sizeSamplingDefects = new List<KlarfDefect>();
                            for (int i = 0; i < sortedDefect.Count; i += counter)
                            {
                                sizeSamplingDefects.Add(sortedDefect[i]);
                            }

                            _reviewKlarfDefects[className] = sizeSamplingDefects;
                            break;
                    }
                }
                else
                    _reviewKlarfDefects[className] = sortedDefect;
            }
        }
    }
}
