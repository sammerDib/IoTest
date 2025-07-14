using System.Collections.Generic;

using UnitsNet;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Emera
{
    public partial class Emera
    {
        private void InstanceInitialization()
        {
            ActorType = ActorType.EMERA;
        }

        protected override void OnStatusVariableChanged(List<StatusVariable> statusVariables)
        {
            foreach (var statusVariable in statusVariables)
            {
                if (statusVariable != null)
                {
                    switch (statusVariable.Name)
                    {
                        case nameof(SVName_EME.State):
                            UpdateProcessModuleState(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_EME.RecipeName):
                            SelectedRecipe = statusVariable.ValueAsString;
                            break;
                        case nameof(SVName_EME.PMProcessProgress_Percentage):
                            RecipeProgress = Ratio.FromPercent(byte.Parse(statusVariable.ValueAsString));
                            break;
                        case nameof(SVName_EME.MaterialPresenceState):
                            UpdateWaferPresence(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_EME.TransferState):
                            UpdateTransferState(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_EME.TransferValidationState):
                            UpdateTransferValidationState(statusVariable.ValueAsString);
                            break;
                        case nameof(SVName_EME.ReadyForProcess):
                        case nameof(SVName_EME.RecipeActive):
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
                $".\\Devices\\{nameof(ProcessModule)}\\{nameof(Emera)}\\Resources\\MessagesConfiguration.xml");
        }
    }
}
