using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true)]
    [Guid("6FCD17FE-2DE4-421A-80BF-C8065AC21F52")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IFlowRecipeItem
    {
        string ProcessModuleId { get; }

        string ModuleRecipeId { get; }

        double AngleInDegree { get; }

        string ModuleRecipeName { get; }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IFlowRecipeItem))]
    [Guid("06FF1481-99C9-4366-A79E-81FB03EDB886")]
    public class FlowRecipeItem : IFlowRecipeItem
    {
        #region Properties

        public string ProcessModuleId { get; }

        public string ModuleRecipeId { get; }

        public double AngleInDegree { get; }

        public string ModuleRecipeName { get; }

        #endregion

        #region Constructor

        public FlowRecipeItem(string processModuleId, string moduleRecipeId, string moduleRecipeName, double angleInDegree)
        {
            ProcessModuleId = processModuleId;
            ModuleRecipeId = moduleRecipeId;
            ModuleRecipeName = moduleRecipeName;
            AngleInDegree = angleInDegree;
        }

        #endregion
    }
}
