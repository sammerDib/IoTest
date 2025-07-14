using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Windows.Forms;

using AxmountainsLib;

using mountainsLib;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Server.Implementation;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.EP.Mountains.Server.ActiveXHost
{
    public partial class MountainsForm : Form
    {
        private ILogger _logger;
        private Dictionary<BaseService, ServiceHost> _hosts;
        private IMountainsActiveXService _activeXService;
        private readonly AxMountainX _mountainsControl;
        public static MountainsForm CurrentInstance { get; } = new MountainsForm();

        public MountainsForm()
        {
            try
            {
                InitializeComponent();
                _logger = ClassLocator.Default.GetInstance<ILogger<MountainsForm>>();
                _activeXService = ClassLocator.Default.GetInstance<IMountainsActiveXService>();
                _hosts = new Dictionary<BaseService, ServiceHost>();
                StartService((BaseService)_activeXService);
                FormClosing += MountainsForm_FormClosing;
                Visible = true;
                // Add ActiveX
                _mountainsControl = new AxMountainX();
                _mountainsControl.Show();

                groupBoxMountains.Controls.Add(_mountainsControl);
                _mountainsControl.Dock = DockStyle.Fill;
                this.WindowState = FormWindowState.Minimized;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Main MountainsForm error in MountainsForm");
                Application.Exit();
            }
        }

        private void MountainsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormClosing -= MountainsForm_FormClosing;
            StopAllServiceHost();
        }

        private void StartService(BaseService service)
        {
            service.Init();
            var host = new ServiceHost(service);
            foreach (var endpoint in host.Description.Endpoints)
                _logger.Information($"Creating {host.Description.Name} service on {endpoint.Address}");
            host.Open();
            _hosts.Add(service, host);
        }

        private void StopService(BaseService service, ServiceHost host)
        {
            _logger.Information($"Stop {host.Description.Name} service..");
            host.Close();
            service.Shutdown();
        }

        private void StopAllServiceHost()
        {
            foreach (var kvp in _hosts)
                StopService(service: kvp.Key, host: kvp.Value);
        }

        public List<ExternalProcessingResultItem> Execute(MountainsExecutionParameters mountainsExecutionParameters)
        {
            var results = new List<ExternalProcessingResultItem>();

            _logger.Information($"Executing template: {mountainsExecutionParameters.TemplateFile} point X{mountainsExecutionParameters.PointData.XCoordinate} Y {mountainsExecutionParameters.PointData.YCoordinate}");

            try
            {
                // Load template
                LoadTemplate(mountainsExecutionParameters.TemplateFile);

                // Substitute studiables
                LoadStudiables(mountainsExecutionParameters.PointData.StudiableFile);

                // Read and create result
                var mountainsResults = _mountainsControl.Document.ResultManager.Results;
                _logger.Information($"Number of results returned: {mountainsResults.count}");

                for (var x = 0; x < mountainsResults.count; x++)
                    results.Add(ResultHolderToPostProcessingData(mountainsResults.element[x]));

                // Export result
                DocumentExportFormat(mountainsExecutionParameters.PrintPDF,
                                        mountainsExecutionParameters.SaveCSV,
                                        mountainsExecutionParameters.SaveResultFile,
                                        mountainsExecutionParameters.ResultFolderPath,
                                        mountainsExecutionParameters.ResultFileName,
                                        mountainsExecutionParameters.PointData.PointNumber,
                                        Path.GetFileNameWithoutExtension(mountainsExecutionParameters.TemplateFile));

                // Use Statistic
                CreateStatistics(mountainsExecutionParameters.UseStatistics,
                                    mountainsExecutionParameters.OpenStatistics,
                                    mountainsExecutionParameters.StatisticsDocumentFilePath,
                                    mountainsExecutionParameters.ResultFolderPath);

                _logger.Information($"Returning {mountainsResults.count} results to gateway service");
                bool endOfSession = false;
                _mountainsControl.DocumentsManager.CloseAllDocuments(endOfSession);
                return results;
            }
            catch (Exception ex)
            {
                bool endOfSession = false;
                _mountainsControl.DocumentsManager.CloseAllDocuments(endOfSession);
                throw new Exception($"[MountainsForm] Execute Exception {0}", ex);
            }
        }

        public List<ExternalProcessingResultItem> GetResultsDefinedInTemplate(string templateName)
        {
            _logger.Information($"Get results defined for template: {templateName}");

            var results = new List<ExternalProcessingResultItem>();
            LoadTemplate(templateName);
            var mountainsResults = _mountainsControl.Document.ResultManager.Results;
            bool endOfSession = false;
            _mountainsControl.DocumentsManager.CloseAllDocuments(endOfSession);
            _logger.Information($"Number of results defined in template: {mountainsResults.count}");

            for (var x = 0; x < mountainsResults.count; x++)
                results.Add(ResultHolderToPostProcessingData(mountainsResults.element[x]));
            return results;
        }

        /// <summary>
        ///     Create the statistics file
        /// </summary>
        /// <param name="useStatistics">Create a statistics file</param>
        /// <param name="openStatistics">Open the statistics file</param>
        /// <param name="statisticsDocumentFilePath">Statistics filePath</param>
        /// <param name="resultFolderPath">Result folder path</param>
        private void CreateStatistics(bool useStatistics, bool openStatistics, string statisticsDocumentFilePath, string resultFolderPath)
        {
            if (useStatistics)
            {
                var staticsResultFile = Path.Combine(resultFolderPath, Path.GetFileName(statisticsDocumentFilePath));

                // Create a new statisics file if the file not already exist
                if (!File.Exists(staticsResultFile))
                {
                    File.Copy(statisticsDocumentFilePath, staticsResultFile);

                    if (openStatistics)
                    {
                        Process.Start(staticsResultFile);
                    }
                }
            }
        }

        /// <summary>
        ///     Export result
        /// </summary>
        /// <param name="printPDF">True to print PDF</param>
        /// <param name="saveCSV">True to save CSV File</param>
        /// <param name="saveResultFile">True to save result file</param>
        /// <param name="resultFolderPath">result file path</param>
        /// <param name="pointNumber">Point Identifier</param>
        private void DocumentExportFormat(bool printPDF, bool saveCSV, bool saveResultFile, string resultFolderPath, string resultFileName, double pointNumber, string templateName)
        {
            if (printPDF || saveCSV || saveResultFile)
            {
                // try and create the result folder if not there, then throw a fault
                if (!Directory.Exists(resultFolderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(resultFolderPath);

                        if (!Directory.Exists(resultFolderPath))
                        {
                            throw new Exception(string.Format("[MountainsActiveX] Result directory {0} : Directory not found exception", resultFolderPath));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("[MountainsActiveX] - Create result directory error : " + ex.Message, ex);
                    }
                }


                if (printPDF)
                {
                    var pdfFileName = Path.Combine(resultFolderPath, string.Concat(resultFileName, ".pdf"));
                    _mountainsControl.PrintAsPDF(pdfFileName, 600, false);
                }

                if (saveCSV)
                {
                    var csvFileName = Path.Combine(resultFolderPath, string.Concat(resultFileName, ".csv"));

                    var ResultManager4 = (IResultManager4)_mountainsControl.Document.ResultManager;
                    int numberResults;
                    ResultManager4.ExportResultsByColumn(csvFileName, out numberResults);
                }

                if (saveResultFile)
                {
                    var mntFileName = Path.Combine(resultFolderPath, string.Concat(resultFileName, ".mnt"));

                    _mountainsControl.SaveDocument(mntFileName);
                }
            }
        }

        private void LoadTemplate(string templateFilePath)
        {
            if (File.Exists(templateFilePath))
            {
                _logger.Information($"Loading template file:  {templateFilePath}");
                _mountainsControl.LoadDocument(templateFilePath);
            }
            else
            {
                _logger.Error($"Template file: {templateFilePath} not found.");
                throw new Exception($"[MountainsForm] TemplateFilePath {templateFilePath} : File not found exception");
            }
        }

        private void LoadStudiables(string studiableFile)
        {
            var studiableIndex = 1;

            // Substitute studiables of the loaded template with the template to execute.
            //foreach (var file in studiableFilesPath)
            if (File.Exists(studiableFile))
            {
                _logger.Information($"Loading studiable file: {studiableFile}");
                _mountainsControl.SubstituteStudiable(studiableFile, studiableIndex, FileFormat.IDFF_BCR_STM, MultiLayerOpeningMode.ML_LOAD_ALL_LAYERS);
                // studiableIndex++;
            }
            else
            {
                _logger.Information($"Studiable file: {studiableFile} not found.");
                throw new Exception($"[MountainsForm] StudiableFilePath {studiableFile} : File not found exception");
            }
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
