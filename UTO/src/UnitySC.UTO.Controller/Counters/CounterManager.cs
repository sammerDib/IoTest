using System;
using System.IO;
using System.Xml.Serialization;

namespace UnitySC.UTO.Controller.Counters
{
    public class CounterManager
    {
        #region Fields

        private readonly string _path;
        private readonly DateTime _startTime;
        #endregion

        #region Properties

        public PersistentCounters PersistentCounters { get; private set; }

        public ulong Uptime
        {
            get => (ulong)(DateTime.Now - _startTime).TotalMinutes;
        }
        #endregion

        #region Constructor

        public CounterManager(string path)
        {
            _startTime = DateTime.Now;
            _path = Path.Combine(path, "PersistentCounters.xml");
            Load();
            IncrementCounter(CounterDefinition.StartCounter);
        }

        #endregion

        #region Public

        public void IncrementCounter(CounterDefinition counterDefinition, bool reset = false)
        {
            switch (counterDefinition)
            {
                case CounterDefinition.ProcessedSubstrateCounter:
                    if (!reset)
                    {
                        PersistentCounters.ProcessedSubstrateCounter++;
                    }
                    else
                    {
                        PersistentCounters.ProcessedSubstrateCounter = 0;
                    }

                    break;
                case CounterDefinition.StartCounter:
                    if (!reset)
                    {
                        PersistentCounters.StartCounter++;
                    }
                    else
                    {
                        PersistentCounters.StartCounter = 0;
                    }

                    break;
                case CounterDefinition.FatalErrorCounter:
                    if (!reset)
                    {
                        PersistentCounters.FatalErrorCounter++;
                    }
                    else
                    {
                        PersistentCounters.FatalErrorCounter = 0;
                    }

                    break;
                case CounterDefinition.JobCounter:
                    if (!reset)
                    {
                        PersistentCounters.JobCounter++;
                    }
                    else
                    {
                        PersistentCounters.JobCounter = 0;
                    }

                    break;
                case CounterDefinition.AbortCounter:
                    if (!reset)
                    {
                        PersistentCounters.AbortCounter++;
                    }
                    else
                    {
                        PersistentCounters.AbortCounter = 0;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(counterDefinition), counterDefinition, null);
            }
            Save();
        }

        #endregion

        #region private

        private void Save()
        {
            // Instantiate XmlSerializer
            XmlSerializer serializer = new XmlSerializer(typeof(PersistentCounters));

            // Open StreamReader to write XML data
            using StreamWriter writer = new StreamWriter(_path);

            // Serialize the object to XML
            serializer.Serialize(writer, PersistentCounters);
        }

        private void Load()
        {
            // Instantiate XmlSerializer
            XmlSerializer serializer = new XmlSerializer(typeof(PersistentCounters));

            // Open StreamReader to read XML data
            using StreamReader reader = new StreamReader(_path);

            // Deserialize object from XML
            PersistentCounters = (PersistentCounters)serializer.Deserialize(reader);
        }
        #endregion
    }
}
