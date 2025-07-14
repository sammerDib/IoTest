using System;
using System.Configuration;
using System.ServiceProcess;

using ADCEngine;

using Serilog;

namespace AdaToAdc
{
    internal partial class AdaToAdcService : ServiceBase
    {
        private Monitor m = new Monitor();
        private AdcToRobot.AdcToRobot adcToRobot = new AdcToRobot.AdcToRobot();

        public AdaToAdcService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Information("Start windows service");
            Start(args);
        }

        protected override void OnStop()
        {
            Log.Information("Stop windows service");
            StopMonitor();
        }

        public void Start(string[] args)
        {
            //-------------------------------------------------------------
            // Init ADC
            //-------------------------------------------------------------
            ADC.Instance.Init();

            //-------------------------------------------------------------
            // Init AdcToRobot
            //-------------------------------------------------------------
            bool enableTransferToRobot = bool.Parse(ConfigurationManager.AppSettings["AdaToAdc.TransferToRobot.Enable"]);
            bool embeddedTransferToRobot = bool.Parse(ConfigurationManager.AppSettings["AdaToAdc.TransferToRobot.Embedded"]);

            if (enableTransferToRobot)
            {
                if (embeddedTransferToRobot)
                {
                    adcToRobot = new AdcToRobot.AdcToRobot();
                    adcToRobot.Start();
                }
                else
                {
                    ADC.Instance.TransferToRobotStub.Connect();
                }
            }

            //-------------------------------------------------------------
            // Init DataBase
            //-------------------------------------------------------------
            bool useExportedDataBase = Convert.ToBoolean(ConfigurationManager.AppSettings["DatabaseConfig.UseExportedDatabase"]);
            //Database.Service.BootStrapper.DefaultRegister(useExportedDataBase);
            // TODO bootstrapper if needed

            //-------------------------------------------------------------
            // Init AdaToAdc Monitor
            //-------------------------------------------------------------
            m.Start(args);
        }

        public void StopMonitor()
        {
            m.Stop();
            if (adcToRobot != null)
                adcToRobot.Stop();
        }

    }
}
