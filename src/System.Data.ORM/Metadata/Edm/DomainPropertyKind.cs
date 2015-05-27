using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.DomainModels {

    /// <summary>
    /// 定义了领域属性其属性的类型
    /// </summary>
    public enum DomainPropertyKind {
        /// <summary>
        /// 属性指向一个简单类型，例如int32
        /// </summary>
        Simple  = 0,

        /// <summary>
        /// 属性指向一个复杂类型，即一个1...0.1 的关系。
        /// </summary>
        Complex = 1,

        /// <summary>
        /// 属性指向一个集合，即一个1....0.n 的关系
        /// </summary>
        Collection = 2

    }
}
