using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.GUI.Components;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E94;

using UnitySC.Equipment.Abstractions.Material;

namespace UnitySC.UTO.Controller.JobQueuer
{
    public class JobSpecification : Notifier
    {
        public List<MaterialNameListElement> MaterialNameList { get; }

        public Collection<string> CarrierInputSpec { get; }

        public List<Wafer> Wafers { get; }

        public string RecipeName { get; }

        public bool OcrReading { get; }

        public string OcrProfileName { get; }

        public bool LoopMode { get; }
        public uint NumberOfExecutions { get; }

        private uint _currentExecution;
        public uint CurrentExecution
        {
            get => _currentExecution;
            set
            {
                if (SetAndRaiseIfChanged(ref _currentExecution, value))
                {
                    OnPropertyChanged(nameof(JobsProgression));
                }
            }
        }

        private IProcessJob _processJob;
        public IProcessJob ProcessJob
        {
            get => _processJob;
            set => SetAndRaiseIfChanged(ref _processJob, value);
        }

        private IControlJob _controlJob;
        public IControlJob ControlJob
        {
            get => _controlJob;
            set => SetAndRaiseIfChanged(ref _controlJob, value);
        }

        public string JobsProgression
        {
            get
            {
                if (LoopMode)
                {
                    return $"{CurrentExecution}/∞";
                }
                return $"{CurrentExecution}/{NumberOfExecutions}";
            }
        }

        public JobSpecification(
            List<MaterialNameListElement> materialNameList,
            Collection<string> carrierInputSpec,
            List<Wafer> wafers,
            string recipeName,
            bool ocrReading,
            string ocrProfileName,
            bool loopMode,
            uint numberOfExecutions)
        {
            MaterialNameList = new List<MaterialNameListElement>(materialNameList);
            CarrierInputSpec = new Collection<string>(carrierInputSpec.ToList());
            Wafers = new List<Wafer>(wafers);
            RecipeName = recipeName;
            OcrReading = ocrReading;
            OcrProfileName = ocrProfileName;
            LoopMode = loopMode;
            NumberOfExecutions = numberOfExecutions;
            CurrentExecution = 0;
        }
    }
}
