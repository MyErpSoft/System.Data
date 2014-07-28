using System;
using System.ComponentModel;
using System.Data.DataEntities.Metadata;

namespace System.Data.DataEntities {

    /// <summary>
    /// The base class for entity classes
    /// </summary>
    public abstract class Entity 
        : INotifyPropertyChanging,
        INotifyPropertyChanged, 
        INotifyPropertyUnchanged,
        ISupportInitialize,
        IObjectWithParent {

        /// <summary>
        /// Returns the current entity's entity type information
        /// </summary>
        /// <returns>Type information from an entity instance</returns>
        public abstract IEntityType GetEntityType();

        #region INotifyPropertyChanged
        /// <summary>
        /// When this event is triggered when a property changes.
        /// </summary>
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Trigger change events
        /// </summary>
        /// <param name="e">Event parameters</param>
        protected internal virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            if (_initializing || this.PropertyChanged == null) {
                return;
            }

            this.PropertyChanged(this, e);
        } 
        #endregion

        #region INotifyPropertyUnchanged
        /// <summary>
        /// The Set is invoked when a property value but did not change when this event is triggered.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        internal void OnPropertyUnchanged(PropertyUnchangedEventArgs e) {
            if (this._initializing || (this._propertyUnchanged == null)) {
                return;
            }
            this._propertyUnchanged(this, e); 
        }

        [NonSerialized]
        private EventHandler<PropertyUnchangedEventArgs> _propertyUnchanged;
        event EventHandler<PropertyUnchangedEventArgs> INotifyPropertyUnchanged.PropertyUnchanged {
            add { this._propertyUnchanged += value; }
            remove { this._propertyUnchanged -= value; }
        }
        #endregion

        #region INotifyPropertyChanging
        /// <summary>
        /// When a property changed event is raised before.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;
        /// <summary>
        /// Fire property change events.
        /// </summary>
        /// <param name="e">Property change event arguments.</param>
        protected internal virtual void OnPropertyChanging(PropertyChangingEventArgs e) {
            if (this._initializing || this.PropertyChanging == null) {
                return;
            }

            this.PropertyChanging(this, e);
        }

        /// <summary>
        /// Property changing event is triggered, and is used internally to reduce object creation
        /// </summary>
        /// <param name="propertyName">Name of the property is about to change</param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">Changed values</param>
        internal bool OnPropertyChanging(string propertyName, object oldValue, object newValue) {
            //Typically, this event no one interception, so it saves the event arguments created.
            if (this._initializing || this.PropertyChanging == null) {
                return false;
            }

            var e = new PropertyChangingCanCancelEventArgs(propertyName, oldValue, newValue);
            this.OnPropertyChanging(e);
            return e.Cancel;
        }
        #endregion

        #region ISupportInitialize
        private bool _initializing;
        void ISupportInitialize.BeginInit() {
            this._initializing = true;
        }

        void ISupportInitialize.EndInit() {
            _initializing = false;
        }
        #endregion

        #region IObjectWithParent 
        private object _parent;
        /// <summary>
        /// Returns the object's parent object.
        /// </summary>
        public object Parent {
            get { return this._parent; }
            internal set { this._parent = value; }
        }
        
        #endregion

        #region ToString
        /// <summary>
        /// Output is an entity recognized string.
        /// </summary>
        /// <returns>String that contains the primary key value and type information.</returns>
        public override string ToString() {
            return this.GetEntityDisplayName(this.GetEntityType());
        }
        #endregion
    }
}
