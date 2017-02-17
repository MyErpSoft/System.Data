using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Edm {

    /// <summary>
    /// 存在成员的类型
    /// </summary>
    public abstract class StructuralType : EdmType {

        private Collection<EdmMember> _members;

        public Collection<EdmMember> Members {
            get { return _members; }
        }

    }
}
