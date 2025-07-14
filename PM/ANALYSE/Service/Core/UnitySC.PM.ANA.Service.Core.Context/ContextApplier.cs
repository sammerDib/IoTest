using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Context
{
    /// <summary>
    /// This class is responsible for applying a given context.
    /// </summary>
    internal class ContextApplier
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;
        private XYZTopZBottomMove _objectiveChangeOffset;

        public ContextApplier(AnaHardwareManager hardwareManager)
        {
            _objectiveChangeOffset = null;
            _hardwareManager = hardwareManager;
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
        }

        public void Apply(ANAContextBase context)
        {
            if (context is null)
                return;

            List<ContextToApply> flatContexts = ExtractFlatContexts(context);
            Dictionary<Type, List<ContextToApply>> contextsByType = OrderByType(flatContexts);
            foreach (var flatContext in flatContexts)
            {
                var typesToApplyBefore = GetTypesToApplyBefore(flatContext);
                if (typesToApplyBefore.Count == 0)
                {
                    flatContext.ApplicationTask.Start();
                }
                else
                {
                    var tasksToDoBefore = new List<Task>();
                    foreach (var type in typesToApplyBefore)
                    {
                        List<ContextToApply> contextsOfType;
                        if (contextsByType.TryGetValue(type, out contextsOfType))
                            tasksToDoBefore.AddRange(contextsOfType.Select(c => c.ApplicationTask));
                    }

                    if (tasksToDoBefore.Count == 0)
                    {
                        flatContext.ApplicationTask.Start();
                    }
                    else
                    {
                        Task allBeforeTask = Task.WhenAll(tasksToDoBefore);
                        allBeforeTask.ContinueWith((_) => flatContext.ApplicationTask.Start());
                    }
                }

                if (!(flatContext.WaitTask is null))
                    flatContext.ApplicationTask.ContinueWith((_) => flatContext.WaitTask.Start());
            }

            IEnumerable<Task> applicationTasks = flatContexts.Select(c => c.ApplicationTask);
            IEnumerable<Task> waitTasks = flatContexts.Select(c => c.WaitTask).Where(task => !(task is null));
            if (!Task.WaitAll(applicationTasks.Concat(waitTasks).ToArray(), 20_000))
                throw new TimeoutException("Context application timeouted.");

            if (_objectiveChangeOffset != null && _objectiveChangeOffset != new XYZTopZBottomMove(0, 0, 0, 0))
            {
                _hardwareManager.Axes.MoveIncremental(_objectiveChangeOffset, AxisSpeed.Normal);
                _hardwareManager.Axes.WaitMotionEnd(20_000);
                _objectiveChangeOffset = null;
            }
        }

        private class ContextToApply
        {
            public ANAContextBase Context { get; }
            public Task ApplicationTask { get; }
            public Task WaitTask { get; }

            public ContextToApply(ANAContextBase context, Task applicationTask, Task waitTask)
            {
                Context = context;
                ApplicationTask = applicationTask;
                WaitTask = waitTask;
            }
        }

        private List<Type> GetTypesToApplyBefore(ContextToApply context)
        {
            return context.Context.GetType().GetCustomAttributes(typeof(ApplyAfterAttribute), true)
                .Select(a => ((ApplyAfterAttribute)a).ContextType).ToList();
        }

        private Dictionary<Type, List<ContextToApply>> OrderByType(List<ContextToApply> contexts)
        {
            var dict = new Dictionary<Type, List<ContextToApply>>();
            foreach (var context in contexts)
            {
                List<ContextToApply> contextsOfType;
                if (!dict.TryGetValue(context.Context.GetType(), out contextsOfType))
                {
                    contextsOfType = new List<ContextToApply>();
                    dict.Add(context.Context.GetType(), contextsOfType);
                }
                contextsOfType.Add(context);
            }
            return dict;
        }

        private ContextToApply CreateContextToApply<ContextType>(ContextType context, Action<ContextType> applicationAction, Action<ContextType> waitAction = null)
            where ContextType : ANAContextBase
        {
            var applicationTask = new Task(() => applicationAction(context));
            Task waitTask = null;
            if (!(waitAction is null))
                waitTask = new Task(() => waitAction(context));
            return new ContextToApply(context, applicationTask, waitTask);
        }

        private List<ContextToApply> ExtractFlatContexts(ANAContextBase context)
        {
            var flatContexts = new List<ContextToApply>();
            if (context is null)
                return flatContexts;

            switch (context)
            {
                case PositionContext positionContext:
                    {
                        flatContexts.Add(CreateContextToApply(positionContext, ApplyPositionContext));
                        break;
                    }
                case ObjectiveContext objectiveContext:
                    {
                        flatContexts.Add(CreateContextToApply(objectiveContext, ApplyObjectiveContext, WaitForObjectiveContextCompletion));
                        break;
                    }
                case LightContext lightContext:
                    {
                        flatContexts.Add(CreateContextToApply(lightContext, ApplyLightContext));
                        break;
                    }
                // Contexts that contain subcontexts as properties or lists
                case ObjectivesContext _:
                case LightsContext _:
                case TopImageAcquisitionContext _:
                case BottomImageAcquisitionContext _:
                case DualImageAcquisitionContext _:
                case ContextsList _:
                case PMContext _:
                case ChamberContext _: // TODO: Chamber context will probably have its own function
                    {
                        FillWithFlatContextWithSubcontexts(context, flatContexts);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"Unknow context type to apply: {context.GetType()}");
                    }
            }
            return flatContexts;
        }

        /// <summary>
        /// Extracts all contexts from a context that is composed of subcontexts (as properties, or as a list),
        /// and fills given list with them.
        /// </summary>
        private void FillWithFlatContextWithSubcontexts(ANAContextBase context, List<ContextToApply> flatContexts)
        {
            var subContexts = SubObjectFinder.GetAllSubObjectOfTypeT<ANAContextBase>(context, maxDepth: 1);
            foreach (var subContext in subContexts.Values)
            {
                flatContexts.AddRange(ExtractFlatContexts(subContext));
            }
        }

        private void ApplyLightContext(LightContext lightContext)
        {
            var currentLights = _hardwareManager.Lights;
            if (!currentLights.ContainsKey(lightContext.DeviceID))
            {
                throw new Exception($"Cannot apply context: no light found with id '{lightContext.DeviceID}'");
            }

            currentLights[lightContext.DeviceID].SetIntensity(lightContext.Intensity);
        }

        private void ApplyPositionContext(PositionContext context)
        {
            _hardwareManager.Axes.GotoPosition(context.GetPosition(), AxisSpeed.Normal);
            // Calls to Axes.GetPos return the actual position, not the one given to GotoPosition. This is why
            // we perform WaitMotionEnd in this function rather than creating a Wait function like for the objective selection.
            _hardwareManager.Axes.WaitMotionEnd(20_000);
        }

        private void ApplyObjectiveContext(ObjectiveContext context)
        {
            // Compute the objective Offset
            var newObjectiveId = context.ObjectiveId;
            var objectiveSelector = _hardwareManager.GetObjectiveSelectorOfObjective(newObjectiveId);
            var previousObjectiveId = _hardwareManager.ObjectivesSelectors[objectiveSelector.DeviceID].GetObjectiveInUse().DeviceID;

            _objectiveChangeOffset = _calibrationManager.GetXYZTopZBottomObjectiveOffset(previousObjectiveId, newObjectiveId, false);

            // The objective has to exist in the objective selector config or else we could not have found the objective selector.
            var objectiveConfig = objectiveSelector.Config.FindObjective(context.ObjectiveId);
            objectiveSelector.SetObjective(objectiveConfig);
            // Further calls to GetObjective will return the objective given to SetObjective here. This is why we
            // can afford to wait for the objective selector motion to end in another function. It allows to start the
            // context applications that depend on the current objective as soon as possible.
        }

        private void WaitForObjectiveContextCompletion(ObjectiveContext context)
        {
            _hardwareManager.GetObjectiveSelectorOfObjective(context.ObjectiveId).WaitMotionEnd();
        }
    }
}
