using System.Reflection;

namespace System.Data.Metadata.DataEntities.Clr {

    internal abstract class MemberMetadataBase<T> : IMemberMetadata where T : MemberInfo {
        /// <summary>
        /// Return CLR member.
        /// </summary>
        protected readonly T ClrMapping;

        /// <summary>
        /// Create MemberMetadataBase instance.
        /// </summary>
        /// <param name="memberInfo">CLR member object.</param>
        protected MemberMetadataBase(T memberInfo) {
            if (null == memberInfo) {
                OrmUtility.ThrowArgumentNullException("memberInfo");
            }

            this.ClrMapping = memberInfo;
        }

        public string Name {
            get { return ClrMapping.Name; }
        }

        #region Equals
        /// <summary>
        /// Override Equals,return true if ClrMember equals
        /// </summary>
        public override bool Equals(object obj) {
            MemberMetadataBase<T> other = obj as MemberMetadataBase<T>;
            if (other != null) {
                return object.Equals(other.ClrMapping, this.ClrMapping);
            }
            return false;
        }

        public override int GetHashCode() {
            return this.ClrMapping.GetHashCode();
        }

        public override string ToString() {
            return this.ClrMapping.ToString();
        }
        #endregion
    }
}
