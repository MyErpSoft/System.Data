using System.Threading;

namespace System.Data.DataEntities.Metadata.Dynamic {

    /// <summary>
    /// Dynamic entity metadata base class
    /// </summary>
    public abstract class DynamicMemberMetadata {
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

        private int _metadataToken;
        /// <summary>
        /// Gets a value that identifies a metadata element.
        /// </summary>
        public int MetadataToken {
            get { return _metadataToken; }
        }

        /// <summary>
        /// Reset metdata token when metadata changed
        /// </summary>
        protected virtual void OnMetadataChanged() {
            this._metadataToken = Interlocked.Increment(ref _currentToken);
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
            return this.Name;
        }
        #endregion
    }
}
