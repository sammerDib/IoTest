using System;
using System.IO;

namespace UnitySC.Shared.Data.ExternalFile
{
    public class ExternalMountainsTemplate : ExternalFileBase
    {
        public override string FileExtension { get; set; } = ".mnt";

        public override void LoadFromFile(string filePath)
        {
            Data = File.ReadAllBytes(filePath);
        }

        public override void SaveToFile(string filePath)
        {
            File.WriteAllBytes(filePath, Data);
        }

        public override void UpdateWith(ExternalFileBase externalFileBase)
        {
            var newExternalTemplateContent = externalFileBase as ExternalMountainsTemplate;
            if (newExternalTemplateContent is null)
                throw new InvalidCastException("ExternalMountainsTemplate is expected in update");
            Data = newExternalTemplateContent.Data;
            FileNameKey = externalFileBase.FileNameKey;
        }
    }
}
