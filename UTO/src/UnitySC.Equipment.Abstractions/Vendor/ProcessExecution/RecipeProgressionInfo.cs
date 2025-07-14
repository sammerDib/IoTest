using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

using Agileo.ProcessingFramework.Instructions;

using UnitsNet;

namespace UnitySC.Equipment.Abstractions.Vendor.ProcessExecution
{
    public class RecipeProgressionInfo : INotifyPropertyChanged
    {
        #region Properties

        private List<Instruction> _instructions = new();

        public List<Instruction> Instructions
        {
            get => _instructions;
            set
            {
                _instructions = value;
                OnPropertyChanged(nameof(Instructions));
            }
        }

        private double _currentProgressionMaximum;

        public double CurrentProgressionMaximum
        {
            get => _currentProgressionMaximum;
            set
            {
                _currentProgressionMaximum = value;
                OnPropertyChanged(nameof(CurrentProgressionMaximum));
            }
        }

        private double _currentProgressionValue;

        public double CurrentProgressionValue
        {
            get => _currentProgressionValue;
            set
            {
                _currentProgressionValue = value;
                OnPropertyChanged(nameof(CurrentProgressionValue));
            }
        }

        private string _currentInformation = string.Empty;

        public string CurrentInformation
        {
            get => _currentInformation;
            set
            {
                _currentInformation = value;
                OnPropertyChanged(nameof(CurrentInformation));
            }
        }

        private string _progressionInfo = string.Empty;

        public string ProgressionInfo
        {
            get => _progressionInfo;
            set
            {
                _progressionInfo = value;
                OnPropertyChanged(nameof(ProgressionInfo));
            }
        }

        private Instruction _currentInstruction;

        public Instruction CurrentInstruction
        {
            get => _currentInstruction;
            set
            {
                _currentInstruction = value;
                OnPropertyChanged(nameof(CurrentInstruction));
            }
        }

        private bool _isStepExpected;

        public bool IsStepExpected
        {
            get => _isStepExpected;
            set
            {
                _isStepExpected = value;
                OnPropertyChanged(nameof(IsStepExpected));
            }
        }

        private bool _isStepDone;

        public bool IsStepDone
        {
            get => _isStepDone;
            set
            {
                _isStepDone = value;
                OnPropertyChanged(nameof(IsStepDone));
            }
        }

        private Duration _stepElapsedTime;

        public Duration StepElapsedTime
        {
            get => _stepElapsedTime;
            set
            {
                _stepElapsedTime = value;
                OnPropertyChanged(nameof(StepElapsedTime));
            }
        }

        private StringBuilder _programError = new();

        public StringBuilder ProgramError
        {
            get => _programError;
            set
            {
                _programError = value;
                OnPropertyChanged(nameof(ProgramError));
            }
        }

        private int _stepProgressionPercent;

        public int StepProgressionPercent
        {
            get => _stepProgressionPercent;
            set
            {
                _stepProgressionPercent = value;
                OnPropertyChanged(nameof(StepProgressionPercent));
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
