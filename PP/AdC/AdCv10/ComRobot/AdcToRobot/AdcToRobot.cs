using System.Collections.Generic;
using System.ServiceModel;

using ADCEngine;

using AdcRobotExchange;

using Serilog;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

using Dto = UnitySC.DataAccess.Dto;

namespace AdcToRobot
{
    public class AdcToRobot
    {
        private ServiceHost svchost;
        private Dictionary<string, AdcStatus> adcStatusMap = new Dictionary<string, AdcStatus>();
        private Dictionary<string, SocketRobot> socketRobotMap = new Dictionary<string, SocketRobot>();
        private TransferToRobot transferToRobot;

        public ITransferToRobot GetTransferToRobotInterinface() { return transferToRobot; }

        //=================================================================
        // 
        //=================================================================
        public void Start()
        {
            //-------------------------------------------------------------
            // Implementation de l'API ITransferToRobot
            //-------------------------------------------------------------
            transferToRobot = new TransferToRobot(adcStatusMap);
            ADC.Instance.TransferToRobotStub.Attach(transferToRobot);

            //-------------------------------------------------------------
            // Server Socket pour le Robot, une par Robot
            //-------------------------------------------------------------
            var dbToolServiceProxy = ClassLocator.Default.GetInstance<DbToolServiceProxy>();
            List<Dto.Tool> tools = dbToolServiceProxy.GetAllTools();

            foreach (Dto.Tool tool in tools)
            {
                AdcStatus adcStatus = new AdcStatus();
                adcStatusMap.Add(tool.Name, adcStatus);

               // // TO-DO : voir pour les liens socket ou dataflow
               // SocketRobot socketRobot = new SocketRobot(adcStatus);
               // socketRobotMap.Add(tool.Name, socketRobot);
               // Log.Information("Listening for: " + tool.Name + " on port: " + tool.PortNumber);
               // socketRobot.Start(tool.Name, tool.PortNumber);
            }

            //-------------------------------------------------------------
            // Démarrage du service WCF exposant ITransferToRobot
            //-------------------------------------------------------------
            svchost = new ServiceHost(transferToRobot);
            foreach (var endpoint in svchost.Description.Endpoints)
                Log.Information("Creating AdcToRobot WCF service on \"" + endpoint.Address + "\"");
            svchost.Open();
        }

        //=================================================================
        // 
        //=================================================================
        public void Stop()
        {
            //-------------------------------------------------------------
            // Stop WCF service
            //-------------------------------------------------------------
            if (svchost != null)
            {
                Log.Information("Stop AdcToRobot WCF service");
                svchost.Close();
            }

            //-------------------------------------------------------------
            // Stoppe la com vers le robot
            //-------------------------------------------------------------
            foreach (SocketRobot socketRobot in socketRobotMap.Values)
                socketRobot.Stop();
        }


    }
}
