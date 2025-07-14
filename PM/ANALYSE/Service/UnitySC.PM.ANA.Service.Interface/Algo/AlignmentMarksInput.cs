using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class AlignmentMarksInput : IANAInputFlow
    {
        public AlignmentMarksInput()
        { }

        public AlignmentMarksInput(List<PositionWithPatternRec> site1Images, List<PositionWithPatternRec> site2Images, AutoFocusSettings autofocusSettings)
        {
            Site1Images = site1Images;
            Site2Images = site2Images;
            AutoFocusSettings = autofocusSettings;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (Site1Images == null || Site1Images.Count == 0)
            {
                validity.IsValid = false;
                validity.Message.Add($"The Site 1 image data is missing.");
            }
            else
            {
                foreach (var image in Site1Images)
                {
                    validity.ComposeWith(image.CheckInputValidity());
                }
            }

            if (Site2Images == null || Site2Images.Count == 0)
            {
                validity.IsValid = false;
                validity.Message.Add($"The Site 2 image data is missing.");
            }
            else
            {
                foreach (var image in Site2Images)
                {
                    validity.ComposeWith(image.CheckInputValidity());
                }
            }

            if (!(AutoFocusSettings is null))
            {
                validity.ComposeWith(AutoFocusSettings.CheckInputValidity());
            }

            return validity;
        }

        /// <summary>
        /// The site 1 images are the top left most image and its backups.
        /// </summary>
        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        /// <summary>
        /// The site 2 images are the bottom right most image and its backups.
        /// </summary>
        [DataMember]
        public List<PositionWithPatternRec> Site1Images { get; set; }

        [DataMember]
        public List<PositionWithPatternRec> Site2Images { get; set; }

        [DataMember]
        public AutoFocusSettings AutoFocusSettings { get; set; }
    }
}
