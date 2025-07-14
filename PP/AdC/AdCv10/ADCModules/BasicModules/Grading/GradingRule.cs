using System.ComponentModel;
using System.Xml.Serialization;

using ADCEngine;

using AdcTools;

namespace BasicModules.Grading
{
    /// <summary>
    /// Grading rule
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class GradingRule : Serializable
    {
        /// <summary>
        /// Grading criteria
        /// </summary> 
        public enum GradingCriteria
        {
            [Description("Number of defect")]
            Count,
            [Description("Sum of defect sizes")]
            Size
        }

        /// <summary>
        /// Specifies the type of grading mark 
        /// </summary>
        [XmlAttribute]
        public Recipe.Grading GradingMark { get; set; }

        /// <summary>
        /// Creteria used for grading
        /// </summary>
        [XmlAttribute]
        public GradingCriteria Criteria { get; set; }

        /// <summary>
        /// Grading limit value
        /// </summary>
        public double BiggerThan { get; set; }
    }
}
