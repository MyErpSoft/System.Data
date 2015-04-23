using System.Collections.Generic;
using System.Data.Metadata.DataEntities.Dynamic;

namespace System.Data.DataEntities.Dynamic {

    internal sealed class DynamicEntityDictStorage : IDynamicEntityStorage {
        private Dictionary<DynamicEntityField,object> _values;

        public object GetValue(DynamicEntityField field) {
            object value;
            if (this._values != null && this._values.TryGetValue(field,out value)) {
                return value;
            }
            return null;
        }

        public void SetValue(DynamicEntityField field, object value) {
            if (this._values == null) {
                this._values = new Dictionary<DynamicEntityField, object>(field.ReflectedType.Fields.Count);
            }

            this._values[field] = value;
        }

        public void ClearValue(DynamicEntityField field) {
            if (_values != null) {
                this._values.Remove(field);
            }
        }
    }
}
