using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.ExternalFile;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class PostProcessingSettings
    {
        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public bool PdfIsSaved { get; set; }

        [DataMember]
        public string TemplateName { get; set; }

        [DataMember]
        public ExternalMountainsTemplate Template {get;set;}

       [DataMember]
        public List<PostProcessingOutput> Outputs { get; set; }
    }
}
