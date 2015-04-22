using System.ComponentModel;
using System.Data.DataEntities.Dynamic;

namespace System.Data.DataEntities.Metadata.Dynamic {

    /// <summary>
    /// DynamicEntity field definitions.
    /// </summary>
    public abstract class DynamicEntityField : DynamicMemberMetadata,IEntityProperty {

        /// <summary>
        /// Derived class can create dynamic entity filed.
        /// </summary>
        /// <param name="name">The identifying name of the field, following the c# field name constraints.</param>
        /// <param name="fieldType">Property return type,for example, returns the int.</param>
        /// <param name="defaultValue">set this property default value when not set value.</param>
        protected DynamicEntityField(
            string name,
            IEntityType propertyType) {
            _name = name;
            _propertyType = propertyType;
        }

        private readonly string _name;
        /// <summary>
        /// Return the identifying name of the field
        /// </summary>
        public override string Name {
            get { return _name; }
        }

        private DynamicEntityType _reflectedType;
        /// <summary>
        /// Return field reflected type.
        /// </summary>
        public DynamicEntityType ReflectedType {
            get { return _reflectedType; }
            internal set { _reflectedType = value; }
        }

        #region Field Get/Set
        private void VerifyEntity(DynamicEntity entity) {
            if (entity == null) {
                OrmUtility.ThrowArgumentNullException("entity");
            }
            if (entity.DynamicEntityType != this._reflectedType) {
                OrmUtility.ThrowArgumentException("entity");
            }
        }

        private DynamicEntity VerifyEntity(object obj) {
            if (obj == null) {
                OrmUtility.ThrowArgumentNullException("entity");
            }
            DynamicEntity entity = obj as DynamicEntity;
            if (entity.DynamicEntityType != this._reflectedType) {
                OrmUtility.ThrowArgumentException("entity");
            }

            return entity;
        }

        /// <summary>
        /// Gets the current value of the field on the entity.
        /// </summary>
        /// <param name="entity">Will return the field value of an entity.</param>
        /// <returns>The current value of the field on the entity.</returns>
        public object GetValue(DynamicEntity entity) {
            this.VerifyEntity(entity);

            return this.GetValueCore(entity);
        }

        protected abstract object GetValueCore(DynamicEntity entity);

        /// <summary>
        /// The entity in the value of this property is set to a new value.
        /// </summary>
        /// <param name="entity">Will set the property value of an entity.</param>
        /// <param name="newValue">a new value</param>
        public void SetValue(DynamicEntity entity, object newValue) {
            this.VerifyEntity(entity);

            this.SetValueCore(entity, newValue);
        }

        protected abstract void SetValueCore(DynamicEntity entity, object newValue);

        /// <summary>
        /// Reset property value.
        /// </summary>
        /// <param name="entity">Will reset the property value of an entity.</param>
        public void ResetValue(DynamicEntity entity) {
            this.VerifyEntity(entity);
            this.ResetValueCore(entity);            
        }

        protected virtual void ResetValueCore(DynamicEntity entity) {
            entity._storage.ClearValue(this);
        }

        object IValueAccessor.GetValue(object entity) {
            return this.GetValueCore(VerifyEntity(entity));
        }

        void IValueAccessor.SetValue(object entity, object newValue) {
            this.SetValueCore(VerifyEntity(entity), newValue);
        }

        void IValueAccessor.ResetValue(object entity) {
            this.ResetValueCore(VerifyEntity(entity));
        }

        /// <summary>
        /// Determines a value that indicates whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="entity">Entity with the property to be examined for persistence.</param>
        /// <returns>If the property should be preserved, it is true, otherwise false.</returns>
        public bool ShouldSerializeValue(DynamicEntity entity) {
            this.VerifyEntity(entity);

            var value = entity._storage.GetValue(this);
            return value == null;
        }

        #endregion

        private readonly IEntityType _propertyType;
        /// <summary>
        /// Gets the return type of the property.
        /// </summary>
        public IEntityType PropertyType {
            get { return this._propertyType; }
        }
    
        private int _ordinal;
        /// <summary>
        /// Gets the property's location in the collection of ReflectedType.
        /// </summary>
        public int Ordinal {
            get { return _ordinal; }
            internal set { _ordinal = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the property is read-only.
        /// </summary>
        bool IValueAccessor.IsReadOnly {
            get { return false; }
        }

        /// <summary>
        /// Returns property information.
        /// </summary>
        /// <returns>Including the name, type and other information.</returns>
        public override string ToString() {
            return string.Format("{0}: {1}",this.Name, this.PropertyType.Name);
        }

    }
}
