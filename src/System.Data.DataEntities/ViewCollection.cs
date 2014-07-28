using System.Collections.Generic;

namespace System.Data.DataEntities{
    
    /// <summary>
    /// Simple view collection, used to position the element into view
    /// </summary>
    /// <typeparam name="TView">The type of view</typeparam>
    /// <typeparam name="TItem">The element type.</typeparam>
    public abstract class ViewCollection<TView,TItem> : IList<TView> {

        /// <summary>
        /// Create ViewCollection instance.
        /// </summary>
        /// <param name="items">The wrapped collection.</param>
        public ViewCollection(IList<TItem> items) {
            if (items == null) {
                OrmUtility.ThrowArgumentNullException("items");
            }
            this._items = items;
        }

        private readonly IList<TItem> _items;
        /// <summary>
        /// Returns a collection of Interior.
        /// </summary>
        protected IList<TItem> Items {
            get { return this._items; }
        }

        /// <summary>
        /// Returns a view instance.
        /// </summary>
        /// <param name="item">The wrapped element.</param>
        /// <returns>New view instance.</returns>
        protected abstract TView CreateView(TItem item);

        /// <summary>
        /// Extract elements from view.
        /// </summary>
        /// <param name="view">View of the data to be extracted.</param>
        /// <returns>View inside the element.</returns>
        protected abstract TItem GetItem(TView view);

        /// <summary>
        /// Returns the element at the specified index of the collection view.
        /// </summary>
        /// <param name="index">The location specified.</param>
        /// <returns>Specifies the location of the view</returns>
        public TView this[int index] {
            get { return this.CreateView(this._items[index]); }
            set { this._items[index] = this.GetItem(value); }
        }

        /// <summary>
        /// Returns the size of the collection.
        /// </summary>
        public int Count {
            get { return this._items.Count; }
        }

        #region IEnumerable<TView> 
        /// <summary>
        /// Returns an enumerator for all views.
        /// </summary>
        /// <returns>An enumerator class.</returns>
        public IEnumerator<TView> GetEnumerator() {
            foreach (var item in this._items) {
                yield return this.CreateView(item);
            }
        }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion

        #region IList<TView> 
        /// <summary>
        /// Determines the index of a specific item in the View collection.
        /// </summary>
        /// <param name="view">To locate in the collection view.</param>
        /// <returns>View the object pointed to by its internal collection at the de position.</returns>
        public int IndexOf(TView view) {
            if (view == null) {
                OrmUtility.ThrowArgumentNullException("view");
            }
            return this._items.IndexOf(this.GetItem(view));
        }

        /// <summary>
        /// Inserts an item to the Items in the view at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index, insert item at that location.</param>
        /// <param name="view">To View the view in the collection you want to insert, and was actually inserted inside view entries.</param>
        public void Insert(int index, TView view) {
            if (view == null) {
                OrmUtility.ThrowArgumentNullException("view");
            }

            this._items.Insert(index, this.GetItem(view));
        }

        /// <summary>
        /// Adding items to the Items directly.
        /// </summary>
        /// <param name="index">The insertion of items to add to this location.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int index, TItem item) {
            this._items.Insert(index, item);
        }

        /// <summary>
        /// Remove the Items at the specified indices.
        /// </summary>
        /// <param name="index">The zero-based index of the item that has been removed.</param>
        public void RemoveAt(int index) {
            this._items.RemoveAt(index);
        }

        #endregion

        #region ICollection<TView> 
        /// <summary>
        /// Hold the view item to the Items.
        /// </summary>
        /// <param name="view">Items that holds the actual data</param>
        public void Add(TView view) {
            if (view == null) {
                OrmUtility.ThrowArgumentNullException("view");
            }

            this._items.Add(this.GetItem(view));
        }

        /// <summary>
        /// Add items directly to the Items in the collection.
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(TItem item) {
            this._items.Add(item);
        }

        /// <summary>
        /// Clears all items in the Items collection.
        /// </summary>
        public void Clear() {
            this._items.Clear();
        }

        /// <summary>
        /// Test view holds the position of an item in the Items.
        /// </summary>
        /// <param name="view">View objects.</param>
        /// <returns>Returns the view held by the position of an item in the Items, if it is not found,-1 is returned</returns>
        public bool Contains(TView view) {
            if (view == null) {
                OrmUtility.ThrowArgumentNullException("view");
            }

            return this._items.Contains(this.GetItem(view));
        }

        /// <summary>
        /// Copy all views into an array.
        /// </summary>
        /// <param name="array">To host an array of views.</param>
        /// <param name="arrayIndex">Will be copied to the start position of the array.</param>
        public void CopyTo(TView[] array, int arrayIndex) {
            if (array == null) {
                OrmUtility.ThrowArgumentNullException("array");
            }

            for (int i = 0; i < this._items.Count; i++) {
                array[i + arrayIndex] = this.CreateView(this._items[i]);
            }
        }

        /// <summary>
        /// Returns whether the Items are read-only.
        /// </summary>
        public bool IsReadOnly {
            get { return this._items.IsReadOnly; }
        }

        /// <summary>
        /// Deleting Items in the view
        /// </summary>
        /// <param name="view">View.</param>
        /// <returns>Returns true if successfully removed, false otherwise.</returns>
        public bool Remove(TView view) {
            if (view == null) {
                OrmUtility.ThrowArgumentNullException("view");
            }

            return this._items.Remove(this.GetItem(view));
        }

        #endregion
    }
}
