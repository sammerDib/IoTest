namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public class ImageFolderAndBaseName
    {
        public ImageFolderAndBaseName(string folder, string baseName)
        {
            Folder = folder;
            BaseName = baseName;
        }

        public string Folder { get; }
        
        public string BaseName { get; }
    }
}
