using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Dataflow.Service.Host
{
    internal class ConsoleMode
    {
        /// <summary>
        /// key : service name (lower case sensitive)
        /// </summary>
        readonly SortedDictionary<string, ServiceHost> _serviceHosts = new SortedDictionary<string, ServiceHost>();

        private ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();
        private CommandLineInterpreter _commandLineInterpreter = new CommandLineInterpreter();

        public CommandLineInterpreter CommandLineInterpreter { get => _commandLineInterpreter; }

        public void Start()
        {

            ReferenceCommand();
            Console.WriteLine(CommandLineInterpreter.ToHelp());

            bool exit = false;

            while (!exit)
            {
                Console.Write("$ ");
                string s = Console.ReadLine();

                exit = CommandLineInterpreter.IsExitCommand(s);
                if (!exit)
                {
                    try
                    {
                        var cmd = CommandLineInterpreter.InterpretCommand(s);
                        if (cmd != null)
                            CommandLineInterpreter.ExecuteCommand(cmd);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

            }
        }

        public void ReferenceCommand()
        {

            CommandLineInterpreter
                .RegisterCommand("status", "get WCF server status", (param) =>
                {
                    if (!_serviceHosts.Any()) Console.WriteLine("no service is started.");

                    foreach (var s in _serviceHosts)
                    {
                        Console.WriteLine($"Service {s.Key} is {s.Value.State}.");
                    }


                });

            CommandLineInterpreter
                .RegisterCommand("Start", "Start the service.", (datas) =>
                {

                    string name = datas.FirstOrDefault().ToUpper();                   
                    Console.WriteLine($"starting {name}.");

                    UnitySC.Dataflow.Service.Interface.IDAP sDAP = ClassLocator.Default.GetInstance<UnitySC.Dataflow.Service.Interface.IDAP>();
                    IUTODFService sUTODFService = ClassLocator.Default.GetInstance<IUTODFService>();
                    IPMDFService sPMDFService = ClassLocator.Default.GetInstance<IPMDFService>();

                    try
                    {

                        switch (name)
                        {
                            case "ALL":                               
                                Task.Run(() => StartService("DAP", sDAP));
                                Task.Run(() => StartService("UTODFService", sUTODFService));
                                Task.Run(() => StartService("PMDFService", sPMDFService));
                                break;                         

                            case "DAP":
                                Task.Run(() => StartService("DAP", sDAP));
                                break;

                        }
                        Console.WriteLine($"{name} started.");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }



                })
                .AddParmeter("Service Name", CommandLineInterpreter.ParameterType.String, "the name of the service to start. can be " + Environment.NewLine +               
                "DAP : DAP, " + Environment.NewLine +
                "ALL : start all services");

            CommandLineInterpreter
                .RegisterCommand("Stop", "Stop the service.", (datas) =>
                {

                    string name = datas.FirstOrDefault();                   
                    Console.WriteLine($"Stoping {name}.");
                    try
                    {
                        StopService(name.ToUpper());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    Console.WriteLine($"{name} stopped.");


                })
                .AddParmeter("Service Name", CommandLineInterpreter.ParameterType.String, "the name of the service to stop. can be " + Environment.NewLine + "DAP : DAP");           

            CommandLineInterpreter
                .RegisterCommand("HELP", "show this help.",
                (datas) =>
                {
                    Console.WriteLine(CommandLineInterpreter.ToHelp());
                });

            CommandLineInterpreter
                .RegisterCommand("exit", "exit the service.",
                (datas) => { });
          
        }

        private void StartService(string name, object service)
        {
            if (_serviceHosts.TryGetValue(name.ToLower(), out ServiceHost h))
            {
                if (h.State.In(CommunicationState.Opened, CommunicationState.Opening))
                {
                    Console.WriteLine($"The Service {name} already started.");
                    return;
                }

                _serviceHosts.Remove(name.ToLower());
            }


            ServiceHost host = new ServiceHost(service);
            foreach (var endpoint in host.Description.Endpoints)
            {
                _logger.Information($"Creating {name} service on {endpoint.Address}");
            }

            _serviceHosts.Add(name.ToLower(), host);


            Task.Run(() => host.Open());

        }
        private void StopService(string name)
        {
            if (_serviceHosts.TryGetValue(name.ToLower(), out ServiceHost h))
            {
                h.Close();

                _serviceHosts.Remove(name.ToLower());
            }
            else
            {
                Console.WriteLine($"The Service {name} is not started.");

            }
        }

    }
}
