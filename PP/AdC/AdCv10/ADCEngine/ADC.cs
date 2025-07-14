using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Matrox.MatroxImagingLibrary;

using Serilog;

using UnitySC.PP.Shared.Configuration;
using UnitySC.Shared.LibMIL;

namespace ADCEngine
{
    public class ADC
    {
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Singleton
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static ADC Instance { get; } = new ADC();
        private ADC() { }	// Constructeur privé pour empécher de créer un autre ADC

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Propriétées
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private Dictionary<string, IModuleFactory> _factories = new Dictionary<string, IModuleFactory>();
        public Dictionary<string, IModuleFactory> Factories
        {
            get { return _factories; }
        }

        public TransferToRobotStub TransferToRobotStub { get; } = new TransferToRobotStub();

        private static string _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string Version { get { return _version; } }

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Méthodes
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        //=================================================================
        // Liste les modules présents sous le dossier PathModules
        //=================================================================
        public void LoadModules()
        {
            // Ajout des modules "en dur" provenant de AdcEngine.dll
            //......................................................
            IModuleFactory factory;
            factory = new RootModuleFactory();
            _factories.Add(factory.ModuleName, factory);
            factory = new TerminationModuleFactory();
            _factories.Add(factory.ModuleName, factory);
            factory = new UnknownModuleFactory();
            _factories.Add(factory.ModuleName, factory);

            // On cherche les DLL contenant des modules
            //.........................................

            //NICO
            //var files = System.IO.Directory.EnumerateFiles(PathString.GetExecutingAssemblyPath().Directory, "*Module*.dll", System.IO.SearchOption.AllDirectories);
            //@"C:\Users\n.chaux\source\ADCvX\ADC\Output\Debug"
            string pathModuleDll = AppParameter.Instance.Get("PathModuleDll");
            //string pathModuleDll = @"C:\Projects\UnitySC\USP2\PP\AdC\Output\Debug";

            var files = System.IO.Directory.EnumerateFiles(pathModuleDll, "*Module*.dll", System.IO.SearchOption.AllDirectories);



            //C:\Users\n.chaux\source\ADC\ADC\Output\Debug

            // 
            //.........................................
            foreach (string f in files)
            {
                try
                {
                    string flower = f.ToLower();

                    if (flower.Contains("cppalgorithms") || flower.Contains("zlibwapi.dll"))
                        continue;

                    Assembly assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(f));

                    foreach (Type type in assembly.GetTypes())

                    {
                        if (!type.IsAbstract && type.IsSubclassOf(typeof(IModuleFactory)))
                        {
                            factory = Activator.CreateInstance(type) as IModuleFactory;
                            _factories.Add(factory.ModuleName, factory);
                            log("Found Module Factory: " + f + " Module: " + factory.ModuleName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Failed to load: " + f);
                    sb.AppendLine(ex.ToString());
                    ReflectionTypeLoadException rex = ex as ReflectionTypeLoadException;
                    if (rex != null)
                    {
                        foreach (Exception lex in rex.LoaderExceptions)
                            sb.AppendLine("\t" + lex.ToString());
                    }
                    logWarning(sb.ToString());
                }
            }
        }


        //=================================================================
        // 
        //=================================================================
        public long ImageMemory()
        {
            return MilImage.ImageMemory;
        }

        private bool initialisated = false;
        //=================================================================
        // 
        //=================================================================
        public void Init()
        {
            if (initialisated) return;


            LoadModules();
            Mil.Instance.Allocate();

            initialisated = true;
        }

        public void Init(MIL_ID applicationId, MIL_ID systemId)
        {
            LoadModules();
            Mil.Instance.InitFromIDs(applicationId, systemId);
        }

        //=================================================================
        // 
        //=================================================================
        public void Shutdown()
        {
            Mil.Instance.Free();
        }

        private static bool logErrorInit = false;

        //=================================================================
        // 
        //=================================================================
        public static void log(string msg)
        {
            if (logErrorInit) return;
            try
            {
                Log.Information(msg);

            }
            catch
            {
                logErrorInit = true;
            }

        }

        public static void logWarning(string msg)
        {
            if (logErrorInit) return;
            try
            {
                Log.Warning(msg);

            }
            catch
            {
                logErrorInit = true;
            }
        }

        public static void logError(string msg)
        {
            if (logErrorInit) return;
            try
            {
                Log.Error(msg);

            }
            catch
            {
                logErrorInit = true;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public IModuleFactory FindModuleFactory(string name)
        {
            IModuleFactory factory;

            bool bOk = _factories.TryGetValue(name, out factory);
            if (!bOk)
                throw new Exception("Invalid module name: " + name);

            return factory;
        }

    }
}
