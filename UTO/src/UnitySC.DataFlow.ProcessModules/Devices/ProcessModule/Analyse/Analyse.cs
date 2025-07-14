using System.Collections.Generic;

using UnitsNet;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Analyse
{
    public partial class Analyse
    {
        private void InstanceInitialization()
        {
            ActorType = ActorType.ANALYSE;
        }

        protected override void OnStatusVariableChanged(List<StatusVariable> statusVariables)
        {
            foreach (var statusVariable in statusVariables)
            {
                if (statusVariable != null)
                {
                    switch (statusVariable.Name)
                    {
                        case nameof(SVName_ANA.State):
                            UpdateProcessModuleState(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_ANA.RecipeName):
                            SelectedRecipe = statusVariable.ValueAsString;
                            break;
                        case nameof(SVName_ANA.PMProcessProgress_Percentage):
                            RecipeProgress = Ratio.FromPercent(byte.Parse(statusVariable.ValueAsString));
                            break;
                        case nameof(SVName_ANA.MaterialPresenceState):
                            UpdateWaferPresence(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_ANA.TransferState):
                            UpdateTransferState(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_ANA.TransferValidationState):
                            UpdateTransferValidationState(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_ANA.ReadyForProcess):
                        case nameof(SVName_ANA.RecipeActive):
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
                $".\\Devices\\{nameof(ProcessModule)}\\{nameof(Analyse)}\\Resources\\MessagesConfiguration.xml");
        }
    }
}
