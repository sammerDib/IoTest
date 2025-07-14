using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.RecipeService
{
    public class DMTResultGeneratedEventArgs
    {
        public string Name;

        public Side WaferSide;

        public string Path;

        public DMTResultGeneratedEventArgs(string name, Side waferSide, string path)
        {
            Name = name;
            WaferSide = waferSide;
            Path = path;
        }
    }
}
