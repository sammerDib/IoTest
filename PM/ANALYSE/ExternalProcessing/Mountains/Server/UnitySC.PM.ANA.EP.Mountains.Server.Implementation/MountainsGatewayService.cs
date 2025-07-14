using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.EP.Mountains.Server.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MountainsGatewayService : BaseService, IMountainsGatewayService
    {
        private const string TemplateExtension = ".mnt";
        private MountainsConfiguration _mountainsServiceConfiguration;
        private MountainsActiveXSupervisor _mountainsActiveXSupervisor;

        public MountainsGatewayService(ILogger logger, MountainsConfiguration configuration) : base(logger, ExceptionType.ExternalProcessingException)
        {
            _mountainsServiceConfiguration = configuration;
            _mountainsActiveXSupervisor = ClassLocator.Default.GetInstance<MountainsActiveXSupervisor>();
          
        }

        public override void Init()
        {
            CheckDigitalSurfActiveX();
        }

        public override void Shutdown()
        {
            KillMountainsfActiveX();
        }

        public void CheckDigitalSurfActiveX()
        {
            if (!Process.GetProcessesByName(_mountainsServiceConfiguration.ActiveXProcessName).Any())
            {
                var directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                try
                {                   

                    var activeXpath = Path.Combine(directoryPath, string.Concat(_mountainsServiceConfiguration.ActiveXProcessName, ".exe"));
                    _logger.Information($"Start MountainsActiveXHost Process: {activeXpath}");
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = activeXpath;
                    processStartInfo.WorkingDirectory = directoryPath;
                    Process.Start(processStartInfo);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Start MountainsActiveXHost Exception");
                }
                Thread.Sleep(5000);
            }           
        }

        public bool IsDigitalSurfActiveXRunning()
        {
            return Process.GetProcessesByName(_mountainsServiceConfiguration.ActiveXProcessName).Any();
        }

        public void KillMountainsfActiveX()
        {
            var activeXProcess = Process.GetProcessesByName(_mountainsServiceConfiguration.ActiveXProcessName)
                                        .FirstOrDefault();

            if (activeXProcess != null)
            {
                _logger.Information($"Stopping MountainsActiveXHost Process: {_mountainsServiceConfiguration.ActiveXProcessName}");
                activeXProcess.Kill();
            }
        }
      
        public void Dispose()
        {
            KillMountainsfActiveX();
            _mountainsActiveXSupervisor.Close();
        }

        public Response<List<ExternalProcessingResultItem>> Execute(MountainsExecutionParameters mountainsExecutionParameters, ExternalMountainsTemplate template = null, ServiceImage serviceImage = null)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                CheckDigitalSurfActiveX();
                string tempTemplateFilePath = string.Empty;
                string tempStudiableFilePath = string.Empty;

                var dtnow = DateTime.Now;

#pragma warning disable CS0219 // The 'fmttime' variable is used indirectly to format the current date and time.
                // It is required for generating the 'tempTemplateFilePath' with a timestamp.
                const string fmttime = "yyyyMMdd-HHmmss";
#pragma warning restore CS0219

                var curDir = Directory.GetCurrentDirectory();
                if (template != null)
                {
                    tempTemplateFilePath = Path.Combine(curDir, $"Template{dtnow:fmttime}.mnt");
                    template.SaveToFile(tempTemplateFilePath);
                    mountainsExecutionParameters.TemplateFile = tempTemplateFilePath;
                }

                if (serviceImage != null)
                {
                    tempStudiableFilePath = Path.Combine(curDir, $"Studiable{dtnow:fmttime}");
                    switch (serviceImage.Type)
                    {
                        case ServiceImage.ImageType.Greyscale:
                        case ServiceImage.ImageType.RGB:
                            tempStudiableFilePath += ".png";
                            serviceImage.SaveToFile(tempStudiableFilePath);
                            break;
                        case ServiceImage.ImageType._3DA:
                            tempStudiableFilePath += ".bcrf";
                            Converter3DA.ToBCRF(serviceImage.Data, tempStudiableFilePath);
                            break;
                    }
                    mountainsExecutionParameters.PointData.StudiableFile = tempStudiableFilePath;
                }

                var result = _mountainsActiveXSupervisor.Execute(mountainsExecutionParameters)?.Result;
                if (serviceImage != null)
                {
                    try
                    {
                        File.Delete(tempStudiableFilePath);
                    }
                    catch
                    {
                        ClassLocator.Default.GetInstance<ILogger>().Warning($"Error during delete mountains studiable {tempStudiableFilePath}");
                    }
                }

                if (template != null)
                {
                    try
                    {
                        File.Delete(tempTemplateFilePath);
                    }
                    catch
                    {
                        ClassLocator.Default.GetInstance<ILogger>().Warning($"Error during delete mountains template {tempTemplateFilePath}");
                    }
                }

                return result;
            });
        }

        public Response<List<string>> GetTemplateFilePaths()
        {
            return InvokeDataResponse(mes =>
            {
                return Directory.GetFiles(_mountainsServiceConfiguration.TemplatesFolderPath, "*"+TemplateExtension, SearchOption.AllDirectories).Select(x=> Path.GetFullPath(x)).ToList();
            });
        }

        public Response<List<ExternalProcessingResultItem>> GetResultsDefinedInTemplate(string templateFile)
        {
            return InvokeDataResponse(mes =>
            {
                CheckDigitalSurfActiveX();
                return _mountainsActiveXSupervisor.GetResultsDefinedInTemplate(templateFile)?.Result;
            });
        }

        public Response<ExternalMountainsTemplate> GetTemplateContent(string templateFile)
        {
            return InvokeDataResponse(mes =>
            {
                CheckDigitalSurfActiveX();
                return _mountainsActiveXSupervisor.GeTemplateContent(templateFile)?.Result;
            });
        }
    }
}
