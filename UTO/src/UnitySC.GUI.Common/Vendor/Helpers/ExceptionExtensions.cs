using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Tracing;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class ExceptionExtensions
    {
        #region Methods

        /// <summary>
        /// Flattens an exception, and all its inner exceptions, into an <see cref="List{Exception}"/>.
        /// </summary>
        /// <param name="exception">The source exception to be flattened. First item in the returned collection.</param>
        /// <returns>
        /// A new <see cref="List{Exception}"/> instance with the exception at index 0 of the list and the inner-most exception at index 'Count - 1'.
        /// </returns>
        public static List<Exception> Flatten(this Exception exception)
        {
            var flattenedExceptions = new List<Exception> { exception };

            if (exception.InnerException != null)
            {
                flattenedExceptions.AddRange(exception.InnerException.Flatten());
            }

            return flattenedExceptions;
        }

        public static TraceParam ToTraceParam(this Exception exception)
        {
            var exMessage = $@"Exception:
{exception}

StackTrace:
{exception.StackTrace}";

            if (exception.InnerException != null)
            {
                exMessage += $@"Inner exceptions:
{string.Join(Environment.NewLine, exception.Flatten().Skip(1).Select(ie => ie.Message))}";
            }

            return new TraceParam(exMessage);
        }

        #endregion Methods
    }
}
