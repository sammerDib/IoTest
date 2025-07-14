using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    public static class MeasureToolsHelper
    {
        // TODO : this function is temporary.
        // Compatible measure for each probe should be saved in config files and use here.
        public static List<ProbeWithObjectivesMaterial> GetLiseTopAndBottomWithObjectives(AnaHardwareManager hardwareManager, params ModulePositions[] probePositions)
        {
            var compatibleProbes = new List<ProbeWithObjectivesMaterial>();
            foreach (var probePosition in probePositions)
            {
                var probesConfig = hardwareManager.GetProbesConfigsByPosition(probePosition);
                foreach (var probe in probesConfig)
                {
                    if (!(probe is ProbeDualLiseConfig) && probe.DeviceID != "ProbeLiseHF")
                    {
                        var objectives = hardwareManager.GetObjectiveConfigsByPosition(probePosition);
                        compatibleProbes.Add(new ProbeWithObjectivesMaterial
                        {
                            ProbeId = probe.DeviceID,
                            CompatibleObjectives = objectives.Where(obj => obj.ObjType == ObjectiveConfig.ObjectiveType.NIR)
                                                             .Select(obj => obj.DeviceID).ToList(),
                        });
                    }
                }
            }
            return compatibleProbes;
        }

        public static ProbeWithObjectivesMaterial GetProbeLiseWithObjectives(AnaHardwareManager hardwareManager, ModulePositions position)
        {
            var probeConfig = hardwareManager.GetProbesConfigsByPosition(position).Where(p => p is ProbeLiseConfig).First();
            string deviceId = probeConfig.DeviceID;
            var objectives = hardwareManager.GetObjectiveConfigsByPosition(position).ToList();
            var objectivesId = new List<string>();
            foreach (var objective in objectives)
            {
                objectivesId.Add(objective.DeviceID);
            }

            var probe = new ProbeWithObjectivesMaterial
            {
                CompatibleObjectives = objectivesId,
                ProbeId = deviceId,
            };

            return probe;
        }

        public static DualProbeWithObjectivesMaterial GetDualLiseProbe(AnaHardwareManager hardwareManager, ProbeWithObjectivesMaterial probeUp, ProbeWithObjectivesMaterial probeDown)
        {
            var ProbeDual = hardwareManager.GetProbesConfigsByPosition(ModulePositions.Up).Where(p => p is ProbeDualLiseConfig).First();
            string dualDeviceID = ProbeDual.DeviceID;
            var dualProbe = new DualProbeWithObjectivesMaterial
            {
                ProbeId = dualDeviceID,
                UpProbe = probeUp,
                DownProbe = probeDown
            };
            return dualProbe;
        }
    }
}
