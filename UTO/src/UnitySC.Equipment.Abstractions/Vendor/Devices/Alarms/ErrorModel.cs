using System;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms.Enums;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms
{
    /// <summary>
    ///     Responsible to hold the error data defined in CSV file.
    /// </summary>
    public sealed class ErrorModel : IEquatable<ErrorModel>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of <see cref="ErrorModel" /> class.
        /// </summary>
        /// <param name="id">
        ///     <see cref="Id" />
        /// </param>
        /// <param name="description">
        ///     <see cref="Description" />
        /// </param>
        /// <param name="hint">
        ///     <see cref="Hint" />
        /// </param>
        /// <param name="criticity">
        ///     <see cref="Criticity" />
        /// </param>
        public ErrorModel(int id, string description, string hint, AlarmCriticity criticity = AlarmCriticity.Undefined)
        {
            Id = id;
            Description = description;
            Hint = hint;
            Criticity = criticity;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///     Gets a text describing the error.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Gets a text indicating the procedure to recover from the error.
        /// </summary>
        public string Hint { get; }

        /// <summary>
        ///     Gets the identifier of the error.
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     Gets the criticity of the error.
        /// </summary>
        public AlarmCriticity Criticity { get; }

        #endregion Properties

        #region Value Equality Support

        /// <summary>
        ///     Determines whether two specified <see cref="ErrorModel" /> have different values.
        /// </summary>
        /// <param name="left">The first <see cref="ErrorModel" /> to compare, or <see langword="null" />.</param>
        /// <param name="right">The second <see cref="ErrorModel" /> to compare, or <see langword="null" />.</param>
        /// <returns>
        ///     <see langword="true" /> if the value of <paramref name="left" /> is different from the value of
        ///     <paramref name="right" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator !=(ErrorModel left, ErrorModel right) => !Equals(left, right);

        /// <summary>
        ///     Determines whether two specified <see cref="ErrorModel" /> have the same values.
        /// </summary>
        /// <param name="left">The first <see cref="ErrorModel" /> to compare, or <see langword="null" />.</param>
        /// <param name="right">The second <see cref="ErrorModel" /> to compare, or <see langword="null" />.</param>
        /// <returns>
        ///     <see langword="true" /> if the value of <paramref name="left" /> is the same as the value of
        ///     <paramref name="right" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool operator ==(ErrorModel left, ErrorModel right) => Equals(left, right);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ErrorModel)obj);
        }

        /// <inheritdoc />
        public bool Equals(ErrorModel other) => Id == other?.Id;

        /// <inheritdoc />
        public override int GetHashCode() => Id;

        #endregion Value Equality Support
    }
}
