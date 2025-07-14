using Agileo.GUI.Components;
using Agileo.GUI.Interfaces;
using Agileo.GUI.Services.Icons;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.WebVisu
{
    public class WebLink : GraphicalElement
    {
        public WebLink()
            : this(string.Empty, "Design mode")
        {
        }

        public WebLink(string link, string id, IIcon icon = null)
            : base(id, icon)
        {
            Link = link;
        }

        public string Link { get; set; }

        public override void Accept(IGuiElementVisitor visitor)
        {
        }
    }
}
