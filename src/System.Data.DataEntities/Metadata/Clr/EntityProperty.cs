using System.ComponentModel;
using System.Reflection;

namespace System.Data.DataEntities.Metadata.Clr
{
    internal class EntityProperty : MemberMetadataBase<PropertyInfo>,IEntityProperty
    {
        private readonly Func<object, object> _getValueHandler;
        private readonly Action<object, object> _setValueHandler;

        public EntityProperty(PropertyInfo property)
            :base(property)
        {
            //If the property and return types are public, then the use of dynamic compilation delegate.
            _getValueHandler = property.GetGetValueFunc();
            if (_getValueHandler == null)
            {
                //Otherwise, use reflection to access.
                _getValueHandler = this.GetValueByReflection;
            }

            _setValueHandler = property.GetSetValueFunc();
            if (_setValueHandler == null)
            {
                _setValueHandler = this.SetValueByReflection;
            }
            _propertyDescriptor = new Lazy<PropertyDescriptor>(this.GetPropertyDescriptor, false);
        }

        public object GetValue(object entity)
        {
            return _getValueHandler(entity);
        }

        private object GetValueByReflection(object entity)
        {
            return this.ClrMember.GetValue(entity, null);
        }

        public void SetValue(object entity, object newValue)
        {
            this._setValueHandler(entity, newValue);
        }

        private void SetValueByReflection(object entity, object newValue)
        {
            this.ClrMember.SetValue(entity, newValue, null);
        }

        private Lazy<PropertyDescriptor> _propertyDescriptor;

        private PropertyDescriptor GetPropertyDescriptor()
        {
            var propertyDesc = TypeDescriptor.GetProperties(this.ClrMember.ReflectedType).Find(this.ClrMember.Name, false);
            if (propertyDesc == null)
            {
                throw new NotSupportedException();
            }
            return propertyDesc;
        }

        public void ResetValue(object entity)
        {
            this._propertyDescriptor.Value.ResetValue(entity);
        }

        public bool ShouldSerializeValue(object entity)
        {
            return this._propertyDescriptor.Value.ShouldSerializeValue(entity);
        }

        public ComponentModel.TypeConverter Converter
        {
            get { return this._propertyDescriptor.Value.Converter; }
        }

        public bool IsReadOnly
        {
            get { return !this.ClrMember.CanWrite; }
        }

        public Type PropertyType
        {
            get { return this.ClrMember.PropertyType; }
        }
    }
}
