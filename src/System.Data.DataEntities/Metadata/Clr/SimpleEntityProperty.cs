using System.Reflection;

namespace System.Data.DataEntities.Metadata.Clr
{
    internal sealed class SimpleEntityProperty : EntityProperty,ISimpleEntityProperty
    {
        public SimpleEntityProperty(PropertyInfo property)
            :base(property)
        {
        }
    }
}
