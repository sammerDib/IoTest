using System;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators
{
    public static class WaitingOperatorExtensions
    {
        public static string ToHumanizedString(this WaitingOperator @operator)
        {
            switch (@operator)
            {
                case WaitingOperator.GreaterThan:
                    return ">";
                case WaitingOperator.SmallerThan:
                    return "<";
                case WaitingOperator.Equals:
                    return "=";
                case WaitingOperator.NotEquals:
                    return "â ";
                case WaitingOperator.GreaterThanOrEqual:
                    return "â¥";
                case WaitingOperator.SmallerThanOrEqual:
                    return "â¤";
                default:
                    throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
            }
        }

        public static WaitingOperator FromHumanizedString(string stringOperator)
        {
            switch (stringOperator)
            {
                case ">":
                    return WaitingOperator.GreaterThan;
                case "<":
                    return WaitingOperator.SmallerThan;
                case "=":
                    return WaitingOperator.Equals;
                case "â ":
                    return WaitingOperator.NotEquals;
                case "â¥":
                    return WaitingOperator.GreaterThanOrEqual;
                case "â¤":
                    return WaitingOperator.SmallerThanOrEqual;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stringOperator), stringOperator, null);
            }
        }
    }
}
