#define MONO_THREAD
#if !MONO_THREAD
#warning MONO_THREAD Flag desactivated  -- Acq multi task feed enabled (to validate)
#endif

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

using AcquisitionAdcExchange;

using ADCEngine;

using AdcTools;

using LibProcessing;

using Matrox.MatroxImagingLibrary;

using MergeContext;

using UnitySC.Shared.Tools;

namespace AdaToAdc
{
    ///////////////////////////////////////////////////////////////////////
    ///<summary>
    /// Classe qui scanne les ADA d'un répertoire et les exécute.
    ///</summary>
    ///////////////////////////////////////////////////////////////////////
    public class Monitor
    {
        //=================================================================
        // Autres propriétés et variables
        //=================================================================
        private IAdcAcquisition _adcAcquisition;

        private string _adaFolder;
        private FileSystemWatcher _adaFileSystemWatcher;

        private ChannelFactory<IAdcAcquisition> _adcAcquisitionChannelFactory;

        private static ProcessingClass s_processingClassMil = new ProcessingClassMil();
        private static ProcessingClass s_processingClassMil3D = new ProcessingClassMil3D();

        private bool _isRecipeRunning;
        private Stopwatch _stopWatch = new Stopwatch();
        private ServiceHost _host;   // Service WCF

        //=================================================================
        //
        //=================================================================
        public void Start(string[] args)
        {
            string engineType = ConfigurationManager.AppSettings["AdcEngine.ProductionMode"];
            if (engineType == "InADC")
            {
                //-------------------------------------------------------------
                // Connection au service WCF: IAdcAcquisition
                //-------------------------------------------------------------
                _adcAcquisitionChannelFactory = new ChannelFactory<IAdcAcquisition>("IAdcAcquisition");
                log("Connecting to service on \"" + _adcAcquisitionChannelFactory.Endpoint.Address + "\"");
                _adcAcquisition = _adcAcquisitionChannelFactory.CreateChannel();
            }
            else if (engineType == "InAcquisition")
            {
                //-------------------------------------------------------------
                // Hébergement du service WCF: IAdcExecutor, IAdcAcquisition
                //-------------------------------------------------------------
                _adcAcquisition = new AdcExecutor();

                //-------------------------------------------------------------
                // Création des services WCF
                //-------------------------------------------------------------
                _host = new ServiceHost(_adcAcquisition);
                foreach (var endpoint in _host.Description.Endpoints)
                    ADC.log("Creating service on \"" + endpoint.Address + "\"");
                _host.Open();
            }
            else
            {
                throw new ApplicationException("unknown mode for AdcEngine.ProductionMode: " + engineType);
            }

            //-------------------------------------------------------------
            // Ligne de commande
            //-------------------------------------------------------------
            foreach (PathString file in args)
            {
                if (file.Extension.ToLower() == ".adc")
                {
                    PathString ada = file.ChangeExtension(".ada");
                    if (File.Exists(file))
                        File.Move(file, ada);
                    ProcessAda(ada);
                }
                else if (file.Extension.ToLower() == ".ada")
                {
                    ProcessAda(file);
                }
                else
                {
                    throw new ApplicationException("File is not a .ada nor .adc: " + file);
                }
            }

            //-------------------------------------------------------------
            // Monitoring des fichiers ADA
            //-------------------------------------------------------------
            _adaFolder = ConfigurationManager.AppSettings["AdaFolder"];
            log("Monitoring directory \"" + _adaFolder + "\"");
            InitAdaFileSystemWatcher();
            //-------------------------------------------------------------
            // Lancement des fichiers ada qui existent déjà.
            //-------------------------------------------------------------
            if (!_isRecipeRunning)
                ListAndProcessAda();
        }

        private void AdaFileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            logError("File monitoring error " + e?.GetException()?.ToString());

            // Nouvelle creation du file watcher.
            Task.Factory.StartNew(() =>
            {
                bool error = true;
                int retryNumber = 0;
                while (error)
                {
                    // Une petite tempo en cas d'erreur en boucle
                    Thread.Sleep(500);
                    try
                    {
                        // Réinitilisation en cas d'erreur
                        if (_adaFileSystemWatcher != null)
                        {
                            _adaFileSystemWatcher.EnableRaisingEvents = false;
                            _adaFileSystemWatcher.Created -= AdaFileSystemWatcher_Created;
                            _adaFileSystemWatcher.Renamed -= AdaFileSystemWatcher_Renamed;
                            _adaFileSystemWatcher.Error -= AdaFileSystemWatcher_Error;
                        }

                        InitAdaFileSystemWatcher();
                        error = false;
                    }
                    catch (Exception ex)
                    {
                        logWarning("File monitoring retry " + retryNumber + " :" + ex.ToString());
                        retryNumber++;
                    }
                }
            });
        }

        private void InitAdaFileSystemWatcher()
        {
            _adaFileSystemWatcher = new System.IO.FileSystemWatcher();
            _adaFileSystemWatcher.Filter = "*.ada";
            _adaFileSystemWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)));
            _adaFileSystemWatcher.Created += AdaFileSystemWatcher_Created;
            _adaFileSystemWatcher.Renamed += AdaFileSystemWatcher_Renamed;
            _adaFileSystemWatcher.Path = _adaFolder;
            _adaFileSystemWatcher.Error += AdaFileSystemWatcher_Error;
            _adaFileSystemWatcher.EnableRaisingEvents = true;
        }

        //=================================================================
        //
        //=================================================================
        public void Stop()
        {
            string engineType = ConfigurationManager.AppSettings["AdcEngine.ProductionMode"];
            if (engineType == "InADC")
            {
                // Stop Client
                //............
                log("Close connection to service");
                ICommunicationObject com = (ICommunicationObject)_adcAcquisition;
                com.Abort();
            }
            else if (engineType == "InAcquisition")
            {
                // Stop server
                //............
                log("Stop service");
                _adcAcquisition.StopADC();
            }
            else
            {
                throw new ApplicationException("unknown mode for AdcEngine.ProductionMode: " + engineType);
            }

            // Stop FileWatcher
            //.................
            log("Stop monitoring");
            _adaFileSystemWatcher.EnableRaisingEvents = false;
        }

        //=================================================================
        //
        //=================================================================
        private void AdaFileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            string path = e.FullPath;
            if (!path.EndsWith(".ada"))
                return;

            if (!_isRecipeRunning)
                ProcessAda(path);
        }

        private void AdaFileSystemWatcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            AdaFileSystemWatcher_Created(sender, e);
        }

        //=================================================================
        //
        //=================================================================
        private void ListAndProcessAda()
        {
            string[] filenames = new string[0];

            // Search for ADE files
            //.....................
            try
            {
                filenames = Directory.GetFiles(_adaFolder, "*.ADA");
            }
            catch (Exception ex)
            {
                log("\n");
                log("error while reading directory \"" + _adaFolder + "\"\n");
                log(ex.Message + "\n");
                log(ex.StackTrace + "\n\n");
            }

            // Process files
            //..............
            if (filenames.Count() > 0 && !_isRecipeRunning)
                ProcessAda(filenames[0]);
        }

        //=================================================================
        //
        //=================================================================
        public void ProcessAda(PathString ada)
        {
            _isRecipeRunning = true;

            Scheduler.StartSingleTask("recipe-execution", () =>
            {
                try
                {
                    log("Processing \"" + ada + "\"");
                    _stopWatch.Restart();
                    AdaLoader adaloader;
                    RecipeId recipeId = StartAdaExecution(ada, out adaloader);
                    FeedImages(recipeId, adaloader.AcqImageList);
                    StopRecipe(recipeId);
                }
                catch (Exception ex)
                {
                    logError("\n" + ex + "\n\n");
                    Thread.Sleep(500);
                }
                finally
                {
                    _isRecipeRunning = false;
                    ListAndProcessAda();
                }
            });
        }

        //=================================================================
        //
        //=================================================================
        public RecipeId StartAdaExecution(PathString ada, out AdaLoader adaloader)
        {
            // Renommage de l'ada
            //...................
            string adc = ada.ChangeExtension("adc");
            if (File.Exists(adc))
                File.Delete(adc);

            int count = 0;
            while (count < 10)
            {
                try
                {
                    Thread.Sleep(500);
                    File.Move(ada, adc);
                    //success
                    count = 20;
                }
                catch
                {
                    //noting for retry
                    count++;
                }
            }

            if (count != 20)
            {
                // last retry, if failed, throw IOexception
                Thread.Sleep(500);
                File.Move(ada, adc);
            }

            // Lecture de l'ada et de la recette
            //..................................
            adaloader = new AdaLoader(adc);
            adaloader.LoadAda();

            // Exécution de la recette
            //........................
            log("Starting recipe: " + adaloader.RecipeData.ADCRecipeFileName);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            RecipeId recipeId = _adcAcquisition.StartRecipe(adaloader.RecipeData);
            _adcAcquisition.SetAcquitionImages(recipeId, adaloader.AcqImageList);

            return recipeId;
        }

        //=================================================================
        //
        //=================================================================
        public void FeedImages(RecipeId recipeId, List<AcquisitionData> acqImageList)
        {
            FdQueue<AcquisitionMilImage> queue = new FdQueue<AcquisitionMilImage>();
            bool hasError = false;

            bool preloadImages = bool.Parse(ConfigurationManager.AppSettings["AdaToAdc.TestMode.PreloadImages"]);
            if (preloadImages)
            {
                foreach (AcquisitionMilImage img in acqImageList)
                {
                    log("Preload " + img.Filename);
                    LoadAcquisitionImage(img);
                    byte[] dummy = img.MilData;
                }
                _stopWatch.Stop();
                log("Preload terminated in " + _stopWatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                _stopWatch.Restart();
            }

            foreach (AcquisitionMilImage img in acqImageList)
                queue.Enqueue(img);
            queue.CloseQueue();

#if !MONO_THREAD
            var task = Scheduler.StartTaskPool("acq-feed", () =>
#endif
            {
                int count = 0;
                while (queue.Dequeue(out AcquisitionMilImage img))
                {
                    try
                    {
                        if (!hasError)
                        {
                            if (count++ % 10 == 0)
                                log("Send " + img.Filename);
                            if (!preloadImages)
                                LoadAcquisitionImage(img);
                            hasError = !_adcAcquisition.FeedImage(recipeId, img);
                        }
                    }
                    catch (Exception ex)
                    {
                        log("Failed to load " + img.Filename + " " + ex.ToString());
                        _adcAcquisition.AbortRecipe(recipeId);
                        break;
                    }
                    finally
                    {
                        img.MilData = null;
                        // Libération des images si l'ADC est distant
                        if (!(_adcAcquisition is AdcExecutor))
                        {
                            if (img.MilBufId != 0)
                                MIL.MbufFree(img.MilBufId);
                        }
                    }
                }
            }
#if !MONO_THREAD
                    );

            task.Wait();
#endif
        }

        //=================================================================
        //
        //=================================================================
        public void StopRecipe(RecipeId recipeId)
        {
            bool distant = ConfigurationManager.AppSettings["AdcEngine.ProductionMode"] == "InADC";

            _adcAcquisition.StopRecipe(recipeId);
            _stopWatch.Stop();
            log("Feed terminated in " + _stopWatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));

            bool processing = true;
            RecipeStatus recipeStatus;
            do
            {
                Thread.Sleep(1000);
                recipeStatus = _adcAcquisition.GetRecipeStatus(recipeId);
                processing = (recipeStatus.Status == eRecipeStatus.Processing);
                if (recipeStatus.Message != null && distant)
                    logWarning(recipeStatus.Message);
            }
            while (processing);

            if (distant)
                log("Recipe stopped");
        }

        //=================================================================
        //
        //=================================================================
        private bool _alwaysSendTheSameImage = string.IsNullOrEmpty(ConfigurationManager.AppSettings["AdaToAdc.TestMode.AlwaysSendTheSameImage"]) ? false : bool.Parse(ConfigurationManager.AppSettings["AdaToAdc.TestMode.AlwaysSendTheSameImage"]);
        private void LoadAcquisitionImage(AcquisitionMilImage acqImage)
        {
            using (ProcessingImage procimg = new ProcessingImage())
            {
                string filename;
                if (_alwaysSendTheSameImage)
                    filename = "false";
                else
                {
                    PathString pathstr = acqImage.Filename;
                    bool berr = pathstr.OptimNetworkPath(out string sError);
                    filename = pathstr;
                }

                if (filename.ToUpper().EndsWith(".3DA"))
                    s_processingClassMil3D.Load(filename, procimg);
                else
                    s_processingClassMil.Load(filename, procimg);

                acqImage.MilBufId = procimg.GetMilImage().DetachMilId();
            }
        }

        //=================================================================
        // 
        //=================================================================
        public void log(string str)
        {
            ADC.log(str);
        }

        public void logWarning(string str)
        {
            ADC.logWarning(str);
        }

        public void logError(string str)
        {
            ADC.logError(str);
        }

    }
}
