using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System.Data.Metadata.Database {

    /// <summary>
    /// 存储数据库元数据信息的容器。
    /// </summary>
    public class DatabaseMetadataContainer : IDatabaseMetadataBinder {

        public DatabaseMetadataContainer() {
            this._tables = new ConcurrentDictionary<string, Table>(_nameComparer);
            this._tableResolveNames = new ConcurrentDictionary<string, int>(_nameComparer);
        }

        #region Get/TryGet

        /// <summary>
        /// 通过名称检索表。
        /// </summary>
        /// <param name="tableName">要检索的表名称。</param>
        /// <returns>如果找到此名称的表，返回实例，否则抛出异常。</returns>
        public Table GetTable(string tableName) {
            Table table;
            if (!TryGetTable(tableName, out table)) {
                OrmUtility.ThrowKeyNotFoundException(
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.TableNotFoundException,tableName));
            }

            return table;
        }

        private readonly ConcurrentDictionary<string, Table> _tables;
        /// <summary>
        /// 尝试通过名称检索表。
        /// </summary>
        /// <param name="tableName">要检索的表名称。</param>
        /// <param name="table">如果找到此名称的表，返回实例，否则返回null.</param>
        /// <returns>如果找到此名称的表，返回true</returns>
        public virtual bool TryGetTable(string tableName, out Table table) {
            if (!string.IsNullOrEmpty(tableName)) {
                if (_tables.TryGetValue(tableName, out table)) {
                    return true;
                }

                //使用事件延迟获取表定义。
                this.OnTableResolve(new TableResolveEventArgs(tableName));

                //再次尝试获取
                if (_tables.TryGetValue(tableName, out table)) {
                    return true;
                }
            }

            table = null;
            return false;
        }

        /// <summary>
        /// 尝试通过名称检索表。如果没有找到不会触发TableResolve事件。
        /// </summary>
        /// <param name="tableName">要检索的表名称。</param>
        /// <param name="table">如果找到此名称的表，返回实例，否则返回null.</param>
        /// <returns>如果找到此名称的表，返回true</returns>
        public virtual bool TryGetTableWithoutResolveEvent(string tableName, out Table table) {
            if (string.IsNullOrEmpty(tableName)) {
                table = null;
                return false;
            }

            return _tables.TryGetValue(tableName, out table);
        }
        #endregion

        #region Add
        /// <summary>
        /// 向容器中添加一个表。
        /// </summary>
        /// <param name="table">要添加的表对象。</param>
        /// <returns>由于存在并发问题，所以可能在添加时之前已经有人添加了此表，如果此次添加成功将返回true，否则返回false.</returns>
        public virtual bool TryAdd(Table table) {
            if (table == null) {
                OrmUtility.ThrowArgumentNullException("table");
            }
            this.VerifyTable(table);
            
            return _tables.TryAdd(table.Name, table);
        }

        /// <summary>
        /// 检测添加的表是否符合规范。
        /// </summary>
        /// <param name="table">要添加的表</param>
        protected virtual void VerifyTable(Table table) {
            if (!OrmUtility.VerifyName(table.Name)) {
                OrmUtility.ThrowArgumentException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.ErrorName, table.Name));
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 当试图检索某个名称的表未找到时，发生此事件。
        /// </summary>
        /// <remarks>
        /// <para>为支持元数据的延迟加载而设计了此事件，在大型ERP中，数据结构非常庞大，在调试模式下加载众多的定义会十分耗时，此事件就允许使用到某个定义时才加载配置。</para>
        /// <para>此事件的参数并没有属性让你回复找到的表定义，你需要在事件中自己调用TryAdd方法加入定义，在事件结束后会尝试重新检索。这样设计的目的主要是允许在事件中一次性加入多个元数据定义。</para>
        /// <para>注意，由于存在并发检索，所以可能两个线程同时检索某个未加载的表元数据，事件将会在两个线程中同时播发，但只有一个线程添加数据。</para>
        /// <para>如果某个名称的表在发生事件后还是没有找到，一般的就不会再次触发此事件，防止不必要的检索。但在并发环境下，可能还是会触发多次，而且如果内部缓存的黑名单多大也可能还会重新再次触发。</para>
        /// </remarks>
        public event EventHandler<MetadataResolveEventArgs> TableResolve;

        private ConcurrentDictionary<string,int> _tableResolveNames;

        /// <summary>
        /// 当试图检索某个名称的表未找到时，触发TableResolve事件。
        /// </summary>
        protected virtual void OnTableResolve(MetadataResolveEventArgs e) {
            var tableResolve = this.TableResolve;
            //防止某个名称一直检索不到，一直调用TryGet方法
            if (tableResolve != null && !_tableResolveNames.ContainsKey(e.Name)) {
                tableResolve(this, e);

                if (!_tables.ContainsKey(e.Name)) { //还是没有找到，加入黑名单
                    if (_tableResolveNames.Count > 10000) {
                        _tableResolveNames.Clear();
                    }
                    _tableResolveNames.TryAdd(e.Name, 0);
                }
            }
        }

        private sealed class TableResolveEventArgs : MetadataResolveEventArgs {
            public TableResolveEventArgs(string name) : base(name) {
            }

            public override Type MetadataType {
                get { return typeof(Table); }
            }
        }
        #endregion

        #region 字符串比较
        private static readonly StringComparer _nameComparer = StringComparer.OrdinalIgnoreCase;
        /// <summary>
        /// 返回名称比较的模式，考虑到设计的复杂性，以及实际需求，仅仅支持全局指定默认的模式。默认是不区分大小写。
        /// </summary>
        public static StringComparer DefaultNameComparer {
            get { return _nameComparer; }
        }
        #endregion
    }
}
