using System.Collections.Generic;
using System.ComponentModel;

namespace System.Data.DataEntities.Metadata.Clr
{
    internal abstract class EntityTypeParser
    {
        public static bool Parse(Type clrType, EntityType entityType)
        {
            if ((new EntityTypeParserForDataObjectAttribute()).TryParse(clrType,entityType) ||
                (new EntityTypeParserForXmlTypeAttribute()).TryParse(clrType,entityType))
            {
                return true;
            }
            return false;
        }

        private Type _clrType;
        /// <summary>Returns need to paste the CLR class</summary>
        public Type ClrType { get { return _clrType; } }

        private  EntityType _entityType;
        /// <summary>Return to the entity class is pasted</summary>
        public EntityType EntityType { get { return _entityType; } }

        public virtual bool TryParse(Type clrType, EntityType entityType)
        {
            this._clrType = clrType;
            this._entityType = entityType;

            return true;
        }

        internal static readonly Type StringType = typeof(string);

    }
}
