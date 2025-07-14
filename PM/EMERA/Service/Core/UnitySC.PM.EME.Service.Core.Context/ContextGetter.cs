using System;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.EME.Service.Core.Context
{
    /// <summary>
    /// This class is responsible for synchronizing a context instance with current context, by updating given context.
    /// </summary>
    internal class ContextGetter
    {
        private readonly EmeHardwareManager _hardwareManager;

        public ContextGetter(EmeHardwareManager hardwareManager)
        {
            _hardwareManager = hardwareManager;
        }

        public T GetCurrent<T>() where T : EMEContextBase, new()
        {
            var context = new T();
            FillContextWithCurrent(context);
            return context;
        }

        //TODO : ADD XYZ CASE
        public void FillContextWithCurrent(EMEContextBase context)
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
                case XYZPositionContext xyzPositionContext:
                    {
                        FillXYZPositionWithCurrent(xyzPositionContext);
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
        private bool FillContextWithSubcontexts(EMEContextBase context)
        {
            // If no properties, then it is not a context with subcontexts
            if (context.GetType().GetProperties().Length == 0)
                return false;

            foreach (var property in context.GetType().GetProperties())
            {
                // If one property is not a context, then it is not handled
                if (!property.PropertyType.IsSubclassOf(typeof(EMEContextBase)))
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
                FillContextWithCurrent(propertyObject as EMEContextBase);
            }
            return true;
        }

        private void FillLightsWithCurrent(LightsContext context)
        {
            //TODO : ADD LIGHTS
            /*
            context.Lights.Clear();
            foreach (var light in _hardwareManager.Lights)
            {
                var newLightContext = new LightContext(light.Key, light.Value.GetIntensity());
                context.Lights.Add(newLightContext);
            }
            */
        }

        private void FillXYPositionWithCurrent(XYPositionContext positionContext)
        {

            var currentPosition = GetCurrentPosition();
            positionContext.Position = currentPosition.ToXYPosition();

        }

        private void FillXYZPositionWithCurrent(XYZPositionContext positionContext)
        {

            var currentPosition = GetCurrentPosition();
            positionContext.Position = currentPosition.ToXYZPosition();

        }

        private XYZPosition GetCurrentPosition()
        {
            var currentPosition = _hardwareManager.MotionAxes.GetPosition();
            if (!(currentPosition is XYZPosition))
            {
                throw new Exception(
                    $"Position type {currentPosition.GetType()} not supported. Only {typeof(XYZPosition)} is supported"
                );
            }
            return (XYZPosition)currentPosition;
        }
    }
}
