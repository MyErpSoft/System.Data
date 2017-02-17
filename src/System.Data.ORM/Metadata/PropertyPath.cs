using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata {

    internal class PropertyPath<T> where T : PropertyPath<T> {

        public PropertyPath(T previous) {
            this._previous = previous;
        }

        private readonly T _previous;
        /// <summary>
        /// 返回此属性的前一个节点。
        /// </summary>
        public T Previous {
            get { return _previous; }
        }


    }
}
