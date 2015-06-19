using System;
using System.Collections.Generic;
using System.Data.Metadata.DataEntities.Clr;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.DataEntities {

    /// <summary>
    /// 包含所有内置数据类型
    /// </summary>
    public static class BuiltInTypes {

        /// <summary>
        /// Int32 类型
        /// </summary>
        public static readonly IEntityType Int32 = EntityType.GetEntityType(typeof(Int32));

        /// <summary>
        /// Boolean 类型
        /// </summary>
        public static readonly IEntityType Boolean = EntityType.GetEntityType(typeof(Boolean));

        /// <summary>
        /// 字符串 类型
        /// </summary>
        public static readonly IEntityType String = EntityType.GetEntityType(typeof(String));

        /// <summary>
        /// 日期 类型
        /// </summary>
        public static readonly IEntityType DateTime = EntityType.GetEntityType(typeof(DateTime));

        /// <summary>
        /// 十进制 类型
        /// </summary>
        public static readonly IEntityType Decimal = EntityType.GetEntityType(typeof(Decimal));

        /// <summary>
        /// Guid 类型
        /// </summary>
        public static readonly IEntityType Guid = EntityType.GetEntityType(typeof(Guid));
    }
}
