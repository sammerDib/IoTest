using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AxmountainsLib;

using mountainsLib;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures
{
    public partial class MountainsWinFormsControl : UserControl
    {
        private AxMountainX _axMountainsControl;
        private ILogger _logger;

        public MountainsWinFormsControl()
        {
            InitializeComponent();
            _axMountainsControl = new AxMountainX();
            _axMountainsControl.Show();
            Controls.Add(_axMountainsControl);
            _axMountainsControl.Dock = DockStyle.Fill;
            _logger = ClassLocator.Default.GetInstance<ILogger<MountainsWinFormsControl>>();
        }
               
        public void InitDoc(string filePath = null)
        {
            if (_axMountainsControl == null)
                return;
            _logger.Debug($"Init doc {filePath}");
            if (string.IsNullOrEmpty(filePath))
                _axMountainsControl.NewDocumentEx(false);
            else
                _axMountainsControl.LoadDocument(filePath);
        }

        public void SubstituteStudiable(string filePath)
        {
            if (_axMountainsControl == null)
                return;

            if (File.Exists(filePath))
            {
                var dtnow = DateTime.Now;
                var curDir = Directory.GetCurrentDirectory();
                var tempStudiableFilePath = Path.Combine(curDir, $"Studiable{dtnow:fmttime}.bcrf");

                int studiableID = 1;
                var studiableBytes = File.ReadAllBytes(filePath);
                Converter3DA.ToBCRF(studiableBytes, tempStudiableFilePath);
                _axMountainsControl.SubstituteStudiable(tempStudiableFilePath, studiableID, FileFormat.IDFF_BCR_STM, MultiLayerOpeningMode.ML_LOAD_ALL_LAYERS);

                try
                {
                    File.Delete(tempStudiableFilePath);
                }
                catch
                {
                    ClassLocator.Default.GetInstance<ILogger>().Warning($"Error during delete mountains studiable {tempStudiableFilePath}");
                }
            }
            
        }

        public void ClearContent()
        {
            if (_axMountainsControl?.DocumentsManager == null)
                return;
            _axMountainsControl.DocumentsManager.CloseAllDocuments(true);
        }

        public void Save(string filePath)
        {
            if (_axMountainsControl == null)
                return;
            _axMountainsControl.SaveDocument(filePath);
        }

        public List<ExternalProcessingResultItem> GetResultsDefinedInCurrentTemplate()
        {
            var results = new List<ExternalProcessingResultItem>();
            if (_axMountainsControl?.Document?.ResultManager?.Results == null)
                return results;
            _logger.Debug($"Get results defined in current template ");
            var mountainsResults = _axMountainsControl.Document.ResultManager.Results;
            _logger.Debug($"Number of results defined in template: {mountainsResults.count}");

            for (var x = 0; x < mountainsResults.count; x++)
                results.Add(ResultHolderToPostProcessingData(mountainsResults.element[x]));
            return results;
        }

        private static ExternalProcessingResultItem ResultHolderToPostProcessingData(IResultHolder resultHolder)
        {
            return new ExternalProcessingResultItem
            {
                Description = resultHolder.DecoratedName,
                Name = resultHolder.Name,
                Unit = resultHolder.Unit,
                StringValue = resultHolder.TextValue,
                DoubleValue = resultHolder.TextValue == string.Empty ? (double?)resultHolder.RealValue : null
            };
        }
    }
}
