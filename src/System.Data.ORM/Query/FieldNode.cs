using System.Data.Metadata;
using System.Data.Metadata.Database;

namespace System.Data.Query {

    internal sealed class FieldNode {

        internal FieldNode(TableNode tableNode, Field field) {
            this._tableNode = tableNode;
            this._field = field;
        }

        private readonly Field _field;
        /// <summary>
        /// 关联的字段
        /// </summary>
        public Field Field {
            get { return _field; }
        }

        private readonly TableNode _tableNode;
        /// <summary>
        /// 返回此字段所在的表节点。
        /// </summary>
        public TableNode TableNode {
            get { return _tableNode; }
        }
        
        private string _alias;
        /// <summary>
        /// 返回/设置字段的别名
        /// </summary>
        public string Alias {
            get { return _alias; }
            set { _alias = value; }
        }

    }

    internal sealed class FieldNodeCollection : MetadataCollection<string, FieldNode> {
        protected override string GetKeyForItem(FieldNode item) {
            return item.Field.Name;
        }
    }
}
