using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("4E46E455-B851-44AE-8BFA-E8980581ACEB")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITableElement
    {
        #region Public Properties

        string ColumnElementDescription { get; set; }

        ISecsItem Data { get; set; }

        #endregion Public Properties
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ITableElement))]
    [Guid("5F9BE014-1250-493F-BA9E-D837F01BFAD2")]
    public class TableElement : ITableElement
    {
        #region Public Constructors

        public TableElement(string columnElementDescription)
        {
            ColumnElementDescription = columnElementDescription;
        }

        public TableElement(string columnElementDescription, SecsItem data) : this(columnElementDescription)
        {
            Data = data;
        }

        #endregion Public Constructors

        #region Public Methods

        public string ToIdentString(int identation)
        {
            var data = Data as SecsItem;
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(TableElement)}:");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(ColumnElementDescription)}: \"{ColumnElementDescription}\"");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Data)}:");
            sb.AppendLine(data.ToIdentString(identation + 2 * Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation)}/{nameof(TableElement)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        public string ColumnElementDescription { get; set; }

        public ISecsItem Data { get; set; }

        #endregion Public Properties
    }
}
