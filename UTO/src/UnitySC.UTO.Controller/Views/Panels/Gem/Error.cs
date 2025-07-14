
namespace UnitySC.UTO.Controller.Views.Panels.Gem
{
    public class Error
    {
        #region Constructor

        public Error()
        {
        }

        public Error(string id, string description)
        {
            Id = id;
            Description = description;
        }

        #endregion

        #region Properties

        public string Id { get; }
        public string Description { get; }

        #endregion
    }
}
