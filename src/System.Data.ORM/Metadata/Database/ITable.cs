using System.Data.Metadata.DataEntities;

namespace System.Data.Metadata.Database {

    /// <summary>
    /// 定义了数据库的一个表或一个查询结果的Schema结构。
    /// </summary>
    public interface ITable : IMemberMetadata {

        /// <summary>
        /// 返回指定名称的字段
        /// </summary>
        IField GetField(string name);

    }
}
