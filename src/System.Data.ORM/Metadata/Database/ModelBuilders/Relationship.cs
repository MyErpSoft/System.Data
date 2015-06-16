using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.DataEntities.Dynamic;
using System.Data.Metadata.DataEntities.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Database.ModelBuilders {

    public class Relationship : DynamicEntity  {
        public static readonly DynamicEntityType RelationshipDynamicEntityType = new DynamicEntityType("Relationship");

        public Relationship() : base(RelationshipDynamicEntityType) {
            this._endMember = new Collection<EndMember>(new List<EndMember>(1));
        }

        private string _name;
        /// <summary>
        /// 返回/设置关系的名称。
        /// </summary>
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        private Collection<EndMember> _endMember;
        /// <summary>
        /// 返回这个关系用于关联的对集合。
        /// </summary>
        public Collection<EndMember> EndMembers {
            get { return _endMember; }
        }

        private string _to;
        /// <summary>
        /// 返回/设置关系对应的目标表
        /// </summary>
        public string To {
            get { return _to; }
            set { _to = value; }
        }

        private bool _isCollection;
        /// <summary>
        /// 返回/设置 此关系返回的数据是否是一个集合。
        /// </summary>
        public bool IsCollection {
            get { return _isCollection; }
            set { _isCollection = value; }
        }

    }
}
