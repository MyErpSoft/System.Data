using System.ComponentModel;
using System.Reflection;

namespace System.Data.DataEntities.Metadata.Clr
{
    internal class EntityProperty : MemberMetadataBase<PropertyInfo>, IEntityProperty {
        private Func<object, object> _getValueHandler;
        private Action<object, object> _setValueHandler;

        public EntityProperty(PropertyInfo property)
            : base(property) {
        }

        #region GetValue

        public object GetValue(object entity) {
            //这些动态编译的工作都是延迟处理的，很多时候属性从来不用，就没有必要初始化。
            if (_getValueHandler == null) {
                System.Threading.Interlocked.CompareExchange<Func<object, object>>(ref _getValueHandler, GetGetValueHandler(), null);
            }

            return _getValueHandler(entity);
        }

        private Func<object, object> GetGetValueHandler() {
            //If the property and return types are public, then the use of dynamic compilation delegate.
            var handler = ClrMember.GetGetValueFunc();
            if (handler == null) {
                //Otherwise, use reflection to access.
                handler = this.GetValueByReflection;
            }

            return handler;
        }

        private object GetValueByReflection(object entity) {
            return this.ClrMember.GetValue(entity, null);
        }

        #endregion

        #region SetValue

        public void SetValue(object entity, object newValue) {
            if (_setValueHandler == null) {
                System.Threading.Interlocked.CompareExchange<Action<object, object>>(ref _setValueHandler, GetSetValueHandler(), null);
            }
            this._setValueHandler(entity, newValue);
        }

        private Action<object, object> GetSetValueHandler() {
            var handler = ClrMember.GetSetValueFunc();
            if (handler == null) {
                handler = this.SetValueByReflection;
            }

            return handler;
        }

        private void SetValueByReflection(object entity, object newValue) {
            this.ClrMember.SetValue(entity, newValue, null);
        }

        #endregion

        #region ResetValue

        private PropertyDescriptor _propertyDescriptor;

        private PropertyDescriptor GetPropertyDescriptor() {
            var propertyDesc = TypeDescriptor.GetProperties(this.ClrMember.ReflectedType).Find(this.ClrMember.Name, false);
            if (propertyDesc == null) {
                throw new NotSupportedException();
            }
            return propertyDesc;
        }

        private PropertyDescriptor PropertyDescriptor {
            get {
                if (_propertyDescriptor == null) {
                    System.Threading.Interlocked.CompareExchange<PropertyDescriptor>(ref _propertyDescriptor, GetPropertyDescriptor(), null);
                }

                return _propertyDescriptor;
            }
        }
    
        public void ResetValue(object entity)
        {
            this.PropertyDescriptor.ResetValue(entity);
        }
        #endregion

        public bool ShouldSerializeValue(object entity)
        {
            return this.PropertyDescriptor.ShouldSerializeValue(entity);
        }

        public ComponentModel.TypeConverter Converter
        {
            get { return this.PropertyDescriptor.Converter; }
        }

        public bool IsReadOnly
        {
            get { return !this.ClrMember.CanWrite; }
        }

        private EntityType _propertyType;
        public EntityType PropertyType
        {
            get {
                if (_propertyType == null) {
                    System.Threading.Interlocked.CompareExchange<EntityType>(
                        ref _propertyType, 
                        EntityType.GetEntityType(this.ClrMember.PropertyType), 
                        null);
                }

                return _propertyType;
            }
        }

        IEntityType IValueAccessor.PropertyType {
            get { return this.PropertyType; }
        }
    }
}
