using System;
using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Data.Metadata.DataEntities.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Database.ModelBuilders {

    /// <summary>
    /// 描述一个表对象
    /// </summary>
    public class Table : DynamicEntity  {
        public static readonly DynamicEntityType TableDynamicEntityType =
            new DynamicEntityType("Table");

        public Table() : base(TableDynamicEntityType) {
            this._fields = new FieldCollection();
            this._relationships = new RelationshipCollection();
        }

        private string _name;
        /// <summary>
        /// 返回/设置表的名称。
        /// </summary>
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        private readonly FieldCollection _fields;
        /// <summary>
        /// 返回所有的字段的集合。
        /// </summary>
        public FieldCollection Fields {
            get { return _fields; }
        }

        private readonly RelationshipCollection _relationships;
        /// <summary>
        /// 返回所有关系的集合。
        /// </summary>
        public RelationshipCollection Relationships {
            get { return _relationships; }
        }
    }
}
