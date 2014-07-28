using System;

namespace System.Data.DataEntities.Metadata
{
    /// <summary>
    /// Properties of the collection type descriptor, indicates that the returned data is a set.
    /// </summary>
    public interface ICollectionEntityProperty : IEntityProperty
    {
        /// <summary>
        /// Returns the return type of this property.
        /// </summary>
        IEntityType ItemPropertyType { get; }
    }
}