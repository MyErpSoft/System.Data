using System.Threading;

namespace System.Data.Metadata.Database {

    /// <summary>
    /// 成员元数据的基类。
    /// </summary>
    public abstract class MemberMetadata {

        protected MemberMetadata(string name) {
            if (string.IsNullOrEmpty(name)) {
                OrmUtility.ThrowArgumentNullException("name");
            }
            this._name = name;
        }

        private readonly string _name;
        /// <summary>
        /// 返回 成员对象的名称。
        /// </summary>
        public string Name {
            get { return _name; }
        }
    }
}
