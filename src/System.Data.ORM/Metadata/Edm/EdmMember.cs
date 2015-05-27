using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Edm {

    public abstract class EdmMember : MetadataItem {

        private StructuralType _declaringType;

        public StructuralType DeclaringType {
            get { return _declaringType; }
        }

    }
}
