using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.SecsGem;
using UnitySC.Shared.DataCollectionConverter;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.Dataflow.Shared
{
    public class DataCollectionConvert : IDataCollectionConvert
    {
        private ILogger _logger;

        [ImportMany(typeof(IDataCollectionConverter))]
        public IEnumerable<Lazy<IDataCollectionConverter>> DataCollectionConverters { get; set; }
        public DataCollectionConvert()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            var path = ".\\DataCollectionConverters";
            try
            {

                _logger.Information("Load DataCollectionConverters: " + path);
                var catalog = new DirectoryCatalog(path, "UnitySC.DataCollectionConverter.*.dll");
                var container = new CompositionContainer(catalog);
                container.ComposeParts(this);
                if (DataCollectionConverters.Count() == 0)
                {
                    throw new Exception($"No converter library found in {path}");
                }

            }
            catch (ReflectionTypeLoadException ex)
            {
                string errorMessage = "Error during loading MEF container part for converters in " + path;
                foreach (var error in ex.LoaderExceptions)
                {
                    errorMessage = errorMessage + Environment.NewLine + "- " + error.Message;
                }
                _logger.Error(errorMessage, ex);
                throw new Exception(errorMessage, ex);
            }
            catch (Exception ex)
            {
                string errorMessage = "Error during loading MEF container part for converters in " + path;
                _logger.Error(errorMessage, ex);
                throw new Exception(errorMessage, ex);
            }
        }
        public SecsVariableList ConvertToSecsVariableList(ModuleDataCollection moduleDataCollection)
        {
            SecsVariableList secsVariableList;
            foreach (var dataCollectionConverter in DataCollectionConverters)
            {
                secsVariableList = dataCollectionConverter.Value.ConvertToSecsVariableList(moduleDataCollection);
                if (secsVariableList != null)
                    return secsVariableList;
            }
            return null;
        }
    }
}
