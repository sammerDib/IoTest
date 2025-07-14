using System.Collections.Generic;

using UnitsNet;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Demeter
{
    public partial class Demeter
    {
        private void InstanceInitialization()
        {
            ActorType = ActorType.DEMETER;
        }

        protected override void OnStatusVariableChanged(List<StatusVariable> statusVariables)
        {
            foreach (var statusVariable in statusVariables)
            {
                if (statusVariable != null)
                {
                    switch (statusVariable.Name)
                    {
                        case nameof(SVName_DMT.State): //Asc
                            UpdateProcessModuleState(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_DMT.RecipeName):
                            SelectedRecipe = statusVariable.ValueAsString;
                            break;
                        case nameof(SVName_DMT.TransferState):
                            UpdateTransferState(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_DMT.MaterialPresenceState):
                            UpdateWaferPresence(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_DMT.PMAcquisitionProgress_Percentage):
                            AcquisitionProgress = Ratio.FromPercent(byte.Parse(statusVariable.ValueAsString));
                            break;
                        case nameof(SVName_DMT.PMAcquisitionProgress_SubstID):
                            AcquisitionWaferId = statusVariable.ValueAsString;
                            break;
                        case nameof(SVName_DMT.PMComputationProgress_Percentage):
                            CalculationProgress = Ratio.FromPercent(byte.Parse(statusVariable.ValueAsString));
                            break;
                        case nameof(SVName_DMT.PMComputationProgress_SubstID):
                            CalculationWaferId = statusVariable.ValueAsString;
                            break;
                        case nameof(SVName_DMT.TransferValidationState):
                            UpdateTransferValidationState(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_DMT.ReadyForProcess):
                        case nameof(SVName_DMT.RecipeActive):
                            break;
                    }
                }
            }

            base.OnStatusVariableChanged(statusVariables);
        }

        public override string GetMessagesConfigurationPath(string path)
        {
            return System.IO.Path.Combine(
                path,
                $".\\Devices\\{nameof(ProcessModule)}\\{nameof(Demeter)}\\Resources\\MessagesConfiguration.xml");
        }
    }
}
