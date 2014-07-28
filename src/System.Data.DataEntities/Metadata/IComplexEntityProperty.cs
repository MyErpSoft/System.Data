using System;

namespace System.Data.DataEntities.Metadata
{
    /// <summary>
    /// Properties of a complex type descriptor, indicating that data is a complex object that is returned.
    /// </summary>
    public interface IComplexEntityProperty : IEntityProperty
    {
        /// <summary>
        /// Returns the return type of this property.
        /// </summary>
        IEntityType ComplexPropertyType { get; }
    }
}
