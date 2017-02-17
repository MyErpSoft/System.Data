using System.Data.Metadata.Database;
using System.Globalization;

namespace System.Data.Query {

    internal class SQLContext {

        public SQLContext(IDatabaseMetadataBinder binder) {
            this._binder = binder;
        }

        private readonly IDatabaseMetadataBinder _binder;
        /// <summary>
        /// 返回关联的元数据绑定
        /// </summary>
        public IDatabaseMetadataBinder Binder {
            get { return this._binder; }
        }

        private int _tableAliasCount;
        /// <summary>
        /// 创建一个唯一的表别名。
        /// </summary>
        /// <returns>一个别名字符串</returns>
        public string CreateTableAlias() {
            return "t" + (_tableAliasCount++).ToString(CultureInfo.InvariantCulture);
        }

        private int _fieldAliasCount;
        public string CreateFieldAlias() {
            return "f" + (_fieldAliasCount++).ToString(CultureInfo.InvariantCulture);
        }
    }
}
