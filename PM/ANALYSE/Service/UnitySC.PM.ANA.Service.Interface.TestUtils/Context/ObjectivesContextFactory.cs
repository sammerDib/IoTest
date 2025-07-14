using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.TestUtils.Context
{
    public static class ObjectiveContextFactory
    {
        public static ObjectiveContext Build(Action<ObjectiveContext> action = null)
        {
            var objective = new ObjectiveContext("objective#1");
            action?.Invoke(objective);
            return objective;
        }
    }

    public static class ObjectivesContextFactory
    {
        public static ObjectivesContext Build(Action<ObjectivesContext> action = null)
        {
            var context = new ObjectivesContext
            {
                Objectives = new List<ObjectiveContext> { ObjectiveContextFactory.Build() }
            };
            action?.Invoke(context);
            return context;
        }
    }
}
