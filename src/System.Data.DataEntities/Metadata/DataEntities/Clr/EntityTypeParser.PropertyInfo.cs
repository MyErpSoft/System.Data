//using System.ComponentModel;
//using System.Reflection;

//namespace System.Data.DataEntities.Metadata.Clr
//{
//    internal abstract class EntityTypeParserForPropertyInfo : EntityTypeParser<PropertyInfo>
//    {
//        private PropertyDescriptorCollection _propertyDescriptors;

//        protected override bool TryGetMembers(out PropertyInfo[] members)
//        {
//            members = this.ClrType.GetProperties(Reflection.BindingFlags.DeclaredOnly |
//                Reflection.BindingFlags.Instance | Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic);
//            _propertyDescriptors = TypeDescriptor.GetProperties(this.ClrType);
//            return true;
//        }

//        protected override Type GetPropertyType(PropertyInfo member)
//        {
//            return member.PropertyType;
//        }

//        protected override bool CanConvertFromString(PropertyInfo propertyInfo)
//        {
//            var pd = _propertyDescriptors[propertyInfo.Name];
//            return (pd != null) && pd.Converter.CanConvertFrom(StringType) && pd.Converter.CanConvertTo(StringType);
//        }

//        protected sealed override SimpleEntityProperty CreateSimpleProperty(PropertyInfo member)
//        { return new SimpleEntityProperty(member); }

//        protected sealed override CollectionEntityProperty CreateCollectionProperty(PropertyInfo member)
//        { return new CollectionEntityProperty(member); }
//    }

//}
