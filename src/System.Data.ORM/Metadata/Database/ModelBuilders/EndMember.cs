using System;
using System.Collections.Generic;
using System.Data.DataEntities.Dynamic;
using System.Data.Metadata.DataEntities.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Metadata.Database.ModelBuilders {

    public class EndMember : DynamicEntity {
        public static readonly DynamicEntityType EndMemberDynamicEntityType = new DynamicEntityType("EndMember");

        public EndMember() : base(EndMemberDynamicEntityType) {
        }

        private string _fromFieldName;
        /// <summary>
        /// 如果是字段关联字段，那么此属性返回左边关联的字段名称，否则为null.
        /// </summary>
        public string FromFieldName {
            get { return this._fromFieldName; }
            set { this._fromFieldName = value; }
        }

        private string _toFieldName;
        /// <summary>
        /// 如果是字段关联字段，那么此属性返回右边关联的字段名称。
        /// </summary>
        public string ToFieldName {
            get { return _toFieldName; }
            set { _toFieldName = value; }
        }

        private object _fromConstant;
        /// <summary>
        /// 如果左边的表使用常量关联，那么此属性返回常量，否则为null。
        /// </summary>
        public object FromConstant {
            get { return _fromConstant; }
            set { _fromConstant = value; }
        }

        private object _toConstant;
        /// <summary>
        /// 如果右边的表使用常量关联，那么此属性返回常量，否则为null。
        /// </summary>
        public object ToConstant {
            get { return _toConstant; }
            set { _toConstant = value; }
        }

        private bool _allowDefaultValue;
        /// <summary>
        /// 在进行内存计算关系时，如果左边的表使用字段关联时，他的值是否允许在空时仍然关联右边。
        /// </summary>
        /// <remarks>
        /// <para>默认值是false，如果左边表的字段值是默认值时，我们假设右边一定找不到记录，不会去关联右边的表。</para>
        /// <para>如果你希望即使是缺省值，仍然去搜索右边的表，则设置为True,仅仅在内存计算时有效。</para>
        /// </remarks>
        public bool AllowDefaultValue {
            get { return _allowDefaultValue; }
            set { _allowDefaultValue = value; }
        }
    }
}
