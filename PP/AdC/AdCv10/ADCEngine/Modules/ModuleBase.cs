using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using System.Xml;

using AdcTools;

namespace ADCEngine
{
    public enum eModuleState { Loaded, Running, Stopping, Aborting, Stopped, Disabled };
    public enum eModuleProperty { Standard, Stage };

    public class ParamReportEvent : EventArgs
    {
        public bool persistant;
    }
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base des modules ADC.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public abstract class ModuleBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public ModuleBase(IModuleFactory factory, int id, Recipe recipe)
        {
            Factory = factory;
            Id = id;
            Recipe = recipe;
        }

        //=================================================================
        // Propriétés
        //=================================================================
        public IModuleFactory Factory { get; private set; }

        /// <summary> Id unique dand la recette </summary>
		public int Id { get; set; }
        public bool HasError { get { return Recipe.FaultyModule == this; } }
        public string ErrorMessage { get { if (HasError) return Recipe.ErrorMessage; else return null; } }
        public int StageIndex;

        public Recipe Recipe { get; set; }
        public WaferBase Wafer { get { return Recipe.Wafer; } }

        public string Name { get { return Factory.ModuleName + "-" + Id; } }
        public virtual string DisplayName { get { return Factory.Label + "-" + Id; } }
        public virtual string DisplayNameInParamView { get { return Factory.Label; } }

        /// <summary>
        /// Nom de l'aide associée au module
        /// </summary>
        public virtual string HelpName => Factory.HelpName;

        /// <summary> Nombre de parents en cours d'exécution </summary>
        private int _nbActiveParents;
        /// <summary> Permet de vérifier qu'on arrête pas les fils deux fois  </summary>
        private int _bChildrenStopped;

        //=================================================================
        // Methodes abstraites et virtuelles
        //=================================================================
        /// <summary> 
        /// Process une données ADC. 
        /// Cette fonction peut-être appellée plusieurs fois et depuis plusieurs thread en parallèle.
        /// </summary>
        public abstract void Process(ModuleBase parent, ObjectBase obj);

        //=================================================================
        // Runt-Time UI
        //=================================================================
        public virtual UserControl GetUI() { return null; }

        //=================================================================
        // Rendering
        //=================================================================
        public bool IsRendering { get; set; }

        // Base of rendering UI
        public virtual UserControl RenderingUI { get { return null; } }


        // Memorize the output data in the case of the rendering
        public ObservableCollection<ObjectBase> RenderingObjects = new ObservableCollection<ObjectBase>();

        /// <summary>
        /// Stocke un object pour le rendering
        /// </summary>
        protected virtual void StoreRenderingObject(ObjectBase obj)
        {
            if (Recipe.IsRendering && obj != null)
            {
                RenderingObjects.Add((ObjectBase)obj.DeepClone());
            }
        }

        /// <summary>
        /// Libère les objects stockés par le rendering
        /// </summary>
        public virtual void ClearRenderingObjects()
        {
            foreach (ObjectBase obj in RenderingObjects)
            {
                if (obj != null)   // RenderingObjects peut avoir des obj nuls
                {
                    obj.Dispose();
                }
            }
            RenderingObjects.Clear();
            IsRendering = false;
            SetState(eModuleState.Loaded);
        }

        //=================================================================
        // Gestion de la limite mémoire
        //=================================================================
        protected static PerformanceCounter AvailableMemoryMB = new PerformanceCounter("Memory", "Available MBytes");
        protected const int AvailableMBytesLimit = 1000;

        public void CheckMemoryLimit()
        {
            if (AvailableMemoryMB.NextValue() < AvailableMBytesLimit)
                throw new OutOfMemoryException("Not enough memory to continue recipe execution");
        }

        //=================================================================
        // Statistiques (nombre d'objets traités).
        //=================================================================
        protected int nbObjectsIn;
        protected int nbObjectsOut;

        public virtual void GetStats(out int nbIn, out int nbOut)
        {
            nbIn = nbObjectsIn;
            nbOut = nbObjectsOut;
        }


        //=================================================================
        // Module Property 
        //=================================================================	
        private eModuleProperty _moduleProperty = eModuleProperty.Standard;

        public eModuleProperty ModuleProperty
        {
            get => _moduleProperty;
            set { _moduleProperty = value; }
        }

        //=================================================================
        //
        // State machine:
        //
        // La state machine des modules se comporte de la façon suivante:
        //     Loaded ou Stopped
        //        +-> Init() x1
        //          +-> OnInit()
        //     Running
        //        +-> Process() x Nb Data 
        //        ↑    +->ProcessChildren() xN
        //        │    │
        //        └─←──┘
        //        -> Stop() x Nb Parents
        //     Stopping
        //        +-> OnStopping() x1
        //          +-> StopChildren()
        //     Stopped
        // L'état Abort est simplement un indicateur que le module ne doit plus
        // faire d'opération longue. Il s'arrêtera réellement quand ses parents 
        // s'arrêteront.
        // 
        // Rendering:
        // En mode rendering, il existe en plus l'état Disabled pour les modules
        // dont on ne veut pas voir les données.
        // 
        //=================================================================
        public eModuleState State { get { return _state; } }

        private object _stateLock = new object();
        private eModuleState _state = eModuleState.Loaded;

        protected eModuleState SetState(eModuleState newState)
        {
            lock (_stateLock)
            {
                eModuleState oldState = _state;
                if (oldState == newState)
                    return oldState;

                // Sanity check
                //.............
                bool valid;
                bool change = true;
                switch (newState)
                {
                    case eModuleState.Loaded:
                        valid = (oldState == eModuleState.Stopped || oldState == eModuleState.Disabled);
                        break;
                    case eModuleState.Running:
                        valid = (oldState == eModuleState.Loaded || oldState == eModuleState.Stopped);
                        break;
                    case eModuleState.Stopping:
                        valid = (oldState == eModuleState.Running || oldState == eModuleState.Aborting);
                        change = (oldState < newState);
                        break;
                    case eModuleState.Aborting:
                        valid = (oldState == eModuleState.Running || oldState == eModuleState.Stopping || oldState == eModuleState.Stopped || oldState == eModuleState.Disabled);
                        change = (oldState < newState);
                        break;
                    case eModuleState.Stopped:
                        valid = (oldState == eModuleState.Stopping || oldState == eModuleState.Aborting || oldState == eModuleState.Disabled);
                        change = (oldState < newState);
                        break;
                    case eModuleState.Disabled:
                        valid = (oldState == eModuleState.Loaded || oldState == eModuleState.Stopped);
                        break;
                    default:
                        throw new ApplicationException("unknown state: " + newState);
                }
                if (!valid)
                    throw new ApplicationException("invalid state transition from " + oldState + " to " + newState);

                // Change state
                //.............
                if (change)
                    _state = newState;
                return oldState;
            }
        }

        //=================================================================
        // Gestion du graphe
        //=================================================================
        private List<ModuleBase> _parentModuleList = new List<ModuleBase>();
        private List<ModuleBase> _childrenModuleList = new List<ModuleBase>();

        public List<ModuleBase> Parents { get { return _parentModuleList; } }
        public List<ModuleBase> Children { get { return _childrenModuleList; } }
        private List<ModuleBase> ReadOnlyChildren;  // les fils qui ne modifient pas les objects qu'on leur passe
        private List<ModuleBase> ReadWriteChildren; // les fils qui modifient les objets

#if DEBUG
        public void AddParent(ModuleBase parent)
        {
            if (!_parentModuleList.TryAdd(parent))
                throw new ApplicationException("Module " + parent + " is already a parent of module " + this);
        }
        public void AddChild(ModuleBase child)
        {
            if (!_childrenModuleList.TryAdd(child))
                throw new ApplicationException("Module " + child + " is already a child of module " + this);
        }
#else
        public void AddParent(ModuleBase parent) { _parentModuleList.Add(parent); }
        public void AddChild(ModuleBase child) { _childrenModuleList.Add(child); }
#endif
        public void RemoveParent(ModuleBase parent) { _parentModuleList.Remove(parent); }
        public void RemoveChild(ModuleBase child) { _childrenModuleList.Remove(child); }

        ///<summary>
        /// Connecte deux modules dans le graphe de la recette
        ///</summary>
        public static void Connect(ModuleBase parent, ModuleBase child)
        {
            parent.AddChild(child);
            child.AddParent(parent);
        }


        ///<summary>
        /// Disconnecte deux modules dans le graphe de la recette
        ///</summary>
        public static void Disconnect(ModuleBase parent, ModuleBase child)
        {
            parent.RemoveChild(child);
            child.RemoveParent(parent);
        }

        ///<summary>
        /// Gestion du graphe: Recherche les parents d'un certain type
        ///</summary>
        public List<ModuleBase> FindAncestors(Predicate<ModuleBase> predicate)
        {
            List<ModuleBase> ancestors = new List<ModuleBase>();
            FindAncestors(ancestors, predicate);
            return ancestors;
        }

        /// <summary> 
        /// Renvoie la listes de tous les parents et grands-parents 
        /// </summary>
        public List<ModuleBase> FindAncestors() => FindAncestors(m => true);

        private void FindAncestors(List<ModuleBase> ancestors, Predicate<ModuleBase> predicate)
        {
            foreach (ModuleBase parent in Parents)
            {
                if (predicate(parent))
                    ancestors.Add(parent);
                parent.FindAncestors(ancestors, predicate);
            }
        }

        /// <summary>
        /// Retourne la liste des fils, des petits-fils, des arrière-petits fils....
        /// </summary>
        public HashSet<ModuleBase> GetAllDescendants()
        {
            HashSet<ModuleBase> set = new HashSet<ModuleBase>();
            GetAllDescendants(set);
            return set;
        }

        private void GetAllDescendants(HashSet<ModuleBase> set)
        {
            foreach (ModuleBase child in Children)
            {
                set.Add(child);
                child.GetAllDescendants(set);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return DisplayName;
        }


        ///=================================================================
        ///<summary>
        /// Initialise le module.
        /// Cette fonction est appellée une fois au démarrage de la recette.
        ///</summary>
        ///
        /// Si le module a besoin d'exécuter des traitements à l'initialisattion,
        /// Il faut surgarcher OnInit() et non Init().
        ///=================================================================
        public void Init()
        {
            log("Starting");
            if (Recipe.IsRendering && !IsRendering && this != Recipe.Termination)
                SetState(eModuleState.Disabled);
            else
                SetState(eModuleState.Running);

            nbObjectsIn = nbObjectsOut = 0;
            _bChildrenStopped = 0;
            _nbActiveParents = Parents.Count;

            ReadOnlyChildren = Children.Where(m => m.IsReadOnlyRecursive()).ToList();
            ReadWriteChildren = Children.Where(m => !ReadOnlyChildren.Contains(m)).ToList();

            if (State != eModuleState.Disabled)
                OnInit();
        }

        /// <summary>
        /// Fonction virtuelle à surcharger par les modules si nécessaire
        /// </summary>
        protected virtual void OnInit()
        {
        }

        //=================================================================
        // Gestion des paramètres
        //=================================================================
        public delegate void ParametersChangedEventHandler(ModuleBase sender, EventArgs e);
        public static event ParametersChangedEventHandler ParametersChanged;
        public void ReportChange(bool persistant = false)
        {
            if (ParametersChanged != null)
            {
                ParamReportEvent reportChangeArgs = new ParamReportEvent();
                reportChangeArgs.persistant = persistant;
                ParametersChanged(this, reportChangeArgs);
            }
        }

        /// <summary>
        /// Lit les paramètres depuis le XML
        /// </summary>
        public virtual void LoadParameters(XmlNodeList parametersNodes)
        {
            String listMissingsParameters = "Missing parameter ";
            bool missingParam = false;

            foreach (ParameterBase param in ParameterList)
            {
                try
                {
                    param.Load(parametersNodes);
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("missing field"))
                    {
                        missingParam = true;
                        listMissingsParameters += " " + param.ExportLabel;
                        param.error = "Missing " + param.ExportLabel;
                    }
                    else
                    {
                        string msg = "Invalid paramter: " + param + " for module: " + param.Module.Name;
                        throw new ApplicationException(msg, ex);
                    }
                }
            }
            if (missingParam)
            {
                ReportChange(true);
                Recipe.SetError(this, listMissingsParameters);
            }
        }

        /// <summary>
        /// Écrit les parmètres dans le XML
        /// </summary>
        public virtual XmlNode Save(XmlDocument xmldoc, bool saveChildren = true)
        {
            XmlElement moduleNode = xmldoc.CreateElement("Module");
            moduleNode.SetAttribute("ModID", Id.ToString());
            moduleNode.SetAttribute("Name", Factory.ModuleName.ToString());

            // Parameters
            //...........
            XmlNode parametersNode = xmldoc.CreateElement("Parameters");
            moduleNode.AppendChild(parametersNode);
            foreach (ParameterBase param in ParameterList)
                param.Save(parametersNode);

            // Children
            //.........
            XmlNode childrenNode = xmldoc.CreateElement("Children");
            moduleNode.AppendChild(childrenNode);
            if (saveChildren)
            {
                foreach (ModuleBase module in Children)
                {
                    XmlElement element = xmldoc.CreateElement("Child");
                    element.SetAttribute("ModID", module.Id.ToString());
                    childrenNode.AppendChild(element);
                }
            }

            return moduleNode;
        }

        //=================================================================
        // Récupération de la liste des paramètres par réflexion
        //=================================================================
        protected List<ParameterBase> _parameterList = null;
        protected List<ParameterBase> _exportableParameterList = null;

        /// <summary>
        /// List des paramètres du module.
        /// </summary>
		public List<ParameterBase> ParameterList
        {
            get
            {
                if (_parameterList == null)
                {
                    Type moduleType = GetType();
                    _parameterList = new List<ParameterBase>(
                        from f in moduleType.GetFields()
                        where f.FieldType.IsSubclassOf(typeof(ParameterBase))
                        select f.GetValue(this) as ParameterBase
                        );

                    if (_parameterList.Count == 0)
                        _parameterList.Add(new ParameterNone(this));
                }
                return _parameterList;
            }
        }

        /// <summary>
        /// Liste des paramètres exportable du modules (i.e. les paramètres
        /// qu'on peut choisir de voir dans la vue simplifiée).
        /// </summary>
        public List<ParameterBase> ExportableParameterList
        {
            get
            {
                if (_exportableParameterList == null)
                {
                    Type moduleType = GetType();
                    var list = new List<FieldInfo>(
                        from f in moduleType.GetFields()
                        where f.FieldType.IsSubclassOf(typeof(ParameterBase))
                        select f
                        );

                    _exportableParameterList = new List<ParameterBase>();
                    foreach (FieldInfo field in list)
                    {
                        bool exportable = true;
                        foreach (Attribute attr in field.GetCustomAttributes(true))
                        {
                            ExportableParameterAttribute xattr = attr as ExportableParameterAttribute;
                            if (xattr != null)
                            {
                                exportable = xattr.exportable;
                                break;
                            }
                        }
                        if (exportable)
                        {
                            ParameterBase param = (ParameterBase)field.GetValue(this);
                            _exportableParameterList.Add(param);
                        }
                    }
                }

                return _exportableParameterList;
            }
        }

        /// <summary>
        /// List des paramètres exportés (i.e. visible dans la vue simplifiée).
        /// </summary>
        public ObservableCollection<ParameterBase> ExportedParameterList
        {
            get
            {
                var list =
                        from p in ParameterList
                        where p.IsExported
                        select p;
                return new ObservableCollection<ParameterBase>(list);
            }
        }

        /// <summary>
        /// Liste des fichiers externes à la recette.
        /// </summary>
        public IEnumerable<ExternalRecipeFile> ExternalRecipeFileList
        {
            get
            {
                return ParameterList.OfType<FileParameter>().Select(p => p.Value);
            }
        }


        ///=================================================================
        ///<summary>
        /// Passe une data aux modules suivants.
        ///</summary>
        ///=================================================================
        protected void ProcessChildren(ObjectBase obj)
        {
            Interlocked.Increment(ref nbObjectsOut);

            //-------------------------------------------------------------
            // Rendering
            //-------------------------------------------------------------
            StoreRenderingObject(obj);

            //-------------------------------------------------------------
            // On envoie d'abord l'objet aux modules "read-only"
            //-------------------------------------------------------------
            foreach (ModuleBase child in ReadOnlyChildren)
                ProcessChild(child, obj);

            //-------------------------------------------------------------
            // Et ensuite on envoie l'objet aux modules qui peuvent le modifier
            //-------------------------------------------------------------
            for (int i = 0; i < ReadWriteChildren.Count; i++)
            {
                ModuleBase child = ReadWriteChildren[i];
                if (child.State == eModuleState.Disabled)
                    continue;

                // Clone l'image s'il y a plusieurs modules
                //.........................................
                ObjectBase obj2 = obj;
                if (obj != null && i != ReadWriteChildren.Count - 1)
                    obj2 = (ObjectBase)obj.DeepClone();

                // Traitement de l'image par le fils
                //..................................
                try
                {
                    ProcessChild(child, obj2);
                }
                finally
                {
                    // Destruction du clone
                    //.....................
                    if (obj2 != null && obj2 != obj)
                        obj2.DelRef();
                }
            }
        }

        //=================================================================
        //
        //=================================================================
        protected void ProcessChild(ModuleBase child, ObjectBase obj)
        {
            try
            {
                if (child.State == eModuleState.Disabled)
                    return;

                child.Process(this, obj);
            }
            catch (Exception ex)
            {
                Recipe.SetError(child, ex.Message);
                throw;
            }
        }

        ///=================================================================
        ///<summary>
        /// Demande au module de s'arrêter parce que le parent s'est arrêté et
        /// n'enverra plus de données.
        /// Cette fonction est appelée une fois pour chaque parent.
        /// Lorsque tous les parents sont arrétés, le module passe à l'état Stopping.
        /// L'arrêt effectif du module peut prendre beaucoup de temps si celui-ci 
        /// a accumulé des données à traiter.
        ///</summary>
        ///
        /// Si le module a besoin d'exécuter des traitements lorsqu'il s'arrête,
        /// Il faut surgarcher OnStopping() et non Stop().
        ///=================================================================
		public void Stop(ModuleBase parent)
        {
            // Stop seulement si tous les parents sont arrêtés
            //................................................
            logDebug("stop from module " + parent?.Id);
            int nb_active_parents = Interlocked.Decrement(ref _nbActiveParents);
            if (nb_active_parents > 0)
                return;

            // Changement d'état
            //..................
            if (State == eModuleState.Disabled)
            {
                StopChildren();
            }
            else
            {
                logDebug("stopping");
                eModuleState oldState = SetState(eModuleState.Stopping);

                OnStopping(oldState);
            }
        }

        ///=================================================================
        ///<summary>
        /// Fonction appellée lorsqu'on passe dans l'état Stopping.
        /// L'arrêt du module peut prendre beaucoup de temps si celui-ci a accumulé
        /// des données à traiter.
        /// Chaque module est responsable d'arrêter ses fils ; il est donc important
        /// que les surcharges appelle base.OnStopping().
        ///</summary>
        ///=================================================================
        protected virtual void OnStopping(eModuleState oldState)
        {
            // Changement d'état: Stopping (ou Aborting) à Stopped
            //....................................................
            log("stop");
            oldState = SetState(eModuleState.Stopped);
            if (oldState == eModuleState.Stopped)
                throw new ApplicationException("module already stopped");

            // On arrête les fils
            //...................
            StopChildren();
        }

        ///=================================================================
        ///<summary>
        /// Demande au module de s'arrêter sans finir ce qu'il a à faire.
        ///</summary>
        ///
        /// Abort() ne provoque pas l'arrêt immédiat du module. 
        /// Le module ne passera à l'état Stopped que lorsque le parents 
        /// appellera Stop().
        ///=================================================================
        public virtual void Abort()
        {
            logDebug("abort");
            SetState(eModuleState.Aborting);
        }

        ///=================================================================
        ///<summary>
        /// Arrête le module et demande aux fils de s'arrêter.
        ///</summary>
        ///=================================================================
        private void StopChildren()
        {
            logDebug("stopping children");

            // Sanity check
            //.............
            int bChildrenStopped = Interlocked.Exchange(ref _bChildrenStopped, 1);
            if (bChildrenStopped == 1)
                throw new ApplicationException("children already stopped");

            // Stop children
            //..............
            foreach (ModuleBase descendant in Children)
                descendant.Stop(this);
        }

        ///=================================================================
        ///<summary>
        /// Traite les exceptions produites lors du processing.
        ///</summary>
        ///=================================================================
        protected virtual void HandleException(Exception e)
        {
            Recipe.SetError(this, e.Message);

            logError("\r\n================================\r\n" + e + "\r\n================================");
            Recipe.Abort();
        }

        ///=================================================================
        ///<summary>
        /// Écrit dans le log de la recette
        ///</summary>
        ///=================================================================
        public void logDebug(string msg)
        {
            Recipe.logDebug(ToString() + ": " + msg);
        }
        protected void log(string msg)
        {
            Recipe.log(ToString() + ": " + msg);
        }
        protected void logWarning(string msg)
        {
            Recipe.logWarning(ToString() + ": " + msg);
        }
        protected void logError(string msg)
        {
            Recipe.logError(ToString() + ": " + msg);
        }

        ///=================================================================
        ///<summary>
        /// Teste si le module est correctement configuré.
        ///</summary>
        ///<returns> le message d'erreur ou null</returns>
        ///=================================================================
        public virtual string Validate()
        {
            //-------------------------------------------------------------
            // Validation du module
            //-------------------------------------------------------------
            if (this is RootModule || this is TerminationModule)
                return null;

            if (Parents.Count == 0)
                return "Module has no parent";

            if (Factory.DataProducer == DataProducerType.Data && Children[0] is TerminationModule)
                return "Module has no child";

            foreach (ModuleBase parent in Parents)
            {
                bool compatible = Recipe.CompatibilityManager.IsFactoryCompatibleWithParent(Factory, parent);
                if (!compatible)
                    return "Module is not compatible with its parent";
            }

            //-------------------------------------------------------------
            // Validation des paramètres
            //-------------------------------------------------------------
            string errorList = "";
            string error;

            foreach (ParameterBase param in ParameterList)
            {
                error = param.Validate();
                if (error != null)
                {
                    if (errorList.Length > 0)
                        errorList += " - ";
                    errorList += error;
                }
            }
            if (errorList.Length > 0)
                return errorList;
            return null;
        }

        ///=================================================================
        ///<summary>
        /// Indique si ce module ou un de ses fils a besoin la Layer converse toutes
        /// les données.
        ///</summary>
        ///=================================================================
        public bool RecursiveNeedAllData()
        {
            if (Factory.NeedAllData)
                return true;

            foreach (ModuleBase child in Children)
            {
                if (child.RecursiveNeedAllData())
                    return true;
            }

            return false;
        }

        //=================================================================
        //
        //=================================================================
        private bool IsReadOnlyRecursive()
        {
            if (this is TerminationModule)
                return true;

            if (Factory.ModifiesData)
                return false;

            foreach (ModuleBase child in Children)
            {
                if (!child.IsReadOnlyRecursive())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Comparaison de la valeur des paramétres entre deux modules
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ParametersValuesAreEquals(ModuleBase obj)
        {
            return ParameterList.ValuesEqual(obj.ParameterList);
        }

    }
}
