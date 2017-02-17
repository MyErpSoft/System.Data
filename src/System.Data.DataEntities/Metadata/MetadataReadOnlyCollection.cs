using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace System.Data.Metadata.DataEntities {

    /// <summary>
    /// Metadata for a read-only collection
    /// </summary>
    /// <typeparam name="T">Element data type</typeparam>
    public abstract class MetadataReadOnlyCollection<T> : ReadOnlyKeyedCollection<string,T> {

        /// <summary>
        /// Construct the metadata for a read-only collection
        /// </summary>
        /// <param name="items">To initialize an array</param>
        protected MetadataReadOnlyCollection(IEnumerable<T> items):base(items) {
        }
        
        /// <summary>
        /// Allows a derived class to add data to the collection.
        /// </summary>
        /// <param name="items">To add a collection of elements</param>
        internal void AddRange(IEnumerable<T> items) {
            InsertItems(items);
        }

        /// <summary>
        /// Allows a derived class to add data.
        /// </summary>
        /// <param name="item">The element to be added.</param>
        internal void Add(T item) {
            InsertItem(item);
        }
    }
}