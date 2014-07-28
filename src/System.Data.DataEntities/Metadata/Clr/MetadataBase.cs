using System.Reflection;

namespace System.Data.DataEntities.Metadata.Clr {

    internal abstract class MemberMetadataBase<T> : IMemberMetadata where T : MemberInfo {
        /// <summary>
        /// Return CLR member.
        /// </summary>
        protected readonly T ClrMember;

        /// <summary>
        /// Create MemberMetadataBase instance.
        /// </summary>
        /// <param name="memberInfo">CLR member object.</param>
        protected MemberMetadataBase(T memberInfo) {
            if (null == memberInfo) {
                OrmUtility.ThrowArgumentNullException("memberInfo");
            }

            this.ClrMember = memberInfo;
        }

        public string Name {
            get { return ClrMember.Name; }
        }

        public int MetadataToken {
            get { return ClrMember.MetadataToken; }
        }

        public object[] GetCustomAttributes(bool inherit) {
            return ClrMember.GetCustomAttributes(inherit);
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return ClrMember.GetCustomAttributes(attributeType, inherit);
        }

        public bool IsDefined(Type attributeType, bool inherit) {
            return ClrMember.IsDefined(attributeType, inherit);
        }

        #region Equals
        /// <summary>
        /// Override Equals,return true if ClrMember equals
        /// </summary>
        public override bool Equals(object obj) {
            MemberMetadataBase<T> other = obj as MemberMetadataBase<T>;
            if (other != null) {
                return object.Equals(other.ClrMember, this.ClrMember);
            }
            return false;
        }

        public override int GetHashCode() {
            return this.ClrMember.GetHashCode();
        }

        public override string ToString() {
            return this.ClrMember.ToString();
        }
        #endregion
    }
}
