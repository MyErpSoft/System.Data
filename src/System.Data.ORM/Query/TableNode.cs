using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// 通过一个关系名，尝试获取对应的子表。必须是当前已经添加到Relations中的数据，而不是Table中已经定义的关系。
        /// </summary>
        /// <param name="relationName">要检索的关系名称</param>
        /// <param name="tableNode">如果此名称的关系找到，将返回此实例，否则返回null.</param>
        /// <returns>如果找到此关系名称的子表，返回true，否则返回false.</returns>
        public bool TryGetChild(string relationName, out TableNode tableNode) {
            if (!this.RelationsIsEmpty) {
                foreach (var item in this.Relations) {
                    if (item.ParentRelationship.Name == relationName) {
                        tableNode = item;
                        return true;
                    }
                }
            }

            tableNode = null;
            return false;
        }

        private Collection<TableNode> _relations;
        /// <summary>
        /// 返回所有待输出的子表，这些表会在SQL语句中出现并使用join关联。
        /// </summary>
        public Collection<TableNode> Relations {
            get {
                if (_relations == null) {
                    _relations = new Collection<TableNode>(new List<TableNode>(2));
                }
                return _relations;
            }
        }

        /// <summary>
        /// 尝试检测此表是否输出了某个字段。
        /// </summary>
        /// <param name="fieldName">要检测的字段名称。</param>
        /// <param name="fieldNode">如果已经包含此字段，将返回此对象，否则返回null.</param>
        /// <returns>返回是否已经包含此字段</returns>
        public bool TryGetField(string fieldName,out FieldNode fieldNode) {
            if (this._fields != null) {
                foreach (var field in this._fields) {
                    if (field.Field.Name == fieldName) {
                        fieldNode = field;
                        return true;
                    }
                }
            }

            fieldNode = null;
            return false;
        }

        private Collection<FieldNode> _fields;
        /// <summary>
        /// 返回此表所有在SQL语句中出现的字段。
        /// </summary>
        public Collection<FieldNode> Fields {
            get {
                if (_fields == null) {
                    _fields = new Collection<FieldNode>();
                }
                return _fields;
            }
        }
    }
}
