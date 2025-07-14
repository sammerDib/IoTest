using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

namespace BasicModules.Grading.ClassGrading
{
    internal class ClassGradingModule : ModuleBase
    {
        private Dictionary<string, List<RuleCounter>> _rulesCounter;

        /// <summary>
        /// Constructor
        /// </summary>
        public ClassGradingModule(IModuleFactory factory, int id, Recipe recipe) : base(factory, id, recipe)
        {
            paramGrading = new ClassGradingParameter(this, "Grading");
        }

        /// <summary>
        /// Init module
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            _rulesCounter = new Dictionary<string, List<RuleCounter>>();
            foreach (var rule in paramGrading.ClassGradingRules)
            {
                if (!_rulesCounter.ContainsKey(rule.DefectClass))
                {
                    _rulesCounter.Add(rule.DefectClass, new List<RuleCounter>()
                    {
                        new RuleCounter { Rule = rule, Counter = 0 }
                    });
                }
                else
                {
                    _rulesCounter[rule.DefectClass].Add(new RuleCounter { Rule = rule, Counter = 0 });
                }
            }
        }

        /// <summary>
        /// Xml parameter
        /// </summary>
        [ExportableParameter(false)]
        public readonly ClassGradingParameter paramGrading;

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            Interlocked.Increment(ref nbObjectsIn);
            Cluster cluster = (Cluster)obj;
            if (_rulesCounter.ContainsKey(cluster.DefectClass))
            {
                foreach (var ruleCounter in _rulesCounter[cluster.DefectClass])
                {
                    switch (ruleCounter.Rule.Criteria)
                    {
                        case GradingRule.GradingCriteria.Count:
                            ruleCounter.Counter++;
                            break;
                        case GradingRule.GradingCriteria.Size:
                            ruleCounter.Counter += (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize];
                            break;
                    }
                }
            }

            ProcessChildren(obj);
        }

        /// <summary>
        /// Stop module
        /// </summary>
        /// <param name="parent"></param>
        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("parent stopped, starting processing task");

            Scheduler.StartSingleTask("ProcessGrading", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                        ProcessGrading();
                    else if (oldState == eModuleState.Aborting)
                        _rulesCounter.Clear();
                    else
                        throw new ApplicationException("invalid state");
                }
                catch (Exception ex)
                {
                    string msg = "Class grading failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    base.OnStopping(oldState);
                }
            });
        }

        /// <summary>
        /// Process class grading
        /// </summary>
        private void ProcessGrading()
        {
            foreach (var ruleCounter in _rulesCounter.SelectMany(x => x.Value))
            {
                if (ruleCounter.Rule.BiggerThan < ruleCounter.Counter)
                {
                    Recipe.GradingMark = ruleCounter.Rule.GradingMark;
                    if (Recipe.GradingMark == Recipe.Grading.Reject)
                        break;
                }
            }

            if (Recipe.GradingMark.HasValue)
            {
                log("Recipe marked for grading as " + Recipe.GradingMark);
            }

            string remotePath = ConfigurationManager.AppSettings["Grading.Path"];
            string remoteLotFilePath = String.Format("{0}\\{1}\\", remotePath, Wafer.GetWaferInfo(eWaferInfo.LotID)); // + Wafer.GetWaferInfo(eWaferInfo.ToolRecipe) + "\\";

            if (!Directory.Exists(remoteLotFilePath))
            {
                Directory.CreateDirectory(remoteLotFilePath);
            }

            string remoteRecipeFileName = String.Format("{0}\\{1}\\{2}.txt", remotePath, Wafer.GetWaferInfo(eWaferInfo.LotID), Wafer.GetWaferInfo(eWaferInfo.ToolRecipe));

            using (StreamWriter wstream = new StreamWriter(remoteRecipeFileName, true))
            {
                int gradingValueToInt = 1;
                switch (Recipe.GradingMark)
                {
                    case Recipe.Grading.Reject:
                        gradingValueToInt = 0;
                        break;
                    case Recipe.Grading.Rework:
                        gradingValueToInt = 1;
                        break;
                }

                wstream.WriteLine(String.Format("{0}_{1}", gradingValueToInt, Wafer.GetWaferInfo(eWaferInfo.SlotID)));
            }
        }

        /// <summary>
        /// Rule counter for rule check
        /// </summary>
        internal class RuleCounter
        {
            internal ClassGradingRule Rule { get; set; }
            internal double Counter { get; set; }
        }
    }
}
