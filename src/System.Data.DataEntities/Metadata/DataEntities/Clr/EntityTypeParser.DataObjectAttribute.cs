//using System.ComponentModel;
//using System.Reflection;

//namespace System.Data.DataEntities.Metadata.Clr
//{
//    /// <summary>
//    /// Return a EntityType from CLR type with DataObjectAttribute.
//    /// </summary>
//    internal sealed class EntityTypeParserForDataObjectAttribute : EntityTypeParserForPropertyInfo
//    {
//        public override bool TryParse(Type clrType, EntityType entityType)
//        {
//            if (clrType.IsDefined(DataObjectAttributeType, false))
//            {
//                return base.TryParse(clrType, entityType);
//            }
//            return false;
//        }

//        private static readonly Type DataObjectAttributeType = typeof(DataObjectAttribute);
//        private static readonly Type DataObjectFieldAttributeType = typeof(DataObjectFieldAttribute);

//        protected override bool Match(PropertyInfo member)
//        {
//            return member.IsDefined(DataObjectFieldAttributeType, false);
//        }

//        protected override bool IsPrimaryKey(PropertyInfo propertyInfo, EntityProperty entityProperty)
//        {
//            var att  = propertyInfo.GetFirstOrDefaultAttribute<DataObjectFieldAttribute>(false);
//            if ((att != null) && (att.PrimaryKey))
//            {
//                return true;
//            }
//            return false;
//        }
//    }
//}
