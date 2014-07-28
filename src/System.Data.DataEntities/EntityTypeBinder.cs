using System;
using System.Data.DataEntities.Metadata;

namespace System.Data.DataEntities {
    /// <summary>
    /// Type of entity with binding, requires a physical entity type that implements a specific interface to return to his message.
    /// </summary>
    public class EntityTypeBinder {
        /// <summary>
        /// Returns an entity type information
        /// </summary>
        /// <param name="obj">The entity to be checked</param>
        /// <returns>This type of entity information. If you are unable to obtain returns null.</returns>
        public virtual IEntityType GetEntityType(object obj) {
            if (obj == null) {
                OrmUtility.ThrowArgumentNullException("obj");
            }

            Entity p = obj as Entity;
            if (p != null) {
                return p.GetEntityType();
            }

            return obj.GetType().GetEntityType();
        }
    }
}
