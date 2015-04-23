using System;

namespace System.Data.Metadata.DataEntities {

    /// <summary>
    /// Describes a type of entity information.
    /// </summary>
    public interface IEntityType : IMemberMetadata {

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
        /// 返回指定名称的属性。
        /// </summary>
        /// <param name="name">要检索的属性名称</param>
        /// <returns>如果找到此名称的属性将返回他，否则（找不到或类型不一致），将抛出异常。</returns>
        IEntityProperty GetProperty(string name);

        /// <summary>
        /// 尝试获取指定名称的成员
        /// </summary>
        /// <param name="name">要检索的成员名称</param>
        /// <param name="member">如果找到将返回他，否则返回null</param>
        /// <returns>如果找到将返回true，否则返回false.</returns>
        bool TryGetProperty(string name,out IEntityProperty member);

        /// <summary>
        /// Return this IEntityType maping runtime type.(CLR Type).
        /// </summary>
        Type UnderlyingSystemType { get; }

        /// <summary>
        /// 确定指定的对象是否是当前 IEntityType 的实例。
        /// </summary>
        /// <param name="obj">要与当前类型进行比较的对象。</param>
        /// <returns>如果可以分配到当前类型，返回true，否则返回false.</returns>
        bool IsInstanceOfType(object obj);

        /// <summary>
        /// Create an instance of this IEntityType
        /// </summary>
        /// <returns></returns>
        object CreateInstance();
    }
}
