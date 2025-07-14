using System.Windows;
using System.Windows.Controls;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Result.CommonUI.View.Search
{
    public class ResultTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AnalyzeTemplate { get; set; }

        public DataTemplate DefaultTemplate { get; set; }

        #region Overrides of DataTemplateSelector

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ResultType resultType && resultType.GetActorType() == ActorType.ANALYSE)
            {
                return AnalyzeTemplate;
            }

            return DefaultTemplate;
        }

        #endregion
    }
}
