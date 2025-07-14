using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("7F4F98AA-4D9E-46C7-B6A3-98474E2A0D4A")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITableData_S13F13
    {
        #region Public Properties

        ISecsAttributeList Attributes { get; }
        string DataId { get; set; }

        string ObjSpec { get; set; }

        byte TableCommand { get; set; }
        ITableElementListList TableElements { get; }

        string TableId { get; set; }

        string TableType { get; set; }

        #endregion Public Properties
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ITableData_S13F13))]
    [Guid("93AB5F46-E5B8-4AE7-BE96-562EE5DD142D")]
    public class TableData_S13F13 : ITableData_S13F13
    {
        #region Public Methods

        public string ToIdentString(int identation)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(TableData_S13F13)}:");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(DataId)}: \"{DataId}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(ObjSpec)}: \"{ObjSpec}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableCommand)}: {TableCommand}");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Attributes)}:");
            sb.AppendLine($"{Attributes.ToIdentString(identation + 2 * Constants.StringIdentation)}");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableElements)}:");
            sb.AppendLine($"{TableElements.ToIdentString(identation + 2 * Constants.StringIdentation)}");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableId)}: \"{TableId}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableType)}: \"{TableType}\"");
            sb.AppendLine($"{new string(' ', identation)}/{nameof(TableData_S13F13)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        public SecsAttributeList Attributes { get; set; }
        ISecsAttributeList ITableData_S13F13.Attributes => Attributes;
        public string DataId { get; set; }

        public string ObjSpec { get; set; }

        public byte TableCommand { get; set; }
        public TableElementListList TableElements { get; set; }

        ITableElementListList ITableData_S13F13.TableElements => TableElements;
        public string TableId { get; set; }

        public string TableType { get; set; }

        #endregion Public Properties
    }
}
