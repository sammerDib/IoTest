using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("6ED226FB-6AC0-4E19-B0BD-36B469F289E3")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITableDataRequest
    {
        #region Public Properties

        IComStringList ColumnElementDescriptions { get; }

        string DataId { get; set; }

        string DecimalSeparator { get; set; }

        string ObjSpec { get; set; }

        byte TableCommand { get; set; }

        ISecsItemList TableElements { get; }

        string TableId { get; set; }

        string TableType { get; set; }

        #endregion Public Properties
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ITableDataRequest))]
    [Guid("0592D20E-D111-45D1-B8B3-338F3D148FCC")]
    public class TableDataRequest : ITableDataRequest
    {
        #region Public Methods

        public string ToIdentString(int identation)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(TableDataRequest)}:");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(ColumnElementDescriptions)}:");
            sb.AppendLine(ColumnElementDescriptions.ToIdentString(identation + 2 * Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(DataId)}: \"{DataId}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(DecimalSeparator)}: \"{DecimalSeparator}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(ObjSpec)}: \"{ObjSpec}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableCommand)}: {TableCommand}");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableElements)}:");
            sb.AppendLine(TableElements.ToIdentString(identation + 2 * Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableId)}: \"{TableId}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableType)}: \"{TableType}\"");
            sb.AppendLine($"{new string(' ', identation)}/{nameof(TableDataResponse)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        public IComStringList ColumnElementDescriptions { get; set; }

        IComStringList ITableDataRequest.ColumnElementDescriptions => ColumnElementDescriptions;
        public string DataId { get; set; }

        public string DecimalSeparator { get; set; }

        public string ObjSpec { get; set; }

        public byte TableCommand { get; set; }

        public SecsItemList TableElements { get; set; }

        ISecsItemList ITableDataRequest.TableElements => TableElements;
        public string TableId { get; set; }

        public string TableType { get; set; }

        #endregion Public Properties
    }
}
