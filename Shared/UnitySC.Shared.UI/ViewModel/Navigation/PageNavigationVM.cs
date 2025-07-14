using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ViewModel.Header;

namespace UnitySC.Shared.UI.ViewModel.Navigation
{
    public abstract class PageNavigationVM : HeaderVM
    {
        private ILogger _logger;

        public ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    var currentType = this.GetType();
                    var logger = typeof(ILogger<>);
                    var typedlogger = logger.MakeGenericType(currentType);
                    _logger = (ILogger)ClassLocator.Default.GetInstance(typedlogger);
                }
                return _logger;
            }
        }

        /// <summary>
        /// Page name for header display
        /// </summary>
        public abstract string PageName { get; }

        /// <summary>
        /// Can Navigate from this page to an other
        /// </summary>
        private bool _canNavigate = true;

        public bool CanNavigate
        {
            get => _canNavigate; set { if (_canNavigate != value) { _canNavigate = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// fonction appelée pour signaler à la page qu'elle vient d'être affichée
        /// </summary>
        public virtual void Loaded() { }

        /// <summary>
        /// fonction appelée pour signaler à la page qu'elle ne va plus être affichée
        /// </summary>
        public virtual void Unloading() { }
    }
}