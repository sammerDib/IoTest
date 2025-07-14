using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Context
{
    /// <summary>
    /// This class is responsible for applying a given context.
    /// </summary>
    internal class ContextApplier
    {
        private readonly EmeHardwareManager _hardwareManager;

        public ContextApplier(EmeHardwareManager hardwareManager)
        {
            _hardwareManager = hardwareManager;
        }

        public void Apply(EMEContextBase context)
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
        }

        private class ContextToApply
        {
            public EMEContextBase Context { get; }
            public Task ApplicationTask { get; }
            public Task WaitTask { get; }

            public ContextToApply(EMEContextBase context, Task applicationTask, Task waitTask)
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
            where ContextType : EMEContextBase
        {
            var applicationTask = new Task(() => applicationAction(context));
            Task waitTask = null;
            if (!(waitAction is null))
                waitTask = new Task(() => waitAction(context));
            return new ContextToApply(context, applicationTask, waitTask);
        }

        private List<ContextToApply> ExtractFlatContexts(EMEContextBase context)
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
                case LightContext lightContext:
                    {
                        flatContexts.Add(CreateContextToApply(lightContext, ApplyLightContext));
                        break;
                    }
                // Contexts that contain subcontexts as properties or lists
                case LightsContext _:
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
        private void FillWithFlatContextWithSubcontexts(EMEContextBase context, List<ContextToApply> flatContexts)
        {
            var subContexts = SubObjectFinder.GetAllSubObjectOfTypeT<EMEContextBase>(context, maxDepth: 1);
            foreach (var subContext in subContexts.Values)
            {
                flatContexts.AddRange(ExtractFlatContexts(subContext));
            }
        }

        private void ApplyLightContext(LightContext lightContext)
        {
            //TODO
            /*
            var currentLights = _hardwareManager.Lights;
            if (!currentLights.ContainsKey(lightContext.DeviceID))
            {
                throw new Exception($"Cannot apply context: no light found with id '{lightContext.DeviceID}'");
            }

            currentLights[lightContext.DeviceID].SetIntensity(lightContext.Intensity);
            */
        }

        private void ApplyPositionContext(PositionContext context)
        {
            _hardwareManager.MotionAxes.GoToPosition(context.GetPosition());
            _hardwareManager.MotionAxes.WaitMotionEnd(20_000);
        }
    }
}
