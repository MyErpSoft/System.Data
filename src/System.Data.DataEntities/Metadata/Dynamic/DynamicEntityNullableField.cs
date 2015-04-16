using System;
using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.DataEntities.Metadata.Dynamic {

    internal sealed class DynamicEntityNullableField : DynamicEntityField {
        private readonly Type _underlyingType;

        public DynamicEntityNullableField(string name, Type propertyType)
            : base(name, propertyType) {
                this._underlyingType = Nullable.GetUnderlyingType(propertyType);
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
                if (newValueType != this._underlyingType) {
                    OrmUtility.ThrowArgumentException("Assigning data types do not match.");
                }
                entity._storage.SetValue(this, newValue);
            }
        }

    }
}
