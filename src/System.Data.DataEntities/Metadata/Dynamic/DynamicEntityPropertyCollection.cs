using System.Collections.Generic;
using System.Collections;

namespace System.Data.DataEntities.Metadata.Dynamic
{
    /// <summary>
    /// DynamicEntityProperty collection.
    /// </summary>
    public sealed class DynamicEntityPropertyCollection : 
        MetadataReadOnlyCollection<DynamicEntityProperty>,IEntityPropertyCollection
    {
        private readonly DynamicEntityType _reflectedType;

        internal DynamicEntityPropertyCollection(DynamicEntityType reflectedType)
            :base(null)
        {
            _reflectedType = reflectedType;
        }

        internal DynamicEntityPropertyCollection(DynamicEntityType reflectedType,IEnumerable<DynamicEntityProperty> properties)
            :base(properties)
        {
            _reflectedType = reflectedType;
            ResetOrdinal(0);
        }

        /// <summary>
        /// When internal add element, update its index.
        /// </summary>
        /// <param name="item">add a element.</param>
        internal override void Add(DynamicEntityProperty item)
        {
            base.Add(item);
            item.Ordinal = this.Items.Length - 1;
            item.ReflectedType = _reflectedType;
        }

        /// <summary>
        /// When internal add element, update its index.
        /// </summary>
        /// <param name="items">add a elements.</param>
        internal override void AddRange(IEnumerable<DynamicEntityProperty> items)
        {
            int startIndex = this.Items.Length;
            base.AddRange(items);
            ResetOrdinal(startIndex);
        }

        private void ResetOrdinal(int startIndex)
        {
            var baseItems = this.Items;
            DynamicEntityProperty item;
            for (int i = startIndex; i < baseItems.Length; i++)
            {
                item = baseItems[i];
                item.Ordinal = i;
                item.ReflectedType = _reflectedType;
            }
        }

        #region IEntityPropertyCollection

        bool IMetadataReadOnlyCollection<IEntityProperty>.Contains(IEntityProperty item)
        {
            return this.Contains(item as DynamicEntityProperty);
        }

        int IMetadataReadOnlyCollection<IEntityProperty>.IndexOf(IEntityProperty item)
        {
            return this.IndexOf(item as DynamicEntityProperty);
        }

        void IMetadataReadOnlyCollection<IEntityProperty>.CopyTo(IEntityProperty[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        IEntityProperty IMetadataReadOnlyCollection<IEntityProperty>.this[int index]
        {
            get { return this[index]; }
        }

        IEntityProperty IMetadataReadOnlyCollection<IEntityProperty>.this[string name]
        {
            get { return this[name]; }
        }

        bool IMetadataReadOnlyCollection<IEntityProperty>.TryGetValue(string name, out IEntityProperty value)
        {
            DynamicEntityProperty property;
            if (this.TryGetValue(name,out property))
            {
                value = property;
                return true;
            }
            value = null;
            return false;
        }

        IEnumerator<IEntityProperty> IEnumerable<IEntityProperty>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
