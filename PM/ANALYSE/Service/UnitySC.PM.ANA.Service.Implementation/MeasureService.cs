using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Loader;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MeasureService : DuplexServiceBase<IMeasureServiceCallback>, IMeasureService
    {
        private MeasureLoader _measureLoader;
        private const string TempResultFolderName = "MeasureTestResults";
        private PMConfiguration _pmConfiguration;
        private Task _currentMeasureTask;
        private IMeasure _currentMeasure;
        private Task _currentStaticMeasureTask;
        private IMeasure _currentStaticMeasure;
        private CancellationTokenSource _cancellationTokenSource;

        public MeasureService(ILogger<MeasureService> logger, MeasureLoader measureLoader, PMConfiguration pmConfiguration) : base(logger, ExceptionType.MeasureException)
        {
            _measureLoader = measureLoader;
            _pmConfiguration = pmConfiguration;
        }      

        private void Measure_MeasureProgressChangedEvent(MeasurePointProgress measurePointProgressBase)
        {
            InvokeCallback(callback => callback.MeasureProgressChanged(measurePointProgressBase));
        }

        public Response<VoidResult> SubscribeToMeasureChanges()
        {
            return InvokeVoidResponse(_ => Subscribe());
        }

        public Response<VoidResult> UnsubscribeToMeasureChanges()
        {
            return InvokeVoidResponse(_ => Unsubscribe());
        }

        public Response<List<MeasureType>> GetAvailableMeasures()
        {
            return InvokeDataResponse(() => _measureLoader.GetAvailableMeasures());
        }

        public Response<MeasureToolsBase> GetMeasureTools(MeasureSettingsBase measureSettings)
        {
            return InvokeDataResponse(() => GetMeasure(measureSettings.MeasureType).GetMeasureTools(measureSettings));
        }

        public Response<MeasureConfigurationBase> GetMeasureConfiguration(MeasureType measureType)
        {
            return InvokeDataResponse(() =>
            {
                var measuresConfiguration = ClassLocator.Default.GetInstance<MeasuresConfiguration>();
                return measuresConfiguration.GetMeasureConfiguration(measureType);
            });
        }

        public Response<VoidResult> StartMeasure(MeasureSettingsBase measureSettings, MeasurePoint measurePoint, DieIndex dieIndex = null)
        {
            return InvokeVoidResponse(_ =>
            {
                if (_currentMeasure != null)
                {
                    throw new InvalidOperationException($"Can't start a new measure, measure {_currentMeasure.MeasureType} is still running.");
                }
                _cancellationTokenSource = new CancellationTokenSource();
                _currentMeasure = GetMeasure(measureSettings.MeasureType);
                _currentMeasure.MeasureProgressChangedEvent += Measure_MeasureProgressChangedEvent;

                MeasureContext measureContext = new MeasureContext(measurePoint, dieIndex, CreateTempResultFolderPath());
                MeasurePointResult res = null;
                
                _currentMeasureTask = Task.Run(() =>
                {
                    res = _currentMeasure.Execute(measureSettings, measureContext, _cancellationTokenSource.Token);
                }, _cancellationTokenSource.Token);

                _currentMeasureTask.GetAwaiter().OnCompleted(()=> { MeasureCompleted(res, measureContext, dieIndex); });
            });
        }

        public Response<VoidResult> StartMeasureWithSubMeasures(MeasureSettingsBase measureSettings, MeasurePoint measurePoint, List<MeasurePoint> subMeasurePoints, DieIndex dieIndex = null)
        {
            return InvokeVoidResponse(_ =>
            {
                if (_currentMeasure != null)
                {
                    throw new InvalidOperationException($"Can't start a new measure, measure {_currentMeasure.MeasureType} is still running.");
                }
                _cancellationTokenSource = new CancellationTokenSource();
                _currentMeasure = GetMeasure(measureSettings.MeasureType);
                _currentMeasure.MeasureProgressChangedEvent += Measure_MeasureProgressChangedEvent;

                MeasureContext measureContext = new MeasureContext(measurePoint, dieIndex, CreateTempResultFolderPath());
                MeasurePointResult res = null;
                
                _currentMeasureTask = Task.Run(() =>
                {
                    var subResults = new List<MeasurePointResult>();
                    foreach (var subMeasurePoint in subMeasurePoints)
                    {
                        var subMeasureContext = new MeasureContext(subMeasurePoint, dieIndex, CreateTempResultFolderPath());
                        subResults.Add(_currentMeasure.ExecuteSubMeasure(measureSettings, subMeasureContext, _cancellationTokenSource.Token));
                    }

                    res = _currentMeasure.ComputeMeasureFromSubMeasures(measureSettings, measureContext, subResults, _cancellationTokenSource.Token);
                }, _cancellationTokenSource.Token);

                _currentMeasureTask.GetAwaiter().OnCompleted(()=> { MeasureCompleted(res, measureContext, dieIndex); });
            });
        }

        public Response<VoidResult> StartStaticRepetaMeasure(MeasureSettingsBase measureSettings, MeasurePoint measurePoint, int nbOfStaticRepeta = 2)
        {
            return InvokeVoidResponse(_ =>
            {
                try
                {
                    if (_currentStaticMeasure != null)
                    {
                        throw new InvalidOperationException($"Can't start a new static measure, measure {_currentStaticMeasure.MeasureType} is still running.");
                    }

                    _cancellationTokenSource = new CancellationTokenSource();
                    MeasureContext measureContext = new MeasureContext(measurePoint, null, CreateTempResultFolderPath());
                    var currentMeasure = GetMeasure(measureSettings.MeasureType);
                    _currentStaticMeasure = currentMeasure;

                    for (int iStaticRepetaIndex = 0; iStaticRepetaIndex < nbOfStaticRepeta; iStaticRepetaIndex++)
                    {
                        var mesureContextRepeat = measureContext.ConvertToMeasureContextRepeat(iStaticRepetaIndex);
                        MeasurePointResult result = new MeasurePointResult();
                        _currentStaticMeasureTask = Task.Run(() =>
                        {
                            StaticMeasureStarted(mesureContextRepeat);
                            result = currentMeasure.Execute(measureSettings, mesureContextRepeat, _cancellationTokenSource.Token, true);
                        }, _cancellationTokenSource.Token);
                        _currentStaticMeasureTask.GetAwaiter().OnCompleted(() => { StaticMeasureCompleted(result, mesureContextRepeat, nbOfStaticRepeta); });
                        _currentStaticMeasureTask.Wait();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error during StartStaticRepetaMeasure", ex);
                }
            });
        }

        private void MeasureCompleted(MeasurePointResult res, MeasureContext measureContext, DieIndex dieIndex)
        {
            InvokeCallback(callback => callback.MeasureResultChanged(res, measureContext.ResultFoldersPath.RecipeFolderPath, dieIndex));
            _currentMeasure.MeasureProgressChangedEvent -= Measure_MeasureProgressChangedEvent;
            _currentMeasure = null;
            _currentMeasureTask = null;
        }

        private void StaticMeasureStarted(MeasureContextRepeat measureContextRepeat)
        {
            InvokeCallback(callback => callback.StaticMeasureResultStarted(measureContextRepeat.RepeatIndex));
        }

        private void StaticMeasureCompleted(MeasurePointResult res, MeasureContextRepeat measureContextRepeat, int nbOfStaticRepeat)
        {
            InvokeCallback(callback => callback.StaticMeasureResultChanged(res, measureContextRepeat.RepeatIndex));
            _currentStaticMeasure = null;
            _currentStaticMeasureTask = null;
        }

        public Response<VoidResult> CancelMeasure()
        {
            return InvokeVoidResponse(_ =>
            {
                _cancellationTokenSource?.Cancel();
            });
        }

        public Response<VoidResult> StopStaticRepetaMeasure()
        {
            return InvokeVoidResponse(_ =>
            {
                _cancellationTokenSource?.Cancel();
                _currentStaticMeasure = null;
                _currentStaticMeasureTask = null;
            });
        }

        private IMeasure GetMeasure(MeasureType measureType)
        {
            IMeasure measure = _measureLoader.GetMeasure(measureType);
            if (measure == null)
            {
                _logger.Error($"The measure { measureType} is not loaded - Check MeasureLoader");
                throw new NullReferenceException($"The measure {measureType} is not loaded");
            }
            return measure;
        }

        public Response<List<string>> GetMeasureLightIds(MeasureType measureType)
        {
            return InvokeDataResponse(() => GetMeasure(measureType).GetLightIds());
        }

        private ResultFoldersPath CreateTempResultFolderPath()
        {
            try
            {
                string mainTempFolderPath = Path.Combine(_pmConfiguration.LocalCacheResultFolderPath, TempResultFolderName);
                string tempFolderPath = Path.Combine(mainTempFolderPath, DateTime.Now.ToString("M-dd-HHmmss-ff"));
                var resultFoldersPath = new ResultFoldersPath();
                if (!Directory.Exists(mainTempFolderPath))
                {
                    Directory.CreateDirectory(tempFolderPath);
                }
                else
                {
                    try
                    {
                        Directory.Delete(mainTempFolderPath, true);
                    }
                    catch (Exception ex)
                    {
                        _logger.Debug($"Error during delete temp folder {mainTempFolderPath}: {ex.Message}");
                    }
                    Directory.CreateDirectory(tempFolderPath);
                }
                resultFoldersPath.RecipeFolderPath = tempFolderPath;
                resultFoldersPath.ExternalFileFolderName = String.Empty;
                resultFoldersPath.ExternalFilePrefix = String.Empty;
                return resultFoldersPath;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during create temp result folder path");
                throw new Exception("Error during create temp result folder path", ex);
            }
        }
    }
}
