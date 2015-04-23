using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace System.Data.Metadata.DataEntities {

    /// <summary>
    /// Metadata for a read-only collection
    /// </summary>
    /// <typeparam name="T">Element data type</typeparam>
    [DebuggerTypeProxy(typeof(MetadataReadOnlyCollection_DebugView<>)), DebuggerDisplay("Count = {Count}")]
    public abstract class MetadataReadOnlyCollection<T> : IEnumerable<T>, ICollection<T> {
        private T[] _items;
        private Dictionary<string, T> _dict;
        private static readonly T[] _emptyArray = new T[0];

        /// <summary>
        /// Construct the metadata for a read-only collection
        /// </summary>
        /// <param name="items">To initialize an array</param>
        protected MetadataReadOnlyCollection(IEnumerable<T> items) {
            if (items != null) {
                AddRangePrivate(items, false);
            }
            else {
                _items = _emptyArray;
            }
        }

        /// <summary>
        /// When implemented in a derived class, extracts the name from the specified element.
        /// </summary>
        /// <param name="item">the specified element</param>
        /// <returns>the name from the specified element</returns>
        protected abstract string GetName(T item);

        /// <summary>
        /// A derived class can directly access the internal data
        /// </summary>
        protected T[] Items {
            get { return _items; }
        }

        /// <summary>
        /// Derived classes can have direct access to the internal dictionary.
        /// </summary>
        protected Dictionary<string, T> Dictionary {
            get {
                if (_dict == null) {
                    Threading.Interlocked.CompareExchange<Dictionary<string, T>>(ref _dict, this.CreateDictionary(), null);
                }

                return _dict;
            }
        }

        private Dictionary<string, T> CreateDictionary() {
            var dict = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            if (_items != null) {
                foreach (T item in _items) {
                    string key = GetName(item);
                    if (key != null) {
                        dict.Add(key, item);
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using the default equality comparer.
        /// </summary>
        /// <param name="item">The value to locate in the sequence.</param>
        /// <returns>true if the source sequence contains an element that has the specified value; otherwise, false.</returns>
        public bool Contains(T item) {
            if (item == null) {
                return false;
            }

            T findItem;
            if (this.TryGetValue(GetName(item), out findItem) &&
                object.Equals(findItem, item)) {
                return true;
            }

            return Array.IndexOf<T>(_items, item) >= 0;
        }

        /// <summary>
        /// Determines whether a sequence contains a specified name by using the default equality comparer.
        /// </summary>
        /// <param name="name">The name to locate in the sequence.</param>
        /// <returns>true if the source sequence contains name that has the specified value; otherwise, false.</returns>
        public bool Contains(string name) {
            return Dictionary.ContainsKey(name);
        }

        /// <summary>
        /// Determines the index of a specific item in the IMetadataReadOnlyCollection.
        /// </summary>
        /// <param name="item">The object to locate in the IMetadataReadOnlyCollection</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item) {
            return Array.IndexOf<T>(_items, item);
        }

        /// <summary>
        /// Copies the elements of the IMetadataReadOnlyCollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from IMetadataReadOnlyCollection. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) {
            _items.CopyTo(array, arrayIndex);
        }

        public T[] ToArray() {
            T[] newArray = new T[_items.Length];
            _items.CopyTo(newArray,0);

            return newArray;
        }

        /// <summary>
        /// Gets the number of elements contained in the IMetadataReadOnlyCollection.
        /// </summary>
        public int Count {
            get {
                return _items.Length;
            }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is not a valid index of IMetadataReadOnlyCollection.</exception>
        public T this[int index] {
            get { return _items[index]; }
        }

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">To obtain the distinguished name of the element.</param>
        /// <returns>An element with the specified name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Retrieves the property name was not found.</exception>
        public T this[string name] {
            get { return Dictionary[name]; }
        }

        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <param name="name">To get the name of the value.</param>
        /// <param name="value">When this method returns, if the specified key is found, returns the value associated with the key; otherwise, it returns the type of the value parameter's default value. The parameter is not initialized is passed.</param>
        /// <returns>If the list element contains an element with the specified name, or true; otherwise, false.</returns>
        public bool TryGetValue(string name, out T value) {
            return Dictionary.TryGetValue(name, out value);
        }
        
        private void AddRangePrivate(IEnumerable<T> items,bool buildDictionary) {
            if (null == items) {
                throw new ArgumentNullException("items");
            }

            int count = 0;
            string name;
            foreach (var item in items) {
                if (null == item) {
                    throw new ArgumentNullException("items[" + count.ToString() + "]");
                }

                name = GetName(item);
                if (string.IsNullOrEmpty(name)) {
                    throw new ArgumentNullException("Add an element name cannot be empty.");
                }

                T findItem;
                if ((_dict != null) && (_dict.TryGetValue(name, out findItem))) {
                    throw new ArgumentNullException(
                        string.Format(CultureInfo.CurrentCulture, "The element name {0} already exists in the collection.", name));
                }

                count++;
            }

            T[] newItems;
            int startIndex;
            if (_items == null) {
                newItems = new T[count];
                startIndex = 0;
            }
            else {
                startIndex = _items.Length;
                newItems = new T[count + startIndex];
                _items.CopyTo(newItems, 0);
            }

            if (buildDictionary && _dict == null) {
                _dict = CreateDictionary();
            }

            foreach (var item in items) {
                if (_dict != null) {
                    _dict.Add(GetName(item), item);
                }
                
                newItems[startIndex] = item;
                startIndex++;
            }

            _items = newItems;
        }

        /// <summary>
        /// Allows a derived class to add data to the collection.
        /// </summary>
        /// <param name="items">To add a collection of elements</param>
        internal virtual void AddRange(IEnumerable<T> items) {
            AddRangePrivate(items, true);
        }

        /// <summary>
        /// Allows a derived class to add data.
        /// </summary>
        /// <param name="item">The element to be added.</param>
        internal virtual void Add(T item) {
            if (null == item) {
                throw new ArgumentNullException("item");
            }

            var name = GetName(item);
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentNullException(
                    "Add an element name cannot be empty.");
            }

            T findItem;
            if (this.TryGetValue(name, out findItem)) {
                throw new ArgumentNullException(
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "The element name {0} already exists in the collection.", name));
            }

            Dictionary.Add(name, item);
            T[] newItems = new T[1 + _items.Length];
            _items.CopyTo(newItems, 0);
            newItems[_items.Length] = item;

            _items = newItems;
        }

        #region ICollection<T>
        //Allows only adding data, so the interface method is not supported.
        void ICollection<T>.Add(T item) {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear() {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item) {
            throw new NotSupportedException();
        }

        bool ICollection<T>.IsReadOnly {
            get { return true; }
        }

        public IEnumerator<T> GetEnumerator() {
            return new ArrayEnumerator(_items);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new ArrayEnumerator(_items);
        }

        #endregion

        #region The enumerator
        private struct ArrayEnumerator : IEnumerator<T> {
            private T[] _array;
            private int _endIndex;
            private int _index;
            private T _current;

            internal ArrayEnumerator(T[] array) {
                this._array = array;
                this._index = -1;
                this._endIndex = array.Length;
                this._current = default(T);
            }

            public bool MoveNext() {
                this._index++;
                if (this._index < this._endIndex) {
                    _current = _array[_index];
                    return true;
                }
                return false;
            }

            public void Reset() {
                this._index = -1;
            }

            public T Current {
                get {
                    return _current;
                }
            }

            object IEnumerator.Current {
                get { return _current; }
            }

            public void Dispose() {
                //
            }
        }
        #endregion
    }

    internal sealed class MetadataReadOnlyCollection_DebugView<T> {
        private MetadataReadOnlyCollection<T> collection;

        public MetadataReadOnlyCollection_DebugView(MetadataReadOnlyCollection<T> collection) {
            if (null == collection) {
                OrmUtility.ThrowArgumentNullException("collection");
            }
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items {
            get {
                return this.collection.ToArray(); 
            }
        }
    }
}