using System;
using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.DataEntities.Metadata.Dynamic {

    internal sealed class DynamicEntityObjectField : DynamicEntityField {
        public DynamicEntityObjectField(string name, Type propertyType)
            : base(name, propertyType) {
        }

        protected override object GetValueCore(DynamicEntity entity) {
            return entity._storage.GetValue(this);
        }

        protected override void SetValueCore(DynamicEntity entity, object newValue) {

            if (newValue == null) {
                entity._storage.ClearValue(this);
            }
            else {
                //Check the data types
                var newValueType = newValue.GetType();
                if ((newValueType != this.PropertyType) &&
                    (!this.PropertyType.IsAssignableFrom(newValueType))) {
                    OrmUtility.ThrowArgumentException("Assigning data types do not match.");
                }
                entity._storage.SetValue(this, newValue);
            }
        }

    }
}
