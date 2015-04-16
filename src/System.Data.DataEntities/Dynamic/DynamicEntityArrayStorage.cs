using System;
using System.Data.DataEntities.Metadata.Dynamic;

namespace System.Data.DataEntities.Dynamic {

    internal struct DynamicEntityArrayStorage : IDynamicEntityStorage {
        private object[] _values;
        private readonly DynamicEntityType _dt;

        public DynamicEntityArrayStorage(DynamicEntityType dt) {
            this._dt = dt;
            this._values = null;
        }

        private void EnsureCapacity() {
            int count = _dt.Fields.Count;
            object[] newValues = new object[count];
            if (_values != null) {
                _values.CopyTo(newValues, 0);
            }
            _values = newValues;
        }

        public object GetValue(DynamicEntityField field) {
            int ordinal = field.Ordinal;
            if ((_values != null) && (_values.Length > ordinal)) {
                return this._values[ordinal];
            }
            else {
                return null;
            }
        }

        public void SetValue(DynamicEntityField field, object value) {
            int ordinal = field.Ordinal;
            if ((_values == null) || (_values.Length <= ordinal)) {
                this.EnsureCapacity();
            }

            this._values[ordinal] = value;
        }

        public void ClearValue(DynamicEntityField field) {
            int ordinal = field.Ordinal;
            if ((_values != null) && (_values.Length > ordinal)) {
                this._values[ordinal] = null;
            }
        }
    }
}
