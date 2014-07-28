using System;

namespace System.Data.DataEntities.Metadata {
    
    /// <summary>
    /// Defines an interface, update the State of the object via this interface allows, for example which field is read from the database has been modified, so that helps the ORM engine only update the modified field.
    /// </summary>
    public interface IEntityState {

        /// <summary>
        /// Returns whether the specified property changes.
        /// </summary>
        /// <param name="property">To test property</param>
        /// <returns>If this entity is read from the database or create a new change after the returns true, otherwise returns false.</returns>
        bool GetIsChanged(IEntityProperty property);

        /// <summary>
        /// Reset all State of an entity, this usually occurs after ORM engine reads data or save the data.
        /// </summary>
        void ResetState();
    }
}
