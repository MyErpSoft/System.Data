using System.ComponentModel;

namespace System.Data.DataEntities.Metadata
{
    /// <summary>
    /// Simple Property interface.
    /// </summary>
    public interface ISimpleEntityProperty : IEntityProperty
    {
        /// <summary>
        /// Resets the value for this property of the component to the default values.
        /// </summary>
        /// <param name="entity">The entity to be reset to the default value of the property value.</param>
        void ResetValue(object entity);

        /// <summary>
        /// Determines a value that indicates whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="entity">Entity with the property to be examined for persistence.</param>
        /// <returns>If the property should be preserved, it is true, otherwise false.</returns>
        bool ShouldSerializeValue(object entity);

        /// <summary>
        /// Gets the type of the converter.
        /// </summary>
        TypeConverter Converter { get; }
    }
}
