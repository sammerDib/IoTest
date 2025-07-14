using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Client.Proxy.Probe.Models
{
    public class ExportResultDoubleLise
    {

        private List<List<double>> _yRawAcquisitionProbeUp = new List<List<double>>();
        private List<float> _xRawAcquisitionProbeUp = new List<float>();
        private List<List<double>> _yRawAcquisitionProbeDown = new List<List<double>>();
        private List<float> _xRawAcquisitionProbeDown = new List<float>();
        private List<List<ProbePoint>> _selectedPeaksProbeUp = new List<List<ProbePoint>>();
        private List<List<ProbePoint>> _selectedPeaksProbeDown = new List<List<ProbePoint>>();
        private String _date;

        public List<List<double>> YRawAcquisitionProbeUp
        {
            get
            {
                return _yRawAcquisitionProbeUp;
            }
            set
            {
                _yRawAcquisitionProbeUp = value;
            }
        }


        public List<float> XRawAcquisitionProbeUp
        {
            get
            {
                return _xRawAcquisitionProbeUp;
            }
            set
            {
                _xRawAcquisitionProbeUp = value;
            }
        }

        public List<List<double>> YRawAcquisitionProbeDown
        {
            get
            {
                return _yRawAcquisitionProbeDown;
            }
            set
            {
                _yRawAcquisitionProbeDown = value;
            }
        }


        public List<float> XRawAcquisitionProbeDown
        {
            get
            {
                return _xRawAcquisitionProbeDown;
            }
            set
            {
                _xRawAcquisitionProbeDown = value;
            }
        }


        public List<List<ProbePoint>> SelectedPeaksProbeUp
        {
            get
            {
                return _selectedPeaksProbeUp;
            }
            set
            {
                _selectedPeaksProbeUp = value;
            }
        }

        public List<List<ProbePoint>> SelectedPeaksProbeDown
        {
            get
            {
                return _selectedPeaksProbeDown;
            }
            set
            {
                _selectedPeaksProbeDown = value;
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
