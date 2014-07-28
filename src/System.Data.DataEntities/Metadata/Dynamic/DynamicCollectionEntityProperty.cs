using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.DataEntities.Dynamic;

namespace System.Data.DataEntities.Metadata.Dynamic
{

    internal sealed class DynamicCollectionEntityProperty : DynamicEntityProperty ,ICollectionEntityProperty
    {
        internal DynamicCollectionEntityProperty(
            string name,
            IEntityType itemPropertyType,
            object[] attributes)
            : base(name, typeof(IList<DynamicEntity>),true,
            null, attributes)
        {
            this._itemPropertyType = itemPropertyType;
        }

        private IEntityType _itemPropertyType;
        public IEntityType ItemPropertyType
        {
            get { return _itemPropertyType; }
        }

        protected override object LazyCreateValue(DynamicEntity entity)
        {
            return new EntityCollection<DynamicEntity>(entity);
        }
    }
}
