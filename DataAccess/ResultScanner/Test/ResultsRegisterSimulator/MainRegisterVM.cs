using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs.FrameworkDialogs.OpenFile;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.ResultScanner.Implementation; // pour le PathComposer
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.Data.Composer;

namespace ResultsRegisterSimulator
{
    public partial class MainRegisterVM : ObservableRecipient
    {
        private ResultSupervisor _resultSupervisor;
        private RegisterSupervisor _registerSupervisor;
        private ILogger _logger;
        private IMessenger _messenger;
        private Random _rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        public ServiceInvoker<IRegisterResultService> Service => _registerSupervisor.Service;
        public DuplexServiceInvoker<IResultService> ResService => _resultSupervisor.Service;

        private string _fmtACQModelDirectory = UnitySC.DataAccess.Service.Implementation.DataAccessConfiguration.Instance.TemplateResultFolderPath;
        private string _fmtACQModelFileName = UnitySC.DataAccess.Service.Implementation.DataAccessConfiguration.Instance.TemplateResultFileName;
        private PathComposer _pathComposer;

        private int _nStepId = -1;
        private int _nRecipeId = -1;

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy; set
            {
                if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); ExecCommand.NotifyCanExecuteChanged(); }
            }
        }

        private string _jobName = string.Empty;

        public string JobName
        {
            get => _jobName; set { if (_jobName != value) { _jobName = value; OnPropertyChanged(); ExecCommand.NotifyCanExecuteChanged(); } }
        }

        private string _lotName = string.Empty;

        public string LotName
        {
            get => _lotName; set { if (_lotName != value) { _lotName = value; OnPropertyChanged(); ExecCommand.NotifyCanExecuteChanged(); } }
        }

        private string _recipeName = string.Empty;

        public string RecipeName
        {
            get => _recipeName; set { if (_recipeName != value) { _recipeName = value; OnPropertyChanged(); ExecCommand.NotifyCanExecuteChanged(); } }
        }

        private string _waferBaseName = string.Empty;

        public string WaferBaseName
        {
            get => _waferBaseName; set { if (_waferBaseName != value) { _waferBaseName = value; OnPropertyChanged(); ExecCommand.NotifyCanExecuteChanged(); } }
        }

        private int _runIter;

        public int RunIter
        {
            get => _runIter; set { if (_runIter != value) { _runIter = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<ProductVM> _products;

        public ObservableCollection<ProductVM> Products
        {
            get => _products; set { if (_products != value) { _products = value; OnPropertyChanged(); } }
        }

        private ProductVM _product;

        public ProductVM SelectedProduct
        {
            get => _product;
            set
            {
                if (_product != value)
                {
                    _product = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ToolVM> _tools;

        public ObservableCollection<ToolVM> Tools
        {
            get => _tools; set { if (_tools != value) { _tools = value; OnPropertyChanged(); } }
        }

        private ToolVM _tool;

        public ToolVM SelectedTool
        {
            get => _tool;
            set
            {
                if (_tool != value)
                {
                    _tool = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(_tool.Chambers));
                }
            }
        }

        private int _nbWaferByFoup = 25;

        public int NbWaferByFoup
        {
            get => _nbWaferByFoup; set { if (_nbWaferByFoup != value) { _nbWaferByFoup = value; OnPropertyChanged(); } }
        }

        private bool _isRandomSlot;

        public bool IsRandomSlot
        {
            get => _isRandomSlot; set { if (_isRandomSlot != value) { _isRandomSlot = value; OnPropertyChanged(); } }
        }

        private bool _isRandomState;

        public bool IsRandomState
        {
            get => _isRandomState; set { if (_isRandomState != value) { _isRandomState = value; OnPropertyChanged(); } }
        }

        private bool _isRandomExtra;

        public bool IsRandomExtra
        {
            get => _isRandomExtra; set { if (_isRandomExtra != value) { _isRandomExtra = value; OnPropertyChanged(); } }
        }

        private int _simuTimeProcessms = 300;

        public int SimuTimeProcessms
        {
            get => _simuTimeProcessms; set { if (_simuTimeProcessms != value) { _simuTimeProcessms = value; OnPropertyChanged(); } }
        }

        private int _extraTimeProcessms = 2000;

        public int ExtraTimeProcessms
        {
            get => _extraTimeProcessms; set { if (_extraTimeProcessms != value) { _extraTimeProcessms = value; OnPropertyChanged(); } }
        }

        private int _progress = 0;

        public int Progress
        {
            get => _progress; set { if (_progress != value) { _progress = value; OnPropertyChanged(); } }
        }

        public MainRegisterVM(RegisterSupervisor registerSupervisor, ILogger<MainRegisterVM> logger, IMessenger messenger, ResultSupervisor resultSupervisor)
        {
            _resultSupervisor = resultSupervisor;
            _registerSupervisor = registerSupervisor;
            _logger = logger;
            _messenger = messenger;
        }

        public void Init()
        {
            _logger.Information("----------------------");
            _logger.Information(" Start Session");

            JobName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            LotName = "RegisterTest";
            RecipeName = "RCP";
            WaferBaseName = "ocr";

            _logger.Information(".");
            _logger.Information("----------------------------");
            _logger.Information("-- Init Path ACQ Composer --");
            _logger.Information("-----------------------------");
            _logger.Information($"-- Template.FileName = ${_fmtACQModelFileName}");
            _logger.Information($"-- Template.FolderPath = ${_fmtACQModelDirectory}");
            _pathComposer = new PathComposer(_fmtACQModelFileName, _fmtACQModelDirectory);
            _logger.Information("-------------------------------------\n");

            try
            {
                Task.Delay(2000).Wait(); // wait in case server is currenlty initializing 

                int nbtrymax = 2;
                int iter = 0;
                bool bOk = false;
                while (iter < nbtrymax & !bOk)
                {
                    try
                    {
                        var toolservice = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), null);
                        var productSteps = toolservice.Invoke(s => s.GetProductAndSteps(false));

                        if (productSteps != null)
                        {
                            foreach (var prod in productSteps)
                            {
                                var stp = prod.Steps.FirstOrDefault();
                                if (stp != null)
                                {
                                    _nStepId = stp.Id;
                                    break;
                                }
                            }
                        }
                        bOk = true; // on sort du while
                    }
                    catch (Exception)
                    {
                        iter++;
                        if (iter >= nbtrymax)
                            throw;

                        Task.Delay(3000).Wait(); // wait in case server is currenlty initializing 
                    }
                }


                var recipeService = new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), null);
                var rcps = recipeService.Invoke(x => x.GetRecipe(ActorType.DEMETER, _nStepId, "PSDSimu", true, false));
                if (rcps == null)
                    MessageBox.Show("recipe <PSDSimu> is missing in DB");
                else
                    _nRecipeId = rcps.Id;
                //var resrecipes = ResService.Invoke(x => x.GetRecipes(null)); // ais job recipes

                var obsP = new ObservableCollection<ProductVM>();
                var products = ResService.Invoke(x => x.GetProducts());
                foreach (var prod in products)
                {
                    obsP.Add(new ProductVM(prod.Id, prod.Name));
                }
                Products = obsP;
                if (Products.Count > 0)
                    SelectedProduct = Products[0];

                var tools = ResService.Invoke(x => x.GetTools());
                var obsT = new ObservableCollection<ToolVM>();
                foreach (var t in tools)
                {
                    var tvm = new ToolVM(t.Id, t.Name);
                    //var toolKey = ResService.Invoke(x => x.RetreiveToolKeyFromToolId(t.Id)).Id;
                    var tchambs = ResService.Invoke(x => x.GetChambers(null, t.Id, false));
                    var obsCh = new ObservableCollection<ChamberVM>();
                    foreach (var ch in tchambs)
                    {
                        obsCh.Add(new ChamberVM(ch.Id, ch.Name, ch.ActorType));
                    }
                    tvm.Chambers = obsCh;
                    obsT.Add(tvm);
                }
                Tools = obsT;
                if (Tools.Count > 0)
                    SelectedTool = Tools[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Check if DataAccess has been launched and can be accessed." + Environment.NewLine + $"Database access error : {ex.Message}", "DataAccess DB Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        public void InitRessources()
        {
        }

        public bool CheckFilesContents()
        {
            string appPath = Directory.GetCurrentDirectory();
            string BasePath = Path.Combine(appPath, "files");

            if (!Directory.Exists(BasePath))
            {
                return WarnNDownloadContent();
            }

            bool missingContents = false;
            for (int i = 0; i < 7; i++)
            {
                // Klarf
                if (!File.Exists($"{BasePath}\\S{i}." + ResultFormatExtension.GetExt(ResultType.ADC_Klarf)))
                {
                    missingContents = true;
                    break;
                }

                // Aso
                if (!File.Exists($"{BasePath}\\S{i}." + ResultFormatExtension.GetExt(ResultType.ADC_ASO)))
                {
                    missingContents = true;
                    break;
                }
            }

            if (!missingContents)
            {
                // check slot 6 thumbnails
                if (!File.Exists($"{BasePath}\\S6.t01"))
                {
                    missingContents = true;
                }
                else
                {
                    if (!Directory.Exists($"{BasePath}\\41570\\Run_9"))
                    {
                        missingContents = true;
                    }
                    else
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            if (!File.Exists($"{BasePath}\\41570\\Run_9\\CLU-{i}-grey.bmp"))
                            {
                                missingContents = true;
                                break;
                            }

                            if (!File.Exists($"{BasePath}\\41570\\Run_9\\CLU-{i}-bw.bmp"))
                            {
                                missingContents = true;
                                break;
                            }
                        }
                        if (!missingContents && !File.Exists($"{BasePath}\\41570\\Run_9\\CLU-8-grey.bmp"))
                        {
                            missingContents = true;
                        }
                    }
                }

                if (!missingContents)
                {
                    // check haze
                    for (int i = 0; i < 6; i++)
                    {
                        // Klarf
                        if (!File.Exists($"{BasePath}\\S{i}." + ResultFormatExtension.GetExt(ResultType.ADC_Haze)))
                        {
                            missingContents = true;
                            break;
                        }
                    }
                }

                if (!missingContents)
                {
                    // check TSV result
                    for (int i = 0; i < 11; i++)
                    {
                        // tsv
                        if (!File.Exists($"{BasePath}\\S{i}." + ResultFormatExtension.GetExt(ResultType.ANALYSE_TSV)))
                        {
                            missingContents = true;
                            break;
                        }
                    }
                }

                if (!missingContents)
                {
                    // check Nanotopo result
                    for (int i = 0; i < 11; i++)
                    {
                        // ntp
                        if (!File.Exists($"{BasePath}\\S{i}." + ResultFormatExtension.GetExt(ResultType.ANALYSE_NanoTopo)))
                        {
                            missingContents = true;
                            break;
                        }
                    }
                }

                if (!missingContents)
                {
                    // check Thickness result
                    for (int i = 0; i < 11; i++)
                    {
                        // ntp
                        if (!File.Exists($"{BasePath}\\S{i}." + ResultFormatExtension.GetExt(ResultType.ANALYSE_Thickness)))
                        {
                            missingContents = true;
                            break;
                        }
                    }
                }

                if (!missingContents)
                {
                    // check Topo result
                    for (int i = 0; i < 11; i++)
                    {
                        // ntp
                        if (!File.Exists($"{BasePath}\\S{i}." + ResultFormatExtension.GetExt(ResultType.ANALYSE_Topography)))
                        {
                            missingContents = true;
                            break;
                        }
                    }
                }
            }

            if (missingContents)
            {
                return WarnNDownloadContent();
            }

            return true;
        }

        public bool WarnNDownloadContent()
        {
            string appPath = Directory.GetCurrentDirectory();
            string ZipDownloadFile = Path.Combine(appPath, "ResultsRegisterSimulator-files.zip");
            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();

            if (!File.Exists(ZipDownloadFile))
            {
                var resultzip = dialogService.ShowMessageBox($"Some Files Content are missing !" + Environment.NewLine +
                 $" ResultsRegisterSimulator-files.zip has not been found, please select zip path", "Missing Content", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (resultzip != MessageBoxResult.OK)
                    return false;

                var settings = new OpenFileDialogSettings
                {
                    Title = "Open ResultsRegisterSimulator-files.zip",
                    Filter = "zip files (*.zip)|*.zip",
                };

                bool? bresult = dialogService.ShowOpenFileDialog(settings);
                if (bresult == true)
                {
                    File.Copy(settings.FileName, ZipDownloadFile);
                }
            }

            var result = dialogService.ShowMessageBox($"Some Files Content are missing !" + Environment.NewLine +
                                                      $"Need to restore data from zip" + Environment.NewLine +
                                                      $"<{ZipDownloadFile}>" + Environment.NewLine +
                                                      $" and extract in App location (Overwrite if newer)" + Environment.NewLine + Environment.NewLine +
                                                      $"Do you want to proceed ?", "Missing Content", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return false;

            bool bSuccess = true;
            try
            {
                //ZipFile.ExtractToDirectory(ZipDownloadFile, appPath); // this will raise exception if file alrady exists

                using (var archive = ZipFile.OpenRead(ZipDownloadFile))
                {
                    foreach (var file in archive.Entries)
                    {
                        string destinationFileName = Path.Combine(appPath, file.FullName);
                        string destinationFilePath = Path.GetDirectoryName(destinationFileName);
                        //Creates the directory (if it doesn't exist) for the new path
                        Directory.CreateDirectory(destinationFilePath);
                        if (!destinationFileName.EndsWith("/")) // folder ends with '/'
                        {
                            bool GOExtract = true;
                            var destFile = new FileInfo(destinationFileName);
                            if (destFile.Exists)
                            {
                                // check date -- overwrite only if newer
                                if (file.LastWriteTime <= destFile.LastWriteTime)
                                {
                                    GOExtract = false;
                                }
                            }
                            if (GOExtract)
                                file.ExtractToFile(destinationFileName, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bSuccess = false;
                MessageBox.Show($"Extraction Failed : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return bSuccess;
        }

        private AutoRelayCommand _execCommand;

        public AutoRelayCommand ExecCommand
        {
            get
            {
                return _execCommand ?? (_execCommand = new AutoRelayCommand(
              () =>
              {
                  IsBusy = true;

                  _logger.Information($" Check Contents");
                  if (!CheckFilesContents())
                  {
                      IsBusy = false;
                      return;
                  }

                  _logger.Information($" Launch registering");
                  var BkgWorker = new BackgroundWorker();
                  BkgWorker.DoWork += new DoWorkEventHandler(BkgWorker_DoWork);
                  BkgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BkgWorker_RunWorkerCompleted);
                  BkgWorker.ProgressChanged += new ProgressChangedEventHandler(BkgWorker_ProgressChanged);
                  BkgWorker.WorkerReportsProgress = true;
                  BkgWorker.RunWorkerAsync();
              },
              () =>
              {
                  if (IsBusy)
                      return false;

                  if (string.IsNullOrEmpty(JobName) || string.IsNullOrEmpty(LotName) || string.IsNullOrEmpty(RecipeName) || string.IsNullOrEmpty(WaferBaseName))
                      return false;

                  if (SelectedTool == null)
                      return false;

                  bool bNoCHamber = true;
                  foreach (var ch in SelectedTool.Chambers)
                  {
                      bNoCHamber &= !ch.IsUsed;
                      if (ch.IsUsed)
                      {
                          bool HasNoResultItemUsed = true;

                          foreach (var resitem in ch.ListResults)
                          {
                              if (resitem.IsUsed)
                              {
                                  HasNoResultItemUsed = false;
                                  break;
                              }
                          }

                          if (!ch.IsAcqUsed)
                          {
                              // Cas SANS Acquisition

                              // si aucun result n'est selctionné on ne lance pas
                              if (HasNoResultItemUsed)
                                  return false;
                          }
                          else
                          {
                              // Cas AVEC Acquisition

                              // si pas d'acquisition item selectionnable
                              if (ch.ChamberAcqVM.ListResTypes.Count == 0)
                                  return false;

                              bool HasNoAcqItemUsed = ch.ChamberAcqVM.ListResTypes.All(s => s.IsUsed == false);
                              // si aucun result d'acquistion n'est selctionné on ne lance pas
                              if (HasNoAcqItemUsed)
                                  return false;
                          }
                      }
                  }
                  if (bNoCHamber)
                      return false;

                  return true;
              }));
            }
        }


        private AutoRelayCommand _execGenerateResFileCommand;

        public AutoRelayCommand ExecGenerateResFileCommand
        {
            get
            {
                return _execGenerateResFileCommand ?? (_execGenerateResFileCommand = new AutoRelayCommand(
              () =>
              {
                  IsBusy = true;

                  _logger.Information($" Launch Generate res File");
                  var BkgWorker = new BackgroundWorker();
                  BkgWorker.DoWork += new DoWorkEventHandler(BkgWorker_GenerateResFile);
                  BkgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BkgWorker_RunWorkerCompleted);
                  BkgWorker.ProgressChanged += new ProgressChangedEventHandler(BkgWorker_ProgressChanged);
                  BkgWorker.WorkerReportsProgress = true;
                  BkgWorker.RunWorkerAsync();
              },
              () =>
              {
                  if (IsBusy)
                      return false;

                  return true;
              }));
            }
        }

        private void BkgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            int total = 0;
            int complete = 0;
            worker.ReportProgress(0);

            int currentToolID = SelectedTool.ID;
            int productID = SelectedProduct.ID;

            var chambers = new Dictionary<int, List<int>>();
            var chambersIdx = new Dictionary<int, List<int>>();
            foreach (var ch in SelectedTool.Chambers)
            {
                if (ch.IsUsed)
                {
                    int chamberID = ch.ID;
                    chambers.Add(chamberID, new List<int>());
                    chambersIdx.Add(chamberID, new List<int>());

                    if (ch.IsAcqUsed)
                    {
                        foreach (var acqitem in ch.ChamberAcqVM.ListResTypes)
                        {
                            if (acqitem.IsUsed)
                            {
                                for (int idx = 0; idx <= acqitem.IdxMax; idx++)
                                {
                                    chambers[chamberID].Add((int)acqitem.ResType);
                                    total += NbWaferByFoup;
                                    chambersIdx[chamberID].Add(idx);
                                }
                            }
                        }
                    }

                    foreach (var resitem in ch.ListResults)
                    {
                        if (resitem.IsUsed)
                        {
                            for (int idx = 0; idx <= resitem.IdxMax; idx++)
                            {
                                chambers[chamberID].Add((int)resitem.ResType);
                                total += NbWaferByFoup;
                                chambersIdx[chamberID].Add(idx);
                            }
                        }
                    }
                }
            }

            var SlotList = new List<int>();
            if (IsRandomSlot)
            {
                if (NbWaferByFoup == 25)
                {
                    for (int i = 1; i <= NbWaferByFoup; i++)
                    {
                        SlotList.Add(i);
                    }
                }
                else if (NbWaferByFoup < 13)
                {
                    // on tire les slots au hasard
                    for (int i = 1; i <= NbWaferByFoup; i++)
                    {
                        int slottry;
                        do
                        {
                            slottry = _rnd.Next(1, 25);
                        } while (SlotList.Contains(slottry));
                        SlotList.Add(slottry);
                    }
                    SlotList.Sort();
                }
                else
                {
                    for (int i = 1; i <= 25; i++)
                    {
                        SlotList.Add(i);
                    }
                    // on enleve les try plutot que de les ajouter
                    for (int i = 1; i <= (25 - NbWaferByFoup); i++)
                    {
                        int slottry;
                        do
                        {
                            slottry = _rnd.Next(1, 25);
                        } while (!SlotList.Contains(slottry));
                        SlotList.Remove(slottry);
                    }
                }
            }
            else
            {
                for (int i = 1; i <= NbWaferByFoup; i++)
                {
                    SlotList.Add(i);
                }
            }

            var startProcess = DateTime.Now;

            foreach (int slotid in SlotList)
            {
                foreach (var chamberkvp in chambers)
                {
                    int nChamberID = chamberkvp.Key;
                    var tsks = new List<Task>();
                    var IdxList = chambersIdx[nChamberID];
                    int nk = 0;
                    foreach (int restype in chamberkvp.Value)
                    {
                        int idx = IdxList[nk];
                        var regtsk = Task.Run(() => PerformRegistering(slotid, (ResultType)restype, currentToolID, nChamberID, productID, startProcess, idx));
                        tsks.Add(regtsk);

                        Task.WaitAll(regtsk);
                        nk++;
                    }

                    Task.WaitAll(tsks.ToArray());

                    foreach (int restype in chamberkvp.Value)
                    {
                        complete++;
                        worker.ReportProgress(CalculateProgress(total, complete));
                    }
                }
            }

            // renew jobname to avoid overwrite
            JobName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        }

        private async Task PerformRegistering(int slotid, ResultType restype, int toolID, int nChamberID, int nProductID, DateTime startprocess, int residx)
        {
            if (restype.GetResultCategory() == ResultCategory.Acquisition)
                await PerformRegisteringAcq(slotid, restype, toolID, nChamberID, nProductID, startprocess, residx);
            else
                await PerformRegisteringResult(slotid, restype, toolID, nChamberID, nProductID, startprocess, residx);
        }

        private async Task PerformRegisteringResult(int slotid, ResultType restype, int toolID, int nChamberID, int nProductID, DateTime startprocess, int residx)
        {
            int delayTime_ms = _simuTimeProcessms;
            if (_isRandomExtra)
                delayTime_ms += _rnd.Next(-_extraTimeProcessms, _extraTimeProcessms);

            // PRE REGISTER
            string reslabelName = residx == 0 ? restype.GetLabelName() : $"{restype.GetLabelName()} {residx}";
            var dtrunTime = DateTime.Now;
            var indata = new InPreRegister()
            {
                JobName = _jobName,
                LotName = _lotName,
                TCRecipeName = _recipeName,
                DateTimeRun = dtrunTime,
                ToolId = toolID,
                ToolKey = -1,

                ChamberId = nChamberID,
                ChamberKey = -1,
                ProductId = nProductID,
                RecipeId = _nRecipeId,

                WaferBaseName = _waferBaseName,
                SlotId = slotid,
                ResultType = restype,
                LabelName = string.Empty, // use automatic default label naming

                Idx = (byte)residx,
            };

            var delaytsk = Task.Delay(delayTime_ms);

            _logger.Information($"Pre Register S={slotid} ch={nChamberID} typ={restype}");
            var outDtoData = Service.Invoke(x => x.PreRegisterResultWithPreRegisterObject(indata));
            int runit = outDtoData.RunIter;
            _logger.Information($" => {JobName} {LotName} {RecipeName} #{runit} => resultid = {outDtoData.InternalDBResId} resultItemid = {outDtoData.InternalDBResItemId}");

            Directory.CreateDirectory(outDtoData.ResultPathRoot);

            string filename = outDtoData.ResultFileName + "." + ResultFormatExtension.GetExt(restype);
            string spath = Path.Combine(outDtoData.ResultPathRoot, filename);

            int slotOrigineTirage = (slotid + residx);
            bool copyThumbs = false;
            int orgid = slotOrigineTirage % 7;
            switch (restype)
            {
                default: break;
                case ResultType.ADC_Klarf:
                    copyThumbs = (orgid == 6);
                    break;
                case ResultType.ADC_ASO:
                    copyThumbs = (orgid == 6);
                    break;
                case ResultType.ADC_Haze:
                    orgid = slotOrigineTirage % 6;
                    break;
                case ResultType.ANALYSE_TSV:
                    orgid = slotOrigineTirage % 12;
                    copyThumbs = (orgid == 11);
                    break;
                case ResultType.ANALYSE_NanoTopo:
                    orgid = slotOrigineTirage % 11;
                    copyThumbs = (orgid == 9);
                    break;
                case ResultType.ANALYSE_Thickness:
                    orgid = slotOrigineTirage % 11;
                    copyThumbs = false;
                    break;
                case ResultType.ANALYSE_Topography:
                    orgid = slotOrigineTirage % 11;
                    copyThumbs = (orgid == 9);
                    break;
                case ResultType.ANALYSE_Step:
                    orgid = slotOrigineTirage % 8;
                    break;
                case ResultType.ANALYSE_Trench:
                    orgid = slotOrigineTirage % 8;
                    break;
                case ResultType.ANALYSE_Bow:
                    orgid = slotOrigineTirage % 8;
                    break;
                case ResultType.ANALYSE_Pillar:
                    orgid = slotOrigineTirage % 6;
                    break;
                case ResultType.ANALYSE_PeriodicStructure:
                    orgid = slotOrigineTirage % 6;
                    break;

            }

            string origin = Path.Combine(".", "files", $"S{orgid}.{restype.GetExt()}");
            if (!File.Exists(origin))
            {
                orgid = 0;
                origin = Path.Combine(".", "files", $"S{orgid}.{restype.GetExt()}");
            }


            bool copyResFile = true;
            if (copyThumbs)
            {
                // copy results thumbnails
                switch (restype)
                {
                    default: break;
                    case ResultType.ADC_Klarf:
                        {
                            _logger.Information($"copy result to {spath}");
                            string KlarfContentTxt = File.ReadAllText(origin);
                            // need to change thumbnail name within klarf file
                            KlarfContentTxt = KlarfContentTxt.Replace($"TiffFileName DEFAULT_EQUIPMENT_ID_41570_D08112019_1108Test_TWND1033.01_LP1_S1_9.t01;", $"TiffFileName {outDtoData.ResultFileName}.t01;");
                            File.WriteAllText(spath, KlarfContentTxt);
                            copyResFile = false; // res file already copied

                            string sthumbpath = Path.Combine(outDtoData.ResultPathRoot, $"{outDtoData.ResultFileName}.t01");
                            string sthumborigin = $".\\files\\S{orgid}.t01";
                            _logger.Information($"copy Klarf multitif thumbnails result to {sthumbpath}");
                            File.Copy(sthumborigin, sthumbpath);
                        }
                        break;
                    case ResultType.ADC_ASO:
                        {
                            string Interfolder = @"41570\Run_9";
                            _logger.Information($"copy ASO thumbnails result to {outDtoData.ResultPathRoot}\\{Interfolder}");
                            var diSource = new DirectoryInfo($".\\files\\{Interfolder}\\");
                            var diTarget = new DirectoryInfo($"{outDtoData.ResultPathRoot}");

                            CopyAll(diSource, diTarget, Interfolder);
                        }
                        break;
                    case ResultType.ANALYSE_TSV:
                        {
                            string Interfolder = $"S{orgid}_TSV_Thumbs";
                            _logger.Information($"copy TSV thumbnails result to {outDtoData.ResultPathRoot}\\{Interfolder}");
                            var diSource = new DirectoryInfo($".\\files\\{Interfolder}\\");
                            var diTarget = new DirectoryInfo($"{outDtoData.ResultPathRoot}");

                            CopyAll(diSource, diTarget, Interfolder);
                        }
                        break;
                    case ResultType.ANALYSE_NanoTopo:
                        {
                            string Interfolder = $"S{orgid}_NTP_Thumbs";
                            _logger.Information($"copy NTP thumbnails result to {outDtoData.ResultPathRoot}\\{Interfolder}");
                            var diSource = new DirectoryInfo($".\\files\\{Interfolder}\\");
                            var diTarget = new DirectoryInfo($"{outDtoData.ResultPathRoot}");

                            CopyAll(diSource, diTarget, Interfolder);
                        }
                        break;
                    case ResultType.ANALYSE_Topography:
                        {
                            string Interfolder = $"S{orgid}_TOPO_Thumbs";
                            _logger.Information($"copy TOPO thumbnails result to {outDtoData.ResultPathRoot}\\{Interfolder}");
                            var diSource = new DirectoryInfo($".\\files\\{Interfolder}\\");
                            var diTarget = new DirectoryInfo($"{outDtoData.ResultPathRoot}");

                            CopyAll(diSource, diTarget, Interfolder);
                        }
                        break;
                }
            }

            if (copyResFile)
            {
                // copy results file
                _logger.Information($"copy result to {spath}");
                File.Copy(origin, spath);
            }

            var resState = ResultState.Ok;
            if (_isRandomState)
                resState = (ResultState)_rnd.Next(0, 4);

            // in order to see some state change in
            await Task.Delay(_rnd.Next(Math.Min(800, delayTime_ms), Math.Min(2300, delayTime_ms + 50)));

            bool bSuccess = Service.Invoke(x => x.UpdateResultState(outDtoData.InternalDBResItemId, resState));

            await Task.WhenAll(delaytsk);
        }

        private async Task PerformRegisteringAcq(int slotid, ResultType restype, int toolID, int nChamberID, int nProductID, DateTime startprocess, int residx)
        {
            int delayTime_ms = _simuTimeProcessms;
            if (_isRandomExtra)
                delayTime_ms += _rnd.Next(-_extraTimeProcessms, _extraTimeProcessms);

            /// ICI pour la simu on va générer les au meme endroit que les resultats d'où l'utilisation du PathComposer
            string reslabelName = residx == 0 ? restype.GetLabelName() : $"{restype.GetLabelName()} {residx}";
            var prm = new ResultPathParams
            {
                ToolName = SelectedTool.Name,
                ToolId = toolID,
                ToolKey = -1,
                ChamberName = SelectedTool.Chambers.ToList().Find(ch => ch.ID == nChamberID).Name,
                ChamberId = nChamberID,
                ChamberKey = -1,
                JobName = _jobName,
                JobId = 666,
                LotName = _lotName,
                RecipeName = _recipeName,
                StartProcessDate = startprocess,
                Slot = slotid,
                RunIter = 0,
                WaferName = _waferBaseName,
                ResultType = restype,
                Index = residx,
                Label = reslabelName
            };
            string PathRoot = _pathComposer.GetDirPath(prm);
            string PathFileName = _pathComposer.GetFileName(prm);
            PathFileName += ".png";

            // PRE REGISTER
            var dtrunTime = DateTime.Now;
            var indata = new InPreRegisterAcquisition()
            {
                JobName = _jobName,
                LotName = _lotName,
                TCRecipeName = _recipeName,
                DateTimeRun = dtrunTime,
                ToolId = toolID,
                ToolKey = -1,

                ChamberId = nChamberID,
                ChamberKey = -1,
                ProductId = nProductID,
                RecipeId = _nRecipeId,

                WaferBaseName = _waferBaseName,
                SlotId = slotid,
                ResultType = restype,
                LabelName = string.Empty, // use automatic default label naming

                Idx = (byte)residx,

                PathName = PathRoot,
                FileName = PathFileName,
            };

            var delaytsk = Task.Delay(delayTime_ms);

            _logger.Information($"Pre Register ACQUISITION S={slotid} ch={nChamberID} typ={restype}");
            var outDtoData = Service.Invoke(x => x.PreRegisterAcquisitionWithPreRegisterObject(indata));
            _logger.Information($" => {JobName} {LotName} {RecipeName} => resulttACQid = {outDtoData.InternalDBResId} resultACQItemid = {outDtoData.InternalDBResItemId}");
            indata = outDtoData.Inputs;

            var resState = ResultState.Ok;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(indata.FullAcqFilePath()));

                using (var MyAcqSmile = MakeSmileImage(indata.SlotId, indata.LabelName))
                {
                    // Main result
                    int destWidth = (int)(MyAcqSmile.Width * 3);
                    int destHeight = (int)(MyAcqSmile.Height * 3);
                    using (var b = new Bitmap(destWidth, destHeight))
                    {
                        using (var g = Graphics.FromImage((System.Drawing.Image)b))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                            g.DrawImage(MyAcqSmile, 0, 0, destWidth, destHeight);
                        }
                        b.Save(indata.FullAcqFilePath(), System.Drawing.Imaging.ImageFormat.Png);
                    }
                    //MyAcqSmile.Save(indata.FullAcqFilePath(), System.Drawing.Imaging.ImageFormat.Png);

                    // Thumbnail
                    MyAcqSmile.Save(indata.FullAcqFilePathThumbnail(), System.Drawing.Imaging.ImageFormat.Png);
                }

                if (_isRandomState)
                    resState = (ResultState)_rnd.Next(0, 4);
            }
            catch (Exception ex)
            {
                _logger.Error($"Generation ACQUISITION Error S={slotid} ch={nChamberID} typ={restype} <{indata.FullAcqFilePath()}>");
                _logger.Error($"Exception message = {ex.Message}");
                resState = ResultState.Error;
            }


            // in order to see some state change in
            await Task.Delay(_rnd.Next(Math.Min(800, delayTime_ms), Math.Min(2300, delayTime_ms + 50)));

            bool bSuccess = Service.Invoke(x => x.UpdateResultAcquisitionState(outDtoData.InternalDBResItemId, resState, null));
            if (!bSuccess)
                _logger.Error($"Update ACQUISITION Error S={slotid} ch={nChamberID} typ={restype} <{indata.FullAcqFilePath()}>");

            await Task.WhenAll(delaytsk);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target, string interfolder)
        {
            string TargetPath = Path.Combine(target.FullName, $"{interfolder}");
            Directory.CreateDirectory(TargetPath);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", TargetPath, fi.Name);
                fi.CopyTo(Path.Combine(TargetPath, fi.Name), true);
            }
        }

        private static int CalculateProgress(int total, int complete)
        {
            // avoid divide by zero error
            if (total == 0) return 0;
            // calculate percentage complete
            double result = (double)complete / (double)total;
            double percentage = result * 100.0;
            // make sure result is within bounds and return as integer;
            return Math.Max(0, Math.Min(100, (int)percentage));
        }

        private void BkgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        private void BkgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
        }

        public void ExecRegistering(double delaytimems)
        {
            _runIter++;

            // Notify UI
            //..........
            Application.Current?.Dispatcher.Invoke(() =>
            {
                OnPropertyChanged(nameof(RunIter));
            });
        }

        private Bitmap MakeSmileImage(int slotid, string labelName)
        {
            int nMarginpx = 1;
            int nThumbSizepx = 256;
            var ThumbBmp = new Bitmap(nThumbSizepx, nThumbSizepx);
            var gImage = Graphics.FromImage(ThumbBmp);
            gImage.Clear(Color.Black);

            var WaferRectImage = new Rectangle(nMarginpx, nMarginpx, nThumbSizepx - 2 * nMarginpx, nThumbSizepx - 2 * nMarginpx); // pixel margin
            var MyWhitePen = new Pen(Color.White, 2.0F);
            float Cx = nMarginpx + (float)WaferRectImage.Width * 0.5F;
            float Cy = nMarginpx + (float)WaferRectImage.Height * 0.5F;
            gImage.FillEllipse(new Pen(Color.FromArgb(80, 80, 80)).Brush, WaferRectImage);

            var rnd = new Random();
            int nXoff = rnd.Next(-15, 15);
            int nYoff = rnd.Next(-18, 18);

            gImage.FillEllipse(new Pen(Color.FromArgb(250, 250, 250)).Brush, 50, 50, 50, 70);
            gImage.FillEllipse(new Pen(Color.FromArgb(10, 10, 10)).Brush, 62 + nXoff, 65 + nYoff, 25, 40);

            gImage.FillEllipse(new Pen(Color.FromArgb(250, 250, 250)).Brush, 150, 50, 50, 70);
            gImage.FillEllipse(new Pen(Color.FromArgb(10, 10, 10)).Brush, 162 + nXoff, 65 + nYoff, 25, 40);

            //Create array of points for curve.
            int nSmile1 = rnd.Next(10, 60);
            int nSmile2 = rnd.Next(-20, 30);
            var point1 = new System.Drawing.Point(80, 180);
            var point2 = new System.Drawing.Point(125, 180 + nSmile1);
            var point3 = new System.Drawing.Point(170, 180);
            var point4 = new System.Drawing.Point(125, 180 - nSmile2);
            System.Drawing.Point[] points = { point1, point2, point3, point4, point1 };
            gImage.FillClosedCurve(new Pen(Color.FromArgb(80, 10, 10)).Brush, points);

            using (var zefont = new Font("Arial", 10.0f))
            {
                int nFHeig = 20;
                var SlotLabelrect = new Rectangle(nMarginpx, nMarginpx, nThumbSizepx, nFHeig);
                var stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Near;
                gImage.DrawString($"S{slotid:00}", zefont, Brushes.BlueViolet, SlotLabelrect, stringFormat);

                var Labelrect = new Rectangle(nMarginpx, nThumbSizepx - nMarginpx - nFHeig, nThumbSizepx - 1, nFHeig - 1);
                stringFormat.Alignment = StringAlignment.Far;
                stringFormat.LineAlignment = StringAlignment.Far;
                //gImage.DrawRectangle(new Pen(Brushes.DarkRed, 2), Labelrect);
                gImage.DrawString($"{labelName}", zefont, Brushes.Red, Labelrect, stringFormat);
            }

            ThumbBmp.MakeTransparent(Color.Black);

            return ThumbBmp;
        }
    }
}
