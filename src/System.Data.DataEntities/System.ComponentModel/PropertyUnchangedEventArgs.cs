
namespace System.ComponentModel {

    /// <summary>
    /// Describes an entity Property unchanged happened event, the event parameters.
    /// </summary>
    public class PropertyUnchangedEventArgs : EventArgs {
        private readonly string _proeprtyName;

        /// <summary>
        /// Create PropertyUnchangedEventArgs instance.
        /// </summary>
        /// <param name="propertyName">Name of the property on which they occurred.</param>
        public PropertyUnchangedEventArgs(string propertyName) {
            this._proeprtyName = propertyName;
        }

        /// <summary>
        /// Returns the associated property name.
        /// </summary>
        public virtual string PropertyName {
            get { return _proeprtyName; }
        }
    }
}
