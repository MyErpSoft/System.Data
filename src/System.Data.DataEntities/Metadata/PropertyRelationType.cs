
namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// Defines the type of relationship property.
    /// </summary>
    public enum PropertyRelationType {

        /// <summary> This property is a simple property, no longer points to the other type/element </summary>
        Simple,

        /// <summary> This property is a complex property, the equivalent of a one-to-one relationship </summary>
        Complex,

        /// <summary> This property is a collection property, which is equivalent to a one to many relationship </summary>
        Collection
    }
}
