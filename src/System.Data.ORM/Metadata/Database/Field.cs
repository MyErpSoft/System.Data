using System.Data.Metadata.DataEntities;

namespace System.Data.Metadata.Database {

    /// <summary>
    /// 描述了一个数据库的字段对象。
    /// </summary>
    public class Field : MemberMetadata {

        public Field(string name,IEntityType fieldType)
            : base(name) {
            if (fieldType == null) {
                OrmUtility.ThrowArgumentNullException("fieldType");
            }
            this._fieldType = fieldType;
        }

        private readonly IEntityType _fieldType;
        /// <summary>
        /// 返回 字段关联的数据类型。
        /// </summary>
        public IEntityType FieldType {
            get { return _fieldType; }
        }

        private Table _table;
        /// <summary>
        /// 返回 字段所在的表。
        /// </summary>
        public Table Table {
            get { return this._table; }
            internal set { this._table = value; }
        }
    }
}
