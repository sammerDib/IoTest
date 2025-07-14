using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.Shared.Logger;
using System.Linq;
using UnitySC.Shared.Tools;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Shared;

namespace UnitySC.PM.ANA.Service.Measure.Loader
{
    public class MeasureLoader
    {
        private ILogger _logger;

        public MeasureLoader(ILogger<MeasureLoader> logger)
        {
            _logger = logger;
            Measures = new List<IMeasure>();
            Init();
        }

        public List<IMeasure> Measures { get; }

        public IMeasure GetMeasure(MeasureType measureType)
        {
            return Measures.FirstOrDefault(x => x.MeasureType == measureType);
        }

        public List<MeasureType> GetAvailableMeasures()
        {
            return Measures.Select(x => x.MeasureType).ToList();
        }

        private void Init()
        {
            var measuresConfiguration = ClassLocator.Default.GetInstance<MeasuresConfiguration>();

            List<string> loadedMeasures = new List<string>();
            foreach(var measureType in measuresConfiguration.AuthorizedMeasures)
            {
                try
                {
                    Measures.Add(MeasureFactory.CreateMeasure(measureType));
                    loadedMeasures.Add(measureType.ToString());
                }
                catch (ArgumentException)
                {
                    _logger.Error($"Could not load measure of type \"{measureType}\"");
                }
            }
            _logger.Debug("Loaded measures: " + string.Join(", ", loadedMeasures));
        }
    }
}
