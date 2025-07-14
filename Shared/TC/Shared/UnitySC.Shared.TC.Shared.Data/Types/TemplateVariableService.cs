using System;
using System.Collections.Generic;
using System.IO;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.Shared.TC.Shared.Data
{
    /// <summary>
    /// Template to regroup all mecanism for SVService, ECService
    /// </summary>
    /// <typeparam name="TCallback"></typeparam>            ITCSVServiceCallback    /   ITCECServiceCallback        /   ITIOCServiceCallback
    /// <typeparam name="TIOperation"></typeparam>            IStatusVariableService  /   IEquipmentConstantService   /   IIoService
    /// <typeparam name="TVariableType"></typeparam>        StatusVariable          /   EquipmentConstant           /   IoChannel
    /// <typeparam name="TenumVariableNames"></typeparam>   SVName                  /   ECName                      /   IOName
    public abstract class TemplateVariableOperations<TIServiceCB, TVariableType, TenumVariableNames>
    {
        private TIServiceCB _utoVidService;
        private Dictionary<string, TVariableType> _currentVDico = new Dictionary<string, TVariableType>();
        private bool _firstGetAllValuesRequestDone = false;
        public TIServiceCB UtoVidService { get => _utoVidService; set => _utoVidService = value; }
        protected Dictionary<string, TVariableType> CurrentVDico { get => _currentVDico; set => _currentVDico = value; }
        protected bool FirstGetAllValuesRequestDone { get => _firstGetAllValuesRequestDone; set => _firstGetAllValuesRequestDone = value; }
        protected abstract TVariableType FindVariableByName(List<TVariableType> list, string ecName);
        protected abstract bool IsVIDsValidated(Dictionary<string, TVariableType> vidDictionary);
        protected abstract VSettings<TVariableType> GetDefaultSettings();
        private bool _initialized = false;

        #region Constructor

        public TemplateVariableOperations()
        {
        }

        public void Init(string configurationFilePath)
        {
            if (_initialized) return;
            List<TVariableType> variableConfigList;
            if (!ListExtension.IsNullOrEmpty<String>(configurationFilePath) && File.Exists(configurationFilePath))
            {
                variableConfigList = XML.Deserialize<VSettings<TVariableType>>(configurationFilePath).VariableList;
            }
            else
            {

                variableConfigList = new List<TVariableType>();
                VSettings<TVariableType> newSetting = GetDefaultSettings();

                XML.Serialize(newSetting, configurationFilePath);
            }

            // For all item in enum TenumVariableNames
            foreach (String vName in Enum.GetNames(typeof(TenumVariableNames)))
            {
                // Find StatuVariable
                TVariableType vItem = FindVariableByName(variableConfigList, vName);
                // add to dico
                CurrentVDico.Add(vName, vItem);
            }

            if (!IsVIDsValidated(CurrentVDico))
                throw new Exception($"A VID at least is invalid in settings in configuration file : {configurationFilePath} ");

            _initialized = true;
        }

        #endregion Constructor
    }
}
