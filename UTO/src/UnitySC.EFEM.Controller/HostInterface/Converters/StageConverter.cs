using System;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Converters
{
    public static class StageConverter
    {
        public static IMaterialLocationContainer ToMaterialLocationContainer(Constants.Stage stage, EfemEquipmentManager equipmentManager)
        {
            switch (stage)
            {
                case Constants.Stage.LP1:
                    return equipmentManager.LoadPorts[1];

                case Constants.Stage.LP2:
                    return equipmentManager.LoadPorts[2];

                case Constants.Stage.LP3:
                    return equipmentManager.LoadPorts[3];

                case Constants.Stage.LP4:
                    return equipmentManager.LoadPorts[4];

                case Constants.Stage.Tilt:
                    return equipmentManager.ProcessModules[1];

                case Constants.Stage.Aligner:
                    return equipmentManager.Aligner;

                case Constants.Stage.TiltPort2:
                    return equipmentManager.ProcessModules[2];

                case Constants.Stage.TiltPort3:
                    return equipmentManager.ProcessModules[3];

                case Constants.Stage.TiltPort4:
                    return equipmentManager.ProcessModules[4];

                default:
                    throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
            }
        }

        public static Constants.Stage FromMaterialLocationContainer(IMaterialLocationContainer locationContainer, EfemEquipmentManager equipmentManager)
        {
            if (locationContainer.Equals(equipmentManager.LoadPorts[1]))
            {
                return Constants.Stage.LP1;
            }

            if (locationContainer.Equals(equipmentManager.LoadPorts[2]))
            {
                return Constants.Stage.LP2;
            }

            if (locationContainer.Equals(equipmentManager.LoadPorts[3]))
            {
                return Constants.Stage.LP3;
            }

            if (locationContainer.Equals(equipmentManager.LoadPorts[4]))
            {
                return Constants.Stage.LP4;
            }

            if (locationContainer.Equals(equipmentManager.ProcessModules[1]))
            {
                return Constants.Stage.Tilt;
            }

            if (locationContainer.Equals(equipmentManager.ProcessModules[2]))
            {
                return Constants.Stage.TiltPort2;
            }

            if (locationContainer.Equals(equipmentManager.ProcessModules[3]))
            {
                return Constants.Stage.TiltPort3;
            }

            if (locationContainer.Equals(equipmentManager.ProcessModules[4]))
            {
                return Constants.Stage.TiltPort4;
            }

            if (locationContainer.Equals(equipmentManager.Aligner))
            {
                return Constants.Stage.Aligner;
            }

            throw new ArgumentOutOfRangeException(nameof(locationContainer), locationContainer, null);
        }
    }
}
