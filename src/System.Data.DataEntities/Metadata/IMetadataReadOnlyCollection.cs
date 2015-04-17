using System.Collections.Generic;

namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// epresents a strongly-typed, read-only collection of IMemberMetadata.
    /// </summary>
    /// <typeparam name="T">Element data type</typeparam>
    public interface IMetadataReadOnlyCollection<out T> : IEnumerable<T> where T : IMemberMetadata {

        /// <summary>
        /// Determines whether a sequence contains a specified name by using the default equality comparer.
        /// </summary>
        /// <param name="name">The name to locate in the sequence.</param>
        /// <returns>true if the source sequence contains name that has the specified value; otherwise, false.</returns>
        bool Contains(string name);

        /// <summary>
        /// Copies the elements of the IMetadataReadOnlyCollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <returns>returns a new Object array containing the contents of the List. </returns>
        T[] ToArray();

        /// <summary>
        /// Gets the number of elements contained in the IMetadataReadOnlyCollection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is not a valid index of IMetadataReadOnlyCollection.</exception>
        T this[int index] { get; }

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">To obtain the distinguished name of the element.</param>
        /// <returns>An element with the specified name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Retrieves the property name was not found.</exception>
        T this[string name] { get; }

        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <param name="name">To get the name of the value.</param>
        /// <returns>If the list element contains an element with the specified name, or true; otherwise, false.</returns>
        T TryGet(string name);
    }
}
