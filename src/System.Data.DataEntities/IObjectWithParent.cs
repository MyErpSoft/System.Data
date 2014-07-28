
namespace System.Data.DataEntities {
    
    /// <summary>
    /// Describes an entity retrieved his parent entity.
    /// </summary>
    public interface IObjectWithParent {

        /// <summary>
        /// Returns the parent object of his.
        /// </summary>
        object Parent { get; }
    }
}
