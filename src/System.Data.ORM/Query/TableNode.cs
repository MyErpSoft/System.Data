using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Metadata;
using System.Data.Metadata.Database;

namespace System.Data.Query {

    /// <summary>
    /// 描述了Select语句中要查询的其中一个表。
    /// </summary>
    /// <remarks>
    /// <para>比如，你希望输出Customers表的CustomerId和Name，那么要查询的表Customers就又此类描述。</para>
    /// <para>此类更重要的功能是描述多个输出表之间的关系，例如你希望输出订单的部分字段，以及次订单的客户部分字段，那么将会出现两个
    /// SelectEntityNode实例，一个描述订单，一个描述客户，客户实例还描述了与订单如何关联的信息</para>
    /// </remarks>
    internal class TableNode {

        public TableNode(Table rootTable) {
            this._table = rootTable;
        }

        public TableNode (Table table, Relationship relation) {
            this._table = table;
            this._parentRelationship = relation;
        }

        private readonly Table _table;
        /// <summary>
        /// 输出节点的类型
        /// </summary>
        public Table Table {
            get { return _table; }
        }

        private readonly Relationship _parentRelationship;
        /// <summary>
        /// 如果此节点通过关系导航到此节点，返回关联的关系对象
        /// </summary>
        public Relationship ParentRelationship {
            get { return _parentRelationship; }
        }

        private string _alias;
        /// <summary>
        /// 返回/设置 此输出定义的别名。
        /// </summary>
        public string Alias {
            get { return _alias; }
            set { _alias = value; }
        }
        
        /// <summary>
        /// 返回是否没有子表，减少对Relations属性的访问造成的对象创建。
        /// </summary>
        /// <returns></returns>
        public bool RelationsIsEmpty {
            get { return _relations == null || _relations.Count == 0; }
        }

        private TableNodeCollection _relations;
        /// <summary>
        /// 返回所有待输出的子表，这些表会在SQL语句中出现并使用join关联。
        /// </summary>
        public TableNodeCollection Relations {
            get {
                if (_relations == null) {
                    _relations = new TableNodeCollection();
                }
                return _relations;
            }
        }
        
        private FieldNodeCollection _fields;
        /// <summary>
        /// 返回此表所有在SQL语句中出现的字段。
        /// </summary>
        public FieldNodeCollection Fields {
            get {
                if (_fields == null) {
                    _fields = new FieldNodeCollection();
                }
                return _fields;
            }
        }
    }

    internal sealed class TableNodeCollection : MetadataCollection<string, TableNode> {
        public TableNodeCollection():base(new List<TableNode>(2),null) {
        }
        protected override string GetKeyForItem(TableNode item) {
            return item.ParentRelationship.Name;
        }
    }
}
