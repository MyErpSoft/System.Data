using System;
using System.ComponentModel;

namespace System.Data.DataEntities.Metadata
{
    /// <summary>
    /// Describe the property of an EntityType and related information through this interface to access the properties and access/set the values of the corresponding properties of the entity.
    /// </summary>
    public interface IEntityProperty : IMemberMetadata
    {
        /// <summary>
        /// Gets the current value of the property on the entity.
        /// </summary>
        /// <param name="entity">Has the property for which to retrieve the value of the entity.</param>
        /// <returns>This property values for a given entity</returns>
        object GetValue(object entity);

        /// <summary>
        /// The entity in the value of this property is set to a new value.
        /// </summary>
        /// <param name="entity">Entities with property values to be set</param>
        /// <param name="newValue">The new value.</param>
        void SetValue(object entity, object newValue);

        /// <summary>
        /// Gets a value indicating whether the property is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        Type PropertyType { get; }
    }
}
