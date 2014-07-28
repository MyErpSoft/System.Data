using System.Collections.Generic;
using System.Threading;

namespace System.Data.DataEntities.Metadata.Dynamic {
    /// <summary>
    /// Dynamic entity metadata base class
    /// </summary>
    public abstract class DynamicMemberMetadata : IMemberMetadata {
        private static int _currentToken;

        /// <summary>
        /// A derived class calls this constructor
        /// </summary>
        protected DynamicMemberMetadata() {
            _metadataToken = Interlocked.Increment(ref _currentToken);
        }

        /// <summary>
        /// Returns the distinguished name of this metadata object.
        /// </summary>
        public abstract string Name {
            get;
        }

        #region Attribute handling
        /// <summary>
        /// Returns an empty array of Attribute.
        /// </summary>
        internal static readonly object[] EmptyAttributes = new object[0];

        /// <summary>
        /// Returns all the custom attributes defined on this type of arrays, or if there are no custom attributes, return an empty array.
        /// </summary>
        /// <param name="inherit">When true, and find inherited custom attribute hierarchy chain.</param>
        /// <returns>Represents a custom property of an array of objects, or an empty array.</returns>
        public abstract object[] GetCustomAttributes(bool inherit);

        /// <summary>
        /// Returns an array of custom attributes defined on this member (identified by type), if no custom attributes of the type, an empty array is returned.
        /// </summary>
        /// <param name="attributeType">Type of the custom attribute.</param>
        /// <param name="inherit">When true, and find inherited custom attribute hierarchy chain.</param>
        /// <returns>Represents a custom property of an array of objects, or an empty array.</returns>
        public object[] GetCustomAttributes(Type attributeType, bool inherit) {
            if (null == attributeType) {
                throw new ArgumentNullException("attributeType");
            }

            List<object> list = null;
            foreach (var att in this.GetCustomAttributes(inherit)) {
                if ((null != att) && (attributeType.IsInstanceOfType(att))) {
                    if (list == null) {
                        list = new List<object>();
                    }
                    list.Add(att);
                }
            }

            if (list == null) {
                return EmptyAttributes;
            }
            return list.ToArray();
        }

        /// <summary>
        /// Indicates whether this member is defined on one or more than one attribute of type instances.
        /// </summary>
        /// <param name="attributeType">Type of the custom attribute.</param>
        /// <param name="inherit">When true, and find inherited custom attribute hierarchy chain.</param>
        /// <returns>If on the members of this attribute type is defined, is true, otherwise false.</returns>
        public bool IsDefined(Type attributeType, bool inherit) {
            if (null == attributeType) {
                throw new ArgumentNullException("attributeType");
            }

            foreach (var att in this.GetCustomAttributes(inherit)) {
                if ((null != att) && (attributeType.IsInstanceOfType(att))) {
                    return true;
                }
            }
            return false;
        }
        #endregion

        private readonly int _metadataToken;
        /// <summary>
        /// Gets a value that identifies a metadata element.
        /// </summary>
        public int MetadataToken {
            get { return _metadataToken; }
        }

        #region Equal treatment
        /// <summary>
        /// When the two metadata Token equal, then there is equal.
        /// </summary>
        /// <param name="obj">The objects to be judged equal</param>
        /// <returns>When the two metadata Token equal, then return true</returns>
        public override bool Equals(object obj) {
            DynamicMemberMetadata other = obj as DynamicMemberMetadata;
            return (other == null) ? false : other.MetadataToken == this.MetadataToken;
        }

        /// <summary>
        /// A Hash of Token code
        /// </summary>
        /// <returns>Hash code values.</returns>
        public override int GetHashCode() {
            return this._metadataToken;
        }

        /// <summary>
        /// Returns the metadata for the name and number.
        /// </summary>
        /// <returns>Metadata for the name and number.</returns>
        public override string ToString() {
            return this.Name + ":" + this._metadataToken.ToString();
        }
        #endregion
    }
}
