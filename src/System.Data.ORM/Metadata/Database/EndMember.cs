namespace System.Data.Metadata.Database {

    /// <summary>
    /// 关系中描述一个关系对，例如 SalesOrder.CustomerId = Customers.Id
    /// </summary>
    public class EndMember {

        public EndMember(Field fromField, Field toField, object fromConstant, object toConstant) {
            if (fromField != null && fromConstant != null) {
                //From不能 字段或常量都指定。
                OrmUtility.ThrowArgumentException(Properties.Resources.EndMemberError1);
            }
            if (toField != null && toConstant != null) {
                OrmUtility.ThrowArgumentException(Properties.Resources.EndMemberError1);
            }
            if (fromField == null && toField == null) {
                //不能两边都是常量。
                OrmUtility.ThrowArgumentException(Properties.Resources.EndMemberError2);
            }

            this._fromField = fromField;
            this._toField = toField;
            this._fromConstant = fromConstant;
            this._toConstant = toConstant;
        }

        private readonly Field _fromField;
        /// <summary>
        /// 如果是字段关联字段，那么此属性返回左边关联的字段，否则为null.
        /// </summary>
        public Field FromField {
            get { return this._fromField; }
        }

        private readonly Field _toField;
        /// <summary>
        /// 如果是字段关联字段，那么此属性返回右边关联的字段，否则为null.
        /// </summary>
        public Field ToField {
            get { return _toField; }
        }

        private readonly object _fromConstant;
        /// <summary>
        /// 如果左边的表使用常量关联，那么此属性返回常量，否则为null。
        /// </summary>
        public object FromConstant {
            get { return _fromConstant; }
        }

        private readonly object _toConstant;
        /// <summary>
        /// 如果右边的表使用常量关联，那么此属性返回常量，否则为null。
        /// </summary>
        public object ToConstant {
            get { return _toConstant; }
        }

        private readonly bool _allowDefaultValue;
        /// <summary>
        /// 在进行内存计算关系时，如果左边的表使用字段关联时，他的值是否允许在空时仍然关联右边。
        /// </summary>
        /// <remarks>
        /// <para>默认值是false，如果左边表的字段值是默认值时，我们假设右边一定找不到记录，不会去关联右边的表。</para>
        /// <para>如果你希望即使是缺省值，仍然去搜索右边的表，则设置为True,仅仅在内存计算时有效。</para>
        /// </remarks>
        public bool AllowDefaultValue {
            get { return _allowDefaultValue; }
        }
    }
}