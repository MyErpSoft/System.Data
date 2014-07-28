using System.ComponentModel;
using System.Data.DataEntities.Dynamic;

namespace System.Data.DataEntities.Metadata.Dynamic {

    /// <summary>
    /// DynamicEntity property definitions.
    /// </summary>
    public class DynamicEntityProperty : DynamicMemberMetadata, IEntityProperty {

        /// <summary>
        /// Derived class can create dynamic entity property descriptor.
        /// </summary>
        /// <param name="name">The identifying name of the property, following the c# property name constraints.</param>
        /// <param name="propertyType">Property return type,for example, returns the int.</param>
        /// <param name="isReadOnly">set this property is a read-only property if value is true.</param>
        /// <param name="defaultValue">set this property default value when not set value.</param>
        /// <param name="attributes">Custom attributes</param>
        protected internal DynamicEntityProperty(
            string name,
            Type propertyType,
            bool isReadOnly,
            object defaultValue,
            object[] attributes) {
            _name = name;
            _propertyType = propertyType;
            _isReadOnly = isReadOnly;
            _defaultValue = defaultValue;
            _attributes = attributes;
        }

        private readonly string _name;
        /// <summary>
        /// Return the identifying name of the property
        /// </summary>
        public override string Name {
            get { return _name; }
        }

        private object[] _attributes;
        /// <summary>
        /// Returns all the custom attributes defined on this type of arrays, or if there are no custom attributes, return an empty array.
        /// </summary>
        /// <param name="inherit">When true, and find inherited custom attribute hierarchy chain.</param>
        /// <returns>Represents a custom property of an array of objects, or an empty array.</returns>
        public override object[] GetCustomAttributes(bool inherit) {
            return _attributes ?? EmptyAttributes;
        }

        private DynamicEntityType _reflectedType;
        /// <summary>
        /// Return property reflected type.
        /// </summary>
        public DynamicEntityType ReflectedType {
            get { return _reflectedType; }
            internal set { _reflectedType = value; }
        }

        #region Property Get/Set
        /// <summary>
        /// Return maped property.
        /// </summary>
        private DynamicEntityProperty GetCoreProperty(DynamicEntity entity) {
            if (null == entity) {
                OrmUtility.ThrowArgumentNullException("entity");
            }

            var dt = entity._dt;
            //If the entity type is the current type of the property is, for direct use.
            if (_reflectedType.MetadataToken == dt.MetadataToken) {
                return this;
            }
            //Aims to split into two methods, we believe that the preceding code hit rate is very high, so keeping this function to streamline.
            return GetCorePropertyOther(dt);
        }

        private DynamicEntityProperty GetCorePropertyOther(DynamicEntityType dt) {
            if (_reflectedType.IsInterface) {
                //If there is an interface, get the property of the interface mapping.
                return dt.GetInterfacePropertyMap(this);
            }
            else {
                //If it is a derived relationship (single inheritance), we actually require a base class property location, and derived classes must also be in this position.
                var properties = dt.Properties;
                DynamicEntityProperty coreProperty;
                if (_ordinal < properties.Count) {
                    coreProperty = properties[_ordinal];
                    if (coreProperty.MetadataToken == this.MetadataToken) {
                        return coreProperty;
                    }
                }
            }
            OrmUtility.ThrowInvalidOperationException("No derivation relationship");
            return null;
        }

        internal static DynamicEntity GetDynamicEntity(object entity) {
            if (null == entity) {
                OrmUtility.ThrowArgumentNullException("entity");
            }
            DynamicEntity result = entity as DynamicEntity;
            if (null == result) {
                OrmUtility.ThrowArgumentException("An instance must be of type DynamicEntity.");
            }

            return result;
        }

        /// <summary>
        /// Gets the current value of the property on the entity.
        /// </summary>
        /// <param name="entity">Will return the property value of an entity.</param>
        /// <returns>The current value of the property on the entity.</returns>
        public object GetValue(DynamicEntity entity) {
            return this.GetCoreProperty(entity).GetValueCore(entity);
        }

        /// <summary>
        /// Gets the current value of the property on the entity.
        /// </summary>
        /// <param name="entity">Will return the property value of an entity.</param>
        /// <returns>The current value of the property on the entity.</returns>
        public T GetValue<T>(DynamicEntity entity) {
            return (T)this.GetCoreProperty(entity).GetValueCore(entity);
        }

        object IEntityProperty.GetValue(object entity) {
            var dynamicEntity = GetDynamicEntity(entity);
            return this.GetCoreProperty(dynamicEntity).GetValueCore(dynamicEntity);
        }

        private static readonly object NullObject = new object();
        /// <summary>
        /// Gets the current value of the property on the entity core method.
        /// </summary>
        /// <param name="entity">Will return the property value of an entity.</param>
        /// <returns>The current value of the property on the entity.</returns>
        internal protected virtual object GetValueCore(DynamicEntity entity) {
            object value = entity._values[_ordinal];
            if (value == null) {
                if (_isReadOnly) {
                    value = LazyCreateValue(entity);
                    entity._values[_ordinal] = value;
                }
                else {
                    value = _defaultValue;
                }
            }
            else if (value == NullObject) {
                value = null;
            }
            return value;
        }

        /// <summary>
        /// Will delay creates its value when the property is read-only or first time getting the value, the value created through the overloads for this method to define the delay.
        /// </summary>
        /// <param name="entity">Will get the property value of an entity.</param>
        /// <returns>Delay creating a default value.</returns>
        protected virtual object LazyCreateValue(DynamicEntity entity) {
            return _defaultValue;
        }

        /// <summary>
        /// The entity in the value of this property is set to a new value.
        /// </summary>
        /// <param name="entity">Will set the property value of an entity.</param>
        /// <param name="newValue">a new value</param>
        public void SetValue(DynamicEntity entity, object newValue) {
            this.GetCoreProperty(entity).SetValueCore(entity, newValue);
        }

        void IEntityProperty.SetValue(object entity, object newValue) {
            var dynamicEntity = GetDynamicEntity(entity);
            this.GetCoreProperty(dynamicEntity).SetValueCore(dynamicEntity, newValue);
        }

        /// <summary>
        /// Core methods for setting values
        /// </summary>
        /// <param name="entity">Will set the property value of an entity.</param>
        /// <param name="newValue">a new value</param>
        internal protected virtual void SetValueCore(DynamicEntity entity, object newValue) {
            if (_isReadOnly) {
                OrmUtility.ThrowInvalidOperationException("This property is readonly.");
            }

            object oldValue = this.GetValueCore(entity);
            //Only really changes before continuing to
            if (!object.Equals(oldValue, newValue)) {
                if (object.Equals(newValue, _defaultValue)) {
                    //reset
                    newValue = null;
                }
                else {
                    if (newValue == null) {
                        if (!_propertyType.IsValueType) {
                            newValue = NullObject;
                        }
                    }
                    else {
                        //Check the data types
                        var newValueType = newValue.GetType();
                        if ((newValueType != _propertyType) &&
                            (!_propertyType.IsAssignableFrom(newValueType))) {
                                OrmUtility.ThrowArgumentException("Assigning data types do not match.");
                        }
                    }

                    //TODO:Call the callback, confirm that the data is qualified.
                }

                if (!entity.OnPropertyChanging(this._name, oldValue, newValue)) {
                    entity._values[_ordinal] = newValue;
                    entity.OnPropertyChanged(this.PropertyChangedEventArgs);
                }
            }
            else {
                entity.OnPropertyUnchanged(this.PropertyUnchangedEventArgs);
            }
        }

        [NonSerialized]
        private PropertyChangedEventArgs _propertyChangedEventArgs;
        private PropertyChangedEventArgs PropertyChangedEventArgs {
            get {
                if (_propertyChangedEventArgs == null) {
                    _propertyChangedEventArgs = new DynamicEntityPropertyChangedEventArgs(this);
                }
                return _propertyChangedEventArgs;
            }
        }

        [NonSerialized]
        private PropertyUnchangedEventArgs _propertyUnchangedEventArgsCache;
        /// <summary>
        /// Returns the cached event did not change the parameter object, in order to reduce the SetValue object creation quantity.
        /// </summary>
        private PropertyUnchangedEventArgs PropertyUnchangedEventArgs {
            get {
                if (_propertyUnchangedEventArgsCache == null) {
                    _propertyUnchangedEventArgsCache = new PropertyUnchangedEventArgs(this.Name);
                }
                return _propertyUnchangedEventArgsCache;
            }
        }

        /// <summary>
        /// Reset property value.
        /// </summary>
        /// <param name="entity">Will reset the property value of an entity.</param>
        public void ResetValue(DynamicEntity entity) {
            this.GetCoreProperty(entity).ResetValue(entity);
        }

        /// <summary>
        /// Core methods for reset values
        /// </summary>
        /// <param name="entity">Will reset the property value of an entity.</param>
        protected virtual void ResetValueCore(DynamicEntity entity) {
            if (_isReadOnly) {
                OrmUtility.ThrowInvalidOperationException("This is property is readonly.");
            }

            object oldValue = this.GetValueCore(entity);
            if (!object.Equals(oldValue, _defaultValue)) {
                entity._values[_ordinal] = null;
                entity.OnPropertyChanged(this.PropertyChangedEventArgs);
            }
        }

        /// <summary>
        /// Determines a value that indicates whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="entity">Entity with the property to be examined for persistence.</param>
        /// <returns>If the property should be preserved, it is true, otherwise false.</returns>
        public bool ShouldSerializeValue(DynamicEntity entity) {
            return this.GetCoreProperty(entity).ShouldSerializeValueCore(entity);
        }

        /// <summary>
        /// Core methods for ShouldSerializeValue
        /// </summary>
        /// <param name="entity">Entity with the property to be examined for persistence.</param>
        /// <returns>If the property should be preserved, it is true, otherwise false.</returns>
        protected virtual bool ShouldSerializeValueCore(DynamicEntity entity) {
            return _isReadOnly ? false : (entity._values[_ordinal] != null);
        }

        #endregion

        private readonly bool _isReadOnly;
        /// <summary>
        /// Gets a value indicating whether the property is read-only.
        /// </summary>
        public bool IsReadOnly {
            get { return _isReadOnly; }
        }

        private readonly Type _propertyType;
        /// <summary>
        /// Gets the return type of the property.
        /// </summary>
        public Type PropertyType {
            get { return _propertyType; }
        }

        private int _ordinal;
        /// <summary>
        /// Gets the property's location in the collection of ReflectedType.
        /// </summary>
        public int Ordinal {
            get { return _ordinal; }
            internal set { _ordinal = value; }
        }

        private readonly object _defaultValue;
        /// <summary>
        /// Returns the default value of this property.
        /// </summary>
        public object DefaultValue {
            get { return _defaultValue; }
        }

        /// <summary>
        /// Copy the current property descriptor.
        /// </summary>
        /// <returns>New property descriptor</returns>
        internal protected virtual DynamicEntityProperty Clone() {
            return (DynamicEntityProperty)this.MemberwiseClone();
        }

        /// <summary>
        /// Returns property information.
        /// </summary>
        /// <returns>Including the name, type and other information.</returns>
        public override string ToString() {
            return string.Format("{0}:{1}[{2}]", this.GetType().Name, this.Name, this.PropertyType.Name);
        }
    }
}
