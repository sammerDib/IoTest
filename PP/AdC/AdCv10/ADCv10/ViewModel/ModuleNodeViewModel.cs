using System;
using System.Collections.Generic;

using ADC.Controls;
using ADC.Model;

using ADCEngine;

using GraphModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.ViewModel
{
    public enum eModuleVisualState { Idle, Running, Stopped, Error, Warning, Exports, Disabled, Different, Same, Added, Removed };

    ///////////////////////////////////////////////////////////////////////
    // Sur-classe permettant de spécialiser pour l'ADC le comportant 
    // d'un NodeViewModel (qui est ADC agnostic) .
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ModuleNodeViewModel : NodeViewModel, ICloneable
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public ModuleNodeViewModel(ModuleBase module)
        {
            Data = module;
        }


        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return "VM: " + Module;
        }

        public ModuleNodeViewModel()
        {
        }

        //=================================================================
        // liste d'Ids pour Undo/Redo
        //=================================================================
        private List<int> _listeParentId = new List<int>();  // Memo of parents

        public List<int> listeParentId
        {
            get { return _listeParentId; }
        }

        private List<int> _listeChildId = new List<int>();   // Memo of parents
        public List<int> listeChildId
        {
            get { return _listeChildId; }
        }

        //=================================================================
        // list parents and childs id
        //=================================================================

        /// <summary>
        /// Memo a child id
        /// </summary>
        /// <param name="child"></param>
        public void MemoChild(ModuleNodeViewModel child)
        {
            _listeChildId.Add(child.Module.Id);
        }

        /// <summary>
        /// Memo a parent id
        /// </summary>
        /// <param name="parent"></param>
        internal void MemoParent(ModuleNodeViewModel parent)
        {
            _listeParentId.Add(parent.Module.Id);
        }

        public object Clone()
        {
            ModuleNodeViewModel cloneNode = new ModuleNodeViewModel();

            cloneNode.Name = Name;
            cloneNode.Info = Info;
            cloneNode.Data = ServiceRecipe.Instance().RecipeCurrent.CopyModule(Module);
            cloneNode.X = X;
            cloneNode.Y = Y;
            cloneNode.ZIndex = ZIndex;
            cloneNode.Size = Size;

            foreach (int id in _listeChildId)
            {
                cloneNode._listeChildId.Add(id);
            }
            foreach (int id in _listeParentId)
            {
                cloneNode._listeParentId.Add(id);
            }
            foreach (ConnectorViewModel connector in cloneNode.OutputConnectors)
                connector.ParentNode = cloneNode;
            foreach (ConnectorViewModel connector in cloneNode.InputConnectors)
                connector.ParentNode = cloneNode;

            return cloneNode;
        }


        //=================================================================
        // Data
        //=================================================================
        // Le module géré
        public ModuleBase Module { get { return (ModuleBase)Data; } }

        public override Object Data
        {
            get { return base.Data; }
            set
            {
                base.Data = value;
                if (Module is RootModule)
                    Fullname = "Recipe";
                else
                    Fullname = Module.DisplayName;
            }
        }

        public List<ParameterBase> ModuleParameterList => Module.ParameterList;
        public string DisplayNameInParamView => Module.DisplayNameInParamView;


        //=================================================================
        // Nom
        //=================================================================
        private string _fullname;
        public string Fullname
        {
            get { return _fullname; }
            set
            {
                if (_fullname == value)
                    return;
                _fullname = value;

                OnFullnameChanged();
                OnPropertyChanged();
            }
        }

        protected virtual void OnFullnameChanged()
        {
            string[] lines = _fullname.Split('\n');
            Name = lines[0];
            if (lines.Length > 1)
                Info = lines[1];
            else
                Info = null;
        }

        //=================================================================
        // ProgressInfo (stats pour le mode run)
        //=================================================================
        private string _progressInfo;
        public string ProgressInfo
        {
            get { return _progressInfo; }
            set
            {
                if (_progressInfo == value)
                    return;
                _progressInfo = value;

                OnFullnameChanged();
                OnPropertyChanged();
            }
        }


        //=================================================================
        // State
        //=================================================================
        private eModuleVisualState _state = eModuleVisualState.Idle;
        public eModuleVisualState State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                    return;
                _state = value;
                OnPropertyChanged();
            }
        }

        //=================================================================
        // State
        //=================================================================
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message == value)
                    return;
                _message = value;
                OnPropertyChanged();
            }
        }

        //=================================================================
        //
        //=================================================================
        public void RefreshStatistics(RecipeStat rstat)
        {
            if (Module is RootModule)
            {
                Fullname = "Recipe";
                State = GetRecipeState(rstat);
                if (State == eModuleVisualState.Error)
                {
                    ModuleBase faultyModule = Module.Recipe.ModuleList[rstat.FaultyModuleId];
                    ModuleStat mstat = rstat.ModuleStat[rstat.FaultyModuleId];
                    Message = "Error on module: " + faultyModule + "\n" + mstat.ErrorMessage;
                }
                else
                    Message = null;
            }
            else
            {
                ModuleStat mstat = rstat.ModuleStat[Module.Id];

                Fullname = Module.DisplayName;
                State = GetModuleState(mstat);
                Message = mstat.ErrorMessage;
                ProgressInfo = mstat.State + " in:" + mstat.nbObjectsIn + " out:" + mstat.nbObjectsOut;
            }
        }

        //=================================================================
        //
        //=================================================================
        public void Validate()
        {
            if (Module is RootModule)
            {
                State = eModuleVisualState.Idle;
                Message = null;
            }
            else
            {
                Fullname = Module.DisplayName;

                if (Module.Parents.Count == 0)
                {
                    Message = "Module has no parent";
                    State = eModuleVisualState.Warning;
                    return;
                }

                string error = Module.Validate();
                Message = error;
                if (error != null)
                {
                    State = eModuleVisualState.Warning;
                    return;
                }

                if (HasExport())
                {
                    State = eModuleVisualState.Exports;
                    Message = "Module exports parameters";
                    return;
                }

                Message = null;
                State = eModuleVisualState.Idle;
            }
        }

        //=================================================================
        // 
        //=================================================================
        private bool HasExport()
        {
            foreach (ParameterBase param in Module.ParameterList)
            {
                if (param.IsExported)
                    return true;
            }

            return false;
        }



        //=================================================================
        //
        //=================================================================
        protected eModuleVisualState GetRecipeState(RecipeStat rstat)
        {
            if (rstat.HasError)
                return eModuleVisualState.Error;
            else if (rstat.IsRunning)
                return eModuleVisualState.Running;
            else
                return eModuleVisualState.Stopped;
        }

        //=================================================================
        //
        //=================================================================
        protected eModuleVisualState GetModuleState(ModuleStat mstat)
        {
            if (mstat.HasError)
            {
                return eModuleVisualState.Error;
            }
            else
            {
                switch (mstat.State)
                {
                    case eModuleState.Stopped:
                        return eModuleVisualState.Stopped;
                    case eModuleState.Loaded:
                    case eModuleState.Running:
                    case eModuleState.Stopping:
                    case eModuleState.Aborting:
                        return eModuleVisualState.Running;
                    case eModuleState.Disabled:
                        return eModuleVisualState.Disabled;
                    default:
                        throw new ApplicationException("unknown state: " + Module.State);
                }
            }
        }

        /// <summary>
        /// Index de la couleur du noeud
        /// </summary>
        private int _backgroundColorIndex;
        public int BackgroundColorIndex
        {
            get => _backgroundColorIndex; set { if (_backgroundColorIndex != value) { _backgroundColorIndex = value; OnPropertyChanged(); } }
        }


        #region commands


        private AutoRelayCommand _openHelpCommand;
        public AutoRelayCommand OpenHelpCommand
        {
            get
            {
                return _openHelpCommand ?? (_openHelpCommand = new AutoRelayCommand(
              () =>
              {
                  ADCHelpDisplay.OpenMainHelp(Module.HelpName);
              },
              () => { return !string.IsNullOrEmpty(Module.HelpName); }));
            }
        }

        #endregion
    }
}
