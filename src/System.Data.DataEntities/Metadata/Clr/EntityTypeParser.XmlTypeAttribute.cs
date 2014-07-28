using System.Reflection;
using System.Xml.Serialization;

namespace System.Data.DataEntities.Metadata.Clr
{
    /// <summary>
    /// Return a EntityType from CLR type with XmlTypeAttribute. 
    /// </summary>
    /// <remarks>
    /// Not yet complete implementation, for example does not recognize custom namespace or node names.
    /// </remarks>
    internal sealed class EntityTypeParserForXmlTypeAttribute : EntityTypeParserForPropertyInfo
    {
        public override bool TryParse(Type clrType, EntityType entityType)
        {
            if (clrType.IsDefined(XmlTypeAttributeType,false))
            {
                return base.TryParse(clrType, entityType);
            }
            return false;
        }

        private static readonly Type XmlTypeAttributeType = typeof(XmlTypeAttribute);
        private static readonly Type XmlElementAttributeType = typeof(XmlElementAttribute);
        
        protected override bool Match(PropertyInfo member)
        {
            return member.IsDefined(XmlElementAttributeType, false);
        }

        protected override bool IsPrimaryKey(PropertyInfo propertyInfo, EntityProperty entityProperty)
        {
            if (string.Equals(entityProperty.Name.ToUpper(),"id", StringComparison.Ordinal) ||
                string.Equals(entityProperty.Name.ToUpper(),"name", StringComparison.Ordinal))
            {
                return true;
            }
            return false;
        }
    }
}
