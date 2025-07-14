using System;
using System.Collections.ObjectModel;
using System.Linq;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.WaitStatusThreshold.ViewModels.Operand
{
    public class EnumerableThresholdOperandViewModel : ThresholdViewModel<EnumerableThresholdOperand>
    {
        public EnumerableThresholdOperandViewModel(EnumerableThresholdOperand model) : base(model)
        {
            Literals = new ObservableCollection<IComparable>(EnumLoader.GetEnumValues(model.Type, model.AssemblyName)
                .OfType<IComparable>());
        }

        #region Properties

        public ObservableCollection<IComparable> Literals { get; }

        public IComparable TypedValue
        {
            get
            {
                return Literals.FirstOrDefault(l => l.ToString().Equals(Model?.Value, StringComparison.Ordinal));
            }
            set
            {
                if (Model == null) return;

                if (string.Equals(value?.ToString(), Model?.Value, StringComparison.Ordinal))
                {
                    return;
                }

                Model.Value = value?.ToString() ?? string.Empty;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}
