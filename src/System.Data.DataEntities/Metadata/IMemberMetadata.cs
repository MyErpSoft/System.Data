using System;
using System.Reflection;

namespace System.Data.DataEntities.Metadata
{
    /// <summary>
    /// The base class for the entity metadata, he can specify that it can provide Attribute and name.
    /// </summary>
    public interface IMemberMetadata: ICustomAttributeProvider
    {
        /// <summary>
        /// Returns the distinguished name of this metadata object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value that identifies a metadata element.
        /// </summary>
        int MetadataToken { get; }
    }
}
