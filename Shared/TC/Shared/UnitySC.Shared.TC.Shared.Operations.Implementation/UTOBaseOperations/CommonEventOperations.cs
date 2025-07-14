using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.Data.SecsGem;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using UnitySC.Shared.DataCollectionConverter;
using UnitySC.Shared.Data.DVID;
using System.Linq;

namespace UnitySC.Shared.TC.Shared.Operations.Implementation
{
    public class CommonEventOperations : TemplateVariableOperations<ICommonEventServiceCB, CommonEvent, CEName>, ICommonEventOperations
    {
        private ILogger _logger;

        [ImportMany(typeof(IDataCollectionConverter))]
        public IEnumerable<Lazy<IDataCollectionConverter>> DataCollectionConverters { get; set; }

        public CommonEventOperations()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<CommonEventOperations>>();
            UtoVidService = ClassLocator.Default.GetInstance<ICommonEventServiceCB>();
  
        }

        protected override bool IsVIDsValidated(Dictionary<string, CommonEvent> vidDictionary)
        {
            return true; // No checking yet
        }

        public List<CommonEvent> CEGetAll()
        {
            _logger.Information("[Recv] CEService - CEGetAll");
            List<CommonEvent> list = new List<CommonEvent>();
            foreach (var item in CurrentVDico.Values)
            {
                list.Add(item);
            }
            return list;
        }

        public void Fire_CE(List<CEName> ceNameList)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < ceNameList.Count; i++)
                {
                    CommonEvent ce;
                    if (CurrentVDico.TryGetValue(ceNameList[i].ToString(), out ce))
                        UtoVidService.FireEvent(ce);
                }
            });
        }

        public void Fire_CE(CEName ceName, SecsVariableList dvids)
        {
            try
            {
                Task.Run(() =>
                {
                    CommonEvent ce;
                    if (CurrentVDico.TryGetValue(ceName.ToString(), out ce))
                    {
                        ce.DataVariables = dvids;
                        UtoVidService.FireEvent(ce);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + ex.StackTrace);
            }
        }

        protected override CommonEvent FindVariableByName(List<CommonEvent> list, string ceName)
        {
            return list.Find(ce => ce.Name == ceName);
        }

        protected override VSettings<CommonEvent> GetDefaultSettings()
        {
            // No default values for the moment
            VSettings<CommonEvent> newItems = new VSettings<CommonEvent>();
            newItems.VariableList.Add(new CommonEvent() { DataVariables= new SecsVariableList()});
            return newItems;

        }
    }
}
