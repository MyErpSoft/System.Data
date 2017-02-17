using System.Collections;
using System.Data.Metadata.Mapping;

namespace System.Data.ORM {
    /* 
     * 整个实体的读取包含几个步骤：
     * 1、组织并执行SQL，返回IDataReader；
     *    EntityReader不负责数据库SQL语句的查询，有利于职责的分离；
     * 2、EntityReader根据数据库字段与实体属性的映射表，恰当的创建实体并填充数据；
     *    外部传入IDataReader和本次读取需要的映射表，在IDataReader的每行数据时，创建对应的实体，并填充属性。
     *    EntityReader被设计成每个Select输出对应一个实例，有两个原因：
     *      1、在诸如主表和明细表时，允许两个Reader并行读取；
     *      2、可以实现非常大的结果集以流的方式输出，而不是一次性读取到内存。
     *    
     * 3、重置实体的状态。
     */

    /// <summary>
    /// 读取实体的读取器。
    /// </summary>
    internal sealed class EntityReader {

        public EntityReader(IDataReader reader,EntitySelector[] entitySelectors) {
            #region 参数检查
            if (reader == null) {
                OrmUtility.ThrowArgumentNullException("reader");
            }
            if (entitySelectors == null || entitySelectors.Length == 0) {
                OrmUtility.ThrowArgumentNullException("entitySelector");
            }
            #endregion

            this._reader = reader;

            _entitySelectors = entitySelectors;
            _propertyFieldPairs = new PropertyFieldPair[_entitySelectors.Length][];
            for (int i = 0; i < _entitySelectors.Length; i++) {
                _propertyFieldPairs[i] = _entitySelectors[i].GetPropertyMappers();
            }

            _values = new object[_reader.FieldCount];
            _entities = new object[_entitySelectors.Length];
            _entitySelectorsLength = _entitySelectors.Length;
        }
        
        //这里放在类变量是有讲究的，当循环填充每行数据时，我们不希望创建太多的对象，会造成GC的压力，
        //第二，CPU与内存具有缓存机制，使用同一个变量有利于命中缓存。
        private object[] _values;

        //从数据库读取出来的每行记录，对应可能创建多个实体，例如 1...0.1的关系，会创建一个复杂属性。
        private object[] _entities;

        private readonly IDataReader _reader;
        private readonly EntitySelector[] _entitySelectors;
        private readonly PropertyFieldPair[][] _propertyFieldPairs;

        private object _entity;
        private int _entitySelectorsLength;
        private PropertyFieldPair[] _propertyFieldMaps;
        private EntitySelector _entityTableMapper;

        /// <summary>
        /// 读取一笔记录，如果返回的是null，表示没有记录了。
        /// </summary>
        /// <returns>如果正确读取到一笔记录，将返回此对象。</returns>
        public object Read() {
            if (_reader.Read()) {
                _reader.GetValues(_values);

                // 填充一整行的所有数据。
                //一个表或Select输出，会转换为多个实体，每个实体对应独立的映射定义。
                for (int i = 0; i < _entitySelectorsLength; i++) {
                    _entityTableMapper = _entitySelectors[i];
                    //创建/或搜索一个实体。只有新创建的对象才会填充数据。
                    if (_entityTableMapper.TryCreateEntity(_values, out _entity)) {
                        // 将一批指定的数据填充到实体，依据是一个映射定义。
                        _propertyFieldMaps = _propertyFieldPairs[i];

                        //循环所有的映射定义，填充值即可。考虑的问题越简单，效率越高，越灵活。
                        //如果一个实体在多个表中取值，可以在组织SQL时将数据组织到一个Select结果，或者建立多个EntityTableMapper实例；
                        //同理，如果一个表有多个实体的数据，也可以建立多个EntityTableMapper实例，而不是让EntityTableMapper变得结构复杂。
                        object value;
                        foreach (var mapper in _propertyFieldMaps) {
                            value = _values[mapper.FieldIndex];
                            if (DBNull.Value != value && null != value) {
                                //数据转换的工作被放在前置reader处理了，传入的values已经转换。这是因为不同的数据库输出需要不同的转换，例如
                                //Oracle没有int，输出的都是小数，所以Oracle驱动要自己转换，而SQLServer不存在此问题。
                                mapper.Property.SetValue(_entity, value);
                            }
                        }
                    }

                    //无论是新创建的对象，还是已有的对象，都放入实体集合。
                    _entities[i] = _entity;
                }

                return _entities[0];
            }

            return null;
        }
    }//end class
}//end namespace
