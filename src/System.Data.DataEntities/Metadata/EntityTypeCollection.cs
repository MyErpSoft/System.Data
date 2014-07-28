using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// Bearing IEntityType of collection classes, IEntityType can be retrieved by name and namespace.
    /// </summary>
    public sealed class EntityTypeCollection : KeyedCollection<string, IEntityType>,
        IMetadataReadOnlyCollection<IEntityType>,
        IList<IEntityType> {
        /// <summary>
        /// IEntityType instances collection is created that is used to store instances of IEntityType.
        /// </summary>
        public EntityTypeCollection()
            : base(StringComparer.Ordinal) {

        }

        /// <summary>
        /// IEntityType instances collection is created that is used to store instances of IEntityType.
        /// </summary>
        /// <param name="comparer">Specifies the name used to retrieve the comparer.</param>
        public EntityTypeCollection(IEqualityComparer<string> comparer)
            : base(comparer) {
        }

        #region Method overloading
        /// <summary>
        /// Insert a IEntityType information.
        /// </summary>
        /// <param name="index">The location of the IEntityType.</param>
        /// <param name="item">IEntityType you want to insert the object cannot be null.</param>
        protected override void InsertItem(int index, IEntityType item) {
            if (null == item) {
                throw new ArgumentNullException("item");
            }

            string currentKey = this.GetKeyForItem(item);
            if (string.IsNullOrEmpty(currentKey)) {
                throw new ArgumentOutOfRangeException("Add the IEntityType whose Full name is empty.");
            }
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Returns the key of the IEntityType, the implementation uses the FullName of the IEntityType.
        /// </summary>
        /// <param name="item">要检测的类型</param>
        /// <returns>The FullName of the IEntityType.</returns>
        protected override string GetKeyForItem(IEntityType item) {
            if (item != null) {
                return item.FullName;
            }
            return string.Empty;
        }
        #endregion

        /// <summary>
        /// By namespace and name, search for an IEntityType.
        /// </summary>
        /// <param name="namespace">IEntityType defined in the namespace for which to search.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="value">Returns the type of search.</param>
        /// <returns>Returns true if found, otherwise returns false.</returns>
        public bool TryGetValue(string @namespace, string typeName, out IEntityType value) {
            var key = (string.IsNullOrEmpty(@namespace) ? typeName : @namespace + "." + typeName);
            return this.TryGetValue(key, out value);
        }

        /// <summary>
        /// By FullName, search for an IEntityType.
        /// </summary>
        /// <param name="name">Search by the IEntityType you want to define the namespace and type name, use segmentation.</param>
        /// <param name="value">Returns the type of search.</param>
        /// <returns>Returns true if found, otherwise returns false.</returns>
        public bool TryGetValue(string name, out IEntityType value) {
            if (string.IsNullOrEmpty(name)) {
                value = null;
                return false;
            }

            var dict = this.Dictionary;

            if (dict == null) {
                for (int i = 0; i < this.Items.Count; i++) {
                    if (this.Comparer.Equals(GetKeyForItem(this.Items[i]), name)) {
                        value = this.Items[i];
                        return true;
                    }
                }
            }
            else {
                if (dict.TryGetValue(name, out value)) {
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}
