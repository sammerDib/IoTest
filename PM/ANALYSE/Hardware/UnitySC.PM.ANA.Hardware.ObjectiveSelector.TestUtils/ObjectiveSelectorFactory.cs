using System.Collections.Generic;

using Moq;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.TestUtils.ObjectiveSelector.Configuration;

namespace UnitySC.PM.ANA.Hardware.ObjectiveSelector.TestUtils
{
    public static class ObjectiveSelectorFactory
    {
        public static IObjectiveSelector Build(ObjectiveConfig config)
        {
            var objectiveSelectorMock = new Mock<IObjectiveSelector>();
            objectiveSelectorMock.Setup(selector => selector.Config)
                .Returns(
                    ObjectivesSelectorConfigBaseFactory.Build(selectorConfig =>
                        selectorConfig.Objectives = new List<ObjectiveConfig> { config }
                    )
                );
            return objectiveSelectorMock.Object;
        }
    }
}
