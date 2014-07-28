using System.Collections.Generic;
using System.Data.DataEntities.Metadata;

namespace System.Data.DataEntities.DcxmlSerialization
{
    /// <summary>
    /// In the differential serialization and de-serialization, the provider name to find and identify the types of functions.
    /// </summary>
    public abstract class DcxmlBinder : EntityTypeBinder
    {
        /// <summary>
        /// Returns whether to ignore case, such as when dealing with the string.
        /// </summary>
        public virtual bool IgnoreCase
        {
            get { return true; }
        }

        /// <summary>
        /// Specify a type, returns its namespace and name, as well as some custom attributes.
        /// </summary>
        /// <param name="serializedType">Object type to serialize</param>
        /// <param name="namespace">namespcae</param>
        /// <param name="typeName">Type's name</param>
        public virtual void BindToName(
            IEntityType serializedType, 
            out string @namespace, 
            out string typeName)
        {
            if (null == serializedType)
            {
                throw new ArgumentNullException("serializedType");
            }

            @namespace = serializedType.Namespace;
            typeName = serializedType.Name;
        }

        /// <summary>
        /// Get custom attributes of entity.
        /// </summary>
        /// <param name="serializedType">Object type to serialize.</param>
        /// <param name="entity">entity instance.</param>
        /// <returns>Return custom attributes of entity.</returns>
        public virtual IEnumerable<CustomAttribute> GetCustomAttribute(IEntityType serializedType, object entity)
        {
            return null;
        }


        /// <summary>
        /// According to the namespace and name, return the entity type match.
        /// </summary>
        /// <param name="namespace">entity namespace string</param>
        /// <param name="typeName">entity type string</param>
        /// <param name="attributes">entity custom attributes</param>
        /// <returns>According to the namespace and name to match, if not found will return null.</returns>
        public abstract IEntityType BindToType(string @namespace, string typeName, IEnumerable<CustomAttribute> attributes);

        /// <summary>
        /// Depending on the type of information to create an instance.
        /// </summary>
        /// <param name="entityType">You need to create an instance of an entity type.</param>
        /// <param name="attributes">Reference custom properties.</param>
        /// <returns>Returns an instance of this type.</returns>
        public object CreateInstance(IEntityType entityType, IEnumerable<CustomAttribute> attributes)
        {
            if (null == entityType)
            {
                throw new ArgumentNullException("entityType");
            }
            return entityType.CreateInstance();
        }

        /// <summary>
        /// Returns a string comparison method.
        /// </summary>
        internal protected StringComparison StringComparison
        {
            get
            {
                if (this.IgnoreCase)
                {
                    return System.StringComparison.OrdinalIgnoreCase;
                }
                return System.StringComparison.Ordinal;
            }
        }

        #region CustomAttribute
        /// <summary>
        /// Custom attributes.
        /// </summary>
        public struct CustomAttribute
        {
            /// <summary>namespace string.</summary>
            public string Namespace;
            /// <summary>name</summary>
            public string Name;
            /// <summary>attribute value</summary>
            public string Value;
        }
        #endregion
    }
}
