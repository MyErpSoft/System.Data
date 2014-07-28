
namespace System.ComponentModel {

    /// <summary>
    /// Notification object is called a Set value, but no property change occurred. Callback on Engine Tuning that is used in the UI.
    /// </summary>
    public interface INotifyPropertyUnchanged {

        /// <summary>
        /// Notification object is called a Set value, but no property change occurred.
        /// </summary>
        event EventHandler<PropertyUnchangedEventArgs> PropertyUnchanged;
    }

}
