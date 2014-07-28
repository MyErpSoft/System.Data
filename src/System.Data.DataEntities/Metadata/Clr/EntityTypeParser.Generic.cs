using System.Collections.Generic;
using System.ComponentModel;

namespace System.Data.DataEntities.Metadata.Clr {

    internal abstract class EntityTypeParser<T> : EntityTypeParser {
        public override bool TryParse(Type clrType, EntityType entityType) {
            T[] members;
            if (base.TryParse(clrType, entityType) && TryGetMembers(out members)) {
                List<EntityProperty> properties;
                ParseBaseType(members, out properties);

                EntityProperty entityProperty;
                foreach (var member in members) {
                    if (TryConvertToEntityProperty(member, out entityProperty)) {
                        properties.Add(entityProperty);
                    }
                }

                entityType._properties = new EntityPropertyCollection(properties);
                return true;
            }
            else {
                return false;
            }
        }

        protected abstract bool TryGetMembers(out T[] members);

        private bool TryConvertToEntityProperty(T member, out EntityProperty entityProperty) {
            if (this.Match(member)) {
                Type propertyType = this.GetPropertyType(member);
                if (TryConvertToSimpleProperty(member, propertyType, out entityProperty) ||
                    TryConvertToCollectionProperty(member, propertyType, out entityProperty) ||
                    TryConvertToComplexProperty(member, propertyType, out entityProperty)) {
                    return true;
                }
            }

            entityProperty = null;
            return false;
        }

        private bool TryConvertToComplexProperty(T member, Type propertyType, out EntityProperty entityProperty) {
            //TODO:throw new NotImplementedException
            entityProperty = null;
            return false;
        }

        private static readonly Type IListType = typeof(System.Collections.IList);
        private static readonly Type ICollectionGenericType = typeof(ICollection<>);
        private bool TryConvertToCollectionProperty(T member, Type propertyType, out EntityProperty entityProperty) {
            if (ICollectionGenericType.IsAssignableFrom(propertyType) ||
                IListType.IsAssignableFrom(propertyType)) {
                entityProperty = this.CreateCollectionProperty(member);
                return true;
            }

            entityProperty = null;
            return false;
        }

        /// <summary>
        /// Create CollectionEntityProperty from CLR member.
        /// </summary>
        /// <param name="member">CLR members, such as properties, fields.</param>
        /// <returns>a CollectionEntityProperty instance.</returns>
        protected abstract CollectionEntityProperty CreateCollectionProperty(T member);

        private bool TryConvertToSimpleProperty(T member, Type propertyType, out EntityProperty entityProperty) {
            //When a property is a value type, string, or can be converted to a string.
            if (propertyType.IsValueType ||
                propertyType == StringType ||
                CanConvertFromString(member)) {
                entityProperty = CreateSimpleProperty(member);

                if ((this.EntityType._primaryKey == null) && IsPrimaryKey(member, entityProperty)) {
                    this.EntityType._primaryKey = (SimpleEntityProperty)entityProperty;
                }
                return true;
            }

            entityProperty = null;
            return false;
        }

        protected abstract bool IsPrimaryKey(T member, EntityProperty entityProperty);

        /// <summary>
        /// Create SimpleEntityProperty from CLR member.
        /// </summary>
        /// <param name="member">CLR members, such as properties, fields.</param>
        /// <returns>a SimpleEntityProperty instance.</returns>
        protected abstract SimpleEntityProperty CreateSimpleProperty(T member);

        protected abstract bool CanConvertFromString(T member);

        private void ParseBaseType(T[] members, out List<EntityProperty> properties) {
            //Get base type.
            var baseType = ClrType.BaseType;
            if ((baseType != null) && (baseType != typeof(object))) {
                EntityType baseEntityType = EntityType.GetEntityType(baseType);
                properties = new List<EntityProperty>(baseEntityType.Properties.Count + members.Length);
                properties.AddRange(baseEntityType._properties);
                this.EntityType._baseType = baseEntityType;
                this.EntityType._primaryKey = baseEntityType._primaryKey;
            }
            else {
                properties = new List<EntityProperty>(members.Length);
            }
        }

        protected abstract bool Match(T member);

        protected abstract Type GetPropertyType(T member);
    }
}
