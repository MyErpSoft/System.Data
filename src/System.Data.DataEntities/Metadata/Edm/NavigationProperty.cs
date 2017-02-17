using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Edm {

    public sealed class NavigationProperty : EdmMember {

        private RelationshipType _relationshipType;
        /// <summary>
        /// 返回/设置 属性使用的关系。
        /// </summary>
        public RelationshipType RelationshipType {
            get { return _relationshipType; }
            set { _relationshipType = value; }
        }

    }
}
