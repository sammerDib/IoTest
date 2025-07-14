using System.Runtime.Serialization;

using Agileo.GUI.Components;

namespace UnitySC.UTO.Controller.Counters
{
    [DataContract]
    public class PersistentCounters : Notifier
    {
        [DataMember]
        private ulong _processedSubstrateCounter;
        public ulong ProcessedSubstrateCounter
        {
            get => _processedSubstrateCounter;
            set { SetAndRaiseIfChanged(ref _processedSubstrateCounter, value); }
        }

        [DataMember]
        private ulong _startCounter;
        public ulong StartCounter
        {
            get => _startCounter;
            set { SetAndRaiseIfChanged(ref _startCounter, value); }
        }

        [DataMember]
        private ulong _fatalErrorCounter;
        public ulong FatalErrorCounter
        {
            get => _fatalErrorCounter;
            set { SetAndRaiseIfChanged(ref _fatalErrorCounter, value); }
        }

        [DataMember]
        private ulong _jobCounter;
        public ulong JobCounter
        {
            get => _jobCounter;
            set { SetAndRaiseIfChanged(ref _jobCounter, value); }
        }

        [DataMember]
        private ulong _abortCounter;
        public ulong AbortCounter
        {
            get => _abortCounter;
            set { SetAndRaiseIfChanged(ref _abortCounter, value); }
        }
    }
}
