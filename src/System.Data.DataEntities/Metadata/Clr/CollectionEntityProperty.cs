using System.Reflection;

namespace System.Data.DataEntities.Metadata.Clr
{
    internal sealed class CollectionEntityProperty : EntityProperty,ICollectionEntityProperty
    {
        public CollectionEntityProperty(PropertyInfo property)
            :base(property)
        {
            var itemProperty = GetTypedIndexer(property.PropertyType);
            if (itemProperty == null)
            {
                throw new NotSupportedException("Unable to parse the element type of the collection. Please provide the basic properties of the index visit.");
            }
            _itemPropertyType = EntityType.GetEntityType(itemProperty.PropertyType);
        }

        private readonly IEntityType _itemPropertyType;
        public IEntityType ItemPropertyType
        {
            get { return _itemPropertyType; }
        }

        /// <summary>
        /// By analyzing the types of information collection, access to the type of information of its elements
        /// </summary>
        private static PropertyInfo GetTypedIndexer(Type type)
        {
            PropertyInfo info;
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < properties.Length; i++)
            {
                info = properties[i];
                if ((info.Name == "Item") && 
                    (info.PropertyType != typeof(object)) && 
                    (info.GetIndexParameters().Length > 0))
                {
                    return info;
                }
            }
            return null;
        }
    }
}
