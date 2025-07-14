using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("5A94E797-3E16-45B2-9BCF-219A49153FFF")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITableDataResponse
    {
        #region Public Properties

        ISecsErrorList Errors { get; }

        bool TableAck { get; set; }

        #endregion Public Properties
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ITableDataResponse))]
    [Guid("F897D493-5CC5-4193-A2FB-39D43969D560")]
    public class TableDataResponse : ITableDataResponse
    {
        #region Public Constructors

        public TableDataResponse(bool tableAck)
        {
            TableAck = tableAck;
        }

        public TableDataResponse(bool tableAck, SecsErrorList errors)
        {
            TableAck = tableAck;
            Errors = errors;
        }

        #endregion Public Constructors

        #region Public Methods

        public string ToIdentString(int identation)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(TableDataResponse)}:");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(TableAck)}: {TableAck}");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Errors)}:");
            sb.AppendLine(Errors.ToIdentString(identation + 2 * Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation)}/{nameof(TableDataResponse)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        public SecsErrorList Errors { get; set; }

        ISecsErrorList ITableDataResponse.Errors => Errors;
        public bool TableAck { get; set; }

        #endregion Public Properties
    }
}
