using System;
using System.Text;


namespace UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.Configuration
{
    /// <summary>
    /// Represent possible stopping positions for current <see cref="TransferLocation"/>.
    /// </summary>
    [Serializable]
    public class ModuleMappingPosition
    {
        public uint ArmFirstMappingPosition { get; set; }

        public uint ArmSecondMappingPosition { get; set; }


        public ModuleMappingPosition()
        {
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.AppendLine();
            builder.AppendLine($"First mapping position {ArmFirstMappingPosition}");
            builder.AppendLine($"Second mapping position {ArmSecondMappingPosition}");

            return builder.ToString();
        }
    }
}
