using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace UnitySC.Equipment.Abstractions.Drivers.Common
{
    /// <summary>
    /// Based class for all statuses defined within the application
    /// </summary>
    public abstract class Status : ICloneable
    {
        /// <summary>
        /// Create an object that it is a DEEP copy of current instance.
        /// </summary>
        /// <returns>A new object instance that it is a DEEP copy</returns>
        public abstract object Clone();

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "[{0}]", GetType().Name));

            foreach (PropertyInfo prop in GetType().GetProperties())
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0} = {1}", prop.Name,
                    prop.GetValue(this, null) == null ? "null" : prop.GetValue(this, null).ToString()));

            return sb.ToString();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                object x = prop.GetValue(this);
                object y = prop.GetValue(obj);

                if (!x.Equals(y))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 0;

            foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                hash += prop.GetHashCode();
            }

            return hash;
        }
    }
}
