using System.Collections;
using System.Collections.Generic;

namespace System.Data.DataEntities.Metadata.Clr
{
    internal sealed class EntityPropertyCollection : MetadataReadOnlyCollection<EntityProperty>,IEntityPropertyCollection
    {
        public EntityPropertyCollection(IEnumerable<EntityProperty> properties)
            : base(properties)
        {}

        #region IEntityPropertyCollection

        bool IMetadataReadOnlyCollection<IEntityProperty>.Contains(IEntityProperty item)
        {
            return this.Contains(item as EntityProperty);
        }

        int IMetadataReadOnlyCollection<IEntityProperty>.IndexOf(IEntityProperty item)
        {
            return this.IndexOf(item as EntityProperty);
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
            EntityProperty property;
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
