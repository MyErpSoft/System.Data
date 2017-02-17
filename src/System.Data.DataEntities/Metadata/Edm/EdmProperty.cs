using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Metadata.Edm {
    /// <summary>
    /// 描述一个领域属性，可以是简单属性、复杂属性或集合属性
    /// </summary>
    public class EdmProperty : EdmMember {

        private SimpleType _propertyType;
        /// <summary>
        /// 返回属性的返回类型.
        /// </summary>
        public SimpleType PropertyType {
            get { return _propertyType; }
            set { _propertyType = value; }
        }
        
    }
}