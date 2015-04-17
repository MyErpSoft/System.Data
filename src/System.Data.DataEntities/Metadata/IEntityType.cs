using System;

namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// Describes a type of entity information.
    /// </summary>
    public interface IEntityType : IMetadataObject {

        /// <summary>
        /// Gets a value, this value indicates whether the entity type is abstract and must be overridden.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets a value, this value indicates whether the entity type a sealed.
        /// </summary>
        bool IsSealed { get; }

        /// <summary>
        ///The namespace of the type
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// The fullname of the type
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Return all properties for this IEntityType.
        /// </summary>
        IEntityPropertyCollection Properties { get; }

        /// <summary>
        /// Return all fields for this IEntityType.
        /// </summary>
        IEntityFieldCollection Fields { get; }

        /// <summary>
        /// Return this IEntityType maping runtime type.(CLR Type).
        /// </summary>
        Type RuntimeType { get; }

        /// <summary>
        /// Create an instance of this IEntityType
        /// </summary>
        /// <returns></returns>
        object CreateInstance();
    }
}
