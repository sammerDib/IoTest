using System;
using System.Collections.Generic;

using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.TC.Shared.Operations.Implementation
{
    public class EquipmentConstantOperations : TemplateVariableOperations<IEquipmentConstantServiceCB, 
                                                                          EquipmentConstant, 
                                                                          ECName>, 
                                               IEquipmentConstantOperations
    {
        private ILogger _logger;

        public EquipmentConstantOperations()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<EquipmentConstantOperations>>();
            UtoVidService = ClassLocator.Default.GetInstance<IEquipmentConstantServiceCB>();
        }
        protected override bool IsVIDsValidated(Dictionary<string, EquipmentConstant> vidDictionary)
        {
            return true; // No checking yet
        }

        public List<EquipmentConstant> ECGetAllRequest()
        {
            List<EquipmentConstant> list = new List<EquipmentConstant>();
            foreach (var item in CurrentVDico.Values)
            {
                list.Add(item);
            }
            if (!FirstGetAllValuesRequestDone)
                FirstGetAllValuesRequestDone = true;
            return list;
        }

        public List<EquipmentConstant> ECGetRequest(List<int> ids)
        {
            try
            {
                List<EquipmentConstant> ecids = new List<EquipmentConstant>();
                foreach (var id in ids)
                {
                    EquipmentConstant result;
                    bool found = CurrentVDico.TryGetValue(id.ToString(), out result);
                    if (found)
                        ecids.Add(result);
                    else
                        ecids.Add(null);
                }
                return ecids;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + ex.StackTrace);
                return null;
            }
        }

        public bool ECSetRequest(EquipmentConstant equipmentConstant)
        {
            try
            {
                EquipmentConstant result;
                bool found = CurrentVDico.TryGetValue(equipmentConstant.Name, out result);
                if (found)
                {
                    result.Value = equipmentConstant.Value;
                    result.DataType = equipmentConstant.DataType;
                    result.Description = equipmentConstant.Description;
                    result.Maximum = equipmentConstant.Maximum;
                    result.Minimum = equipmentConstant.Minimum;
                    result.Units = equipmentConstant.Units;
                    result.DefaultValue = equipmentConstant.DefaultValue;
                }
                else
                    throw new Exception($"Set EC requested by UTO service for EC named {equipmentConstant.Name} failed.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + ex.StackTrace);
                return false;
            }
        }

        protected override EquipmentConstant FindVariableByName(List<EquipmentConstant> list, string ecName)
        {
            return list.Find(ec => ec.Name == ecName);
        }

        public void SetECValue(EquipmentConstant equipmentConstant)
        {
            SetECValues(new List<EquipmentConstant>() { equipmentConstant });
        }

        public void SetECValues(List<EquipmentConstant> equipmentConstants)
        {
            UtoVidService.SetECValues(equipmentConstants);
        }

        protected override VSettings<EquipmentConstant> GetDefaultSettings()
        {
            // No default values for the moment
            return new VSettings<EquipmentConstant>();
        }
    }
}
