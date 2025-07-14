using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command.Parameter
{
    /// <summary>
    /// Material location container choice ViewModel.
    /// </summary>
    public class MaterialLocationContainerChoiceViewModel : ParameterViewModel
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MaterialLocationContainerChoiceViewModel"/>.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <param name="commandViewModel">The command ViewModel.</param>
        public MaterialLocationContainerChoiceViewModel(Agileo.EquipmentModeling.Parameter parameter, DeviceCommandViewModel commandViewModel)
            : base(parameter, commandViewModel, typeof(IMaterialLocationContainer))
        {
            MaterialLocationContainers =
                new List<IMaterialLocationContainer>(commandViewModel.Device.GetEquipment()
                    .AllOfType<IMaterialLocationContainer>());

            var selectedLocationContainer = MaterialLocationContainers.FirstOrDefault(mlc =>
                mlc.QualifiedName.Equals((Value as IMaterialLocationContainer)?.QualifiedName, StringComparison.Ordinal));

            Value = selectedLocationContainer ?? MaterialLocationContainers.FirstOrDefault();
        }

        /// <summary>
        /// Gets the available material location containers.
        /// </summary>
        public List<IMaterialLocationContainer> MaterialLocationContainers { get; }
    }
}
