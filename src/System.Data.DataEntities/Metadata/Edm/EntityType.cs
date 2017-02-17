using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Edm {

    /// <summary>
    /// 描述一个领域模型的类型信息
    /// </summary>
    public class EntityType : EntityTypeBase {


        public EdmPropertyCollection Propertites {
            get {
                throw new System.NotImplementedException();
            }
        }

        public NavigationPropertyCollection NavigationPropertites {
            get { throw new NotImplementedException(); }
        }
    }
}
