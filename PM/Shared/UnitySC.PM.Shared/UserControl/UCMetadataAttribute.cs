using System;
using System.ComponentModel.Composition;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.UC
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UCMetadataAttribute : ExportAttribute
    {
        public UCMetadataAttribute() : base(typeof(IPmUc))
        {
        }

        public ActorType ActorType { get; set; }
    }
}
