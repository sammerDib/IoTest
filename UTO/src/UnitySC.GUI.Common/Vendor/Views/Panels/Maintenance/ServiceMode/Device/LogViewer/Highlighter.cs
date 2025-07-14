using System;
using System.Collections.Generic;

using ICSharpCode.AvalonEdit.Highlighting;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.LogViewer
{
    public class Highlighter : IHighlightingDefinition
    {
        public Highlighter()
        {
            MainRuleSet = new HighlightingRuleSet();
        }

        public HighlightingRuleSet MainRuleSet { get; }

        public string Name { get; }

        public IEnumerable<HighlightingColor> NamedHighlightingColors { get; }

        public IDictionary<string, string> Properties { get; }

        public HighlightingColor GetNamedColor(string name)
        {
            throw new NotImplementedException();
        }

        public HighlightingRuleSet GetNamedRuleSet(string name)
        {
            throw new NotImplementedException();
        }
    }
}
