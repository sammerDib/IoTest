using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace UnitySC.Shared.Data.SecsGem
{
    public interface ISecsVariable
    {
        #region Public Properties

        string Name { get; set; }

        SecsItem Value { get; set; }

        #endregion Public Properties
    }

    [DataContract]
    public class SecsVariable : ISecsVariable
    {
        #region Public Constructors

        public SecsVariable()
        {

        }

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

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public SecsItem Value { get; set; }

        #endregion Public Properties
    }
}
