using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("77BDD8BC-56FF-4E4F-B09B-EB04C487F433")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISecsAttribute
    {
        #region Public Properties

        ISecsItem Data { get; set; }

        string Id { get; set; }

        #endregion Public Properties
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISecsAttribute))]
    [Guid("2E1D2003-F7E6-4FC6-A214-73E87FDFA051")]
    public class SecsAttribute : ISecsAttribute
    {
        #region Public Constructors

        public SecsAttribute(string id)
        {
            Id = id;
        }


        public SecsAttribute(string id, SecsItem data)
        {
            Id = id;
            Data = data;
        }

        public SecsAttribute(ISecsAttribute value)
        {
            Id = value.Id;
            Data = new SecsItem(value.Data);
        }

        #endregion Public Constructors

        #region Public Methods

        public string ToIdentString(int identation)
        {
            var data = Data as SecsItem;
            var sb = new StringBuilder();
            sb.AppendLine($"{new string(' ', identation)}{nameof(SecsAttribute)}:");
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Data)}:");
            sb.AppendLine(data.ToIdentString(identation + 2 * Constants.StringIdentation));
            sb.AppendLine($"{new string(' ', identation + Constants.StringIdentation)}{nameof(Id)}: \"{Id}\"");
            sb.AppendLine($"{new string(' ', identation)}/{nameof(SecsAttribute)}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToIdentString(0);
        }

        #endregion Public Methods

        #region Public Properties

        public ISecsItem Data { get; set; }

        public string Id { get; set; }

        #endregion Public Properties
    }
}
