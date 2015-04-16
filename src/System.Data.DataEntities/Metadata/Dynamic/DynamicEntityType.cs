using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Globalization;
using System.Linq;

namespace System.Data.DataEntities.Metadata.Dynamic {

    /// <summary>
    /// Dynamic entity type.
    /// </summary>
    public sealed class DynamicEntityType : DynamicMemberMetadata {

        /// <summary>
        /// Dynamic entity types, allows creating a type at run time and used to carry the physical structure of the data.
        /// </summary>
        /// <param name="name">The identifying name of the type, following the c# property name constraints.</param>
        /// <param name="nameSpace">The dynamic type's namespace.</param>
        public DynamicEntityType(
            string name,
            string nameSpace = null) {
            #region Parameter checking
            if (!OrmUtility.VerifyName(name)) {
                throw new ArgumentException("name");
            }
            if ((!string.IsNullOrEmpty(nameSpace)) &&
                !OrmUtility.VerifyNameWithNamespace(nameSpace)) {
                throw new ArgumentException("nameSpace");
            }
            #endregion

            _name = name;
            _namespace = nameSpace;
            this._fields = new DynamicEntityFieldCollection(this);
        }

        #region about name，ex Name,Namespcae,FullName

        private readonly string _name;
        /// <summary>
        /// The identifying name of the type
        /// </summary>
        public override string Name {
            get { return _name; }
        }

        private readonly string _namespace;
        /// <summary>
        ///The namespace of the type
        /// </summary>
        public string Namespace { get { return _namespace; } }

        /// <summary>
        /// The fullname of the type
        /// </summary>
        public string FullName {
            get { return string.IsNullOrEmpty(_namespace) ? _name : string.Concat(_namespace, ".", _name); }
        }

        #endregion

        private readonly DynamicEntityFieldCollection _fields;
        /// <summary>
        /// Return all properties for this EntityType.
        /// </summary>
        public DynamicEntityFieldCollection Fields { get { return this._fields; } }

        /// <summary>
        /// Create an instance of this EntityType
        /// </summary>
        /// <returns></returns>
        public object CreateInstance() {
            return new DynamicEntity(this);
        }

        #region Properties Field

        /// <summary>
        /// Registered with the current dynamic entity type is a simple property.
        /// </summary>
        /// <param name="name">Unique name for this simple property, duplicate names cannot be combined with other properties.</param>
        /// <param name="propertyType">The return type of the property.</param>
        /// <param name="defaultValue">set this property default value when not set value.</param>
        /// <returns>A new field</returns>
        public DynamicEntityField RegisterField(
            string name, Type propertyType) {

            if (!OrmUtility.VerifyName(name)) {
                throw new ArgumentException(string.Format("Register a field name {0} is not correct, can only be a combination of letters, numbers, and underscores.", name));
            }
            //TODO:Check parameters

            DynamicEntityField property;

            if (propertyType.IsValueType) {
                if (Nullable.GetUnderlyingType(propertyType) == null) {
                    property = new DynamicEntityStructField(name, propertyType);
                }
                else {
                    property = new DynamicEntityNullableField(name, propertyType);
                }
            }
            else {
                property = new DynamicEntityObjectField(name, propertyType);
            }

            this._fields.Add(property);
            this.OnMetadataChanged();

            return property;
        }

        #endregion
    }
}
