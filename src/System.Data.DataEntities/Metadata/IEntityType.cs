using System;

namespace System.Data.DataEntities.Metadata
{
    /// <summary>
    /// Describes a type of entity information.
    /// </summary>
    public interface IEntityType : IMemberMetadata
    {
        /// <summary>
        /// Returns the primary key for this entity.
        /// </summary>
        ISimpleEntityProperty PrimaryKey { get; }

        /// <summary>
        /// Gets the type from which the current type directly inherits.
        /// </summary>
        IEntityType BaseType { get; }

        /// <summary>
        /// Gets a value, this value indicates whether the entity type is abstract and must be overridden.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets a value, this value indicates whether the entity type a sealed.
        /// </summary>
        bool IsSealed { get; }

        /// <summary>
        /// Gets a value, this value indicates whether the entity type a interface.
        /// </summary>
        bool IsInterface { get; }

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
        /// Create an instance of this IEntityType
        /// </summary>
        /// <returns></returns>
        object CreateInstance();

        /// <summary>
        /// Returns the entity type supported interfaces
        /// </summary>
        /// <returns>the entity type supported interfaces</returns>
        IEntityType[] GetInterfaces();

        /// <summary>
        /// Determines whether the current instance of an DynamicEntityType can be assigned from the specified DynamicEntityType instance.
        /// </summary>
        /// <param name="c">Compare with the current DynamicEntityType an DynamicEntityType.</param>
        /// <returns>
        /// If any of the following conditions are true:
        ///     if c and the current Type represent the same type, 
        ///     or if the current Type is in the inheritance hierarchy of c,
        ///     or if the current Type is an interface that c implements,
        ///  false if none of these conditions are true, or if c is null.
        /// </returns>
        bool IsAssignableFrom(IEntityType c);

    }
}
