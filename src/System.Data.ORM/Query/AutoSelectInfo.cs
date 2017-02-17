using System.Collections.Generic;
using System.Data.Metadata.Database;

namespace System.Data.Query {

    internal sealed class AutoSelectInfo : BasicAutoSqlInfo {

        public AutoSelectInfo(SQLContext sqlContext, string rootTable)
            : this(sqlContext, sqlContext.Binder.GetTable(rootTable)) {
        }

        public AutoSelectInfo(SQLContext sqlContext, Table rootTable)
            : base(sqlContext, rootTable) {
            _selectFields = new List<FieldNode>();
        }

        /// <summary>
        /// 加入一批要输出的字段。
        /// </summary>
        /// <param name="path">要输出的字段所在表的路径，可能包含关系。</param>
        /// <param name="fields">要输出的字段名称</param>
        public void AddSelectFields(string path,params string[] fields ) {
            if (path == null) {
                path = string.Empty;
            }

            if (fields == null || fields.Length == 0) {
                return;
            }

            var tableNode = GetTableNode(path);
            foreach (var fieldName in fields) {
                var fieldNode = GetOrCreateField(tableNode, fieldName);
                _selectFields.Add(fieldNode); //这里不处理字段重复的问题。
            }
        }

        private List<FieldNode> _selectFields;
        /// <summary>
        /// 返回输出的字段列表
        /// </summary>
        public IEnumerable<FieldNode> SelectFields {
            get { return _selectFields; }
        }
        
    }
}
