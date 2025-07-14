using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("826FAD1C-3F8C-4026-80B0-837A1FBAFEAB")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITableData_S13F16
    {
        #region Public Properties

        ISecsAttributeList Attributes { get; }

        ISecsErrorList Errors { get; }

        bool TableAck { get; set; }

        ITableElementListList TableElements { get; }

        string TableId { get; set; }

        string TableType { get; set; }

        #endregion Public Properties
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ITableData_S13F16))]
    [Guid("CDF28F32-9FD6-497F-9277-517F05DB6E04")]
    public class TableData_S13F16 : ITableData_S13F16
    {
        #region Public Methods

        public string ToIdentString(int identation)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(TableData_S13F16)}:");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Attributes)}:");
            sb.AppendLine(Attributes.ToIdentString(identation + 2 * Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Errors)}:");
            sb.AppendLine(Errors.ToIdentString(identation + 2 * Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableAck)}: \"{TableAck}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableElements)}:");
            sb.AppendLine(TableElements.ToIdentString(identation + 2 * Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableId)}: \"{TableId}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableType)}: \"{TableType}\"");
            sb.AppendLine($"{new string(' ', identation)}/{nameof(TableData_S13F16)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        public SecsAttributeList Attributes { get; set; }

        ISecsAttributeList ITableData_S13F16.Attributes => Attributes;
        public SecsErrorList Errors { get; set; }

        ISecsErrorList ITableData_S13F16.Errors => Errors;
        public bool TableAck { get; set; }

        public TableElementListList TableElements { get; set; }

        ITableElementListList ITableData_S13F16.TableElements => TableElements;
        public string TableId { get; set; }

        public string TableType { get; set; }

        #endregion Public Properties
    }
}
