using System.Collections.Generic;
using System.Data.DataEntities.Metadata;

namespace System.Data.DataEntities.DcxmlSerialization
{
    /// <summary>
    /// The default DcxmlBinder implementation，He managed entity types available through EntityTypes.
    /// </summary>
    public class DefaultDcxmlBinder : DcxmlBinder
    {
        private const string Element = "Element";
        private EntityTypeCollection _types;

        /// <summary>
        /// Create new instance.
        /// </summary>
        public DefaultDcxmlBinder()
        {
            _types = new EntityTypeCollection(
                this.IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        }

        /// <summary>
        /// Create new instance and specify a default set of entity types
        /// </summary>
        /// <param name="entityTypes">entity type collection instance.</param>
        public DefaultDcxmlBinder(EntityTypeCollection entityTypes)
        {
            if (null == entityTypes)
            {
                throw new ArgumentNullException("entityTypes");
            }
            _types = entityTypes;
        }

        /// <summary>
        /// Returns a collection of the current Binder management entity type.
        /// </summary>
        public EntityTypeCollection EntityTypes
        {
            get { return _types; }
        }

        /// <summary>
        /// According to the namespace and name, return the entity type match.
        /// </summary>
        /// <param name="namespace">entity namespace string</param>
        /// <param name="typeName">entity type string</param>
        /// <param name="attributes">entity custom attributes</param>
        /// <returns>According to the namespace and name to match, if not found will return null.</returns>
        public override IEntityType BindToType(string @namespace, string typeName, IEnumerable<DcxmlBinder.CustomAttribute> attributes)
        {
            IEntityType dt;
            //Not found, and the name is not the end of the 'Element'. Try add 'Element' search.
            if (!_types.TryGetValue(@namespace, typeName, out dt) &&
                !typeName.EndsWith(Element, System.StringComparison.OrdinalIgnoreCase))
            {
                _types.TryGetValue(@namespace, typeName + Element, out dt);
            }
            return dt;
        }

        /// <summary>
        /// Specify a type, returns its namespace and name, as well as some custom attributes.
        /// </summary>
        /// <param name="serializedType">Object type to serialize</param>
        /// <param name="namespace">namespcae</param>
        /// <param name="typeName">Type's name</param>
        public override void BindToName(
            IEntityType serializedType,
            out string @namespace,
            out string typeName)
        {
            base.BindToName(serializedType, out @namespace, out typeName);
            //Element end to delete Element
            if (typeName.EndsWith(Element, System.StringComparison.OrdinalIgnoreCase))
            {
                typeName = typeName.Substring(0, typeName.Length - 7);
            }
        }
    }
}
