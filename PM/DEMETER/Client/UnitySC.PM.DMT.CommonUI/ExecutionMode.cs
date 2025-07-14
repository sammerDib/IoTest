using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.DMT.CommonUI
{
    public class ExecutionMode : ObservableRecipient,IExecutionMode
    {
  

        private ExecutionModes _currentExecutionMode = ExecutionModes.Integrated;

        public ExecutionModes CurrentExecutionMode
        {
            get
            {
                return _currentExecutionMode;
            }

            set
            {
                if (_currentExecutionMode == value)
                {
                    return;
                }

                _currentExecutionMode = value;
                OnPropertyChanged(nameof(CurrentExecutionMode));
            }
        }
 
    }
}
