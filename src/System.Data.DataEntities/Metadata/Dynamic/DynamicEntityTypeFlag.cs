
namespace System.Data.DataEntities.Metadata.Dynamic
{
    /// <summary>
    /// Dynamic entity symbol, used to identify abstract, interface, and so on.
    /// </summary>
    public enum DynamicEntityTypeFlag
    {
        /// <summary>class</summary>
        Class = 0,
        
        /// <summary>sealed Entity type, no further derivations are allowed.</summary>
        Sealed = 1,

        /// <summary>Abstract entity type that cannot be instantiated directly</summary>
        Abstract = 2,

        /// <summary>An interface type and cannot be instantiated directly.</summary>
        Interface = 3
    }
}
