
namespace System.Data.Metadata.Edm {

    /// <summary>
    /// 一个元数据对象的名称
    /// </summary>
    public struct MetadataName {

        public static MetadataName Create(string fullName) {
            if (string.IsNullOrEmpty(fullName)) {
                return new MetadataName();
            }

            var lastIndex = fullName.LastIndexOf('.');
            if (lastIndex < 0) {
                return new MetadataName(fullName, null);
            }

            var len = fullName.Length;
            return new MetadataName(fullName.Substring(0, len - lastIndex), fullName.Substring(len - lastIndex));
        }

        public static MetadataName Create(string name, string strNamespace) {
            return new MetadataName(string.Intern(name), string.Intern(strNamespace));
        }

        public static MetadataName CreateNotIntern(string name, string strNamespace) {
            return new MetadataName(name,strNamespace);
        }

        private MetadataName(string name,string strNamespace) {
            this.Name = name;
            this.Namespace = strNamespace;
        }

        /// <summary>
        /// 返回命名空间
        /// </summary>
        public readonly string Namespace;

        /// <summary>
        /// 返回 模型对象的名称。
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 返回/设置 模型对象的全名称。
        /// </summary>
        public string FullName {
            get {
                return string.IsNullOrEmpty(this.Namespace) ? this.Name : this.Namespace + "." + this.Name;
            }
        }

        #region Debug
        /// <summary>
        /// 返回此对象的名称。
        /// </summary>
        /// <returns>此对象的名称</returns>
        public override string ToString() {
            return this.Name;
        }
        #endregion

        #region 相等
        /// <summary>
        /// 检测相等
        /// </summary>
        /// <param name="obj">要判断的对象</param>
        /// <returns>如果所有字段相等则返回true，否则返回false</returns>
        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }
            
            if (obj is MetadataName) {
                return Equals(this,(MetadataName)obj);
            }

            return false;
        }

        /// <summary>
        /// 判断两个名称是否相等。
        /// </summary>
        /// <param name="a">要判断的对象1</param>
        /// <param name="b">要判断的对象2</param>
        /// <returns>如果Name和Namespace相等返回true，否则返回false</returns>
        public static bool Equals(MetadataName a,MetadataName b) {
            return a.Name == b.Name && a.Namespace == b.Namespace;
        }

        /// <summary>
        /// 判断两个名称是否相等。
        /// </summary>
        /// <param name="a">要判断的对象1</param>
        /// <param name="b">要判断的对象2</param>
        /// <returns>如果Name和Namespace相等返回true，否则返回false</returns>
        public static bool operator ==(MetadataName a, MetadataName b) {
            return Equals(a, b);
        }

        /// <summary>
        /// 判断两个名称是否不相等。
        /// </summary>
        /// <param name="a">要判断的对象1</param>
        /// <param name="b">要判断的对象2</param>
        /// <returns>如果Name和Namespace相等返回false，否则返回true</returns>
        public static bool operator !=(MetadataName a, MetadataName b) {
            return !(Equals(a, b));
        }

        /// <summary>
        /// 返回对象的自定义hashCode
        /// </summary>
        /// <returns>通过Name和Namespace计算出来的值</returns>
        public override int GetHashCode() {
            var code = this.Name == null ? 0 : this.Name.GetHashCode();
            return this.Namespace == null ? code : code ^ this.Namespace.GetHashCode();
        }
        #endregion
        
    }
}
