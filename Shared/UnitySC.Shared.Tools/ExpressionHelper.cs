using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace UnitySC.Shared.Tools
{
    public static class ExpressionHelper
    {
        /// <summary>
        /// Get the member name of the expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string GetName<TSource, TField>(Expression<Func<TSource, TField>> expr)
        {
            if (object.Equals(expr, null))
                throw new ArgumentNullException(nameof(expr));

            MemberExpression memberExpressionExpr;
            if (expr.Body is MemberExpression)
            {
                memberExpressionExpr = (MemberExpression)expr.Body;
            }
            else if (expr.Body is UnaryExpression)
            {
                memberExpressionExpr = (MemberExpression)((UnaryExpression)expr.Body).Operand;
            }
            else
            {
                const string Format = "Expression '{0}' not supported.";
                string message = string.Format(Format, expr);

                throw new ArgumentException(message, nameof(expr));
            }

            return memberExpressionExpr.Member.Name;
        }

        /// <summary>
        /// Get the declared type of the expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Type GetType<TSource, TField>(Expression<Func<TSource, TField>> expr)
        {
            if (object.Equals(expr, null))
                throw new ArgumentNullException(nameof(expr));

            MemberExpression memberExpressionExpr;
            if (expr.Body is MemberExpression)
            {
                memberExpressionExpr = (MemberExpression)expr.Body;
            }
            else if (expr.Body is UnaryExpression)
            {
                memberExpressionExpr = (MemberExpression)((UnaryExpression)expr.Body).Operand;
            }
            else
            {
                const string Format = "Expression '{0}' not supported.";
                string message = string.Format(Format, expr);

                throw new ArgumentException(message, nameof(expr));
            }

            return memberExpressionExpr.Member.DeclaringType;
        }

        public static bool IsMethodCall(LambdaExpression expression)
        {
            return expression.Body is MethodCallExpression;
        }

        /// <summary>
        /// Get the method name of the expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetMethodName(LambdaExpression expression)
        {
            if (expression.Body is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)expression.Body;
                return methodCallExpression.Method.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the method arguments of the expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<Expression> GetMethodArguments(LambdaExpression expression)
        {
            if (expression.Body is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)expression.Body;
                return methodCallExpression.Arguments;
            }
            else
            {
                return null;
            }
        }
    }
}
