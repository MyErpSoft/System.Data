using System;
using System.Collections.Generic;
using System.Data.Query;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Drivers {
    internal class BasicAutoSqlBuilder : IDisposable {

        #region StringBuilder Cache

        private static readonly LockFreeStack<StringBuilder> _sbCache = new LockFreeStack<StringBuilder>();

        private static StringBuilder GetStringBuilder() {
            StringBuilder sb;
            if (!_sbCache.TryPop(out sb)) {
                sb = new StringBuilder();
            }

            return sb;
        }

        private static void Push(StringBuilder sb) {
            sb.Clear();
            _sbCache.Push(sb);
        }

        private StringBuilder _sql;
        public StringBuilder Sql {
            get {
                if (_sql == null) {
                    OrmUtility.ThrowObjectDisposedException(this.GetType().Name);
                }
                return _sql;
            }
        }

        #endregion

        #region IDisposable Support

        protected virtual void Dispose(bool disposing) {
            if (this._sql != null) {
                Push(this._sql); //Dispose本身没有并发，所以不考虑重复加入到列表
                this._sql = null;
            }
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose() {
            Dispose(true);
        }
        #endregion

        /// <summary>
        /// 输出一个字段，例如： t1.CustomerId f1
        /// </summary>
        /// <param name="field">要输出的字段</param>
        protected virtual void AppendField(FieldNode field) {
            Sql.Append(field.TableNode.Alias);
            Sql.Append(".");
            Sql.Append(field.Field.Name);
            Sql.Append(" ");
            Sql.Append(field.Alias);
        }

        /// <summary>
        /// 输出一个表的所有子关系，例如： LEFT JOIN Customers t2 ON t1.f1 = t2.f20 LEFT JOIN ....
        /// </summary>
        /// <param name="tableNode"></param>
        protected virtual void AppendRelations(TableNode tableNode) {
            if (tableNode.RelationsIsEmpty) {
                return;
            }

            foreach (var relation in tableNode.Relations) {
                AppendRelation(tableNode, relation);

                AppendRelations(relation); //递归
            }
        }

        /// <summary>
        /// 输出一个关系，例如： LEFT JOIN Customers t2 ON t1.f1 = t2.f20
        /// </summary>
        protected virtual void AppendRelation(TableNode parentTable, TableNode childTable) {
            Sql.Append(" LEFT JOIN ");
            AppendTable(childTable);
            Sql.Append(" ON ");

            foreach (var endMember in childTable.ParentRelationship.EndMembers) {

            }
        }

        /// <summary>
        /// 输出一个表名称+别名，例如： Customers t2
        /// </summary>
        /// <param name="table">一个表</param>
        protected virtual void AppendTable(TableNode table) {
            Sql.Append(table.Table.Name);
            Sql.Append(" ");
            Sql.Append(table.Alias);
        }
    }
}
