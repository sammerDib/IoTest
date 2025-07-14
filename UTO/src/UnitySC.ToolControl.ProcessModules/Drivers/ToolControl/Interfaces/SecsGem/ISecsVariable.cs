using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("907F9A7E-8516-43FD-A258-7032C948F357")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISecsVariable
    {
        #region Public Properties

        string Name { get; set; }

        ISecsItem Value { get; set; }

        #endregion Public Properties
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISecsVariable))]
    [Guid("6C69EDE8-9DCE-494E-9D6B-2C88AE96C297")]
    public class SecsVariable : ISecsVariable
    {
        #region Public Constructors

        public SecsVariable(string name)
        {
            Name = name;
            Value = new SecsItem(SecsFormat.Undefined);
        }

        public SecsVariable(string name, SecsItem value)
        {
            Name = name;
            Value = value;
        }

        #endregion Public Constructors

        #region Public Methods

        public string ToIdentString(int identation)
        {
            var value = Value as SecsItem;
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(SecsVariable)}:");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Name)}: {Name}");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Value)}:");
            sb.AppendLine(value.ToIdentString(identation + 2 * Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation)}/{nameof(SecsVariable)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        public string Name { get; set; }

        public ISecsItem Value { get; set; }

        #endregion Public Properties
    }
}
