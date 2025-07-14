using System;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Context
{
    /// <summary>
    /// This class is responsible for synchronizing a context instance with current context, by updating given context.
    /// </summary>
    internal class ContextGetter
    {
        private readonly AnaHardwareManager _hardwareManager;

        public ContextGetter(AnaHardwareManager hardwareManager)
        {
            _hardwareManager = hardwareManager;
        }

        public T GetCurrent<T>() where T : ANAContextBase, new()
        {
            var context = new T();
            FillContextWithCurrent(context);
            return context;
        }

        public void FillContextWithCurrent(ANAContextBase context)
        {
            switch (context)
            {
                case ContextsList contextsList:
                    {
                        foreach (var contextInList in contextsList.Contexts)
                        {
                            FillContextWithCurrent(contextInList);
                        }
                        break;
                    }
                case LightsContext lightsContext:
                    {
                        FillLightsWithCurrent(lightsContext);
                        break;
                    }
                case XYPositionContext xyPositionContext:
                    {
                        FillXYPositionWithCurrent(xyPositionContext);
                        break;
                    }
                case XYZTopZBottomPositionContext xyzTopZBottomPositionContext:
                    {
                        FillXYZTopZBottomPositionWithCurrent(xyzTopZBottomPositionContext);
                        break;
                    }
                case AnaPositionContext anaPositionContext:
                    {
                        FillAnaPositionWithCurrent(anaPositionContext);
                        break;
                    }
                case ObjectivesContext objectivesContext:
                    {
                        FillObjectivesContext(objectivesContext);
                        break;
                    }
                case TopObjectiveContext topObjectiveContext:
                    {
                        FillTopObjectiveContext(topObjectiveContext);
                        break;
                    }
                case BottomObjectiveContext bottomObjectiveContext:
                    {
                        FillBottomObjectiveContext(bottomObjectiveContext);
                        break;
                    }
                default:
                    {
                        if (!FillContextWithSubcontexts(context))
                        {
                            throw new ArgumentException($"Unknow context type to get: {context.GetType()}");
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Fills a context that is composed of subcontexts as properties
        /// </summary>
        /// <returns>True if the given context is indeed composed ONLY of subcontexts</returns>
        private bool FillContextWithSubcontexts(ANAContextBase context)
        {
            // If no properties, then it is not a context with subcontexts
            if (context.GetType().GetProperties().Length == 0)
                return false;

            foreach (var property in context.GetType().GetProperties())
            {
                // If one property is not a context, then it is not handled
                if (!property.PropertyType.IsSubclassOf(typeof(ANAContextBase)))
                    return false;

                object propertyObject = property.GetValue(context);
                if (propertyObject is null)
                {
                    try
                    {
                        propertyObject = Activator.CreateInstance(property.PropertyType);
                        property.SetValue(context, propertyObject);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"Context of type {context.GetType()} has a subcontext property \"{property.Name}\" " +
                            $"that is null but that has no parameterless constructor to instantiate: {ex.Message}");
                    }
                }
                FillContextWithCurrent(propertyObject as ANAContextBase);
            }
            return true;
        }

        private void FillLightsWithCurrent(LightsContext context)
        {
            context.Lights.Clear();
            foreach (var light in _hardwareManager.Lights)
            {
                var newLightContext = new LightContext(light.Key, light.Value.GetIntensity());
                context.Lights.Add(newLightContext);
            }
        }

        private AnaPosition GetCurrentPosition()
        {
            var currentPosition = _hardwareManager.Axes.GetPos();
            if (!(currentPosition is AnaPosition position))
            {
                throw new Exception(
                    $"Position type {currentPosition.GetType()} not supported. Only {typeof(AnaPosition)} is supported"
                );
            }
            return (AnaPosition)currentPosition;
        }

        private void FillXYPositionWithCurrent(XYPositionContext positionContext)
        {
            var currentPosition = GetCurrentPosition();
            positionContext.Position = currentPosition.ToXYPosition();
        }

        private void FillXYZTopZBottomPositionWithCurrent(XYZTopZBottomPositionContext positionContext)
        {
            var currentPosition = GetCurrentPosition();
            positionContext.Position = currentPosition.ToXYZTopZBottomPosition();
        }

        private void FillAnaPositionWithCurrent(AnaPositionContext positionContext)
        {
            positionContext.Position = GetCurrentPosition();
        }

        private void FillTopObjectiveContext(TopObjectiveContext objectiveContext)
        {
            foreach (var objectiveSelector in _hardwareManager.ObjectivesSelectors)
            {
                if (objectiveSelector.Value.Position != Shared.Hardware.Service.Interface.ModulePositions.Up)
                    continue;

                objectiveContext.ObjectiveId = objectiveSelector.Value.GetObjectiveInUse().DeviceID;
                return;
            }
            throw new Exception("Current top objective not found");
        }

        private void FillBottomObjectiveContext(BottomObjectiveContext objectiveContext)
        {
            foreach (var objectiveSelector in _hardwareManager.ObjectivesSelectors)
            {
                if (objectiveSelector.Value.Position != Shared.Hardware.Service.Interface.ModulePositions.Down)
                    continue;

                objectiveContext.ObjectiveId = objectiveSelector.Value.GetObjectiveInUse().DeviceID;
                return;
            }
            throw new Exception("Current bottom objective not found");
        }

        private void FillObjectivesContext(ObjectivesContext objectivesContext)
        {
            objectivesContext.Objectives.Clear();
            foreach (var objectiveSelector in _hardwareManager.ObjectivesSelectors)
            {
                string objectiveId = objectiveSelector.Value.GetObjectiveInUse().DeviceID;
                ObjectiveContext objectiveContext;
                if (objectiveSelector.Value.Position == Shared.Hardware.Service.Interface.ModulePositions.Up)
                    objectiveContext = new TopObjectiveContext(objectiveId);
                else
                    objectiveContext = new BottomObjectiveContext(objectiveId);
                objectivesContext.Objectives.Add(objectiveContext);
            }
        }
    }
}
