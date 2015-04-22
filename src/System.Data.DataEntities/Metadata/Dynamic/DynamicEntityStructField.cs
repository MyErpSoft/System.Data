using System;
using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.DataEntities.Metadata.Dynamic {

    internal sealed class DynamicEntityStructField : DynamicEntityField {

        public DynamicEntityStructField(string name, IEntityType propertyType)
            : base(name, propertyType) {
            this._propertySystemType = this.PropertyType.UnderlyingSystemType;
            this._defaultValue = Activator.CreateInstance(_propertySystemType);
        }

        private readonly object _defaultValue;
        /// <summary>
        /// Returns the default value of this property.
        /// </summary>
        public object DefaultValue {
            get { return _defaultValue; }
        }

        protected override object GetValueCore(DynamicEntity entity) {
            object value = entity._storage.GetValue(this);
            if (value == null) {
                return this._defaultValue;
            }

            return value;
        }

        //only cache
        private readonly Type _propertySystemType;
        /// <summary>
        /// Gets the return system type of the property.
        /// </summary>
        public Type PropertySystemType {
            get { return _propertySystemType; }
        }

        protected override void SetValueCore(DynamicEntity entity, object newValue) {

            if (newValue == null) {
                OrmUtility.ThrowArgumentNullException("newValue");
            }

            if (object.Equals(newValue, _defaultValue)) {
                //reset
                entity._storage.ClearValue(this);
            }
            else {
                //Check the data types
                var newValueType = newValue.GetType();
                if (newValueType != _propertySystemType) {
                    OrmUtility.ThrowArgumentException("Assigning data types do not match.");
                }
                entity._storage.SetValue(this, newValue);
            }
        }

    }
}
