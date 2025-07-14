using System;
using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true)]
    [Guid("05399615-0B1E-44CE-AB57-8C29276B1957")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IProcessJob
    {
        #region Public Properties

        DateTime CreationTime { get; }
        DateTime FinishTime { get; }
        IFlowRecipeCollection FlowRecipe { get; }
        string Id { get; }
        IMaterialCarrierCollection MaterialCarriers { get; }
        string Name { get; }
        string RecipeName { get; }
        DateTime StartTime { get; }

        #endregion Public Properties
    }

    [ComVisible(true),
     ClassInterface(ClassInterfaceType.None),
     ComDefaultInterface(typeof(IProcessJob)),
     Guid(Constants.ProcessJobInterfaceString)]
    public class ProcessJob : IProcessJob
    {
        #region Public Constructors

        public ProcessJob(string recipeName, string name, MaterialCarrier[] materialCarriers, DateTime startTime)
        {
            RecipeName = recipeName;
            CreationTime = startTime;
            Id = Guid.NewGuid().ToString();
            Name = name;
            MaterialCarriers = new MaterialCarrierCollection(materialCarriers);
            FlowRecipe = new FlowRecipeCollection();
            StartTime = startTime;
            FinishTime = startTime;
        }

        #endregion Public Constructors

        #region Public Properties

        public DateTime CreationTime { get; }
        public DateTime FinishTime { get; }
        public IFlowRecipeCollection FlowRecipe { get; }
        public string Id { get; }
        public IMaterialCarrierCollection MaterialCarriers { get; }
        public string Name { get; }
        public string RecipeName { get; }
        public DateTime StartTime { get; }

        #endregion Public Properties
    }
}
