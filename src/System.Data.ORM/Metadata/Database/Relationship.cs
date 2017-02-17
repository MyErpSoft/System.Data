
using System.Collections.Generic;
using System.Globalization;

namespace System.Data.Metadata.Database {

    /// <summary>
    /// 描述关系
    /// </summary>
    public class Relationship : MemberMetadata {

        public Relationship(string name, Table to, EndMember[] endMembers)
            : base(name) {

            if (to == null) {
                OrmUtility.ThrowArgumentNullException("to");
            }
            this._to = new DictectObjectReference<Table>(to);

            InitEndMembers(endMembers);
            this._endMembers = endMembers;
        }

        public Relationship(string name, string toName, EndMember[] endMembers)
            : base(name) {

            if (string.IsNullOrEmpty(toName)) {
                OrmUtility.ThrowArgumentNullException("toName");
            }
            this._to = new RelationshipToTableReference(toName, this);

            InitEndMembers(endMembers);
            this._endMembers = endMembers;
        }

        private void InitEndMembers(EndMember[] endMembers) {
            if (endMembers == null || endMembers.Length == 0) {
                OrmUtility.ThrowArgumentNullException("endMembers");
            }

            for (int i = 0; i < endMembers.Length; i++) {
                var endMember = endMembers[i];
                if (endMember == null) {
                    OrmUtility.ThrowArgumentNullException("endMembers[" + i.ToString(CultureInfo.CurrentCulture) + "]");
                }

                if (endMember.Relationship != null) {
                    OrmUtility.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.ItemExistedCollection, "endMembers[" + i.ToString(CultureInfo.CurrentCulture) + "]"));
                }
                endMember.Relationship = this;
            }
        }

        private Table _from;
        /// <summary>
        /// 返回关系所在的表对象。
        /// </summary>
        public Table From {
            get { return this._from; }
            internal set { this._from = value; }
        }
        
        private readonly ObjectReference<Table> _to;
        /// <summary>
        /// 返回关系指向的右边的表
        /// </summary>
        public Table To {
            get { return _to.Value; }
        }

        private readonly EndMember[] _endMembers;
        /// <summary>
        /// 返回此关系的连接关系对集合。
        /// </summary>
        public IEnumerable<EndMember> EndMembers {
            get { return (IEnumerable<EndMember>)this._endMembers; }
        }
    }
}
