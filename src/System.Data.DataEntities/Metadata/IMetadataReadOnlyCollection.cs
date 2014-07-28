using System.Collections.Generic;

namespace System.Data.DataEntities.Metadata
{
    /// <summary>
    /// epresents a strongly-typed, read-only collection of IMemberMetadata.
    /// </summary>
    /// <typeparam name="T">Element data type</typeparam>
    public interface IMetadataReadOnlyCollection<T> : IEnumerable<T> where T:IMemberMetadata
    {
        /// <summary>
        /// Determines whether a sequence contains a specified element by using the default equality comparer.
        /// </summary>
        /// <param name="item">The value to locate in the sequence.</param>
        /// <returns>true if the source sequence contains an element that has the specified value; otherwise, false.</returns>
        bool Contains(T item);

        /// <summary>
        /// Determines whether a sequence contains a specified name by using the default equality comparer.
        /// </summary>
        /// <param name="name">The name to locate in the sequence.</param>
        /// <returns>true if the source sequence contains name that has the specified value; otherwise, false.</returns>
        bool Contains(string name);

        /// <summary>
        /// Determines the index of a specific item in the IMetadataReadOnlyCollection.
        /// </summary>
        /// <param name="item">The object to locate in the IMetadataReadOnlyCollection</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        int IndexOf(T item);

        /// <summary>
        /// Copies the elements of the IMetadataReadOnlyCollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from IMetadataReadOnlyCollection. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        void CopyTo(T[] array, int arrayIndex);

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
        /// <param name="value">When this method returns, if the specified key is found, returns the value associated with the key; otherwise, it returns the type of the value parameter's default value. The parameter is not initialized is passed.</param>
        /// <returns>If the list element contains an element with the specified name, or true; otherwise, false.</returns>
        bool TryGetValue(string name, out T value);
    }
}
