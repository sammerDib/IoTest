using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Test.Context
{
    [TestClass]
    public class ContextExtensionsTest
    {
        [TestMethod]
        public void AppendWith_NominalCase()
        {
            // Given 2 contexts
            var context = new TopObjectiveContext();
            var otherContext = new BottomObjectiveContext();

            // When appending second context to first
            ANAContextBase contextsTogether = context.AppendWith(otherContext);

            // Then result is a ContextsList with the initial contexts
            Assert.IsInstanceOfType(contextsTogether, typeof(ContextsList));
            ContextsList contextsList = contextsTogether as ContextsList;
            Assert.AreEqual(2, contextsList.Contexts.Count);
            Assert.IsTrue(contextsList.Contexts.Contains(context));
            Assert.IsTrue(contextsList.Contexts.Contains(otherContext));
        }

        [TestMethod]
        public void AppendWith_OtherNull()
        {
            // Given 2 contexts with other null
            var context = new TopObjectiveContext();
            ANAContextBase otherContext = null;

            // When appending second context to first
            ANAContextBase contextsTogether = context.AppendWith(otherContext);

            // Then result is the initial context
            Assert.AreEqual(contextsTogether, context);
        }

        [TestMethod]
        public void AppendWith_MainNull()
        {
            // Given 2 contexts with main null
            ANAContextBase context = null;
            var otherContext = new TopObjectiveContext();

            // When appending second context to first
            ANAContextBase contextsTogether = context.AppendWith(otherContext);

            // Then result is the other context
            Assert.AreEqual(contextsTogether, otherContext);
        }

        [TestMethod]
        public void AppendWith_BothNull()
        {
            // Given 2 null contexts
            ANAContextBase context = null;
            ANAContextBase otherContext = null;

            // When appending second context to first
            ANAContextBase contextsTogether = context.AppendWith(otherContext);

            // Then result is null
            Assert.IsNull(contextsTogether);
        }

        [TestMethod]
        public void AppendWithPositionContext_NominalCase()
        {
            // Given a context and a position
            var context = new TopObjectiveContext();
            var position = new XYPosition();

            // When appending context to position context
            ANAContextBase contextsTogether = context.AppendWithPositionContext(position);

            // Then result is a ContextsList with the initial context and a new position context
            Assert.IsInstanceOfType(contextsTogether, typeof(ContextsList));
            ContextsList contextsList = contextsTogether as ContextsList;
            Assert.AreEqual(2, contextsList.Contexts.Count);
            Assert.IsTrue(contextsList.Contexts.Contains(context));
            Assert.IsTrue(contextsList.Contexts.Exists(c => (c as XYPositionContext)?.Position == position));
        }

        [TestMethod]
        public void AppendWithPositionContext_NullPosition()
        {
            // Given a context and a position
            var context = new TopObjectiveContext();
            PositionBase position = null;

            // When appending context to position context
            ANAContextBase contextsTogether = context.AppendWithPositionContext(position);

            // Then result is the initial context
            Assert.AreEqual(contextsTogether, context);
        }
    }
}
