using System.Collections.Generic;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.SecsGem;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Analyse
{
    public partial class Analyse
    {
        public override List<StatusVariable> SimulateGetStatusVariables(List<int> ids)
        {
            var statusVariables = new List<StatusVariable>();
            if (ids.Count == 0)
            {
                statusVariables.Add(
                    new StatusVariable(
                        nameof(SVName_ANA.State),
                        VidDataType.String,
                        "Get PM state to Toolcommander") { Value = "OffLine" });
                statusVariables.Add(
                    new StatusVariable(
                        nameof(SVName_ANA.RecipeName),
                        VidDataType.String,
                        "Get selected Recipe name to Toolcommander") { Value = "JobTest" });
                statusVariables.Add(
                    new StatusVariable(
                        nameof(SVName_ANA.ReadyForProcess),
                        VidDataType.Boolean,
                        "Get state if PM is ready for for Processing") { Value = true });
                statusVariables.Add(
                    new StatusVariable(
                        nameof(SVName_ANA.RecipeActive),
                        VidDataType.Boolean,
                        "Get state if recipe is currently running") { Value = false });
                return statusVariables;
            }

            foreach (var id in ids)
            {
                switch (id)
                {
                    case (int)SVName_ANA.State:
                        statusVariables.Add(
                            new StatusVariable(
                                nameof(SVName_ANA.State),
                                VidDataType.String,
                                "Get PM state to Toolcommander") { Value = "OffLine" });
                        break;
                    case (int)SVName_ANA.RecipeName:
                        statusVariables.Add(
                            new StatusVariable(
                                nameof(SVName_ANA.RecipeName),
                                VidDataType.String,
                                "Get selected Recipe name to Toolcommander") { Value = "JobTest" });
                        break;
                    case (int)SVName_ANA.ReadyForProcess:
                        statusVariables.Add(
                            new StatusVariable(
                                nameof(SVName_ANA.ReadyForProcess),
                                VidDataType.Boolean,
                                "Get state if PM is ready for for Processing") { Value = true });
                        break;
                    case (int)SVName_ANA.RecipeActive:
                        statusVariables.Add(
                            new StatusVariable(
                                nameof(SVName_ANA.RecipeActive),
                                VidDataType.Boolean,
                                "Get state if recipe is currently running") { Value = false });
                        break;
                }
            }

            return statusVariables;
        }

        public override List<EquipmentConstant> SimulateGetEquipmentConstants(List<int> ids)
        {
            var equipmentConstant = new List<EquipmentConstant>();
            if (ids.Count == 0)
            {
                equipmentConstant.Add(
                    new EquipmentConstant(
                        nameof(ECName.ECExample1),
                        VidDataType.String,
                        "Get something for Toolcommander",
                        "unknown"));
                equipmentConstant.Add(
                    new EquipmentConstant(
                        nameof(ECName.ECExample2),
                        VidDataType.Boolean,
                        "Get selected Recipe name to Toolcommander",
                        false));
                return equipmentConstant;
            }

            foreach (var id in ids)
            {
                switch (id)
                {
                    case (int)ECName.ECExample1:
                        equipmentConstant.Add(
                            new EquipmentConstant(
                                nameof(ECName.ECExample1),
                                VidDataType.String,
                                "Get something for Toolcommander",
                                "unknown"));
                        break;
                    case (int)ECName.ECExample2:
                        equipmentConstant.Add(
                            new EquipmentConstant(
                                nameof(ECName.ECExample2),
                                VidDataType.Boolean,
                                "Get selected Recipe name to Toolcommander",
                                false));
                        break;
                }
            }

            return equipmentConstant;
        }

        public override List<CommonEvent> SimulateGetCollectionEvents()
        {
            var dataVariables = new SecsVariableList();
            dataVariables.Add(new SecsVariable("JobId", new SecsItem(SecsFormat.Ascii, "JobTest")));

            return new List<CommonEvent>()
            {
                new()
                {
                    ID = 304900,
                    Name = nameof(CEName.WaferMeasurementResults),
                    Description = "Signal wafer measurement results available",
                    DataVariables = dataVariables
                }
            };
        }
    }
}
