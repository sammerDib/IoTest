using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.TC.Shared.Data 
{
    public class ProcessStepProgressInfo
    {
        public int Percentage { get; set; }
        public String SubstID { get; set; }
    }

    public class PMProgressInfo 
    {
        public List<ProcessStepProgressInfo> StepProgressInfo { get; set; }
        public String RecipeName { get; set; }
        public PMProgressInfo(int stepNumber, string recipeName) 
        {
            RecipeName = recipeName;
            StepProgressInfo = new List<ProcessStepProgressInfo>();
            for (int i = 0; i < stepNumber; i++)
            {
                ProcessStepProgressInfo newPspi = new ProcessStepProgressInfo();
                newPspi.Percentage = 0;
                newPspi.SubstID = String.Empty;
                StepProgressInfo.Add(newPspi);
            }
        }

        public int TotalProgress_Percentage 
        { 
            get
            {
                int result = 0;
                foreach (var stepProgressInfo in StepProgressInfo)
                {
                    result += stepProgressInfo.Percentage;
                }
                return result; 
            }
        }
    }
}
