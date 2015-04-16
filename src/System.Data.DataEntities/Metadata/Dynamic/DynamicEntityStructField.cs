using System;
using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.DataEntities.Metadata.Dynamic {

    internal sealed class DynamicEntityStructField : DynamicEntityField {

        public DynamicEntityStructField(string name, Type propertyType)
            : base(name, propertyType) {
            this._defaultValue = Activator.CreateInstance(this.PropertyType);
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
                if (newValueType != this.PropertyType) {
                    OrmUtility.ThrowArgumentException("Assigning data types do not match.");
                }
                entity._storage.SetValue(this, newValue);
            }
        }

    }
}
