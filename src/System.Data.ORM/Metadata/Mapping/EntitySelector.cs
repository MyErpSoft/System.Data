using System.Collections.ObjectModel;

namespace System.Data.Metadata.Mapping {

    /// <summary>
    /// 描述了一个实体对应到某张表的映射信息。
    /// </summary>
    internal abstract class EntitySelector {

        public EntitySelector() {
            _propertyFieldMaps = new Collection<PropertyFieldPair>();
        }

        /// <summary>
        /// 获取或初始化当前的行，创建或获取一个Entity并存放到EntityRow。
        /// </summary>
        /// <param name="values">与之关联的行所有数据。</param>
        /// <param name="entity">如果创建或定位了一个实体，返回他</param>
        /// <returns>如果对象是创建的，返回true，否则返回false</returns>
        /// <remarks>
        /// <para>例如在一个明细行中，此方法可能创建一个明细实体，并将这个明细实体关联到主实体的某个集合属性下。</para>
        /// </remarks>
        public abstract bool TryCreateEntity(object[] values, out object entity);
        
        private Collection<PropertyFieldPair> _propertyFieldMaps;
        /// <summary>
        /// 返回所有属性的映射集合。
        /// </summary>
        public Collection<PropertyFieldPair> PropertyFieldMaps {
            get { return _propertyFieldMaps; }
        }

        internal PropertyFieldPair[] GetPropertyMappers() {
            PropertyFieldPair[] values = new PropertyFieldPair[_propertyFieldMaps.Count];
            _propertyFieldMaps.CopyTo(values, 0);

            return values;
        }
    }
}
