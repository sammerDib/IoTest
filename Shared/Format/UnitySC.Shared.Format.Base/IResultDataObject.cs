using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Format.Base
{
    public interface IResultDataObject
    {
        ResultType ResType { get; set; }

        string ResFilePath { get; set; }

        long DBResId { get; set; }

        bool ReadFromFile(string resFilePath, out string sError);

        bool WriteInFile(string resFilePath, out string sError);

        object InternalTableToUpdate(object table); // specific ex : Klarf with rough bin settings
    }
}
