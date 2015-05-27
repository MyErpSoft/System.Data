using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Edm {

    public abstract class MetadataItem {

        private MetadataName _name;
        /// <summary>
        /// 返回/设置 模型对象的名称。
        /// </summary>
        public MetadataName Name {
            get { return _name; }
            set { _name = value; }
        }

        #region Debug
        /// <summary>
        /// 返回此对象的名称。
        /// </summary>
        /// <returns>此对象的名称</returns>
        public override string ToString() {
            return this.Name.ToString();
        }
        #endregion
    }
}
