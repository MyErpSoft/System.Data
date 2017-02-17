
using System.Data.Metadata.Database;

namespace System.Data.Metadata {

    /// <summary>
    /// 当元数据容器检索某个名称的对象未找到时，播发事件对应的参数。
    /// </summary>
    public abstract class MetadataResolveEventArgs : EventArgs {

        /// <summary>
        /// 创建 MetadataResolveEventArgs 实例。
        /// </summary>
        /// <param name="name">指定检索不到的元数据名称。</param>
        protected MetadataResolveEventArgs(string name) {
            this._name = name;
        }
        
        private readonly string _name;
        /// <summary>
        /// 返回未找到信息的名称
        /// </summary>
        public virtual string Name {
            get { return _name; }
        }

        /// <summary>
        /// 返回要检索的元数据类型。
        /// </summary>
        public abstract Type MetadataType {
            get;
        }

    }
}
