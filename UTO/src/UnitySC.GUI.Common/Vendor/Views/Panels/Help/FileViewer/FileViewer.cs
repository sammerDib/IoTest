using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

using Agileo.Common.Tracing;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.FileViewer
{
    /// <summary>
    /// Inheritance from the A²ECF ViewModel to allow associating
    /// local custom views with framework ViewModel
    /// </summary>
    public class FileViewer : BusinessPanel
    {
        #region Constructor

        public FileViewer() : this(string.Empty, nameof(L10N.BP_USER_MANUAL), PathIcon.FileViewer)
        {

        }

        public FileViewer(string filePath, string id, IIcon icon = null) : base(id, icon)
        {
            _filePath = filePath;
            Refresh();
        }

        public FileViewer(string id, IIcon icon = null) : this(string.Empty, id, icon)
        {
            //Under developer responsibility to load the file
        }

        #endregion

        private string _filePath;
        private IDocumentPaginatorSource _document;

        /// <summary>Gets or sets the document.</summary>
        /// <value>The document.</value>
        public IDocumentPaginatorSource Document
        {
            get
            {
                return _document;
            }
            private set
            {
                _document = value;
                OnPropertyChanged(nameof(Document));
            }
        }

        public void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                TraceManager.Instance().Trace(TraceLevelType.Error, $"File {filePath} doesn't exist. Cannot be loaded");
                return;
            }

            _filePath = filePath;
            Document = null;
            Refresh();
        }

        private void Refresh()
        {
            if (Document != null || string.IsNullOrEmpty(_filePath))
                return;
            var xpsDocument = new XpsDocument(_filePath, FileAccess.Read);
            Application.Current.Dispatcher.Invoke(delegate { Document = xpsDocument.GetFixedDocumentSequence(); });
            xpsDocument.Close();
        }

        public override void OnShow()
        {
            base.OnShow();
            Refresh();
        }

        public override void OnHide()
        {
            base.OnHide();
            Document = null;
        }
    }
}
