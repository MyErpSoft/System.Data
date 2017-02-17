using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Edm {

    public abstract class EdmType : GlobalItem {

        private bool _abstract;
        /// <summary>
        /// 返回 当前类型是否是抽象类型
        /// </summary>
        public bool Abstract {
            get { return _abstract; }
            set { _abstract = value; }
        }

        private bool _sealed;
        /// <summary>
        /// 返回/设置类型是否已密封，密封的类不能继承
        /// </summary>
        public bool Sealed {
            get { return _sealed; }
            set { _sealed = value; }
        }

        private EdmType _baseType;
        /// <summary>
        /// 返回/设置类型的基础类型
        /// </summary>
        public EdmType BaseType {
            get { return _baseType; }
            set { _baseType = value; }
        }

    }
}
