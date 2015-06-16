namespace System.Data.Metadata.Database {

    /// <summary>
    /// 定义了一个接口，允许通过名称检索数据库元数据中的对象。
    /// </summary>
    public interface IDatabaseMetadataBinder {

        Table GetTable(string name);
    }
}
