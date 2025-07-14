using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;
using System.Linq;

namespace UnitySC.Shared.TC.Shared.Operations.Implementation
{
    public abstract class StatusVariableOperations<TSVName> : TemplateVariableOperations<IStatusVariableServiceCB, 
                                                                                         StatusVariable, 
                                                                                         TSVName>, 
                                                              IStatusVariableOperations
                                                              where TSVName : Enum
    {
        private ILogger _logger;
        public StatusVariableOperations()
        {
            Logger = ClassLocator.Default.GetInstance<ILogger<StatusVariableOperations<TSVName>>>();
            UtoVidService = ClassLocator.Default.GetInstance<IStatusVariableServiceCB>();
        }
        public bool IsReadyToExchangeWithTC
        {
            get => FirstGetAllValuesRequestDone;
        }

        public ILogger Logger { get => _logger; private set => _logger = value; }
        protected override StatusVariable FindVariableByName(List<StatusVariable> list, string svName)
        {
            return list.Find(sv => sv.Name == svName);
        }

        protected StatusVariable GetNewStatusVariable(TSVName sv, string description, String value, string unit)
        {
            StatusVariable newItem = new StatusVariable(sv.ToString(), VidDataType.String, description);
            newItem.Value = value;
            newItem.Units = unit;
            return newItem;
        }

        protected override bool IsVIDsValidated(Dictionary<string, StatusVariable> svidDictionary)
        {
            // No StatusVariable == null
            var badPairs = svidDictionary.Where(pair => pair.Value == null).ToList();
            bool isValidated = !(badPairs.Count() > 0);
            if (!isValidated)
            {
                foreach (var badSvid in badPairs)
                {
                    Logger.Error($"SVID={badSvid.Key} is not correctly configured in StatusVariables configuration file");
                }
            }            
            if (isValidated)
            {
                var list = Enum.GetValues(typeof(TSVName)).Cast<TSVName>().Select(enumValue => enumValue.ToString()); // enum in list of string
                isValidated = list.All(name => svidDictionary.Count(entry => entry.Value.Name == name) == 1);         // checks if, for each name of te list, the number of occurrences in the dictionary is equal to 1.             
            }

            return isValidated;
        }
        #region IStatusVariableOperations
        public void SVSetRequest(List<StatusVariable> svids)
        {
            UtoVidService.SVSetMessage(svids);
        }

        public List<StatusVariable> SVGetAllRequest()
        {
            List<StatusVariable> list = new List<StatusVariable>();
            foreach (var item in CurrentVDico.Values)
            {
                list.Add(item);
            }
            if (!FirstGetAllValuesRequestDone)
                FirstGetAllValuesRequestDone = true;
            return list;
        }

        public List<StatusVariable> SVGetRequest(List<int> ids)
        {
            List<StatusVariable> list = new List<StatusVariable>();
            foreach (int id in ids)
            {
                StatusVariable result;
                bool found = CurrentVDico.TryGetValue(id.ToString(), out result);
                if (found)
                    list.Add(result);
                else
                    list.Add(null);
            }
            return list;
        }


        #endregion IStatusVariableOperations
    }
}
