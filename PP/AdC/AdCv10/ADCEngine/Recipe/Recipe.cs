using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

using AcquisitionAdcExchange;

using AdcRobotExchange;

using AdcTools;
using AdcTools.Serilog;

using Serilog;
using Serilog.Core;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// La recette ADC.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class Recipe : RecipeInfo, IDisposable
    {
        public enum Grading { Reject, Rework };

        //=================================================================
        // Propriétés et variables membres
        //=================================================================
        public event EventHandler recipeExecutedEvent;
        public int NbLayers { get; set; }

        // From XML
        //.........
        public PathString OutputDir { get; set; }
        public PathString InputDir { get; set; }
        private Dictionary<int, ModuleBase> _moduleList = new Dictionary<int, ModuleBase>();
        public Dictionary<int, ModuleBase> ModuleList { get { return _moduleList; } }
        public List<InputInfoBase> InputInfoList;
        public string Toolname;

        // Modules spéciaux
        //.................
        public RootModule Root { get; private set; }
        public TerminationModule Termination { get; private set; }
        public WaferBase Wafer { get; set; }

        public bool IsRunning { get; private set; }
        public bool IsRendering { get; set; }
        public bool HasError { get; private set; }
        /// <summary> Indique que la recette n'a pas traité tous les défauts </summary>
        public bool PartialAnalysis { get; set; }
        public bool IsAborted { get; set; }
        public string ErrorMessage { get; private set; }
        public ModuleBase FaultyModule { get; private set; }
        public Grading? GradingMark { get; set; }
        public PathString WaferLogOutputDir
        {
            get
            {
                PathString logfolder = ConfigurationManager.AppSettings["LogFolder"];
                logfolder = logfolder / "Wafers";
                PathString logWaferOutputDir = logfolder / "wafer-" + Wafer.Basename;
                return logWaferOutputDir;
            }
        }
        // Internal data
        //..............
        private Stopwatch _stopWatch = new Stopwatch();
        private Logger _serilog;

        private CompatibilityManager _compatibilityManager = new CompatibilityManager();
        public CompatibilityManager CompatibilityManager { get { return _compatibilityManager; } }

        private Object _lockWaferSession = null;
        private Object _lockWaferSessionHigh = null;

        //=================================================================
        // Constructeur
        //=================================================================
        public Recipe()
        {
            _lockWaferSession = new Object();
            _lockWaferSessionHigh = new Object();
            InputDir = "";
            OutputDir = "";
            _compatibilityManager.LoadSettings();

            // Ajout des modules Root and Termination
            //.......................................
            IModuleFactory factory = ADC.Instance.FindModuleFactory("Root");
            Root = (RootModule)factory.FactoryMethod(-1, recipe: this);
            _moduleList.Add(Root.Id, Root);
            _compatibilityManager.AddModule(Root);

            factory = ADC.Instance.FindModuleFactory("Termination");
            Termination = (TerminationModule)factory.FactoryMethod(-2, recipe: this);
            _moduleList.Add(Termination.Id, Termination);
            _compatibilityManager.AddModule(Termination);
            ConnectModules(Root, Termination);
        }

        //=================================================================
        // session verrou commune à la recette
        //=================================================================
        public Object GetLockSession()
        {
            return _lockWaferSession;
        }

        public Object GetLockSessionHigh()
        {
            return _lockWaferSessionHigh;
        }

        //=================================================================
        // 
        //=================================================================
        public void Dispose()
        {
            if (_serilog != null)
            {
                _serilog.Dispose();
                _serilog = null;
            }
        }

        ///=================================================================
        ///<summary>
        /// Écrit dans le log de la recette.
        ///</summary>
        ///=================================================================
        public void logDebug(string msg)
        {
            if (_serilog != null)
                _serilog.Debug(msg);
            else
                Log.Debug(msg);
        }

        public void log(string msg)
        {
            if (_serilog != null)
                _serilog.Information(msg);
            else
                Log.Information(msg);
        }

        public void logWarning(string msg)
        {
            if (_serilog != null)
                _serilog.Warning(msg);
            else
                Log.Warning(msg);
        }

        public void logError(string msg)
        {
            if (_serilog != null)
                _serilog.Error(msg);
            else
                Log.Error(msg);
        }

        private void SaveEntete(XmlDocument xmldoc)
        {
            XmlNode node = xmldoc.CreateComment("Generated by " + System.Windows.Forms.Application.ProductName);
            xmldoc.AppendChild(node);

            node = xmldoc.CreateComment("GenerationDate: " + DateTime.Now.ToString());
            xmldoc.AppendChild(node);
        }

        //=================================================================
        // 
        //=================================================================
        public void Save(string filename)
        {
            string fileExtension = Path.GetExtension(filename);
            bool saveWithMerge = fileExtension == ".adcmge" && IsMerged;

            XmlDocument xmldoc = new XmlDocument();
            SaveEntete(xmldoc);
            if (String.IsNullOrEmpty(InputDir))
                SetInputDir(filename);
            XmlNode node = Save(xmldoc, saveWithMerge);
            xmldoc.AppendChild(node);


            xmldoc.Save(filename);
        }

        /// <summary>
        ///  Retourne un noeud XML contenant la recette.
        ///  On doit ensuite attacher le noeud à un autre noeud du XML.
        /// </summary>
        /// <param name="xmldoc"> Xml doc </param>
        /// <param name="saveWithMerge"> Sauvegarde la recette avec la partie mergée </param>
        /// <returns></returns>
        public XmlNode Save(XmlDocument xmldoc, bool saveWithMerge)
        {
            XmlNode node;
            XmlNode recipeNode = xmldoc.CreateElement("Recipe");

            if (OutputDir != null)
                recipeNode.AppendValueElement("OutputDir", saveWithMerge ? OutputDir.ToString() : string.Empty);

            if (InputDir != null)
                recipeNode.AppendValueElement("InputDir", InputDir);

            //-------------------------------------------------------------
            // Modules
            //-------------------------------------------------------------
            XmlNode graphNode = xmldoc.CreateElement("Graph");
            recipeNode.AppendChild(graphNode);

            foreach (ModuleBase module in _moduleList.Values)
            {
                node = module.Save(xmldoc);
                graphNode.AppendChild(node);
            }

            //-------------------------------------------------------------
            // Wafer
            //-------------------------------------------------------------
            if (saveWithMerge && Wafer != null)
            {
                node = Wafer.Save(xmldoc);
                recipeNode.AppendChild(node);
            }

            //-------------------------------------------------------------
            // Inputs
            //-------------------------------------------------------------
            if (saveWithMerge)
            {
                XmlNode inputImagesNode = xmldoc.CreateElement("Input");
                recipeNode.AppendChild(inputImagesNode);
                if (Toolname != null)
                    inputImagesNode.AppendTextElement("Toolname", Toolname ?? "<unknown>");

                foreach (InputInfoBase input in InputInfoList)
                {
                    XmlNode inputnode = input.SerializeAsChildOf(inputImagesNode);

                    // Merge contextes
                    //................
                    XmlNode contextsNode = xmldoc.CreateElement("Contexts");
                    inputnode.AppendChild(contextsNode);
                    foreach (ContextMachine ctx in input.ContextMachineList)
                    {
                        XmlNode ctxnode = ctx.Configuration.SerializeAsChildOf(contextsNode);
                        ctxnode.AddAttribute("ContextType", ctx.Type);
                    }
                }
            }

            return recipeNode;
        }

        /// <summary>
        /// Saves a list of modules present in the recipe and constituting a meta block
        /// </summary>
        /// <param name="listIdModules">list of module ids</param>
        /// <param name="fileName">Name of meta block registration file</param>
        public void SaveMetaBlock(List<int> listIdModules, String fileName)
        {
            XmlDocument xmldoc = new XmlDocument();

            SaveEntete(xmldoc);

            XmlNode metablockNode = xmldoc.CreateElement("Metablock");

            // Modules
            //........
            XmlNode graphNode = xmldoc.CreateElement("Graph");
            metablockNode.AppendChild(graphNode);

            foreach (int modId in listIdModules)
            {
                ModuleBase module = _moduleList[modId];
                bool bSaveChildren = true;
                foreach (ModuleBase children in module.Children)
                {
                    if (listIdModules.Contains(children.Id) == false)
                    {
                        // C'est le dernier module du metabloc; On ne sauvegarde pas ses fils
                        bSaveChildren = false;
                        break;
                    }
                }
                graphNode.AppendChild(module.Save(xmldoc, bSaveChildren));
            }
            xmldoc.AppendChild(metablockNode);

            xmldoc.Save(fileName);
        }


        /// <summary>
        /// Create a copy of ModulesSrce
        /// </summary>
        /// <param name="modulesSrce"></param>
        /// <returns></returns>
        public ModuleBase CopyModule(ModuleBase modulesSrce)
        {
            ModuleBase returnModule;

            XmlDocument xmldoc = new XmlDocument();

            SaveEntete(xmldoc);

            XmlNode xnode = xmldoc.CreateElement("Modules");

            // Modules
            //........
            XmlNode graphNode = xmldoc.CreateElement("Graph");
            xnode.AppendChild(graphNode);

            graphNode.AppendChild(modulesSrce.Save(xmldoc, false));
            xmldoc.AppendChild(xnode);

            // Copy
            Dictionary<int, ModuleBase> moduleList = new Dictionary<int, ModuleBase>();

            //-------------------------------------------------------------
            // Modules
            //-------------------------------------------------------------
            XmlNodeList nodes = xmldoc.SelectSingleNode(".//Graph").SelectNodes("Module");
            LoadModuleList(nodes, moduleList);
            returnModule = moduleList.ElementAt(0).Value;
            return returnModule;
        }
        /// <summary>
        /// Load a meta block. Create a list of modules
        /// </summary>
        /// <param name="fileName">Name of meta block registration file</param>
        /// <param name="moduleList">List of modules in meta block</param>
        public void LoadMetaBlock(String fileName, ref List<ModuleBase> metablock)
        {
            Dictionary<int, ModuleBase> moduleList = new Dictionary<int, ModuleBase>();
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            //-------------------------------------------------------------
            // Modules
            //-------------------------------------------------------------
            XmlNodeList nodes = doc.SelectSingleNode(".//Graph").SelectNodes("Module");
            LoadModuleList(nodes, moduleList);
            metablock = moduleList.Values.ToList();
            if (metablock.Count == 0)
                throw new Exception("File \"" + fileName + "\" contains no metablock");
        }

        ///=================================================================
        ///<summary>
        /// Charge une recette à partir du XML
        ///</summary>
        ///=================================================================
		public void Load(XmlNode recipeNode)
        {
            //-------------------------------------------------------------
            // Inputdir
            //-------------------------------------------------------------
            InputDir = recipeNode.GetStringValue(".//InputDir");

            if (!string.IsNullOrEmpty(recipeNode.BaseURI))
                SetInputDir(new Uri(recipeNode.BaseURI).AbsolutePath);

            //-------------------------------------------------------------
            // Wafer
            //-------------------------------------------------------------
            XmlNode node = recipeNode.SelectSingleNode(".//Wafer");
            if (node != null)
                Wafer = WaferBase.LoadWafer(node);

            //-------------------------------------------------------------
            // OutpurDir
            //-------------------------------------------------------------
            OutputDir = recipeNode.GetStringValue(".//OutputDir");

            //-------------------------------------------------------------
            // Modules
            //-------------------------------------------------------------
            XmlNodeList nodes = recipeNode.SelectSingleNode(".//Graph").SelectNodes("Module");
            LoadModules(nodes);

            //-------------------------------------------------------------
            // Inputs
            //-------------------------------------------------------------
            if (IsMerged)
            {
                InputInfoList = new List<InputInfoBase>();

                node = recipeNode.SelectSingleNode(".//Input");
                Toolname = node.GetTextElement("Toolname");


                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name == "Toolname")
                        continue;

                    InputInfoBase input = Serializable.LoadFromXml<InputInfoBase>(n);
                    InputInfoList.Add(input);

                    // Merge Contextes
                    //................
                    XmlNode contextsNode = n.SelectSingleNode(".//Contexts");
                    foreach (XmlNode ctxnode in contextsNode.ChildNodes)
                    {
                        ContextMachine ctx = new ContextMachine();
                        ctx.Type = ctxnode.GetAttributeValue("ContextType");
                        ctx.Configuration = Serializable.LoadFromXml<Serializable>(ctxnode);

                        input.ContextMachineList.Add(ctx);
                    }
                }
            }

            _compatibilityManager.AddRecipe(this);
        }


        ///=================================================================
        ///<summary>
        /// Charge les modules d'une recette à partir du XML
        ///</summary>
        ///=================================================================
        public void LoadModules(XmlNodeList nodes)
        {
            //-------------------------------------------------------------
            // Chargement du graphe
            //-------------------------------------------------------------
            int version = LoadModuleList(nodes, ModuleList);

            //-------------------------------------------------------------
            // Création du graphe
            //-------------------------------------------------------------
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];

                int modId = int.Parse(node.Attributes["ModID"].Value);
                ModuleBase module = ModuleList[modId];

                XmlNodeList children = node.SelectNodes(".//Child");
                foreach (XmlNode child in children)
                {
                    int childModId = int.Parse(child.Attributes["ModID"].Value);
                    ModuleBase childModule = ModuleList[childModId];
                    if (module is RootModule && childModule is TerminationModule)
                        continue;
                    ModuleBase.Connect(parent: module, child: childModule);
                }
            }

            //-------------------------------------------------------------
            // Connection des modules Root et Term
            //-------------------------------------------------------------
            foreach (ModuleBase module in ModuleList.Values)
            {
                if (module != Root && module != Termination)
                {
                    if (version == 0)
                    {
                        // On n'accepte pas les modules sans parents
                        if (module.Parents.Count == 0)
                            ModuleBase.Connect(parent: Root, child: module);
                    }
                    if (module.Children.Count == 0)
                        ModuleBase.Connect(parent: module, child: Termination);
                }
            }
        }

        ///=================================================================
        ///<summary>
        /// Charge une liste de modules à partir du XML, sans les connecter
        ///</summary>
        ///=================================================================
        protected int LoadModuleList(XmlNodeList nodes, Dictionary<int, ModuleBase> moduleList)
        {
            int version = 0;

            foreach (XmlNode node in nodes)
            {
                string name = node.Attributes["Name"].Value;
                int modId = int.Parse(node.Attributes["ModID"].Value);
                ModuleBase module;

                try
                {
                    // Création du module
                    //...................
                    IModuleFactory factory = ADC.Instance.FindModuleFactory(name);
                    if (factory is RootModuleFactory)
                    {
                        version = 1;
                        module = Root;
                    }
                    else if (factory is TerminationModuleFactory)
                    {
                        module = Termination;
                    }
                    else
                    {
                        module = factory.FactoryMethod(modId, recipe: this);
                    }

                    // Chargement des paramètres
                    //..........................
                    XmlNodeList parameters = node.SelectNodes(".//Parameter");
                    module.LoadParameters(parameters);
                }
                catch (Exception ex)
                {
                    // En cas d'erreur, on crée un module bidon
                    //.........................................
                    logWarning("Failed to load module " + name + "-" + modId + " : " + ex.ToString());
                    IModuleFactory factory = ADC.Instance.FindModuleFactory("Unknown");
                    module = factory.FactoryMethod(modId, recipe: this);
                    ((UnknownModule)module).Error = "Failed to load module " + name + "\r\n" + ex.Message;
                    SetError(module, ex.ToString());
                }

                if (!(module is RootModule) && !(module is TerminationModule))
                    moduleList.Add(modId, module);
            }

            return version;
        }


        ///=================================================================
        ///<summary>
        /// Démarre la recette qui s'exécutera en tache de fond
        ///</summary>
        ///=================================================================
        public bool Start(bool reprocess)
        {
            bool initialized = false;
            try
            {
                _stopWatch.Reset();
                _stopWatch.Start();

                Init();
                initialized = true;

                if (HasError)
                    Abort();

                if (reprocess)
                {
                    log("------ Reprocess ------------");
                    foreach (IDataLoader loader in ModuleList.Values.OfType<IDataLoader>())
                    {
                        if (((ADCEngine.ModuleBase)loader).State != eModuleState.Disabled)
                            loader.StartReprocess();
                    }
                    Stop();

                }
                else
                {
                    log("------ Process ------------");
                }

                return !HasError;
            }
            catch (Exception ex)
            {
                logError("Recipe failed to start\r\n================================\r\n" + ex + "\r\n================================");
                if (!initialized)
                {
                    SetError(Root, ex.Message);
                    RecipeExecuted();
                }
                return false;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public void Feed(ObjectBase obj)
        {
            try
            {
                Root.Process(null, obj);
            }
            catch (Exception ex)
            {
                logError("Recipe failed in feed\r\n================================\r\n" + ex + "\r\n================================");
                SetError(Root, ex.Message);
                RecipeExecuted();
            }
        }

        //=================================================================
        // 
        //=================================================================
        public void Stop()
        {
            try
            {
                Root.Stop(null);
            }
            catch (Exception ex)
            {
                logError("Recipe failed to stop\r\n================================\r\n" + ex + "\r\n================================");
                SetError(Root, ex.Message);
                RecipeExecuted();
            }
        }


        //=================================================================
        // 
        //=================================================================
        protected void Init()
        {
            ADC.log("Starting recipe, BaseName: " + Wafer.Basename);
            IsRunning = true;
            IsAborted = false;
            PartialAnalysis = false;
            ClearError();
            SendWaferReport();

            //-------------------------------------------------------------
            // Création du log
            //-------------------------------------------------------------
            PathString logfolder = ConfigurationManager.AppSettings["LogFolder"];
            logfolder = logfolder / "Wafers";
            PathString logfile = "wafer-" + Wafer.Basename + ".log";

            if (!Directory.Exists(logfolder))
                Directory.CreateDirectory(logfolder);

            PathString logPath = logfolder / logfile;
            LoggerConfiguration sericonf = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.File(logPath);
            if (AdcTools.Serilog.StringSink.IsInitialized)
                sericonf = sericonf.WriteTo.StringSink();
            _serilog = sericonf.CreateLogger();
            string str = "";
            if (!logPath.IsPathRooted)
                str = " (" + logPath.FullPath + ")";
            Log.Information("Creating wafer-specific log in " + logPath + str);

            _serilog.Information("\nBaseName: " + Wafer.Basename + "\n\n");
            log("------ Init ------------");
            log("ADCv" + ADC.Version);
            log("Wafer: " + Wafer.GetWaferInfo(eWaferInfo.WaferID));
            log("Recipe: " + Wafer.GetWaferInfo(eWaferInfo.ADCRecipeFileName));
            log("Output folder: " + Wafer.GetWaferInfo(eWaferInfo.ADCOutputDataFilePath));

            //-------------------------------------------------------------
            // OutpurDir
            //-------------------------------------------------------------

            // Nettoyage d'un ancien répertoire de sortie
            //...........................................
            // OPF_WARNING = A NE SURTOUT PAS FAIRE, on garde tous les résultats
            // La chose qui peut être pensé est un effacement des données d'entrées en option de la recette.

            //if (Directory.Exists(OutputDir))
            //{
            //    try
            //    {
            //        PathString delFoldername = OutputDir + ".del-" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss");
            //        Directory.Move(OutputDir, delFoldername);
            //        Scheduler.StartSingleTask("recipe-cleaner", () =>
            //        {
            //            Directory.Delete(delFoldername, recursive: true);
            //        });
            //    }
            //    catch (Exception)
            //    {
            //        // Ce n'est pas grave si on ne peut pas nettoyer :-)
            //    }
            //}

            //-------------------------------------------------------------

            // Init des modules
            //-------------------------------------------------------------
            NbLayers = 0;

            // Vérification sanitaire
            //.......................
            foreach (ModuleBase module in _moduleList.Values)
            {
                if (module.Parents.Count == 0 && module != Root)
                    throw new ApplicationException("module has no parent: " + module);
                if (module.Children.Count == 0 && module != Termination)
                    throw new ApplicationException("module has no child: " + module);
            }

            // Init les modules
            //.................
            InitModule(Root);
        }


        private void InitModule(ModuleBase module)
        {
            try
            {
                module.Init();
            }
            catch (Exception ex)
            {
                logError(ex.ToString());
                SetError(module, ex.Message);
            }

            foreach (ModuleBase child in module.Children)
            {
                if (child.State != eModuleState.Running) // State == Running signifie que le module à déjà été initialsé
                    InitModule(child);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public void Abort()
        {
            log("------ Abort ------------");
            logDebug("NbAllocatedObjects: " + ObjectBase.NbObjects);

            foreach (ModuleBase module in _moduleList.Values)
                module.Abort();

            IsAborted = true;
        }

        //=================================================================
        // 
        //=================================================================
        public void RecipeExecuted()
        {
            //-------------------------------------------------------------
            // Fin de l'exécution
            //-------------------------------------------------------------
            IsRunning = false;

            SendWaferReport();
            _stopWatch.Stop();
            string msg = "Recipe terminated in " + _stopWatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
            log(msg);
            if (HasError)
                ADC.logWarning("UniqueID: " + Wafer.Basename + " Error: " + ErrorMessage);
            ADC.log(msg);

            //-------------------------------------------------------------
            // Nettoyage 
            //-------------------------------------------------------------
            if (_serilog != null)
            {
                _serilog.Dispose();
                _serilog = null;
            }

            //-------------------------------------------------------------
            // Informe l'application
            //-------------------------------------------------------------
            if (recipeExecutedEvent != null)
                recipeExecutedEvent(this, new EventArgs());
        }

        //=================================================================
        //
        //=================================================================
        public ModuleBase CreateModule(string moduleName)
        {
            // Recherche de la factory
            //........................
            IModuleFactory factory = ADC.Instance.FindModuleFactory(moduleName);

            if ((factory is RootModuleFactory) || (factory is TerminationModuleFactory) || (factory is UnknownModuleFactory))
                throw new ApplicationException("Can't add Root or Termination module");

            // Création du module
            //...................
            ModuleBase newModule = factory.FactoryMethod(GetNewId(), recipe: this);

            return newModule;
        }

        //=================================================================
        //
        //=================================================================
        public void AddModule(ModuleBase parent, ModuleBase module)
        {
            // Ajout dans la liste
            //....................
            _moduleList.Add(module.Id, module);

            // Connexion du module entre son parent et _term
            //..............................................
            if (parent != null)
                ConnectModules(parent, module);
            ConnectModules(module, Termination);

            // Insertion dans la liste de compatibilité
            //.........................................
            _compatibilityManager.AddModule(module);
        }

        //=================================================================
        //
        //=================================================================
        public void ConnectModules(ModuleBase parent, ModuleBase child)
        {
            if ((parent.Children.Count == 1) && (parent.Children[0] is TerminationModule))
                ModuleBase.Disconnect(parent, Termination);

            if ((child.Parents.Count == 1) && (child.Parents[0] is RootModule))
                ModuleBase.Disconnect(Root, child);

            ModuleBase.Connect(parent, child);
        }

        //=================================================================
        //
        //=================================================================
        public void DisconnectModules(ModuleBase parent, ModuleBase child)
        {
            ModuleBase.Disconnect(parent, child);

            if (parent.Children.Count == 0)
                ModuleBase.Connect(parent, Termination);

            // Ne pas connecter à la racine, on accepte les modules orphelins
            //if (child.Parents.Count == 0)
            //    ModuleBase.Connect(Root, child);
        }

        /// <summary>
        /// Deconnecte a module frome parents and childs
        /// </summary>
        /// <param name="parentModule"></param>
        /// <param name="module"></param>
        public void DeleteModule(ModuleBase module)
        {
            //-------------------------------------------------------------
            // Déconnecte les parents
            //-------------------------------------------------------------
            for (int i = module.Parents.Count - 1; i >= 0; i--)
            {
                ModuleBase parent = module.Parents[i];
                ModuleBase.Disconnect(parent, module);
                // If parent has no child, add _term
                if (parent.Children.Count == 0)
                    ModuleBase.Connect(parent, Termination);
            }

            //-------------------------------------------------------------
            // Déconnecte les children
            //-------------------------------------------------------------
            for (int i = module.Children.Count - 1; i >= 0; i--)
            {
                ModuleBase child = module.Children[i];
                ModuleBase.Disconnect(module, child);
            }

            //-------------------------------------------------------------
            // Suppression du module
            //-------------------------------------------------------------
            if (_moduleList.Remove(module.Id) == false)
                throw new ApplicationException("Delete module failed : " + module);

            //-------------------------------------------------------------
            // Mise à jour du Compatibility Manager
            //-------------------------------------------------------------
            _compatibilityManager.RemoveModule(module);
            _compatibilityManager.AddRecipe(this);
        }

        /// <summary>
        /// Calculates a new module id 
        /// </summary>
        /// <returns>new id</returns>
        public int GetNewId()
        {
            int newId = 1;
            bool ok = false;

            while (!ok)
            {
                ok = true;
                foreach (ModuleBase module in _moduleList.Values)
                {
                    if (newId == module.Id)
                    {
                        newId++;
                        ok = false;
                        break;
                    }
                }
            }
            return newId;
        }


        //=================================================================
        // Liste des parameters exportés par les modules
        //=================================================================
        public List<ParameterBase> GetExportedParameterList()
        {
            List<ParameterBase> list = new List<ParameterBase>();

            foreach (ModuleBase module in ModuleList.Values)
                list.AddRange(module.ExportedParameterList);

            return list;
        }


        //=================================================================
        // 
        //=================================================================
        private void SendWaferReport()
        {
            //-------------------------------------------------------------
            // Construction de la structure
            //-------------------------------------------------------------
            WaferReport report = new WaferReport();
            report.LotID = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.LotID);
            report.WaferID = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.WaferID);
            report.SlotID = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.SlotID);
            report.LoadPortID = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.LoadPortID);
            report.ProcessStartTime = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.StartProcess);
            report.ProcessStartTime = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.StartProcess);
            report.JobID = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.JobID);
            report.JobStartTime = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.JobStartTime);

            report.KlarfFilename = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.KlarfFileName);
            report.OutputDirectory = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.ADCOutputDataFilePath);
            report.defectCount_tot = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.TotalDefectCount);

            if (HasError)
            {
                report.WaferStatus = WaferReport.eWaferStatus.Error;
                report.ErrorMessage = ErrorMessage;
                if (FaultyModule != null)
                    report.FaultyModule = FaultyModule.ToString();
                else
                    report.FaultyModule = string.Empty;
            }
            else if (IsRunning)
            {
                report.WaferStatus = WaferReport.eWaferStatus.Processing;
            }
            else
            {
                report.WaferStatus = WaferReport.eWaferStatus.Complete;
                report.defectCount_tot = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.TotalDefectCount);
            }


            if (GradingMark.HasValue)
            {
                switch (GradingMark.Value)
                {
                    case Grading.Reject:
                        report.AnalysisStatus = WaferReport.eAnalysisStatus.GradingReject;
                        break;
                    case Grading.Rework:
                        report.AnalysisStatus = WaferReport.eAnalysisStatus.GradingRework;
                        break;
                }
            }
            else if (PartialAnalysis)
            {
                report.AnalysisStatus = WaferReport.eAnalysisStatus.Partial;
            }
            else if (report.WaferStatus == WaferReport.eWaferStatus.Complete)
            {
                report.AnalysisStatus = WaferReport.eAnalysisStatus.Success;
            }
            else if (IsAborted)
            {
                report.AnalysisStatus = WaferReport.eAnalysisStatus.Aborted;
            }

            //-------------------------------------------------------------
            // Envoi
            //-------------------------------------------------------------
            ADC.Instance.TransferToRobotStub.TransferWaferReport(Toolname, $"{Wafer.GetWaferInfo(eWaferInfo.JobID)}{Wafer.Basename}", report);
        }

        //=================================================================
        // Erreur
        //=================================================================
        private void ClearError()
        {
            HasError = false;
            ErrorMessage = null;
            FaultyModule = null;
        }

        public void SetError(ModuleBase module, string message)
        {
            if (HasError)
                return; // on ne garde que la première erreur

            HasError = true;
            FaultyModule = module;
            ErrorMessage = message;
        }

        /// <summary>
        /// Clean data in modules
        /// </summary>
        public void ClearRenderingObjects()
        {
            foreach (ModuleBase module in ModuleList.Values)
                module.ClearRenderingObjects();
        }

        /// <summary>
        /// Liste des fichiers externes à la recette.
        /// </summary>
        public IEnumerable<ExternalRecipeFile> ExternalRecipeFileList
        {
            get
            {
                return ModuleList.Values.SelectMany(x => x.ExternalRecipeFileList).GroupBy(x => x.FileName).Select(x => x.First());
            }
        }

        /// <summary>
        /// Liste des types de dataloader utilisés dans la recette
        /// </summary>
        public IEnumerable<ActorType> DataLoaderActorTypes
        {
            get
            {
                return ModuleList.Values.OfType<IDataLoader>().Select(x => x.DataLoaderActorType).Distinct();
            }
        }

        /// <summary>
        /// Liste des dataloader utilisés dans la recette
        /// </summary>
        public IEnumerable<IDataLoader> DataLoaders
        {
            get
            {
                return ModuleList.Values.OfType<IDataLoader>();
            }
        }

        /// <summary>
        /// Défini le repertoire des fichier externes en fonction du chemin de la recette
        /// </summary>
        /// <param name="recipeFilePath"></param>
        public void SetInputDir(string recipeFilePath)
        {
            if (!string.IsNullOrEmpty(recipeFilePath))
                InputDir = Path.Combine(Path.GetDirectoryName(recipeFilePath), string.Format("{0}_Files", Path.GetFileNameWithoutExtension(recipeFilePath)));
        }

        /// <summary>
        /// Détermine si la recette est mergée
        /// </summary>
        public bool IsMerged
        {
            get { return Wafer != null; }
        }

        /// <summary>
        ///   Tous les dataloaders non pas encore de Compatible Result Types
        /// </summary>
        /// <returns></returns>
        public bool IsOkBase()
        {
            foreach (var dataLoader in DataLoaders)
                if (!dataLoader.CompatibleResultTypes.Any())
                    return false;
            return DataLoaders.Count()>0;
        }
        public bool IsRenderingNodeSeletedOnly { get; set; }
        

        public bool HasUnknwonModules()
        {
            bool b = ModuleList.Values.Any(m => m is UnknownModule);
            return b;
        }

        /// <summary>
        /// Pour afficher l'état des modules dans le debuggueur
        /// </summary>
        public IEnumerable<Tuple<string, string>> DebugState()
        {
            return ModuleList.Values.ToList().Select(m => new Tuple<string, string>(m.DisplayName, m.State.ToString()));
        }

    }
}
