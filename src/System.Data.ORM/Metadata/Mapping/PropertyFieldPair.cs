using System.Data.Metadata.DataEntities;
using System.Diagnostics.Contracts;

namespace System.Data.Metadata.Mapping {

    /// <summary>
    /// 一个数据库字段与实体属性的映射。
    /// </summary>
    internal struct PropertyFieldPair {

        /// <summary>
        /// 返回此字段在SQL输出中的位置，这将加快数据库的值填充到属性上。
        /// </summary>
        public readonly int FieldIndex;

        /// <summary>
        /// 返回此映射中，属性的部分。
        /// </summary>
        public readonly IValueAccessor Property;

        /// <summary>
        /// 创建实例，必须传入有效的映射。
        /// </summary>
        /// <param name="fieldIndex">字段在SQL输出中的位置</param>
        /// <param name="property">映射中的属性</param>
        public PropertyFieldPair(int fieldIndex, IValueAccessor property) {
            Contract.Requires(fieldIndex >= 0);
            Contract.Requires(property != null);

            this.FieldIndex = fieldIndex;
            this.Property = property;
        }
    }
}
