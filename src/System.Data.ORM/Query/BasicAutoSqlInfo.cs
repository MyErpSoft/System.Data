using System;
using System.Collections.Generic;
using System.Data.Metadata.Database;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Query {

    internal abstract class BasicAutoSqlInfo {
        private readonly TableNode _rootTableNode;
        private readonly SQLContext _sqlContext;

        protected BasicAutoSqlInfo(SQLContext sqlContext, Table rootTable) {
            this._sqlContext = sqlContext;
            _rootTableNode = new TableNode(rootTable);
        }

        public TableNode RootTableNode {
            get { return this._rootTableNode; }
        }
        
        protected TableNode GetTableNode(string path) {
            //根对象。
            if (string.IsNullOrEmpty(path)) {
                return _rootTableNode;
            }

            if (path.Contains('.')) {
                //将路径表达式，拆分。
                string[] pathNames = path.Split('.');
                var startIndex = 0;
                if (pathNames[0] == OrmUtility.ActiveObject) {
                    startIndex = 1;
                }

                var currentNode = _rootTableNode;
                for (int i = startIndex; i < pathNames.Length; i++) {
                    var relationName = pathNames[i];
                    currentNode = GetOrCreateRelation(currentNode, relationName);
                }

                return currentNode;
            }
            else {
                //简单模式下(只有一级或0级关系），减少split操作造成的字符串分配
                if (path == OrmUtility.ActiveObject) {
                    return _rootTableNode;
                }

                return GetOrCreateRelation(_rootTableNode, path);
            }
        }

        //检索下一级关系的对象。
        private TableNode GetOrCreateRelation(TableNode tableNode, string relationName) {
            if (string.IsNullOrWhiteSpace(relationName)) {
                OrmUtility.ThrowArgumentException(nameof(relationName));
            }

            //已经输出过使用现有的，否则建立一个。
            TableNode result;
            if (!tableNode.TryGetChild(relationName, out result)) {
                var relation = tableNode.Table.GetRelationship(relationName);
                var alias = _sqlContext.CreateTableAlias();
                result = new TableNode(relation.To, relation) { Alias = alias };
                tableNode.Relations.Add(result);
            }

            return result;
        }

        protected FieldNode GetOrCreateField(TableNode tableNode, string fieldName) {
            FieldNode result;
            if (tableNode.TryGetField(fieldName, out result)) {
                var alias = _sqlContext.CreateFieldAlias();
                result = new FieldNode(tableNode, tableNode.Table.GetField(fieldName)) { Alias = alias };
                tableNode.Fields.Add(result);
            }

            return result;
        }
    }
}
