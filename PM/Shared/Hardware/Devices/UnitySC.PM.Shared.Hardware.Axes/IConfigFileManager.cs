namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public interface IConfigFileManager
    {
        string DefFileName { get; }
        string IniFileSection { get; }
        string IniFileKey { get; }
        string FileName { get; set; }

        IConfigFileManager ReadXMLInitFile(string fileName);
        void WriteXMLInitFile();
        bool Init();
    }
}
