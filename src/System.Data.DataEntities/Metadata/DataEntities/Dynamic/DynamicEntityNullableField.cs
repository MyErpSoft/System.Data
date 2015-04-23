using System.Data.DataEntities.Dynamic;

namespace System.Data.Metadata.DataEntities.Dynamic {

    internal sealed class DynamicEntityNullableField : DynamicEntityField {
        private readonly Type _underlyingType;

        public DynamicEntityNullableField(string name, IEntityType propertyType)
            : base(name, propertyType) {
                this._underlyingType = Nullable.GetUnderlyingType(propertyType.UnderlyingSystemType);
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
                //值类型没有派生关系
                if (newValueType != this._underlyingType) {
                    OrmUtility.ThrowArgumentException("Assigning data types do not match.");
                }
                entity._storage.SetValue(this, newValue);
            }
        }

    }
}
