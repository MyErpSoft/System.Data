
namespace System.Data.Metadata.DataEntities.Clr {
    /// <summary>
    /// 描述了从CLR Type转换为EntityType的方法。注：现在还没有打算公开EntityType，所以此方法也不公开
    /// </summary>
    internal class EntityTypeParser {
        #region Providers
        private static EntityTypeParser[] _parsers =
            {
            new EntityTypeParser()
        };
        private static readonly object _lock = new object();

        /// <summary>
        /// 注册一个第三方的Parser.注：现在还没有打算公开EntityType，所以此方法也不公开
        /// </summary>
        /// <param name="parser">新的实例</param>
        public static void Register(EntityTypeParser parser) {
            if (parser == null) {
                OrmUtility.ThrowArgumentNullException("parser");
            }

            lock (_lock) {
                //不能同时写入，否则检查不准确
                foreach (var item in _parsers) {
                    if (object.Equals(item,parser)) {
                        return;
                    }
                }

                EntityTypeParser[] newArray = new EntityTypeParser[_parsers.Length + 1];
                _parsers.CopyTo(newArray, 0);
                newArray[_parsers.Length] = parser;

                _parsers = newArray;
            }
        }

        public static EntityType Parse(Type clrType) {
            var parsers = _parsers;
            EntityType et;
            foreach (var item in parsers) {
                if (item.TryParse(clrType, out et)) {
                    return et;
                }
            }

            throw new NotSupportedException();
        }
        #endregion

        public virtual bool TryParse(Type clrType, out EntityType entityType) {
            entityType = new EntityType(clrType);
            return true;
        }

    }
}
