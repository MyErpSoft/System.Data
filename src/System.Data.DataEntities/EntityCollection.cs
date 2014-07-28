using System.Collections.ObjectModel;

namespace System.Data.DataEntities {
    
    /// <summary>
    /// Default entity collection class that provides maintenance to the Parent.
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    [Serializable]
    public class EntityCollection<T> : ObservableCollection<T> where T:Entity {

        private Entity _owner;
        /// <summary>
        /// Returns a collection of hosts.
        /// </summary>
        public Entity Parent {
            get { return this._owner; }
        }

        /// <summary>
        /// Creates a new collection and specify the entities hosting
        /// </summary>
        /// <param name="owner">Collection of hosts.</param>
        public EntityCollection(Entity owner) {
            if (owner == null) {
                OrmUtility.ThrowArgumentNullException("owner");
            }
            this._owner = owner;
        }

        /// <summary>
        /// When you insert a element, set the new parent object for the object.
        /// </summary>
        /// <param name="index">The position of an element.</param>
        /// <param name="item">To insert the new element.</param>
        protected override void InsertItem(int index, T item) {
            if (item != null) {
                item.Parent = this._owner;
            }

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Clears all elements.
        /// </summary>
        protected override void ClearItems() {
            foreach (var item in this.Items) {
                if (item != null) {
                    item.Parent = null;
                }
            }
            base.ClearItems();
        }

        /// <summary>
        /// Deletes the specified index element.
        /// </summary>
        /// <param name="index">Index of the element you want to delete.</param>
        protected override void RemoveItem(int index) {
            var item = base.Items[index];
            if (item != null) {
                item.Parent = null;
            }
            base.RemoveItem(index);
        }

        /// <summary>
        /// When the replacement at the specified index of the element, set the Parent property.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        /// <param name="item">The element on which you want to set.</param>
        protected override void SetItem(int index, T item) {
            var oldItem = base.Items[index];
            if (oldItem != null) {
                oldItem.Parent = null;
            }

            base.SetItem(index, item);

            if (item != null) {
                item.Parent = this._owner;
            }
        }
    }
}
