using System;

namespace System.ComponentModel {

    /// <summary>
    /// Parameters Property needed for the changing event event is raised
    /// </summary>
    public class PropertyChangingCanCancelEventArgs : PropertyChangingEventArgs {

        /// <summary>
        /// Create PropertyChangingCanCancelEventArgs instance
        /// </summary>
        /// <param name="propertyName">About to induce a change of property name</param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">Changed values </param>
        public PropertyChangingCanCancelEventArgs(string propertyName, object oldValue, object newValue)
            : base(propertyName) {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        private readonly object _oldValue;
        /// <summary>
        /// Returns the old value
        /// </summary>
        public object OldValue {
            get { return _oldValue; }
        }

        private readonly object _newValue;
        /// <summary>
        /// Returns the value of change 
        /// </summary>
        public object NewValue {
            get { return _newValue; }
        }

        private bool _cancel;
        /// <summary>
        /// Returns/sets whether or not to cancel the change, do not trigger an exception.
        /// </summary>
        public bool Cancel {
            get { return this._cancel; }
            set { this._cancel = value; }
        }
    }
}
