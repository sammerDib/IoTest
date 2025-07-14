using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.PM.ANA.Service.Interface;

using UnitySC.PM.ANA.Service.Interface.Probe;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbeResultsLiseVM : ObservableObject, IProbeThicknessesResult
    {
       public List<ProbeThicknessMeasure> LayersThickness { get; set; }
       
       public DateTime Timestamp { get; set; }
       
        public string Message { get; set; }
    }
}
