using System.Collections.Generic;
using System.Linq;

using Agileo.GUI.Services.Icons;
using Agileo.Semi.Gem.Abstractions.E30;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary
{
    public class StatusVariablesPanel : BaseE30VariablesPanel
    {
        public StatusVariablesPanel(string relativeId, IIcon icon = null) : base(true, relativeId, icon)
        {
        }

        #region Overrides of BaseE30VariablesPanel

        protected override IEnumerable<E30Variable> LoadVariables()
        {
            return App.ControllerInstance.GemController.E30Std.DataServices == null
                ? Enumerable.Empty<E30Variable>()
                : App.ControllerInstance.GemController.E30Std.DataServices.GetVariables(Class.SV);
        }

        #endregion
    }
}
