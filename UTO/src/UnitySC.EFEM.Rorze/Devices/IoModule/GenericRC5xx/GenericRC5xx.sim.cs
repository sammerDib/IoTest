using System;

using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx
{
    public partial class GenericRC5xx
    {
        protected virtual void InternalSimulateSetOutputSignal(
            SignalData signalData,
            Tempomat tempomat)
        {
            // Implement the simulated behavior of the device.
            //
            // Statuses can be changed like this:
            // TemperatureSensor = Temperature.FromDegreesCelsius(30);
            //
            // And HW processing time can be simulated with the tempomat like this:
            // tempomat.Sleep(Duration.FromSeconds(5));
            // If you want to accelerate the simulation time by a factor of 2, write this:
            // Simulation.Instance.AssignSpeed(Ratio.FromPercent(200));
            // If you want to decelerate the simulation time by a factor of 2, write this:
            // Simulation.Instance.AssignSpeed(Ratio.FromPercent(50));
            throw new NotImplementedException("Command SetOutputSignal has not been implemented");
        }

        protected virtual void InternalSimulateSetDateAndTime(Tempomat tempomat)
        {
            // Implement the simulated behavior of the device.
            //
            // Statuses can be changed like this:
            // TemperatureSensor = Temperature.FromDegreesCelsius(30);
            //
            // And HW processing time can be simulated with the tempomat like this:
            // tempomat.Sleep(Duration.FromSeconds(5));
            // If you want to accelerate the simulation time by a factor of 2, write this:
            // Simulation.Instance.AssignSpeed(Ratio.FromPercent(200));
            // If you want to decelerate the simulation time by a factor of 2, write this:
            // Simulation.Instance.AssignSpeed(Ratio.FromPercent(50));
            throw new NotImplementedException("Command SetDateAndTime has not been implemented");
        }

        protected virtual void InternalSimulateGetStatuses(Tempomat tempomat)
        {
            // Implement the simulated behavior of the device.
            //
            // Statuses can be changed like this:
            // TemperatureSensor = Temperature.FromDegreesCelsius(30);
            //
            // And HW processing time can be simulated with the tempomat like this:
            // tempomat.Sleep(Duration.FromSeconds(5));
            // If you want to accelerate the simulation time by a factor of 2, write this:
            // Simulation.Instance.AssignSpeed(Ratio.FromPercent(200));
            // If you want to decelerate the simulation time by a factor of 2, write this:
            // Simulation.Instance.AssignSpeed(Ratio.FromPercent(50));
            throw new NotImplementedException("Command GetStatuses has not been implemented");
        }
    }
}
