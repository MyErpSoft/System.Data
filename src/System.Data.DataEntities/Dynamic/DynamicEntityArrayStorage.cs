using System.Data.Metadata.DataEntities.Dynamic;

namespace System.Data.DataEntities.Dynamic {

    internal sealed class DynamicEntityArrayStorage : IDynamicEntityStorage {
        private object[] _values;
        private readonly DynamicEntityType _dt;
        private static readonly object[] _empty = new object[0];

        public DynamicEntityArrayStorage(DynamicEntityType dt) {
            this._dt = dt;
            this._values = _empty;
        }

        private void EnsureCapacity() {
            int count = _dt.Fields.Count;
            object[] newValues = new object[count];
            _values.CopyTo(newValues, 0);
            _values = newValues;
        }

        public object GetValue(DynamicEntityField field) {
            int ordinal = field.Ordinal;
            //但数组不够用时，GetValue特殊处理了，这里没有必要扩充空间，因为没有写操作
            return (ordinal < _values.Length) ? this._values[ordinal] : null;
        }

        public void SetValue(DynamicEntityField field, object value) {
            int ordinal = field.Ordinal;
            //由于要写入，所以这里必须扩充空间
            if (_values.Length <= ordinal ) {
                this.EnsureCapacity();
            }

            this._values[ordinal] = value;
        }

        public void ClearValue(DynamicEntityField field) {
            int ordinal = field.Ordinal;
            //注意这里，空间口没有分配的话，没有必要清空。
            if (ordinal < _values.Length) {
                this._values[ordinal] = null;
            }
        }
    }
}
