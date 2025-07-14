using System.Linq;

using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;

namespace UnitySC.GUI.Common.Vendor.Extensions
{
    public static class AgileoSemiExtensions
    {
        public static string GetValueAsString(this DataItem dataItem, string emptyValue)
        {
            if (dataItem is DataList dataList)
            {
                return dataList.ToString();
            }

            if (dataItem is DataArray dataArray)
            {
                if (dataArray.DataValues.Count == 0)
                {
                    return emptyValue;
                }
                return string.Join(" ", dataArray.DataValues.Select(x => x.ToValueString()));
            }

            return emptyValue;
        }

        public static DataItem NewDataItemFromValueString(this E30Variable variable, string value)
        {
            return DataItemFactory.NewDataItem(variable.Format, value);
        }
    }
}
