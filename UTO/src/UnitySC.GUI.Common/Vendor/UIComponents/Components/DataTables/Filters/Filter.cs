using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters
{
    public interface IFilter
    {
        IText Name { get; }
    }

    /// <summary>
    /// Base class of filter
    /// </summary>
    public abstract class Filter : Notifier, IFilter
    {
        /// <summary>
        /// Reset configured parameters.
        /// After calling clear, the filter must not filter any element.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Gets the name displayed by the graphical representation of the filter.
        /// </summary>
        public IText Name { get; }

        protected Filter(IText name)
        {
            Name = name;
        }

        /// <summary>
        /// Save the current filter state and returns if the filter is active.
        /// </summary>
        /// <returns><c>true</c> if the filter is active; otherwise <c>false</c></returns>
        public abstract bool Apply();
    }

    /// <summary>
    /// Base class of generic filter
    /// </summary>
    /// <typeparam name="T">Type of model to filter</typeparam>
    public abstract class Filter<T> : Filter
    {
        /// <summary>
        /// Returns if the element passed in parameter is filtered by the filter.
        /// </summary>
        /// <param name="model">Model to test</param>
        /// <returns>True if the element is not affected by the filter, False if it is filtered.</returns>
        public abstract bool Validate(T model);

        /// <summary>
        /// Interprets the values supplied by the filter.
        /// </summary>
        public abstract void UpdatePossibleValues();

        protected Filter(IText name) : base(name)
        {
        }
    }
}
