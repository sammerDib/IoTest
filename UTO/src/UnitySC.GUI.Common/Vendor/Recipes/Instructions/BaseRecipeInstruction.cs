using System.Collections.Generic;
using System.Xml.Serialization;

using Agileo.ProcessingFramework.Instructions;
using Agileo.Recipes.Components;

using Newtonsoft.Json;

using UnitySC.GUI.Common.Vendor.UIComponents.Behaviors;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions
{
    public abstract class BaseRecipeInstruction : RecipeInstruction
    {
        public abstract string LocalizationKey { get; }

        public abstract Instruction ToProcessingInstruction();

        [XmlIgnore]
        [JsonIgnore]
        public virtual List<AdvancedStringFormatDefinition> FormattedLabel
        {
            get
            {
                return new List<AdvancedStringFormatDefinition>
                {
                    new AdvancedStringFormatDefinition(PrettyLabel)
                    {
                        Bold = true,
                        Highlighted = true
                    }
                };
            }
        }
    }
}
