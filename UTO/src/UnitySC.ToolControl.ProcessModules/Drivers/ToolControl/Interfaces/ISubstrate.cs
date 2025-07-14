using System;
using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true),
     Guid("FC69C11C-0195-4AF7-A321-03226CFC1D59")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISubstrate
    {
        #region Public Properties

        string AcquiredId { get; }
        DateTime CreationTime { get; }
        string Id { get; }
        DateTime LoadTime { get; }
        string LotId { get; }
        string Name { get; }
        DateTime UnloadTime { get; }

        #endregion Public Properties
    }

    [ComVisible(true),
     ClassInterface(ClassInterfaceType.None),
     ComDefaultInterface(typeof(ISubstrate)),
     Guid(Constants.SubstrateInterfaceString)]
    public class Substrate : ISubstrate
    {
        #region Public Constructors

        public Substrate(string name, string lotId, string acquiredId)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            LotId = lotId;
            AcquiredId = acquiredId;
            CreationTime = DateTime.Now;
            LoadTime = DateTime.Now;
            UnloadTime = DateTime.Now;
        }

        #endregion Public Constructors

        #region Public Properties

        public string AcquiredId { get; }
        public DateTime CreationTime { get; }
        public string Id { get; }
        public DateTime LoadTime { get; }
        public string LotId { get; }
        public string Name { get; }
        public DateTime UnloadTime { get; }

        #endregion Public Properties
    }
}
