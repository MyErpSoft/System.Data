using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Globalization;
using System.Linq;

namespace System.Data.DataEntities.Metadata.Dynamic {

    /// <summary>
    /// Dynamic entity type.
    /// </summary>
    public class DynamicEntityType : DynamicMemberMetadata,IEntityType {

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
                OrmUtility.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                    Properties.Resources.ErrorName, name), "name");
            }
            if ((!string.IsNullOrEmpty(nameSpace)) &&
                !OrmUtility.VerifyNameWithNamespace(nameSpace)) {
                OrmUtility.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                    Properties.Resources.ErrorNamespace, nameSpace), "nameSpace");
            }
            #endregion

            _name = name;
            _namespace = nameSpace;

            this._members = new MemberCollection();
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

        bool IEntityType.IsAbstract {
            get { return false; }
        }

        bool IEntityType.IsSealed {
            get { return false; }
        }

        Type IEntityType.UnderlyingSystemType {
            get { return typeof(DynamicEntity); }
        }
        #endregion

        //all members, include field,property,event...
        private readonly MemberCollection _members; 
        //only fields
        private readonly DynamicEntityFieldCollection _fields;
        /// <summary>
        /// Return all properties for this EntityType.
        /// </summary>
        public DynamicEntityFieldCollection Fields { get { return this._fields; } }

        #region CreateInstance
        /// <summary>
        /// 允许派生类自定义实体数据的存储形式或初始化对象。
        /// </summary>
        /// <param name="entity">要初始化的对象</param>
        /// <returns>一个存储对象</returns>
        internal protected virtual IDynamicEntityStorage InitializeEntity(DynamicEntity entity) {
            IDynamicEntityStorage storage;
            //没有限制冻结的结构才能创建实例。
            if (this._fields.Count < 30) {
                storage = new DynamicEntityArrayStorage(this);
            }
            else {
                storage = new DynamicEntityDictStorage();
            }

            return storage;
        }
        
        /// <summary>
        /// Create an instance of this EntityType
        /// </summary>
        /// <returns></returns>
        public object CreateInstance() {
            return new DynamicEntity(this);
        }
        #endregion

        #region Properties Field
        private sealed class MemberCollection : MetadataReadOnlyCollection<DynamicMemberMetadata> {
            public MemberCollection() : base(null) { }
            protected override string GetName(DynamicMemberMetadata item) {
                return item.Name;
            }
        }

        /// <summary>
        /// Registered with the current dynamic entity type is a simple property.
        /// </summary>
        /// <param name="name">Unique name for this simple property, duplicate names cannot be combined with other properties.</param>
        /// <param name="propertyType">The return type of the property.</param>
        /// <param name="defaultValue">set this property default value when not set value.</param>
        /// <returns>A new field</returns>
        public DynamicEntityField RegisterField(
            string name, Type propertyType) {

            if (propertyType == null) {
                OrmUtility.ThrowArgumentNullException("propertyType");
            }

            return this.RegisterField(name, Clr.EntityType.GetEntityType(propertyType));
        }

        public DynamicEntityField RegisterField(string name, IEntityType propertyType) {
            #region Parameter checking
            this.CheckIsFrozen();

            if (!OrmUtility.VerifyName(name)) {
                OrmUtility.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                                    Properties.Resources.ErrorName, name), "name");
            }
            if (propertyType == null) {
                OrmUtility.ThrowArgumentNullException("propertyType");
            }
            if (_members.Contains(name)) {
                OrmUtility.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                    Properties.Resources.KeyIsExisted, this.Name, name));
            }
            #endregion

            DynamicEntityField field;
            var systemType = propertyType.UnderlyingSystemType;

            if (systemType.IsValueType) {
                if (Nullable.GetUnderlyingType(systemType) == null) {
                    field = new DynamicEntityStructField(name, propertyType);
                }
                else {
                    field = new DynamicEntityNullableField(name, propertyType);
                }
            }
            else {
                field = new DynamicEntityObjectField(name, propertyType);
            }

            this._fields.Add(field);
            this._members.Add(field);
            this.OnMetadataChanged();

            return field;
        }

        IEntityProperty IEntityType.GetProperty(string name) {
            IEntityProperty property;
            if (((IEntityType)this).TryGetProperty(name, out property)) {
                return property;
            }

            OrmUtility.ThrowKeyNotFoundException(string.Format(CultureInfo.CurrentCulture,
                Properties.Resources.KeyNotFoundException, this.Name, name));
            return null;
        }

        bool IEntityType.TryGetProperty(string name, out IEntityProperty member) {
            DynamicMemberMetadata item;
            if(this._members.TryGetValue(name,out item)) {
                member = item as IEntityProperty;
                return member != null;
            }

            member = null;
            return false;
        }

        /// <summary>
        /// 确定指定的对象是否是当前 IEntityType 的实例。
        /// </summary>
        /// <param name="obj">要与当前类型进行比较的对象。</param>
        /// <returns>如果可以分配到当前类型，返回true，否则返回false.</returns>
        public bool IsInstanceOfType(object obj) {
            if (obj == null) {
                return false; //根据CLR Type的实现，应该是返回false.
            }

            DynamicEntity entity = obj as DynamicEntity;
            if (entity == null) {
                return false;
            }

            return entity.DynamicEntityType == this;
        }
        #endregion

        #region Freeze
        private bool _isFrozen;
        /// <summary>
        /// 冻结当前类型，使其不能再扩展。
        /// </summary>
        /// <remarks>
        /// <para>当支持派生功能时，如果没有冻结机制，会造成B已经派生A，但A又注册了新的属性，导致与B新增的属性冲突；</para>
        /// <para>值类型的DynamicEntityType包装，例如int32对应的包装，是不支持任何改动的，所以必须冻结；</para>
        /// </remarks>
        public void Freeze() {
            if (!_isFrozen) {
                _isFrozen = true;
            }
        }

        /// <summary>
        /// 返回当前类型是否已经冻结，冻结的类型不能再扩展。
        /// </summary>
        public bool IsFrozen {
            get { return this._isFrozen; }
        }

        private void CheckIsFrozen() {
            if (_isFrozen) {
                OrmUtility.ThrowInvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture, 
                    Properties.Resources.ObjectIsFrozen,
                    this.Name));
            }
        }

        #endregion
    }
}
