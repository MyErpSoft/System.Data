using System.Data.Metadata.DataEntities;

namespace System.Data.Metadata.Database {

    /// <summary>
    /// 描述了一个数据库的字段对象。
    /// </summary>
    public class Field : MemberMetadata {

        public Field(string name)
            : base(name) {

        }
    }
}
