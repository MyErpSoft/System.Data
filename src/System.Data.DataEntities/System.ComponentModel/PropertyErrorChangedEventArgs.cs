
namespace System.ComponentModel {

    /// <summary>
    /// Triggered when the error messages when the property changes, which is conducive to changed or wrong information that is outside the data changes.
    /// </summary>
    public class PropertyErrorChangedEventArgs : PropertyChangedEventArgs {

        /// <summary>
        /// Create PropertyErrorChangedEventArgs instance.
        /// </summary>
        /// <param name="propertyName">Properties that have changed names.</param>
        public PropertyErrorChangedEventArgs(string propertyName)
            : base(propertyName) {
        }
    }
}
