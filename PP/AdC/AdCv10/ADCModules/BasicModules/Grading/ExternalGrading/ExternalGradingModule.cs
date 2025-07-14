using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using ADCEngine;

using BasicModules.Edition.KlarfEditor;

using UnitySC.Shared.Tools;

namespace BasicModules.Grading.ExternalGrading
{
    internal class ExternalGradingModule : ModuleBase
    {
        private GradingRecipe _gradingRecipe;

        private Dictionary<int, List<RuleCounter>> _rulesCounter;

        public ExternalGradingModule(IModuleFactory factory, int id, Recipe recipe) : base(factory, id, recipe)
        {
        }

        protected override void OnInit()
        {
            base.OnInit();
            string gradingRecipeFilePath = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.GradingRecipeFilePath);
            if (string.IsNullOrEmpty(gradingRecipeFilePath))
                throw new InvalidOperationException("Grading recipe file is missing in wafer info");
            else if (!File.Exists(gradingRecipeFilePath))
                throw new FileNotFoundException("Grading recipe file not found: " + gradingRecipeFilePath);

            try
            {
                _gradingRecipe = XML.Deserialize<GradingRecipe>(gradingRecipeFilePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error during opening grading recipe ", ex);
            }

            _rulesCounter = new Dictionary<int, List<RuleCounter>>();
            foreach (var rule in _gradingRecipe.Rules)
            {
                if (!_rulesCounter.ContainsKey(rule.RoughBinNumber))
                {
                    _rulesCounter.Add(rule.RoughBinNumber, new List<RuleCounter>()
                    {
                        new RuleCounter { Rule = rule, Counter = 0 }
                    });
                }
                else
                {
                    _rulesCounter[rule.RoughBinNumber].Add(new RuleCounter { Rule = rule, Counter = 0 });
                }
            }
        }

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            Interlocked.Increment(ref nbObjectsIn);
            KlarfCluster klarfCluster = obj as KlarfCluster;

            if (_rulesCounter.ContainsKey(klarfCluster.RoughBinNum))
            {
                foreach (var ruleCounter in _rulesCounter[klarfCluster.RoughBinNum])
                {
                    switch (ruleCounter.Rule.Criteria)
                    {
                        case Rule.GradingCriteria.Count:
                            ruleCounter.Counter++;
                            break;
                        case Rule.GradingCriteria.Size:
                            ruleCounter.Counter += klarfCluster.TotalDefectSize;
                            break;
                    }
                }
            }
        }

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
                    string msg = "External grading failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    base.OnStopping(oldState);
                }
            });
        }

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
        }
    }

    internal class RuleCounter
    {
        internal Rule Rule { get; set; }
        internal double Counter { get; set; }
    }
}
