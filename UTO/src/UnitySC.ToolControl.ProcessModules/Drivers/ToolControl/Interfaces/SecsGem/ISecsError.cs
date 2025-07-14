using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("EBEC82EA-0E17-4FE4-8DA1-819632C271D3")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISecsError
    {
        #region Public Properties

        int Code { get; set; }

        string Text { get; set; }

        #endregion Public Properties
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISecsError))]
    [Guid("21684123-D5C6-4C98-B929-205D21E5B752")]
    public class SecsError : ISecsError
    {
        #region Public Constructors

        public SecsError(int code, string text)
        {
            Code = code;
            Text = text;
        }

        #endregion Public Constructors

        #region Public Methods

        public string ToIdentString(int identation)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(SecsError)}:");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Code)}: {Code}");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Text)}: \"{Text}\"");
            sb.AppendLine($"{new string(' ', identation)}/{nameof(SecsError)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        public int Code { get; set; }

        public string Text { get; set; }

        #endregion Public Properties
    }
}
