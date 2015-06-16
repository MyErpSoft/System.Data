
using System.Collections.Generic;

namespace System.Data.Metadata.Database {

    /// <summary>
    /// 描述关系
    /// </summary>
    public class Relationship : MemberMetadata {

        public Relationship(string name, Table to, EndMember[] endMembers)
            :base(name){
            if (to == null) {
                OrmUtility.ThrowArgumentNullException("to");
            }
            if (endMembers == null || endMembers.Length == 0) {
                OrmUtility.ThrowArgumentNullException("endMembers");
            }

            this._to = to;
            this._endMembers = endMembers;
        }
        
        private readonly Table _to;
        /// <summary>
        /// 返回关系指向的右边的表
        /// </summary>
        public Table To {
            get { return this._to; }
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
