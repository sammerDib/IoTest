using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace WpfUnityControlRegisterValidation
{
    public class MainRegisterVM : ViewModelBase
    {
        private RegisterSupervisor _registerSupervisor;
        private ILogger _logger;
        private IMessenger _messenger;
        private Random _rnd = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
        public ServiceInvoker<IRegisterResultService> Service => _registerSupervisor.Service;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); }
            }
        }

        private bool _isPSD21 = true;
        public bool IsPSD21
        {
            get => _isPSD21; set { if (_isPSD21 != value) { _isPSD21 = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }

        private bool _isKlarf21 = true;
        public bool IsKlarf21
        {
            get => _isKlarf21; set { if (_isKlarf21 != value) { _isKlarf21 = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }

        private bool _isASO21;
        public bool IsASO21
        {
            get => _isASO21; set { if (_isASO21 != value) { _isASO21 = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }

        private bool _isBF2D27;
        public bool IsBF2D27
        {
            get => _isBF2D27; set { if (_isBF2D27 != value) { _isBF2D27 = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }

        private bool _isKlarf27;
        public bool IsKlarf27
        {
            get => _isKlarf27; set { if (_isKlarf27 != value) { _isKlarf27 = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }

        private bool _isASO27;
        public bool IsASO27
        {
            get => _isASO27; set { if (_isASO27 != value) { _isASO27 = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }

        private string _jobName = String.Empty;
        public string JobName
        {
            get => _jobName; set { if (_jobName != value) { _jobName = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }

        private string _lotName = String.Empty;
        public string LotName
        {
            get => _lotName; set { if (_lotName != value) { _lotName = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }

        private string _recipeName = String.Empty;
        public string RecipeName
        {
            get => _recipeName; set { if (_recipeName != value) { _recipeName = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }

        private string _waferBaseName = String.Empty;
        public string WaferBaseName
        {
            get => _waferBaseName; set { if (_waferBaseName != value) { _waferBaseName = value; RaisePropertyChanged(); ExecCommand.RaiseCanExecuteChanged(); } }
        }
          
        private int _runIter;
        public int RunIter
        {
            get => _runIter; set { if (_runIter != value) { _runIter = value; RaisePropertyChanged(); } }
        }

        private int _nbWaferByFoup = 25;
        public int NbWaferByFoup
        {
            get => _nbWaferByFoup; set { if (_nbWaferByFoup != value) { _nbWaferByFoup = value; RaisePropertyChanged(); } }
        }

        private bool _isRandomSlot;
        public bool IsRandomSlot
        {
            get => _isRandomSlot; set { if (_isRandomSlot != value) { _isRandomSlot = value; RaisePropertyChanged(); } }
        }

        private bool _isRandomState;
        public bool IsRandomState
        {
            get => _isRandomState; set { if (_isRandomState != value) { _isRandomState = value; RaisePropertyChanged(); } }
        }

        private bool _isRandomExtra;
        public bool IsRandomExtra
        {
            get => _isRandomExtra; set { if (_isRandomExtra != value) { _isRandomExtra = value; RaisePropertyChanged(); } }
        }

        private int _simuTimeProcessms = 3000;
        public int SimuTimeProcessms
        {
            get => _simuTimeProcessms; set { if (_simuTimeProcessms != value) { _simuTimeProcessms = value; RaisePropertyChanged(); } }
        }

        private int _extraTimeProcessms = 2000;
        public int ExtraTimeProcessms
        {
            get => _extraTimeProcessms; set { if (_extraTimeProcessms != value) { _extraTimeProcessms = value; RaisePropertyChanged(); } }
        }

        private int _progress = 0;
        public int Progress
        {
            get => _progress; set { if (_progress != value) { _progress = value; RaisePropertyChanged(); } }
        }

        public MainRegisterVM(RegisterSupervisor resultSupervisor, ILogger<MainRegisterVM> logger, IMessenger messenger)
        {
            _registerSupervisor = resultSupervisor;
            _logger = logger;
            _messenger = messenger;
        }

        public void Init()
        {
            _logger.Information("----------------------");
            _logger.Information(" Start Seesion");

            JobName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            LotName = "RegisterTest";
            RecipeName = "RCP";
            WaferBaseName = "ocr";
        }

        public void InitRessources()
        {
         
        }

        private RelayCommand _execCommand;
        public RelayCommand ExecCommand
        {
            get
            {
                return _execCommand ?? (_execCommand = new RelayCommand(
              () =>
              {
                  IsBusy = true;
                  _logger.Information($" Launch registering");
                  BackgroundWorker BkgWorker = new BackgroundWorker();
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

                  if (!IsPSD21 && !IsBF2D27)
                      return false;

                  if (String.IsNullOrEmpty(JobName) || String.IsNullOrEmpty(LotName) || String.IsNullOrEmpty(RecipeName) || String.IsNullOrEmpty(WaferBaseName))
                      return false;

                  if (IsPSD21)
                  {
                      if (!IsKlarf21 && !IsASO21)
                          return false;
                  }

                  if (IsBF2D27)
                  {
                      if (!IsKlarf27 && !IsASO27)
                          return false;
                  }
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

            int ToolID = 3; //tsvhandy fixe pour test
            int nProductID = 3; // fixe pour test  300mm notch (3) 200mm notch (4)
            var chambers = new Dictionary<int, List<int>>();
            if (IsPSD21)
            {
                chambers.Add(21, new List<int>());
                if (IsKlarf21)
                {
                    chambers[21].Add((int)ResultType.Klarf);
                    total += NbWaferByFoup;
                }
                if (IsASO21)
                {
                    chambers[21].Add((int)ResultType.ASO);
                    total += NbWaferByFoup;
                }
            }
            if (IsBF2D27)
            {
                chambers.Add(27, new List<int>());
                if (IsKlarf27)
                {
                    chambers[27].Add((int)ResultType.Klarf);
                    total += NbWaferByFoup;
                }
                if (IsASO27)
                {
                    chambers[27].Add((int)ResultType.ASO);
                    total += NbWaferByFoup;
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
                    for (int i = 1; i <= (25-NbWaferByFoup); i++)
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

            DateTime startProcess = DateTime.Now;

            foreach (var slotid in SlotList)
            {
                foreach (var chamberkvp in chambers)
                {
                    int nChamberID = chamberkvp.Key;
                    List<Task> tsks = new List<Task>();
                    foreach (var restype in chamberkvp.Value)
                    {
                        Task regtsk = Task.Run(() => PerformRegistering(slotid, restype, nChamberID, nProductID, startProcess));
                        tsks.Add(regtsk);                          
                    }
                    Task.WaitAll(tsks.ToArray());

                    foreach (var restype in chamberkvp.Value)
                    {
                        complete++;
                        worker.ReportProgress(CalculateProgress(total, complete));
                    }
                }
            }
        }

        private async Task PerformRegistering(int slotid, int restype, int nChamberID, int nProductID, DateTime startprocess)
        {

            int delayTime_ms = _simuTimeProcessms;
            if (_isRandomExtra)
                delayTime_ms += _rnd.Next(-_extraTimeProcessms, _extraTimeProcessms);

            if (delayTime_ms <= 50)
                delayTime_ms = 50;

            // PRE REGISTER
            var dtrunTime = DateTime.Now;
            Job job = new Job()
            {
                JobName = _jobName,
                LotName = _lotName,
                Date = startprocess,
                RecipeName = _recipeName,
                RunIter = 0,
            };

            WaferResult waferResult = new WaferResult()
            {
                SlotId = slotid,
                WaferName = _waferBaseName,
                Date = dtrunTime,
                ProductId = nProductID
            };

            Result result = new Result()
            {
                Type = restype,
                Date = dtrunTime,
                State = (int)ResultState.NotProcess,
                InternalState = 0, // aka not process by default
                                   //FileName = regprm.sBaseName, // Pas besoin ici 
                ChamberId = nChamberID,
                ChannelIds = String.Empty
            };

            Task delaytsk = Task.Delay(delayTime_ms);

            _logger.Information($"Pre Register S={slotid} ch={nChamberID} typ={restype}");
            OutPreRegister outDtoData = Service.Invoke(x => x.AddResult(job, waferResult, result));
            int runit = outDtoData.RunIter;
            _logger.Information($" => {JobName} {LotName} {RecipeName} #{runit} => resultid = {outDtoData.InternalDBResId}");

            Directory.CreateDirectory(outDtoData.ResultPathRoot);

            string filename = outDtoData.ResultFileName + "." + ResultTypeExtension.GetExt(restype);
            string spath = Path.Combine(outDtoData.ResultPathRoot, filename);

            int orgid = slotid % 5;
            string origin = $"E:\\AltaBDResults\\S{orgid}." + ResultTypeExtension.GetExt(restype);
            _logger.Information($"copy result to {spath}");
            File.Copy(origin, spath);

            ResultState resState = ResultState.Ok;
            if (_isRandomState)
                resState = (ResultState)_rnd.Next(0, 3);

            // in order to see somme state change in 
            await Task.Delay(_rnd.Next(Math.Min(800, delayTime_ms), Math.Min(2300, delayTime_ms +50))); 

            bool bSuccess = Service.Invoke(x => x.UpdateResultState(outDtoData.InternalDBResId, resState));

            await Task.WhenAll(delaytsk);
        }

        private static int CalculateProgress(int total, int complete)
        {
            // avoid divide by zero error
            if (total == 0) return 0;
            // calculate percentage complete
            var result = (double)complete / (double)total;
            var percentage = result * 100.0;
            // make sure result is within bounds and return as integer;
            return Math.Max(0, Math.Min(100, (int)percentage));
        }

        private void BkgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Progress = e.ProgressPercentage;
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
                RaisePropertyChanged(() => RunIter);
            });
        }

    }
}
