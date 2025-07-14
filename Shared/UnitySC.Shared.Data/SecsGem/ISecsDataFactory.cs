using System.Runtime.InteropServices;

namespace UnitySC.Shared.Data.SecsGem
{
    [ComVisible(true)]
    [Guid("53D6D51A-F416-446D-90FC-A59B04494DD2")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISecsDataFactory
    {
        #region Public Methods
        ISecsError CreateEmptySecsError();

        ISecsErrorList CreateEmptySecsErrorList();

        ISecsItem CreateEmptySecsItem();

        ISecsItemList CreateEmptySecsItemList();

        ISecsVariable CreateEmptySecsVariable();

        ISecsVariableList CreateEmptySecsVariableList();

        #endregion Public Methods
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISecsDataFactory))]
    [Guid("C12B917D-3919-45AF-993D-D18D18910B0D")]
    public class SecsDataFactory : ISecsDataFactory
    {
        #region Public Constructors

        public SecsDataFactory()
        {

        }

        #endregion Public Constructors

        #region Public Methods

        public ISecsError CreateEmptySecsError() => new SecsError(-1, "");

        public ISecsErrorList CreateEmptySecsErrorList() => new SecsErrorList();

        public ISecsItem CreateEmptySecsItem() => new SecsItem(SecsFormat.Undefined);

        public ISecsItemList CreateEmptySecsItemList() => new SecsItemList();

        public ISecsVariable CreateEmptySecsVariable() => new SecsVariable("");

        public ISecsVariableList CreateEmptySecsVariableList() => new SecsVariableList();

        #endregion Public Methods
    }
}
