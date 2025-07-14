using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Client.Proxy.Probe.Models
{
    public class ExportResultSimpleLise
    {
        private List<List<double>> _yRawAcquisition = new List<List<double>>();
        private List<float> _xRawAcquisition = new List<float>();
        private List<List<ProbePoint>> _selectedPeaks = new List<List<ProbePoint>>();
        private String _date;
      

        public List<List<double>> YRawAcquisition
        {
            get
            {
                return _yRawAcquisition;
            }
            set
            {
                _yRawAcquisition = value;
            }
        }


        public List<float> XRawAcquisition
        {
            get
            {
                return _xRawAcquisition;
            }
            set
            {
                _xRawAcquisition = value;
            }
        }

     


        public List<List<ProbePoint>> SelectedPeaks
        {
            get
            {
                return _selectedPeaks;
            }
            set
            {
                _selectedPeaks = value;
            }
        }

        public String Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }

    }

}
