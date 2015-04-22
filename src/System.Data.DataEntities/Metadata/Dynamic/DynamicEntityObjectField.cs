using System;
using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.DataEntities.Metadata.Dynamic {

    internal sealed class DynamicEntityObjectField : DynamicEntityField {
        public DynamicEntityObjectField(string name, IEntityType propertyType)
            : base(name, propertyType) {
        }

        private readonly Type _propertySystemType;
        protected override object GetValueCore(DynamicEntity entity) {
            return entity._storage.GetValue(this);
        }

        protected override void SetValueCore(DynamicEntity entity, object newValue) {

            if (newValue == null) {
                entity._storage.ClearValue(this);
            }
            else {
                //Check the data types
                if (!this.PropertyType.IsInstanceOfType(newValue)) {
                    OrmUtility.ThrowArgumentException("Assigning data types do not match.");
                }
                
                entity._storage.SetValue(this, newValue);
            }
        }

    }
}
