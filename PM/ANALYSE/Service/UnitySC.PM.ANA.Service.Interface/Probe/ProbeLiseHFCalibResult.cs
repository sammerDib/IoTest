using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeLiseHFCalibResult : ProbeCalibResultsBase
    {
        [DataMember]
        public string ObjectiveId { get; set; }

        [DataMember]
        public double IntegrationTime_ms { get; set; } // from int to double [02/09/2024]

        [DataMember]
        public int AttenuationId { get; set; }

        [DataMember]
        public int NbAverage { get; set; }

        [DataMember]
        public RawCalibrationSignal DarkRawSignal { get; set; } // Normalized Smoothed Signal

        [DataMember]
        public RawCalibrationSignal RefRawSignal { get; set; } // Normalized Smoothed Signal
    }

    [DataContract]
    public class RawCalibrationSignal
    {
        [DataMember]
        public List<double> WaveLength_nm { get; set; }

        [DataMember]
        public List<double> RawSignal { get; set; }

        public double ComputeRawSignalMax()
        {
            if (RawSignal != null && RawSignal.Count > 0)
            {
                return RawSignal.Max();
            }
            return 0.0;
        }

        public double ComputeNormalisedEnergy()
        {
            var max = ComputeRawSignalMax();
            if (max == 0.0)
            {
                return 0.0;
            }

            List<double> normalisedSignal = RawSignal.Select(s => (s / max) * (s / max)).ToList();

            // Calcul de l'aire par addition des trapèzoides
            double energy = 0.0;
            int count = Math.Min(WaveLength_nm.Count, normalisedSignal.Count);
            for (int i = 0; i < count - 1; i++)
            {
                energy += ((normalisedSignal[i + 1] + normalisedSignal[i]) * (WaveLength_nm[i + 1] - WaveLength_nm[i])) * 0.5;
            }
            return energy;
        }

        public double ComputeRawSignalZone()
        {
            if (RawSignal != null && RawSignal.Count > 0)
            {
                return RawSignal.Max();
            }
            return 0.0;
        }

        public List<double> ComputeZoneIntensity()
        {
            var delimzones = new List<double>(6) { 400.0, 500.0, 600.0, 700.0, 800.0, 900.0 };
            var zones = new List<double>(6) { 0.0, 0.0, 0.0, 0.0, 0.0, -1.0 }; // last should be -1
           
            int currentzoneindex = 0;
            for (int i = 0; i < WaveLength_nm.Count(); i++)
            {
                if (WaveLength_nm[i] >= delimzones[currentzoneindex + 1])
                    ++currentzoneindex;

                if (currentzoneindex >= (zones.Count - 1))
                    break; // last zone has been outranged

                if (WaveLength_nm[i] >= delimzones[currentzoneindex])
                    zones[currentzoneindex] += RawSignal[i];
            }
            zones.RemoveAt(zones.Count - 1);
            return zones;
        }
    }
}
