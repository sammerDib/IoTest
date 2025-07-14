using System;
using System.Collections.Generic;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Simulation;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740
{
    public partial class PC1740 : ISimDevice
    {
        #region Properties

        protected internal PC1740SimulationData SimulationData { get; private set; }

        #endregion

        #region ISimDevice

        public ISimDeviceView SimDeviceView
            => new PC1740SimulatorUserControl() { DataContext = SimulationData };

        private void SetUpSimulatedMode()
        {
            SimulationData = new PC1740SimulationData(this);
        }

        #endregion ISimDevice

        #region Commands

        protected override void InternalSimulateRead(string recipeName, Tempomat tempomat)
        {
            if (SimulationData.IsCommandExecutionFailed)
            {
                SubstrateId = string.Empty;
                throw new InvalidOperationException("Read command failed");
            }

            SubstrateId = SimulationData.SubstrateId;
        }

        protected override void InternalSimulateGetImage(string imagePath, Tempomat tempomat)
        {
            //Do nothing in simulation
        }

        protected override void InternalSimulateRequestRecipes(Tempomat tempomat)
        {
            var recipes = new List<RecipeModel>();
            if (Configuration.UseOnlyOneT7)
            {
                if (Configuration.T7Recipe == null)
                {
                    throw new Exception($"{nameof(Configuration.T7Recipe)} is null.");
                }

                recipes.Add(new RecipeModel(1, Configuration.T7Recipe.Name, false, true, 0));
            }
            else
            {
                recipes.AddRange(RecipeLibrarian.GetRecipes(Configuration.RecipeFolderPath));
            }

            Recipes.Clear();
            Recipes.AddRange(recipes);
        }

        #endregion
    }
}
