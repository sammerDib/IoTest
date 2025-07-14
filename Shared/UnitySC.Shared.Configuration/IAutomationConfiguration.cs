using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.Configuration
{
    public interface IAutomationConfiguration
    {
        string AlarmConfigurationFilePath { get; }
        string CEConfigurationFilePath { get; }
        string ECConfigurationFilePath { get; }
        string SVConfigurationFilePath { get; }
    }
}
