using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Implementation
{
    [XmlInclude(typeof(BackLightMeasureConfiguration))]
    [XmlInclude(typeof(BrightFieldMeasureConfiguration))]
    [XmlInclude(typeof(DeflectometryMeasureConfiguration))]
    [XmlInclude(typeof(HighAngleDarkFieldMeasureConfiguration))]
    public class MeasuresConfiguration
    {
        private const string MeasuresConfigurationFileName = "MeasuresConfiguration.xml";

        public List<MeasureConfigurationBase> Measures { get; set; }

        public static MeasuresConfiguration Init(IDMTServiceConfigurationManager configurationManager)
        {
            MeasuresConfiguration measuresConfiguration;
            string fileName = Path.Combine(configurationManager.ConfigurationFolderPath, MeasuresConfigurationFileName);
            if (File.Exists(fileName))
            {
                measuresConfiguration = XML.Deserialize<MeasuresConfiguration>(fileName);
            }
            else
            {
                measuresConfiguration = new MeasuresConfiguration { Measures = new List<MeasureConfigurationBase>() };
            }

            if (!measuresConfiguration.Measures.OfType<BackLightMeasureConfiguration>().Any())
            {
                measuresConfiguration.Measures.Add(new BackLightMeasureConfiguration());
            }

            if (!measuresConfiguration.Measures.OfType<BrightFieldMeasureConfiguration>().Any())
            {
                measuresConfiguration.Measures.Add(new BrightFieldMeasureConfiguration());
            }

            if (!measuresConfiguration.Measures.OfType<DeflectometryMeasureConfiguration>().Any())
            {
                measuresConfiguration.Measures.Add(new DeflectometryMeasureConfiguration());
            }

            if (!measuresConfiguration.Measures.OfType<HighAngleDarkFieldMeasureConfiguration>().Any())
            {
                measuresConfiguration.Measures.Add(new HighAngleDarkFieldMeasureConfiguration());
            }

            return measuresConfiguration;
        }

        public T GetConfiguration<T>() where T : MeasureConfigurationBase
        {
            return Measures.OfType<T>().SingleOrDefault();
        }

        public MeasureConfigurationBase GetMeasureConfiguration(MeasureType measureType)
        {
            switch (measureType)
            {
                case MeasureType.BacklightMeasure:
                    return Measures.OfType<BackLightMeasureConfiguration>().SingleOrDefault();

                case MeasureType.BrightFieldMeasure:
                    return Measures.OfType<BrightFieldMeasureConfiguration>().SingleOrDefault();

                case MeasureType.DeflectometryMeasure:
                    return Measures.OfType<DeflectometryMeasureConfiguration>().SingleOrDefault();

                case MeasureType.HighAngleDarkFieldMeasure:
                    return Measures.OfType<HighAngleDarkFieldMeasureConfiguration>().SingleOrDefault();

                default:
                    throw new Exception($"The measure type {measureType} doesn't have an associated configuration");
            }
        }
    }
}
