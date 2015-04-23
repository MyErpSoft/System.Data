using System;

namespace System.Data.Metadata.DataEntities {

    public static class MetadataExtensions {

        public static IEntityType GetEntityType(this Type type) {
            return Clr.EntityType.GetEntityType(type);
        }
    }
}
